using System.CommandLine;
using System.ComponentModel;
using System.Reflection;
using Synapse.CLI.Abstractions.Attributes;

namespace Synapse.CLI.Entities;

/// <summary>
/// Handles the mapping between command-line options and a specific options class.
/// This class uses reflection to discover properties and bind their values during execution.
/// </summary>
/// <typeparam name="TOptions">The type of the class that holds command-line arguments.</typeparam>
internal class CommandOptionBinding<TOptions>
   where TOptions : new()
{
   private readonly Dictionary<PropertyInfo, Option> _options = new Dictionary<PropertyInfo, Option>();

   /// <summary>
   /// Initializes a new instance of the <see cref="CommandOptionBinding{TOptions}"/> class
   /// and registers discovered options into the specified <see cref="Command"/>.
   /// </summary>
   /// <param name="command">The command to which the discovered options will be added.</param>
   public CommandOptionBinding(Command command)
   {
      Initialize(command);
   }

   /// <summary>
   /// Creates an instance of <typeparamref name="TOptions"/> and populates its properties
   /// with values extracted from the <see cref="ParseResult"/>.
   /// </summary>
   /// <param name="result">The result of the command-line parsing operation.</param>
   /// <returns>A fully populated instance of <typeparamref name="TOptions"/>.</returns>
   public TOptions Bind(ParseResult result)
   {
      var options = new TOptions();

      foreach (var (property, option) in _options)
      {
         var method = typeof(CommandOptionBinding<TOptions>)
            .GetMethod(nameof(GetOptionValue), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(property.PropertyType);

         var value = method.Invoke(this, [result, option]);
         property.SetValue(options, value, null);
      }

      return options;
   }

   /// <summary>
   /// Reflects over the properties of <typeparamref name="TOptions"/> to create and register
   /// corresponding <see cref="Option"/> instances.
   /// </summary>
   /// <param name="command">The command to register the options with.</param>
   /// <exception cref="InvalidOperationException">Thrown if an option instance cannot be created.</exception>
   private void Initialize(Command command)
   {
      var properties = typeof(TOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance);

      foreach (var property in properties)
      {
         var alis = property.GetCustomAttribute<OptionAttribute>()?.Aliases ?? [$"--{property.Name.ToLower()}"];
         var desc = property.GetCustomAttribute<DescriptionAttribute>()?.Description;

         var type = typeof(Option<>).MakeGenericType(property.PropertyType);

         var name = alis[0];
         var extr = alis.Skip(1).ToArray();

         var instance = Activator.CreateInstance(type, [name, extr]);
         if (instance is not Option option)
         {
            throw new InvalidOperationException(
                $"Failed to create an option instance for property '{property.Name}'. " +
                $"Ensure that '{type.Name}' has a compatible constructor.");
         }

         option.Description = desc;

         command.Add(option);
         _options.Add(property, option);
      }
   }

   /// <summary>
   /// A helper method called via reflection to invoke the strongly-typed
   /// <see cref="ParseResult.GetValue{T}(Option{T})"/> method.
   /// </summary>
   private T? GetOptionValue<T>(ParseResult result, Option option)
   {
      return result.GetValue((Option<T>)option);
   }
}
