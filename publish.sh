dotnet build -c Release
dotnet nuget push duanemckdev.dotnet.tools.testx/bin/Release/dotnet-testx.$1.nupkg -s https://api.nuget.org/v3/index.json -k $2
