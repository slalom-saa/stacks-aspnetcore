# Stacks - AspNetCore

[![Build status](https://ci.appveyor.com/api/projects/status/t5ja4yo6bbfxuuwh/branch/master?svg=true)](https://ci.appveyor.com/project/slalom-saa/stacks-aspnetcore/branch/master)   [![NuGet Version](http://img.shields.io/nuget/v/Slalom.Stacks.Web.AspNetCore.svg?style=flat)](https://www.nuget.org/packages/Slalom.Stacks.Web.AspNetCore/)

## Getting Started
1. Create the **HelloWorldService** from [Stacks - Getting Started](https://github.com/slalom-saa/stacks/blob/master/README.md).
2.	Add the **Slalom.Stacks.Web.AspNetCore** NuGet package.  

```
Install-Package Slalom.Stacks.Web.AspNetCore
```
3. Add an **EndPoint** attribute and a return type to **HelloWorld**.
```csharp
[EndPoint("api/hello")]
public class HelloWorld : EndPoint<HelloWorldRequest, string>
{
    public override string Receive(HelloWorldRequest instance)
    {
        return "Hello " + instance.Name + "!";
    }
}
```
4.	Open **Program.cs** and modify Main to host the service using AspNetCore.
```csharp
public class Program
{
    public static void Main(string[] args)
    {
        using (var stack = new Stack())
        {
            stack.RunWebHost();
        }
    }
}
```
5. Start the application.  
6. In a web browser, navigate to http://localhost:5000/swagger. 

