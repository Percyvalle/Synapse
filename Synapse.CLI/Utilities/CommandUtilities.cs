using System.CommandLine;
using System.ComponentModel;
using System.Reflection;
using Synapse.CLI.Abstractions.Attributes;
using Synapse.CLI.Abstractions.Interfaces;
using Synapse.CLI.Entities;

namespace Synapse.CLI.Utilities;

/// <summary>
/// Provides extension methods and helper logic for bridging
/// application command interfaces with the System.CommandLine infrastructure.
/// </summary>
internal static class CommandUtilities
{
   /// <summary>
   /// Builds a <see cref="Command"/> instance by reflecting on the provided <see cref="IApplicationCommand"/>.
   /// It automatically extracts command metadata and binds generic options if the command implements <see cref="IExecutableCommand{TOptions}"/>.
   /// </summary>
   /// <param name="instance">The application command instance to wrap.</param>
   /// <returns>A configured <see cref="Command"/> ready to be added to the command-line tree.</returns>
   public static Command Build(this IApplicationCommand instance)
   {
      var type = instance.GetType();
      var name = type.GetCustomAttribute<CommandAttribute>()?.Name ?? string.Empty;
      var desc = type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;

      var command = new Command(name, desc);

      var option = Array.Find(
         instance.GetType().GetInterfaces(),
         inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IExecutableCommand<>))
         ?.GetGenericArguments()[0];

      if (option != null)
      {
         var method = typeof(CommandUtilities)
            .GetMethod(nameof(BindCommandHandler), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(option);

         method.Invoke(null, [instance, command]);
      }

      return command;
   }

   /// <summary>
   /// Sets up the command handler for a specific options type.
   /// Uses <see cref="CommandOptionBinding{TOptions}"/> to map command-line arguments to the options class properties.
   /// </summary>
   /// <typeparam name="TOptions">The type of the options class used by the command.</typeparam>
   /// <param name="instance">The executable command instance.</param>
   /// <param name="command">The system command to attach the handler to.</param>
   private static void BindCommandHandler<TOptions>(IExecutableCommand<TOptions> instance, Command command)
      where TOptions : new()
   {
      var binding = new CommandOptionBinding<TOptions>(command);

      command.SetAction(async (result, token) =>
      {
         var options = binding.Bind(result);
         return await instance.ExecuteAsync(options, token);
      });
   }
}
