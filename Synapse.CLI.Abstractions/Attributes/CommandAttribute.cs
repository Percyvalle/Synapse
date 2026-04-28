namespace Synapse.CLI.Abstractions.Attributes;

/// <summary>
/// Specifies the string identifier used to resolve and execute a command via reflection.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
   /// <summary>
   /// Initializes a new instance of the <see cref="CommandAttribute"/> class with a specific name.
   /// </summary>
   /// <param name="name">The unique name of the command.</param>
   public CommandAttribute(string name)
   {
      Name = name;
   }

   /// <summary>
   /// Gets the unique name of the command.
   /// </summary>
   public string Name { get; }
}
