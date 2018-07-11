# dotnet-opencover

A simple wrapper nuget package to call opencover using the dotnet cli "dotnet opencover"

_This does not bundle in open cover itself. You need to include that as a dependency._

Currently assumes OpenCover is located in `%userprofile%/.nuget/packages/opencover/<versionspec>/tools/OpenCover.Console.exe`

## Usage

`dotnet opencover --help`

## Reports

If you have the ReportGenerator and OpenCoverToCoberturaConverter packages installed, it can run these as well once test are done

## Tip

In .net core 2.1 onwards, you can run this as part of dotnet watch

`dotnet watch opencover`

This will re-run your tests and update the coverage reports on each file change
