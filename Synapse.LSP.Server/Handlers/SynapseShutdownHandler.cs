using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Serilog;

namespace Synapse.LSP.Server.Handlers;

/// <summary>
/// Handles the "shutdown" request sent from the client to the server.
/// </summary>
internal class SynapseShutdownHandler : IShutdownHandler
{
   /// <inheritdoc/>
   public async Task<Unit> Handle(ShutdownParams request, CancellationToken cancellationToken)
   {
      await Log.CloseAndFlushAsync();
      return Unit.Value;
   }
}
