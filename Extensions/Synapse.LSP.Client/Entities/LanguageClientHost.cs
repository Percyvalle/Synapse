using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;

namespace Synapse.LSP.Client.Entities;

/// <summary>
/// Represents the client-side implementation of the Language Server Protocol (LSP) for Visual Studio.
/// </summary>
[ContentType("CSharp")]
[Export(typeof(ILanguageClient))]
public class LanguageClientHost : ILanguageClient
{
   /// <inheritdoc/>
   public event AsyncEventHandler<EventArgs> StartAsync;

   /// <inheritdoc/>
   public event AsyncEventHandler<EventArgs> StopAsync;

   /// <inheritdoc/>
   public string Name => "Synapse";

   /// <inheritdoc/>
   public IEnumerable<string> ConfigurationSections
   {
      get
      {
         yield return "synapse";
      }
   }

   /// <inheritdoc/>
   public object InitializationOptions => null;

   /// <inheritdoc/>
   public IEnumerable<string> FilesToWatch => null;

   /// <inheritdoc/>
   public bool ShowNotificationOnInitializeFailed => true;

   /// <inheritdoc/>
   public async Task<Connection> ActivateAsync(CancellationToken token)
   {
      var pipe = $"synapse-{Guid.NewGuid()}";
      var stream = new NamedPipeClientStream(".", pipe, PipeDirection.InOut, PipeOptions.Asynchronous);

      var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var worked = Path.Combine(path, "Host");
      var server = Path.Combine(worked, "Synapse.CLI.exe");

      if (!File.Exists(server))
      {
         throw new FileNotFoundException($"LSP Server not found at: {server}");
      }

      ProcessStartInfo info = new ProcessStartInfo
      {
         FileName = server,
         Arguments = $"start --pipe={pipe}",
         WorkingDirectory = worked,
         UseShellExecute = false,
         CreateNoWindow = false,
      };

      Process process = new Process { StartInfo = info };

      if (process.Start())
      {
         await stream.ConnectAsync(token);
         return new Connection(stream, stream);
      }

      return null;
   }

   /// <inheritdoc/>
   public async Task OnLoadedAsync()
   {
      await StartAsync.InvokeAsync(this, EventArgs.Empty);
   }

   /// <inheritdoc/>
   public Task OnServerInitializedAsync()
   {
      return Task.CompletedTask;
   }

   /// <inheritdoc/>
   public Task<InitializationFailureContext> OnServerInitializeFailedAsync(ILanguageClientInitializationInfo initializationState)
   {
      var context = new InitializationFailureContext()
      {
         FailureMessage = initializationState.StatusMessage,
      };

      return Task.FromResult(context);
   }
}
