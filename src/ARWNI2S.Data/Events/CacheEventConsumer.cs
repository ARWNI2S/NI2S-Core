﻿using ARWNI2S.Caching;
using ARWNI2S.Context;
using ARWNI2S.Events;

namespace ARWNI2S.Data.Events
{
    /// <summary>
    /// Represents the base entity cache event consumer
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract partial class CacheEventConsumer<TEntity> :
        IEventConsumer<EntityCreatedEvent<TEntity>>,
        IEventConsumer<EntityUpdatedEvent<TEntity>>,
        IEventConsumer<EntityDeletedEvent<TEntity>>
        where TEntity : DataEntity
    {
        #region Fields

        protected readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        protected CacheEventConsumer()
        {
            _staticCacheManager = NI2SContext.Current.Resolve<IStaticCacheManager>();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ClearCacheAsync(TEntity entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(EntityCacheDefaults<TEntity>.ByIdsPrefix);
            await RemoveByPrefixAsync(EntityCacheDefaults<TEntity>.AllPrefix);

            if (entityEventType != EntityEventType.Insert)
                await RemoveAsync(EntityCacheDefaults<TEntity>.ByIdCacheKey, entity);

            await ClearCacheAsync(entity);
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual Task ClearCacheAsync(TEntity entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            await _staticCacheManager.RemoveByPrefixAsync(prefix, prefixParameters);
        }

        /// <summary>
        /// Remove the value with the specified key from the cache
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="cacheKeyParameters">Parameters to create cache key</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            await _staticCacheManager.RemoveAsync(cacheKey, cacheKeyParameters);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle entity inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task HandleEventAsync(EntityCreatedEvent<TEntity> eventMessage)
        {
            await ClearCacheAsync(eventMessage.Entity, EntityEventType.Insert);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task HandleEventAsync(EntityUpdatedEvent<TEntity> eventMessage)
        {
            await ClearCacheAsync(eventMessage.Entity, EntityEventType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task HandleEventAsync(EntityDeletedEvent<TEntity> eventMessage)
        {
            await ClearCacheAsync(eventMessage.Entity, EntityEventType.Delete);
        }

        #endregion

        #region Nested

        protected enum EntityEventType
        {
            Insert,
            Update,
            Delete
        }

        #endregion
    }
}