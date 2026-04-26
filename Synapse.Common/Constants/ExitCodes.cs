namespace Synapse.Common.Constants;

/// <summary>
/// Defines standard exit codes used to communicate the application's termination status to the operating system.
/// </summary>
public static class ExitCodes
{
   /// <summary>
   /// Indicates that the application completed its task successfully (EXIT_SUCCESS).
   /// </summary>
   public const int Success = 0;

   /// <summary>
   /// Indicates that the application terminated due to an unhandled exception or a critical failure (EXIT_FAILURE).
   /// </summary>
   public const int Failure = 1;
}
