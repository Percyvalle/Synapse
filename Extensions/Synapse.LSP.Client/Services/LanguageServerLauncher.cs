using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Synapse.LSP.Client.Interfaces;

namespace Synapse.LSP.Client.Services;

/// <summary>
/// Provides a concrete implementation of <see cref="ILanguageServerLauncher"/>
/// to manage the lifecycle, process spawning, and IPC channel for the Synapse LSP server.
/// </summary>
[Export(typeof(ILanguageServerLauncher))]
[PartCreationPolicy(CreationPolicy.NonShared)]
internal class LanguageServerLauncher : ILanguageServerLauncher
{
   private readonly string _pipe;

   private Process? _process;
   private NamedPipeClientStream? _stream;

   /// <summary>
   /// Initializes a new instance of the <see cref="LanguageServerLauncher"/> class.
   /// </summary>
   public LanguageServerLauncher()
   {
      _pipe = $"pipe-{Guid.NewGuid()}";
   }

   /// <inheritdoc/>
   public async Task<Connection> ActivateAsync(CancellationToken token)
   {
      var server = ResolveServerPath();
      var arguments = ResolveArguments();

      var process = new ProcessStartInfo
      {
         FileName = server,
         Arguments = arguments,
         CreateNoWindow = false,
         UseShellExecute = false,
      };

      _stream = new NamedPipeClientStream(".", _pipe, PipeDirection.InOut, PipeOptions.Asynchronous);

      try
      {
         _process = new Process
         {
            StartInfo = process,
         };

         if (!_process.Start())
         {
            throw new Exception("Failed to start LSP process.");
         }

         await _stream.ConnectAsync(token);

         return new Connection(_stream, _stream);
      }
      catch (Exception)
      {
         await DisposeAsync();
         throw;
      }
   }

   /// <inheritdoc/>
   public async ValueTask DisposeAsync()
   {
      _stream?.Dispose();

      if (_process != null && !_process.HasExited)
      {
         _process.Kill();
         _process.WaitForExit();
         _process.Dispose();
      }
   }

   private string ResolveServerPath()
   {
      var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var executable = Path.Combine(directory, LanguageServerDefaults.ExecutableFolder, LanguageServerDefaults.ExecutableName);

      if (!File.Exists(executable))
      {
         throw new FileNotFoundException($"LSP Server not found at: {executable}");
      }

      return executable;
   }

   private string ResolveArguments()
   {
      return $"start --pipe={_pipe}";
   }
}
