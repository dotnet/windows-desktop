# NET Core vs .NET Framework
> GOAL: How they are interchangeable now, but what benefits compared with .NET Framework, including 

The differences between the .NET Framework and .NET Core 3 in three points:

 - NuGet-based: .NET Core is distributed as a set of NuGet packages that allow app-local deployments. In contrast, the .NET Framework is always installed in a system-wide location. 
These difference doesn’t matter so much for class libraries, but it matters for applications as those are expected to deploy the closure of their dependencies. 

 - Well layered: .NET Core was explicitly designed to be layered.
 
 - Free of problematic tech: .NET Core doesn’t include certain technologies which are going to be discontinued because they are considered to be problematic, for instance, AppDomain, sandboxing and other unsupported technologies.
 
Let’s look at some areas you should be aware of:

 - .NET Frameworks is going to be deprecated in the long term.
 - On-demand deployment.
 - Side by side deployment.
 - Self-containing applications (shipping .NET Core inside the app)    
   - .NET Core can be packaged besides application for independent installation.
 - Improved CoreFX.
 - Better performance of Core vs. Framework.
 - No desktop designers are running on Core 3.

Some point to get into consideration:

## Reflection
Since the idea is to make Reflection as an optional component in .NET Core more pay-for-play friendly. It is to ensure that you don't have to pay for features you don't use. This isolation has tricky aspects mostly related to dependencies like Object.GetType() and Reflection. To make this isolation possible, System.Type in .NET Core no longer contains APIs such as GetMembers().

In order to get access to additional type information, you have to invoke an extension method called GetTypeInfo() that lives in System.Reflection. It returns the new type TypeInfo which is what Type used to be:

```csharp
var members = obj.GetType().GetMembers();
```

is now

```csharp
using System.Reflection;
...
var members = obj.GetType().GetTypeInfo().GetMembers();
```

If you already have experience with using reflection in .NET Core, you may have figured out that certain API concepts, such as BindingFlags, were also removed. Recently many of these concepts were added back.

## Technologies discontinued for .NET Core

Though the .NET platform is a very mature stack that is almost 15 years old and proved to be reliable and flexible over the years, with the emergence of .NET Core and the new mindset of making it portable, fast and modular; some technologies existing in .NET platform were revisited, and some of them removed because they proved to be complicated or problematic.

### App Domains

AppDomains require runtime support and are generally quite expensive. While CoreCLR still implements it, it’s not available in .NET Native.
 AppDomains serve different purposes
 - For code isolation, processes and containers are highly recommended
 - For dynamic loading of assemblies, the new AssemblyLoadContext class is the preferred alternative.

### Remoting

The idea of .NET remoting has been identified as a problematic architecture.
 Outside of that realm, it is also used for cross AppDomain communication. On top of that, remoting requires runtime support and is quite heavyweight.
For communication across processes, inter-process communication (IPC) should be used, such as pipes or memory mapped files.
Across machines, you should use a network-based solution, preferably a low-overhead plain text protocol such as HTTP.

### Binary serialization

After a decade of servicing, binary serialization has proven to be complicated and a substantial compatibility burden for the types supporting it.
However, binary serialization requires intimate knowledge of the types because it allows serializing object graphs, which includes private state.

Instead of binary serialization, you should pick the serialization technology that fits your goals for formatting and footprint.
Popular choices include data contract serialization, XML serialization, JSON.NET, and protobuf-net.

### Sandboxing

Sandboxing applications and components is also really hard to get right, which is why the customers are recommended not to rely on it.

It also makes the implementation more complicated and often negatively affects the performance of applications that don’t use sandboxing.

Instead of Sandboxing like relying on the runtime of the framework to constrain which resources an application can get access to, use operating system provided security boundaries, such as user accounts for running processes with the least set of privileges.

Sandboxing is considered a non-goal for .NET Core.