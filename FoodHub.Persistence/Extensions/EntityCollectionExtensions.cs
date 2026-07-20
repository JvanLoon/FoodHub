namespace FoodHub.Persistence.Extensions;

/// <summary>
/// Helpers for reconciling a tracked EF Core navigation collection (e.g.
/// <c>recipe.Ingredients</c>) with an incoming set of items in one pass:
/// existing entities that are still present are updated in place, entities that
/// are no longer present are removed, and genuinely new items are added.
///
/// This lets a single <c>SaveChanges</c> emit the correct UPDATE / DELETE /
/// INSERT statements, instead of clearing the collection and re-inserting every
/// row (which throws away entity identity, CreatedDate, etc. and churns the DB).
/// </summary>
public static class EntityCollectionExtensions
{
	/// <summary>
	/// Reconciles <paramref name="target"/> with <paramref name="source"/>, keyed
	/// by <paramref name="keyOfExisting"/> / <paramref name="keyOfIncoming"/>.
	/// </summary>
	/// <typeparam name="TEntity">The tracked entity type in the collection.</typeparam>
	/// <typeparam name="TSource">The incoming item type (e.g. a DTO).</typeparam>
	/// <typeparam name="TKey">The type used to match incoming items to existing entities.</typeparam>
	/// <param name="target">The tracked navigation collection to reconcile.</param>
	/// <param name="source">The desired set of items (a null source is treated as empty, removing everything).</param>
	/// <param name="keyOfExisting">Extracts the match key from an existing entity.</param>
	/// <param name="keyOfIncoming">Extracts the match key from an incoming item.</param>
	/// <param name="create">Builds a new entity from an incoming item with no matching entity.</param>
	/// <param name="update">Applies an incoming item onto its matching existing entity.</param>
	public static void Sync<TEntity, TSource, TKey>(
		this ICollection<TEntity> target,
		IEnumerable<TSource>? source,
		Func<TEntity, TKey> keyOfExisting,
		Func<TSource, TKey> keyOfIncoming,
		Func<TSource, TEntity> create,
		Action<TSource, TEntity> update)
		where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(target);
		ArgumentNullException.ThrowIfNull(keyOfExisting);
		ArgumentNullException.ThrowIfNull(keyOfIncoming);
		ArgumentNullException.ThrowIfNull(create);
		ArgumentNullException.ThrowIfNull(update);

		var incoming = source?.ToList() ?? [];

		var existingByKey = new Dictionary<TKey, TEntity>();
		foreach (var entity in target)
		{
			existingByKey[keyOfExisting(entity)] = entity;
		}

		var seenKeys = new HashSet<TKey>();

		// Update matched entities and add new ones.
		foreach (var item in incoming)
		{
			var key = keyOfIncoming(item);
			seenKeys.Add(key);

			if (existingByKey.TryGetValue(key, out var existing))
			{
				update(item, existing);
			}
			else
			{
				target.Add(create(item));
			}
		}

		// Remove entities that are no longer present in the incoming set.
		foreach (var entity in target.ToList())
		{
			if (!seenKeys.Contains(keyOfExisting(entity)))
			{
				target.Remove(entity);
			}
		}
	}
}
