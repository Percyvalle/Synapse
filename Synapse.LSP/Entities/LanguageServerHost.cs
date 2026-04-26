using Synapse.LSP.Abstractions.Interfaces;

namespace Synapse.LSP.Entities;

public class LanguageServerHost : ILanguageServer
{
   /// <inheritdoc/>
   public Task<int> RunAsync(CancellationToken token = default)
   {
      throw new NotImplementedException();
   }

   /// <inheritdoc/>
   public ValueTask DisposeAsync()
   {
      throw new NotImplementedException();
   }
}
