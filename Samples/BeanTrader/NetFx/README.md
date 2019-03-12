# Bean Trader

## About the App

This sample application is used to demonstrate migrating a WPF app to .NET Core. It is a simple client for an Azure-hosted service which allows users to propose and complete trades for different colors of virtual beans. To keep the sample simple, there is no authentication (as that would require user management). Instead, users may log in by simply providing a user name and users who don't yet exist will be created automatically.

This app is not meant to demonstrate WPF best practices (in fact, some of the code is intentionally not optimal to demonstrate migration challenges). Instead, the sample is meant to illustrate common migration challenges.

## Directory Contents

The BeanTrader solution is comprised of three projects:

* BeanTraderServer is the backend WCF service (console app) that keeps track of outstanding trade offers and user bean counts.
* BeanTraderInterfaces contains the service interfaces that BeanTraderServer implements.
* BeanTraderClient is the front-end WPF application that enables users to interact with the backend service. Although it obviously depends on the backend service to be available at runtime, this project has no build-time dependency on either of the other services. If you are only interested in seeing (and maybe porting) the WPF app, this is the only portion of the sample you need to concern yourself with.

## Building and Running the Sample

Building the sample is as easy as using MSBuild to compile the solution (or project) and running the sample only requires launching the generated app (though the app's config file may need updated to point at a valid backend server). To make testing the WPF app easier, an instance of the BeanTrader backend service is running in Azure at net.tcp://beantrader.eastus.cloudapp.azure.com:8090/BeanTraderService, though this may not always be available in the future.
