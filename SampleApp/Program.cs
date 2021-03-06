namespace SampleApp {
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using SimpleProxy.Caching;
    using SimpleProxy.Diagnostics;
    using SimpleProxy.Extensions;
    using SimpleProxy.Logging;
    using SimpleProxy.Strategies;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Configure the Service Provider
            var services = new ServiceCollection();

            // Required
            services.AddOptions();
            services.AddMemoryCache();
            services.AddLogging(p => p.AddConsole(x => x.IncludeScopes = true).SetMinimumLevel(LogLevel.Trace));

            services.EnableSimpleProxy(p => p
                .AddInterceptor<LogAttribute, LogInterceptor>()
                .AddInterceptor<DiagnosticsAttribute, DiagnosticsInterceptor>()
                .AddInterceptor<CacheAttribute, CacheInterceptor>()
                .WithOrderingStrategy<PyramidOrderStrategy>());

            services.AddTransientWithProxy<ITestClass, TestClass>();

            // Get a Proxied Class and call a method
            var serviceProvider = services.BuildServiceProvider();

            var testProxy = serviceProvider.GetService<ITestClass>();

            testProxy.TestMethod();

            await testProxy.TestMethodAsync();

            testProxy.TestMethodWithExpirationPolicy(); // set the cache
            testProxy.TestMethodWithExpirationPolicy(); // execute just using the cache value
            System.Threading.Thread.Sleep(25000); // time to expire the registry
            testProxy.TestMethodWithExpirationPolicy(); // execute the method again

            Console.WriteLine("====> All Test Methods Complete.  Press a key. <====");

            Console.ReadLine();
        }
    }
}
