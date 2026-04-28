namespace Synapse.CLI.Abstractions.Attributes;

/// <summary>
/// Specifies the command-line aliases for a property used as a command option.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class OptionAttribute : Attribute
{
   /// <summary>
   /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
   /// </summary>
   /// <param name="aliases">A pipe-separated string of aliases (e.g., "--pipe|-p").</param>
   public OptionAttribute(string? aliases = default)
   {
      Aliases = aliases?.Split("|");
   }

   /// <summary>
   /// Gets the collection of aliases defined for this option.
   /// </summary>
   public string[]? Aliases { get; }
}
