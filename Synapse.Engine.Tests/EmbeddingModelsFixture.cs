namespace Synapse.Engine.Tests;

/// <summary>
/// A test fixture that manages the paths to the ONNX models and tokenizer configuration files
/// used across embedding generator unit tests.
/// </summary>
public class EmbeddingModelsFixture
{
   /// <summary>
   /// Initializes a new instance of the <see cref="EmbeddingModelsFixture"/> class.
   /// Resolves the absolute paths to the test model and vocabulary files.
   /// </summary>
   public EmbeddingModelsFixture()
   {
      var baseDir = AppContext.BaseDirectory;

      Model768Path = Path.Combine(AppContext.BaseDirectory, "Data", "model_quint8_avx2.onnx");
      Vocab768Path = Path.Combine(AppContext.BaseDirectory, "Data", "tokenizer.json");
   }

   /// <summary>
   /// Gets the absolute file path to the 768-dimensional quantized ONNX model.
   /// </summary
   public string Model768Path { get; }

   /// <summary>
   /// Gets the absolute file path to the tokenizer configuration (vocabulary) file.
   /// </summary>
   public string Vocab768Path { get; }
}
