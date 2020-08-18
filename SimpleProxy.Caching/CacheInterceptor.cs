﻿namespace SimpleProxy.Caching
{
    using Extensions;
    using Interfaces;
    using Microsoft.Extensions.Caching.Memory;
    using SimpleProxy;
    using System;

    /// <summary>
    /// Interceptor for Caching Method return values
    /// </summary>
    public class CacheInterceptor : IMethodInterceptor
    {
        /// <summary>
        /// Memory Cache Instance
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Initialises a new instance of the <see cref="CacheInterceptor"/>
        /// </summary>
        public CacheInterceptor(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        /// <inheritdoc />
        public void BeforeInvoke(InvocationContext invocationContext)
        {
            // Check the Memory Cache for a cached value for this method
            this.memoryCache.TryGetValue(invocationContext.GetExecutingMethodName(), out var result);

            // If a cached value is found; replace the method return value and dont execute
            // the real underlying method
            if (result != null)
            {
                invocationContext.OverrideMethodReturnValue(result);
                invocationContext.BypassInvocation();
            }
        }

        /// <inheritdoc />
        public void AfterInvoke(InvocationContext invocationContext, object methodResult)
        {
            // Save the result to the MemoryCahe with an expiration time if availble
            if (invocationContext.GetAttributeFromMethod<CacheAttribute>() != null
                && invocationContext.GetAttributeFromMethod<CacheAttribute>().MillisecondsToExpire > 0)
            {
                this.memoryCache.Set(invocationContext.GetExecutingMethodName(), methodResult, TimeSpan.FromMilliseconds(invocationContext.GetAttributeFromMethod<CacheAttribute>().MillisecondsToExpire));
            }
            else
            {
                // Save the result to the MemoryCache without any expiration time
                this.memoryCache.Set(invocationContext.GetExecutingMethodName(), methodResult);
            }
        }
    }
}
