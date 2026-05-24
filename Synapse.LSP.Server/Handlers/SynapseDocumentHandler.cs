using MediatR;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Synapse.Engine.Roslyn;
using Synapse.LSP.Abstractions;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Synapse.LSP.Handlers;

/// <summary>
/// Handles text document synchronization events between the IDE and the language server.
/// </summary>
internal class SynapseDocumentHandler : ITextDocumentSyncHandler
{
   private readonly ILanguageServerFacade _server;

   /// <summary>
   /// Initializes a new instance of the <see cref="SynapseDocumentHandler"/> class.
   /// </summary>
   /// <param name="server">The language server facade used to send notifications to the client.</param>
   public SynapseDocumentHandler(ILanguageServerFacade server)
   {
      _server = server;
   }

   /// <inheritdoc/>
   public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
   {
      return new TextDocumentAttributes(uri, LanguageConstants.Selector);
   }

   /// <inheritdoc/>
   public Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken token)
   {
      return Task.FromResult(Unit.Value);
   }

   /// <inheritdoc/>
   public async Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken token)
   {
      // TODO: [TEMPORARY] Proof-of-concept implementation for testing diagnostic publishing.
      // Replace with full document analysis pipeline before release.
      var source = request.TextDocument.Text;
      var tree = CSharpSyntaxTree.ParseText(source);
      var method = (await tree.GetRootAsync(token)).DescendantNodes().OfType<MethodDeclarationSyntax>().First();
      var visitor = new MethodContextVisitor();

      if (method.Body != null)
      {
         visitor.Visit(method.Body);
      }
      else if (method.ExpressionBody != null)
      {
         visitor.Visit(method.ExpressionBody);
      }

      var result = visitor.Build();

      var diagnostics = result.BlockingCalls.Select(call => new Diagnostic
      {
         Range = new Range(
            new Position(call.Line, call.Column),
            new Position(call.Line, call.Column + call.Expression.Length)),
         Severity = DiagnosticSeverity.Warning,
         Message = $"Blocking async call: '{call.Expression}'. Use 'await' instead.",
         Source = "synapse",
      }).ToList();

      _server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
      {
         Uri = request.TextDocument.Uri,
         Diagnostics = diagnostics,
      });

      return Unit.Value;
   }

   /// <inheritdoc/>
   public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken token)
   {
      return Task.FromResult(Unit.Value);
   }

   /// <inheritdoc/>
   public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken token)
   {
      return Task.FromResult(Unit.Value);
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
