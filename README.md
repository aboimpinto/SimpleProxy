# Simple Proxy (for Net Core)

![Build status](https://dev.azure.com/mambosoftware/SimpleProxy/_apis/build/status/SimpleProxy-CI)

Simple Proxy is a simple extension library built to solve the problem of Aspect Orientated Programming in Net Core projects.

### Installation Options
- Download the code and build the project in Visual Studio. SimpleProxy.dll will be built. Reference this file in your Net Core project.
- Its available as a nuget package: https://www.nuget.org/packages/SimpleProxy/1.0.0#
- Install-Package SimpleProxy -Version 1.0.0

### Getting Started
The code also contains a SampleApp which shows the code running, and a simple example of how it is configured.

Building your own interceptors is a simple process.

1) Create a Method Interceptor Attribute by inheriting from the ```MethodInterceptorAttribute``` class

```csharp
    public class ConsoleLogAttribute : MethodInterceptorAttribute
    {
    }
```

2) Create a Method Interceptor by inheriting from the ```MethodInterceptor``` class

```csharp
public class ConsoleLogInterceptor : MethodInterceptor
{
    public override void BeforeInvoke(IInterceptionContext interceptionContext)
    {
        Console.WriteLine($"Method executing: {interceptionContext.CurrentMethod.Name}");
    }

    public override void AfterInvoke(IInterceptionContext interceptionContext, object methodResult)
    {
        Console.WriteLine($"Method executed: {interceptionContext.CurrentMethod.Name}");
    }
}
```
3) In your application startup, configure Simple Proxy to know which interceptors and attributes you will be using.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Configure Simple Proxy
    services.EnableSimpleProxy(p => p.AddInterceptor<ConsoleLogAttribute, ConsoleLogInterceptor>());

    // Configure your services using the Extension Methods
    services.AddTransientWithProxy<ITestClass, TestClass>();
}

```

4) Add the Attribute to your method - the Interceptor that was configured for the attribute will be called when the method is called
```csharp
public class TestClass : ITestClass
{
    /// <summary>
    /// Test Method with the <see cref="ConsoleLogAttribute"/> applied
    /// </summary>
    [ConsoleLog]
    public void TestMethod()
    {
    }
}
```

### What is IInterceptionContext?

The InterceptionContext is built during the Proxy process and contains all the information about the proxied object. This enables you to see during Debug, exactly what is being executed and what configuration is being used.

### Help! I'm stuck

More help and a full "Getting Started" guide is on the way soon.

### License

This project is released under the MIT License and as such, its terms and conditions apply.
