using Synapse.Engine.Embeddings;

namespace Synapse.Engine.Tests;

/// <summary>
/// Contains unit tests for the <see cref="EmbeddingsGenerator"/> class.
/// </summary>
public class EmbeddingsGeneratorTests : IClassFixture<EmbeddingModelsFixture>
{
   private readonly EmbeddingModelsFixture _fixture;

   /// <summary>
   /// Initializes a new instance of the <see cref="EmbeddingsGeneratorTests"/> class.
   /// </summary>
   /// <param name="fixture">The shared test fixture containing paths to the ONNX models and vocabularies.</param>
   public EmbeddingsGeneratorTests(EmbeddingModelsFixture fixture)
   {
      _fixture = fixture;
   }

   [Fact]
   public async Task GenerateEmbeddingsAsync_With768HiddenSize_ReturnsCorrectVectorLength()
   {
      // Arrange
      var hidden = 768;
      var text = "embeddings";

      using var generator = new EmbeddingsGenerator(
          _fixture.Model768Path,
          _fixture.Vocab768Path,
          hidden);

      // Act
      float[] result = await generator.GenerateEmbeddingsAsync(text);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(hidden, result.Length);
      Assert.Contains(result, x => x != 0.0f);
   }
}
