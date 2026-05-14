using Build5Nines.SharpVector.Embeddings;
using Microsoft.ML.OnnxRuntime;
using Tokenizers.DotNet;

namespace Synapse.Engine.Embeddings;

/// <summary>
/// Generates vector embeddings from text using an ONNX model.
/// Utilizes a zero-allocation tensor approach for optimal inference performance.
/// </summary>
public class EmbeddingsGenerator : IEmbeddingsGenerator, IDisposable
{
   private readonly InferenceSession _session;
   private readonly Tokenizer _tokenizer;

   private readonly string[] _input = ["input_ids", "attention_mask"];
   private readonly string[] _output;

   private readonly int _hidden;

   /// <summary>
   /// Initializes a new instance of the <see cref="EmbeddingsGenerator"/> class.
   /// </summary>
   /// <param name="model">The file path to the pre-trained ONNX model (e.g., model.onnx).</param>
   /// <param name="vocab">The file path to the tokenizer vocabulary or configuration file.</param>
   /// <param name="hidden">The dimensionality of the output embedding vector (e.g., 384 for MiniLM, 768 for BERT).</param>
   public EmbeddingsGenerator(string model, string vocab, int hidden)
   {
      _session = new InferenceSession(model, new SessionOptions
      {
         IntraOpNumThreads = Environment.ProcessorCount,
      });
      _tokenizer = new Tokenizer(vocab);

      _output = _session.OutputMetadata.Keys.ToArray();
      _hidden = hidden;
   }

   /// <inheritdoc/>
   public async Task<float[]> GenerateEmbeddingsAsync(string text)
   {
      var ids = _tokenizer.Encode(text).Select(x => (long)x).ToArray();
      var mask = Enumerable.Repeat(1L, ids.Length).ToArray();
      var shape = new long[] { 1, ids.Length };

      using var inpid = OrtValue.CreateTensorValueFromMemory(OrtMemoryInfo.DefaultInstance, ids.AsMemory(), shape); // Input Ids
      using var attnt = OrtValue.CreateTensorValueFromMemory(OrtMemoryInfo.DefaultInstance, mask.AsMemory(), shape); // Attention Mask

      var oshape = new long[] { 1, ids.Length, _hidden };
      var buffer = new float[ids.Length * _hidden];
      using var outten = OrtValue.CreateTensorValueFromMemory(OrtMemoryInfo.DefaultInstance, buffer.AsMemory(), oshape);

      var options = new RunOptions
      {
         LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_INFO,
      };

      var inputs = new OrtValue[] { inpid, attnt };
      var outputs = new OrtValue[] { outten };
      var result = await _session.RunAsync(options, _input, inputs, _output, outputs);

      var vector = new float[_hidden];
      buffer.AsSpan(0, _hidden).CopyTo(vector);
      return vector;
   }

   /// <inheritdoc/>
   public void Dispose()
   {
      _session.Dispose();
   }
}
