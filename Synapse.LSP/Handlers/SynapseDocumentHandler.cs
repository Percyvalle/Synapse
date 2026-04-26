using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Synapse.LSP.Abstractions;

namespace Synapse.LSP.Handlers;

internal class SynapseDocumentHandler : ITextDocumentSyncHandler
{
   /// <inheritdoc/>
   public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
   {
      return new TextDocumentAttributes(uri, LanguageConstants.Selector);
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
         DocumentSelector = LanguageConstants.Selector,
         SyncKind = TextDocumentSyncKind.Incremental,
      };
   }

   /// <inheritdoc/>
   TextDocumentOpenRegistrationOptions IRegistration<TextDocumentOpenRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
   {
      return new TextDocumentOpenRegistrationOptions
      {
         DocumentSelector = LanguageConstants.Selector,
      };
   }

   /// <inheritdoc/>
   TextDocumentCloseRegistrationOptions IRegistration<TextDocumentCloseRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
   {
      return new TextDocumentCloseRegistrationOptions
      {
         DocumentSelector = LanguageConstants.Selector,
      };
   }

   /// <inheritdoc/>
   TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
   {
      return new TextDocumentSaveRegistrationOptions
      {
         IncludeText = true,
         DocumentSelector = LanguageConstants.Selector,
      };
   }
}
