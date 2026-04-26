namespace Synapse.CLI.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
   public CommandAttribute(string name)
   {
      Name = name;
   }

   public string Name { get; }
}
