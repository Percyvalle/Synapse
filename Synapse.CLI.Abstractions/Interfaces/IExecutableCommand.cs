namespace Synapse.CLI.Abstractions.Interfaces;

/// <summary>
/// Marker interface for all application commands.
/// </summary>
public interface IApplicationCommand
{
}

/// <summary>
/// Defines a contract for a CLI command that can be executed with a specific set of options.
/// </summary>
/// <typeparam name="TOptions">The type of the options class containing command arguments.</typeparam>
public interface IExecutableCommand<in TOptions> : IApplicationCommand
{
   /// <summary>
   /// Executes the command logic asynchronously.
   /// </summary>
   /// <param name="options">An instance of <typeparamref name="TOptions"/> populated from the command-line arguments.</param>
   /// <param name="token">A token that can be used to signal cancellation of the command execution.</param>
   /// <returns>A task representing the operation, returning an exit code (0 for success, non-zero for failure).</returns>
   Task<int> ExecuteAsync(TOptions options, CancellationToken token = default);
}
