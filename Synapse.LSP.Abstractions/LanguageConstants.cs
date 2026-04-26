using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Synapse.LSP.Abstractions;

/// <summary>
/// Provides common constants and shared objects used across the Synapse LSP implementation.
/// </summary>
public static class LanguageConstants
{
   /// <summary>
   /// The language identifier for C#.
   /// </summary>
   public const string CSharpLanguageId = "csharp";

   /// <summary>
   /// Gets the standard document selector used to identify C# source files.
   /// </summary>
   public static readonly TextDocumentSelector Selector = TextDocumentSelector.ForLanguage(CSharpLanguageId);
}
