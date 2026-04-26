using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Synapse.LSP.Handlers;

internal class SynapseDocumentHandler : ITextDocumentSyncHandler
{
   private const string TypeDocumentSelector = "csharp";

   /// <inheritdoc/>
   public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
   {
      return new TextDocumentAttributes(uri, TypeDocumentSelector);
   }

   /// <inheritdoc/>
   public Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   /// <inheritdoc/>
   public Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   /// <inheritdoc/>
   public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   /// <inheritdoc/>
   public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   /// <inheritdoc/>
   TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
   {
      return new TextDocumentChangeRegistrationOptions
      {
         DocumentSelector = TextDocumentSelector.ForLanguage(TypeDocumentSelector),
         SyncKind = TextDocumentSyncKind.Full, // TODO: Convert to incremental interaction
      };
   }

   /// <inheritdoc/>
   TextDocumentOpenRegistrationOptions IRegistration<TextDocumentOpenRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
   {
      return new TextDocumentOpenRegistrationOptions
      {
         DocumentSelector = TextDocumentSelector.ForLanguage(TypeDocumentSelector),
      };
   }

   /// <inheritdoc/>
   TextDocumentCloseRegistrationOptions IRegistration<TextDocumentCloseRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
   {
      return new TextDocumentCloseRegistrationOptions
      {
         DocumentSelector = TextDocumentSelector.ForLanguage(TypeDocumentSelector),
      };
   }

   /// <inheritdoc/>
   TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
   {
      return new TextDocumentSaveRegistrationOptions
      {
         IncludeText = true,
         DocumentSelector = TextDocumentSelector.ForLanguage(TypeDocumentSelector),
      };
   }
}
