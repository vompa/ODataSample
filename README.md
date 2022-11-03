# ODataSample

<br/>

## OData Server and Connected Service Client Examples

### Folder Structure ###
| Folder | Description |
| :------ | :------ |
| data | database json files |
| doc  | documentation & Files |
| shell | startup shell command files  |
| src | projects |

<br/>

## Projects Structure ###
| Project | Description |
| :------ | :------ |
| OData.Sample.WebApi | NET6 Web Server |
| OData.Sample.Client| Console ODAta Query App ( Connected Service ) |

<br/>

## Tech
Projects uses a number of open source projects to work properly:

* [NET6] - NET6
* [EFCore] - Entity Framework Core (O/RM) + Microsoft.Data.Sqlite Provider
* [SQLite] - SQLite Relational Database
* [Serilog] - Logging Framework
* [AutoMapper] - Object Mapper with Profiles and Dependency Injection
* [Polly] - Retry policies helper ( used for database migrations )
* [Swashbuckle] - [swagger] tooling for API's built with ASP.NET Core

<br/>

## Installation

### Requirements

* [NET6] - latest 6.x SDK
* [VS2022] - Visual Studio 2022 (optional [VSCode])
* [VSOdataExt] - OData Connected Service 2022+ Extension

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job. There is no need to format nicely because it shouldn't be seen. Thanks SO - http://stackoverflow.com/questions/4823468/store-comments-in-markdown-syntax)

   [NET6]: <https://dotnet.microsoft.com/en-us/download/dotnet/6.0>
   [EFCore]: <https://github.com/dotnet/efcore/>
   [Serilog]: <https://serilog.net/>
   [AutoMapper]: <http://automapper.org/>
   [Swashbuckle]: <https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/README.md>
   [Swagger]: <https://swagger.io/>
   [Newtonsoft.Json]: <https://www.newtonsoft.com/json>
   [SQLite]: https://www.sqlite.org/
   [Polly]: https://github.com/App-vNext/Polly
   [VS2022]: https://visualstudio.microsoft.com/
   [VSCode]: https://code.visualstudio.com/
   [VSOdataExt]: https://marketplace.visualstudio.com/items?itemName=marketplace.ODataConnectedService2022