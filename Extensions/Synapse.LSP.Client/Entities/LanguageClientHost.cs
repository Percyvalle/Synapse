using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using Synapse.LSP.Client.Interfaces;

namespace Synapse.LSP.Client.Entities;

/// <summary>
/// Represents the client-side implementation of the Language Server Protocol (LSP) for Visual Studio.
/// </summary>
[ContentType("CSharp")]
[Export(typeof(ILanguageClient))]
public class LanguageClientHost : ILanguageClient
{
   /// <summary>
   /// Initializes a new instance of the <see cref="LanguageClientHost"/> class.
   /// </summary>
   public LanguageClientHost()
   {
      StopAsync += OnStopAsync;
   }

   /// <inheritdoc/>
   public event AsyncEventHandler<EventArgs>? StartAsync;

   /// <inheritdoc/>
   public event AsyncEventHandler<EventArgs>? StopAsync;

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

   [Import]
   private ILanguageServerLauncher Launcher { get; set; } = null!;

   /// <inheritdoc/>
   public async Task<Connection?> ActivateAsync(CancellationToken token)
   {
      try
      {
         return await Launcher.ActivateAsync(token);
      }
      catch
      {
         // TODO: Logging the error and forwarding it further for recording in the Visual Studio Log
         return null;
      }
   }

   /// <inheritdoc/>
   public async Task OnLoadedAsync()
   {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
      await StartAsync?.InvokeAsync(this, EventArgs.Empty);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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

   private async Task OnStopAsync(object? sender, EventArgs args)
   {
      StopAsync -= OnStopAsync;

      if (Launcher != null)
      {
         await Launcher.DisposeAsync();
      }
   }
}
