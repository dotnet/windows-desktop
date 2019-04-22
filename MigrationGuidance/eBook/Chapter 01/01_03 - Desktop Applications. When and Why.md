# Modernizing desktop applications

## When to modernize a desktop application 

We understand that not all desktop applications would benefit from application modernization. There are precise steps which must be taken to modernize an app, and this eBook provides in-depth guidance  to ensure a successful application migration. The e-book also highlights the challenges associated with modernizing an app and, solutions to solve those challenges.

The modernization of desktop applications should be understood as obtaining the improvements inherent to the .NET Core (Open Source code, Performance, etc.). These improvements should have enough weight to justify modernization. Here are the pointers which you must consider when planning for application modernization:

* **User interface overloaded with components**:  
Desktop application development and design practices are very diverse in comparison to web application development and design practices. Web applications for years have been designed keeping simplicity and usability as the primary criteria for developing the user interface. This usability in web applications was initially necessary due to the limitations of the browsers, and later was the result of specific usability analysis. In contrast, desktop applications did not follow the simplified user interface design practices in most cases. Desktop applications have been abusing the power of running within a PC environment, and in many cases the developers did not take into account aspects of performance or overload of controls in one Windows Form. This is because with the power of Windows running the application, almost everything was possible. Forms with hundred of lines of code, control events, grids with editable cells, controls from different vendors, and others were usual.

<< Include an image of Windows App with a Form with a complex grid, buttons, … >>

* **Access to resources and devices**: Desktop applications have access to local sources on the machine such as connected devices, and other machine resources. Hence many applications developed by ISV's (Independent Software Vendors) can operate with devices from an extensive list of vendors. It is crucial to verify that all those access capabilities to external and internal devices are available and working in .NET Core.

<< Include image of Windows App using external device COM/USB barcode scanner >>

* **Access to services**: Until the consolidation of REST services, the primary option for communication with data services for desktop applications was the use of WCF (Windows Communication Foundation). WCF continues to be supported by the .NET Framework, and in .NET Core, we can generate the Client Proxy to connect to existing WCF services. WCF Server is not a functionality of .NET Core. Hence this technology, which is generally accessed by desktop applications cannot be upgraded to a WCF version on .NET Core. Client proxy generation is limited to the cases specified in (https://github.com/dotnet/wcf). In the case of an application using WCF services, we must validate if it is possible to generate the client proxy. In case the generation of the proxy is not feasible, migrating to REST  services must be considered. 

* **Code previously migrated from VB to .NET Framework**: If your desktop application is developed utilizing previous Visual Basic migration to C#/WinForm code; using a code conversion tool, application modernization to .NET Core is not a recommended choice.
