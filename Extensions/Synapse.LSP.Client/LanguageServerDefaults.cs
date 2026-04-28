namespace Synapse.LSP.Client;

/// <summary>
/// Provides default configuration values and file system constants
/// for the Synapse Language Server.
/// </summary>
internal static class LanguageServerDefaults
{
   /// <summary>
   /// The name of the Language Server executable file.
   /// </summary>
   public const string ExecutableName = "Synapse.CLI.exe";

   /// <summary>
   /// The relative subdirectory where the Language Server host files are located.
   /// </summary>
   public const string ExecutableFolder = "Host";
}
