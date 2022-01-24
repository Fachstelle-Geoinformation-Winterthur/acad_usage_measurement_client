# Usage Measurement Client for AutoCAD
This is a .NET plug-in for AutoCAD that makes it possible for admins to centrally measure the usage times of AutoCAD installations in a corporate network. The storage of the usage data is done anonymously, since it only stores usage data of groups in the database, not of individuals.

## How to operate
This plug-in only works in tandem with a web service that is also provided as open-source in another repository, which is:

https://github.com/Fachstelle-Geoinformation-Winterthur/acad_usage_measurement_service

You need to install and run the web service first, in order to operate this plug-in.

## How to install
The installation of this plug-in is quite simple. You can place the plug-in DLL (acad_usage_measurement_client.dll) into the respective plug-in folder. For example, in AutoCAD 2021 this is the following location:

> C:\Program Files\Autodesk\AutoCAD 2021\Plugins\acad_usage_measurement_client\acad_usage_measurement_client.dll

Additionally, you need to add a properties text file. It must have the following name: "acad_usage_measurement_properties.txt" and needs to be placed in the same folder as the DLL, like so:

> C:\Program Files\Autodesk\AutoCAD 2021\Plugins\acad_usage_measurement_client\acad_usage_measurement_properties.txt

The properties file has to contain information on the URL of the web service in the following form:

> server=https://your_server.com/path_to_the_web_service

## Usable versions of AutoCAD
This plug-in has originally been compiled for and tested with AutoCAD 2017, but it should also work out-of-the-box with all subsequent versions of AutoCAD.

## Additional system requirements
.NET Framework version 4.5 (at least) is required (and of course the Windows version that runs it).
