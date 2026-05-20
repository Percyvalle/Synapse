using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Embeddings;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.Vocabulary;

namespace Synapse.Engine.Persistence;

/// <summary>
/// Represents an in-memory repository for storing and querying vector embeddings along with their associated metadata.
/// This implementation uses integer identifiers, cosine similarity for vector comparisons, and a dictionary-based storage with vocabulary support.
/// </summary>
/// <typeparam name="TMetadata">The type of the metadata associated with each vector entry.</typeparam>
public class VectorMetadataRepository<TMetadata>
   : MemoryVectorDatabaseBase<
      int,
      TMetadata,
      MemoryDictionaryVectorStoreWithVocabulary<int, TMetadata, DictionaryVocabularyStore<string>, string, int>,
      IntIdGenerator,
      CosineSimilarityVectorComparer>, IMemoryVectorDatabase<int, TMetadata>, IVectorDatabase<int, TMetadata>
{
   /// <summary>
   /// Initializes a new instance of the <see cref="VectorMetadataRepository{TMetadata}"/> class.
   /// </summary>
   /// <param name="generator">The embeddings generator used to create vector representations from text or other inputs.</param>
   public VectorMetadataRepository(IEmbeddingsGenerator generator)
      : base(
         generator,
         new MemoryDictionaryVectorStoreWithVocabulary<int, TMetadata, DictionaryVocabularyStore<string>, string, int>(
            new DictionaryVocabularyStore<string>()))
   {
   }
}
