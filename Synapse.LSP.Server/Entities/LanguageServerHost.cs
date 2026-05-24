using System.Diagnostics;
using System.Reactive;
using Build5Nines.SharpVector.Embeddings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Utilities;
using Serilog;
using Synapse.Common.Constants;
using Synapse.Engine.Abstraction.Models;
using Synapse.Engine.Embeddings;
using Synapse.Engine.Persistence;
using Synapse.LSP.Abstractions.Interfaces;
using Synapse.LSP.Handlers;
using Synapse.LSP.Server.Handlers;

namespace Synapse.LSP.Server.Entities;

/// <summary>
/// Orchestrates the Language Server Protocol (LSP) lifecycle and request handling.
/// </summary>
public class LanguageServerHost : ILanguageServer
{
   private static readonly TimeSpan DefaultMonitorInterval = TimeSpan.FromMilliseconds(100);

   private readonly LanguageServerOptions _options;
   private readonly CancellationTokenSource _cancellation;

   /// <summary>
   /// Initializes a new instance of the <see cref="LanguageServerHost"/> class.
   /// </summary>
   /// <param name="input">Input Stream.</param>
   /// <param name="output">Output Stream.</param>
   /// <param name="cancellation">Cancellation Token Source.</param>
   public LanguageServerHost(Stream input, Stream output, CancellationTokenSource cancellation)
   {
      _options = new LanguageServerOptions()
         .WithInput(input)
         .WithOutput(output)
         .ConfigureLogging(
            builder => builder
               .AddSerilog()
               .AddLanguageProtocolLogging()
               .SetMinimumLevel(LogLevel.Debug))
         .WithServices(services =>
         {
            // TODO: [TEMPORARY] Hardcoded paths for local testing.
            // Make sure to replace these with dynamic paths (e.g., AppContext.BaseDirectory) before release,
            // otherwise the server won't be able to find the model on users' machines!
            var model = @"C:\Users\goman\Desktop\Synapse\Synapse.Engine.Tests\Data\model_quint8_avx2.onnx";
            var vocab = @"C:\Users\goman\Desktop\Synapse\Synapse.Engine.Tests\Data\tokenizer.json";

            if (!File.Exists(model))
            {
               return;
            }

            var generator = new EmbeddingsGenerator(model, vocab, 768);
            services.AddSingleton<IEmbeddingsGenerator>(generator);
            services.AddSingleton<VectorMetadataRepository<RoslynChunkMetadata>>();
         })

         .WithHandler<SynapseDocumentHandler>()
         .WithHandler<SynapseShutdownHandler>();

      _cancellation = cancellation;
   }

   private LanguageServer Server { get; set; } = null!;

   /// <inheritdoc/>
   public async Task<int> RunAsync()
   {
      Server = await LanguageServer.From(_options);
      Server.Exit.Subscribe(Observer.Create<int>(i => Cancel()));

      if (Server.ClientSettings?.ProcessId != null &&
          Server.ClientSettings.ProcessId != -1)
      {
         try
         {
            var id = (int)Server.ClientSettings.ProcessId.Value;
            var process = Process.GetProcessById(id);
            process.EnableRaisingEvents = true;
            process.OnExit(Cancel);
         }
         catch (Exception ex)
         {
            // If the process dies before we get here then request shutdown immediately
            Cancel();
         }
      }

      await Server.WaitForExit.WaitAsync(_cancellation.Token);
      return ExitCodes.Success;
   }

   private void Cancel()
   {
      try
      {
         _cancellation.Cancel();
      }
      catch (ObjectDisposedException)
      {
      }
   }
}
