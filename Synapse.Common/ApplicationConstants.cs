namespace Synapse.Common;

/// <summary>
/// Provides global constant values used throughout the Synapse ecosystem.
/// </summary>
public static class ApplicationConstants
{
   /// <summary>
   /// The description of the root command, displayed in the help output.
   /// </summary>
   public const string RootCommandDescription = "Synapse Language Server Protocol (LSP) Server";

   /// <summary>
   /// The default filename used for the Serilog logging sink if no other name is specified.
   /// </summary>
   public const string DefaultLogFileName = "synapse-.log";
}
