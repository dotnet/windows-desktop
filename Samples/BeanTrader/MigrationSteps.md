# Migration Steps

This is the step-by-step process used to migrate the Bean Trader sample app to .NET Core 3. The details of any migration will depend on the app being worked on, of course, but hopefully these steps (along with the [YouTube videos](https://www.youtube.com/watch?v=5MomsgkWkVw&list=PLS__JrkRveTMiWxG-Lv4cBwYfMQ6m2gmt) covering the same topic) are a useful example.

## Prepare to migrate
1. Use Visual Studio to [migrate the app's NuGet packages references](https://docs.microsoft.com/nuget/reference/migrate-packages-config-to-package-reference) to use <PackageReference> elements in the csproj (instead of packages.config).
1. Use the [.NET Portability Analyzer](https://docs.microsoft.com/dotnet/standard/analyzers/portability-analyzer) to understand the app's .NET Framework dependencies.
    1. `ApiPort.exe analyze -f . -t ".NET Core, Version=3.0" -r html -r excel`
    1. The only missing APIs that the sample app uses are `ClientBase.Close` and `ClientBase.Open`.
    1. Missing APIs used by dependencies are not very interesting. Just look at binaries belonging directly to the app, itself (BeanTraderClient, in this case) and understand whether the missing APIs are likely to be blocking or can be worked around without too much trouble.
1. Review top-level dependencies (NuGet packages, dll references, and project-to-project references) for .NET Standard or .NET Core compatibility.
    1. nuget.org and fuget.org provide useful information on NuGet packages.
    1. If a dependency doesn't target .NET Core or .NET Standard, there are a few options. In order of preference we can:
        1. Use a newer version of the dependency that supports .NET Standard or .NET Core.
        1. Find an alternative dependency that can be used instead. Is there a similar package with a .Core name suffix? Are there other libraries with similar functionality that can be used instead? Or maybe just re-target the dependency yourself if source is available.
        1. Use the existing .NET Framework-targeted dependency. This often works, but will require extra testing to make sure the app won't have issues at runtime (which could happen if the app uses the dependency in a way that exercises an API that doesn't exist on .NET Core).
    1. In the case of the BeanTrader app, all the NuGet dependencies either supported .NET Core/.NET Standard or had newer version that did, so there shouldn't be any blocking issues related to missing dependencies.

## Migrate the project file
1. Create a new project file
    1. There are three options for where to put the new project file:
        1. Replace the existing project file. This isn't a good option yet because Visual Studio designers don't work with the new project file format.
        1. Put the new project file next to the existing one. This allows the project file to automatically include source, pages, and resx files. However, it will require custom output and intermediate output paths so that the old and new projects don't use the same obj/ and bin/ folders.
            1. May encounter https://github.com/dotnet/wpf/issues/366.
        1. Put the new project file in a separate directory. This requires adding source, pages, etc. explicitly with include statements in the project file.
            1. Some resources will have different paths unless they are given proper `<link>` attributes. https://github.com/dotnet/sdk/issues/2697.
    1. For this sample, create a new .NET Core 3 WPF project file in the same directory as BeanTraderClient.csproj called BeanTraderClient.Core.csproj.
        1. This can be done by running `dotnet new wpf` in a different directory and copying/renaming the generated csproj file and adding it to the solution (either using Visual Studio or the .NET CLI).
1. Compare the old project file with the new one and copy the following elements from the old one to the new one:
    1. `<RootNamespace>`
    1. `<AssemblyName>`
    1. `<ApplicationIcon>`
    1. `<Resource>` elements (such as images, icons, etc.)
    1. `<Content>` elements (config files and other files to be deployed with the project)
    1. `<PackageReference>` elements (NuGet references)
    1. Add `<DefineConstants>NETCORE</DefineConstants>` to enable having different code paths for .NET Core and .NET Framework projects in the future (using `#if NETCORE`).
    1. Add `<GenerateAssemblyInfo>false</GenerateAsemblyInfo>` so that assembly-level attributes aren't auto-generated (they would conflict with the attributes in AssemblyInfo.cs).
    1. *Remove* the Resources\Themes\Default.Accent.xaml file from `<Page>` since it is meant to be treated as content instead of as a page.
        1. `<Page Remove="**\Default.Accent.xaml" />`
    1. Elements that aren't needed in the new project file include:
        1. Imports of common MSBuild props and targets files
        1. `<AutoGenerateBindingRedirects>`
        1. `<Compile>`, `<Page>`, `<EmbeddedResource>`, and `<ApplicationDefinition>` includes for items that the new project file format will include automatically.
1. Review NuGet packages and update versions, if needed, to be compatible with .NET Standard or .NET Core. Changes made for the BeanTrader sample:
    1. Upgrade MahApps.Metro from 1.6.5 to 2.0.0-alpha
    1. Upgrade Microsoft.Azure.Common from 2.0.4 to 2.2.1
    1. Upgrade Microsoft.IdentityModel.Clients.ActiveDirectory from 2.29.0 to 4.5.1
    1. Upgrade Nito.AsyncEx from 4.0.1 to 5.0.0-pre
1. Run `dotnet restore` to confirm that packages restore without unexpected warnings.
    1. If there are unexpected dependency warnings, it's possible to learn more about why certain packages (and particular versions of packages) are included by looking at the project.assets.json file in the project's intermediate output folder (obj). Project.assets.json includes all transitive NuGet dependencies and why they are needed.
    1. The BeanTrader sample will have one expected warning - the Microsoft.Xaml.Behaviors.Wpf package (which is pulled in by MahApps.Metro v2.0) targets .NET Framework instead of .NET Core or .NET Standard.

## Fix build issues

1. Address issues related to the .NET Framework project and .NET Core project sharing output and intermediate output paths.
    1. Add a Directory.Build.props file to set common MSBuild properties for projects in (or under) the BeanTraderClient folder specifying per-project base output paths. (more details in https://github.com/Microsoft/msbuild/1603)
        ```Xml
        <Project>
        <PropertyGroup>
            <BaseOutputPath>$(MSBuildProjectDirectory)/out/$(MSBuildProjectName)/bin</BaseOutputPath>
            <BaseIntermediateOutputPath>$(MSBuildProjectDirectory)/out/$(MSBuildProjectName)/obj</BaseIntermediateOutputPath>
        </PropertyGroup>
        </Project>

        ```
    1. Note that this also could have been solved by removing the SDK from the new project file and importing necessary props and targets files explicitly after configuring `<BaseOutputPath>` and `<BaseIntermediateOutputPath>`.
1. Update the .NET Core project's csproj to remove source in the .NET Framework project's output folders from compiling.
    1. `<Compile Remove="out\BeanTraderClient\**\*.cs" />`
1. Change Styles.xaml to set the `TitleCharacterCasing` property on `DefaultWindowStyle` instead of the `TitleCaps` property due to breaking changes in MahApps.Metro between 1.6.5 and 2.0.
    ```Xml
    <Style x:Key="DefaultWindowStyle" TargetType="{x:Type mahAppsControls:MetroWindow}">
        <Setter Property="TitleCharacterCasing"  Value="Normal" />
    </Style>
    ```
1. Add a reference to the [Microsoft.Windows.Compatibility](https://www.nuget.org/packages/Microsoft.Windows.Compatibility/2.1.0-preview3.19128.7) package.
    1. Alternatively, add references to the specific packages needed for the app. For the Bean Trader sample, that includes System.ServiceModel.NetTcp and System.ServiceModel.Duplex.
    1. https://apisof.net/ can help with finding necessary .NET packages.
1. Make sure any source files that weren't included in the previous project (but happen to be on disk) are removed.
    1. In the case of the Bean Trader sample, remove ViewModels\OldUnusedViewModel.cs from compiling.
        ```
        <Compile Remove="ViewModels\OldUnusedViewModels.cs" />
        ```
    1. If there are many C# or XAML files that need to be excluded this way, it might be easier to disable auto-inclusion of compile or page items and include sources to be compiled explicitly (which can be done by copying items from the old project file).
        ```
        <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
        <EnableDefaultPageItems>false</EnableDefaultPageItems>
        <!-- 'EnableDefaultItems' can  control both of these at once -->
        ```
1. Replace missing dependency APIs with .NET Core alternatives.
    1. Update Castle.Windsor usage to replace `Classes.FromThisAssembly` calls with `Classes.FromAssemblyContaining` and `FromAssembly.This` with `FromAssembly.Containing`.
1. Revert the MahApps.Metro dependency to 1.6.5.
    1. Earlier, we upgraded MahApps.Metro to 2.0.0-alpha since that is the earliest version to target .NET Core. There are some breaking changes around theme and accent APIs in that version of MahApps, though (as is to be expected when moving between major versions). Ideally, we would update the app to use the newer API. In the interest of time, though, it's ok to temporarily use the older (.NET Framework-targeted) package. Many, but not all, .NET Framework libraries work on .NET Core. Some reasons this one is lower risk than many are:
        1. The dependency on the library is fairly small (just theming and dialog APIs).
        1. The library is a UI package and .NET Core and .NET Framework WPF surface areas are quite similar.
1. Update auto-generated WCF clients to work with .NET Standard (rather than .NET Framework)
    1. This can be done in Visual Studio by adding a [connected service reference](https://docs.microsoft.com/dotnet/core/additional-tools/wcf-web-service-reference-guide) or [dotnet-svcutil](https://docs.microsoft.com/dotnet/core/additional-tools/dotnet-svcutil-guide?tabs=dotnetsvcutil2x).
    1. Use the VS connected service UI to add a reference to the WCF service endpoint http://beantrader.eastus.cloudapp.azure.com:8080/ with a namespace of BeanTrader.Service.
    1. Now we have two clients - one used by the old project and one used by the new one. This could be made to work (though you'll have to deal with small differences in the generated clients). Alternatively, though, if the existing project can be changed, the two projects can be kept in sync by adding the new client to the old project (and removing the old client).
    1. For porting Bean Trader, it's easiest to just update the existing csproj so that both projects use the same WCF client.
    1. Remove the old WCF client by deleting BeanTrader.cs from the .NET Framework Bean Trader project. This should remove the client from both projects (since it is auto-included in the new one).
    1. Add a reference to the new WCF client (reference.cs) in the old Bean Trader project, making sure to 'Add As Link' so that the file isn't copied.
    1. The new WCF client has slightly different namespaces (BeanTrader.Service instead of BeanTrader.Model or no namespace), so update 'using' statements, as needed.
        1. Note that some XAML files (which refer to the Bean model type) will also need namespaces updated.
    1. In order to add a constructor to the WCF client with an argument list matching what our project expects, create a partial `BeanTraderServiceClient` class (in the old project). Give that class the following constructor:
        ```CSharp
        public BeanTraderServiceClient(System.ServiceModel.InstanceContext callbackInstance, EndpointConfiguration endpointConfiguration) :
            base(callbackInstance, endpointConfiguration)
            { }
        ```
    1. Update `BeanTraderServiceClientFactory` to use this new constructor.
    1. Update `TradingService.GetOrOpenClientAsync` to call `newClient.OpenAsync` instead of `newClient.Open`.

## Test and fix runtime issues

1. Use [platform compatibility analyzers](https://github.com/dotnet/platform-compat) to identify APIs that may fail at runtime.
1. Remove the system.serviceModel section from app.config.
    1. Because the new WCF client is meant to work with .NET Standard APIs, it doesn't use app configuration. This section is no longer supported in app.config.
1. Remove `Delegate.BeginInvoke` and `.EndInvoke` usage.
    1. In TradingViewModel.cs, replace the `userInfoRetriever.BeginInvoke` call with a call to `Invoke` with `Task.Run` instead.
        ```CSharp
        Task.Run(() =>
        {
            return userInfoRetriever.Invoke();
        }).ContinueWith(result =>
        {
            var task = result.ConfigureAwait(false);
            CurrentTrader = task.GetAwaiter().GetResult();
        }, TaskScheduler.Default);
        ```
1. Run the .NET Core Bean Trader app and confirm that it is working as expected.
