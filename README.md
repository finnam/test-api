# TEST API 
## API
As there is only one end point and it takes no parmeters, the API is implemented as an Azure Serverless function.  It ie easily extensible by adding query parmeters to the function, or by additional functions to the Azure Function App.  

It is called as follows:

```
https://amf-test-api.azurewebsites.net/api/GetLondonPeople
```


 * Updates to documentation - installation guide and developers guide

## Developer Notes
The function is written in C# from .NET Core 2.  
It was developed in Visual Studio Code (VS) with the following extensions:  

 * C#
 * Azure Functions
 * NuGet Package Manager
 * .NET Core Test Explorer

The code can be run locally within VS from the Debug menu.  

There's a test project in the repo that contains some unit tests for the API can either be run from the terminal in VS using the following command:
```
dotnet test
```
Or interactivly with the .NET Core Test Explorer