using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Synapse.LSP.Server.Handlers;

internal class SynapseInitializedHandler : ILanguageProtocolInitializedHandler
{
   /// <inheritdoc/>
   public Task<Unit> Handle(InitializedParams request, CancellationToken cancellationToken)
   {
      return Task.FromResult(Unit.Value);
   }
}
