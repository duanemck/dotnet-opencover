# dotnet-testx

Extensions to the `dotnet test` command that enable code coverage reporting using [OpenCover](https://github.com/OpenCover/opencover) and a few other bonus features

NOTE: Due to OpenCover being Windows only, this tool will only work on Windows

## Features

1.  Run `dotnet test` with code coverage tracking by OpenCover
1.  Generate cobertura covberage results (useful for VSTS/TFS coverage reporting). Uses [OpenCovertToCobeturaConverter](https://www.nuget.org/packages/OpenCoverToCoberturaConverter/)
1.  Generate an HTML coverage report using [ReportGenerator](https://www.nuget.org/packages/ReportGenerator)
1.  Discover all test projects in a folder and run tests for all of them (not natively supported by `dotnet test`)

_The coverage and reporting tools are not bundles into this tool. They need to be installed as dependencies on the project that you're running this on._

Currently assumes the dependencies are located in `%userprofile%/.nuget/packages/opencover/<versionspec>/tools/OpenCover.Console.exe`
`%userprofile%/.nuget/packages/reportgenerator/<versionspec>/tools/ReportGenerator.exe`
`%userprofile%/.nuget/packages/opencovertocobeturaconverter/<versionspec>/tools/OpenCoverToCoberturaConverter.exe`

## Installation

The tool is installed as a global dotnet tool (.net Core 2.1+)

`dotnet tool install -g dotnet-testx`

Nuget package: https://www.nuget.org/packages/dotnet-testx

## Usage

`dotnet testx --help`

```text
  --discover-projects         Discover all files in the working directory
                              matching the pattern and run tests on them. Alias
                              for '--project all'

  --project                   Project to test. If you specify 'all' then it
                              will find all projects in the folder and
                              subfolders matching '*Tests.csproj'

  --opencover-version         Optional OpenCover version.

  --html                      (Default: false) Generate an HTML report

  --browser                   (Default: false) If generating an HTML report,
                              open in the default browser

  --cobertura                 (Default: false) Generate a cobertura report

  --test-results-format       (Default: trx) The format to use for the VSTest
                              test results

  --opencover-filters         (Default: +[*]*) Filters for opencover, i.e.
                              files to include/exclude from report

  --opencover-mergeresults    (Default: true) Merge multiple runs into the same
                              results file

  --opencover-options         (Default: ) Any other options you'd like passed into OpenCover

  --verbose                   (Default: false) Log more verbose details of
                              whats happening

  --help                      Display this help screen.

  --version                   Display version information.
```

## Tips

In .net core 2.1 onwards, you can run this as part of dotnet watch

`dotnet watch` requires its own project argument so it knows which files to watch

Use -- to separate `dotnet watch` arguments from `dotnet testx` arguments

`dotnet watch --project [project-file] testx -- [dotnet testx arguments]`

This will re-run your tests and update the coverage reports on each file change. You can then keep a browser window open and just refresh the coverage report to ensure your changes are improving coverage.
