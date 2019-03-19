# Windows Desktop Application Modernization Guidance

This repo contains information to help you modernize your .NET desktop applications. We are prioriizing the following *high level* topics to document and provide samples, but feel free to suggest new topics in our [issues](https://github.com/dotnet/windows-desktop/issues) section.

## Porting WPF and WinForms apps to .NET Core 3

Samples, options, tips and tricks to convert WPF and WinForms projects based on .NET Framework to .NET Core 3.

## Using Windows 10 features in WPF and WinForms apps

Windows 10 include a number of APIs that are accessible from Windows 10 SDKs. You can use these APIs in .NET apps by using the Microsoft.Windows.SDK.Contracts NuGet package. [More information](/docs/win10apis/README.md) 

## Data Access

ADO.NET can be used in .NET Core 3 applications, however not all data source providers are available. 

We have plans to support Entity Framework 6 in .NET Core, and Entity Framework Core is already supported.

Other non-relational data stores (such as CosmosDB) are also available to .NET Core.

### Connecting to Services

WCF has been partially ported to .NET Core 3; however, there is no support for WCF server. WCF clients features are documented here. Additional services that support .NET Core will be included as well.

### Deployment


## Known issues

- AppDomains
- Remoting
- WCF Server
- WCF Client supported features
- Non-String Resources


## Samples

- BeanTraders
- WPF Concepts
- Memory Game
