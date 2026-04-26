namespace Synapse.CLI.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class OptionAttribute : Attribute
{
   public OptionAttribute(string? aliases = default)
   {
      Aliases = aliases?.Split("|");
   }

   public string[]? Aliases { get; }
}
