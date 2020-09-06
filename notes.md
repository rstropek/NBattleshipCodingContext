# Notes

## Required Setup

* Latest (preview) version of [.NET 5](https://dot.net)
* Visual Studio Code
* [C# Plugin](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
* [EditorConfig Plugin](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig)

## Create Project

```bash
dotnet new sln --name NBattleshipCodingContest
dotnet new console --framework net5.0 --langVersion latest --name NBattleshipCodingContest
dotnet new classlib --framework net5.0 --langVersion latest --name NBattleshipCodingContest.Logic
dotnet new xunit --framework net5.0 --name NBattleshipCodingContest.Logic.Tests
dotnet sln NBattleshipCodingContest.sln add NBattleshipCodingContest
dotnet sln NBattleshipCodingContest.sln add NBattleshipCodingContest.Logic

dotnet add NBattleshipCodingContest/NBattleshipCodingContest.csproj reference NBattleshipCodingContest.Logic/NBattleshipCodingContest.Logic.csproj
dotnet add NBattleshipCodingContest.Logic.Tests/NBattleshipCodingContest.Logic.Tests.csproj reference NBattleshipCodingContest.Logic/NBattleshipCodingContest.Logic.csproj
dotnet sln NBattleshipCodingContest.sln add NBattleshipCodingContest.Logic.Tests
```

* Run `dotnet list package --outdated` in test project and update all outdated references `dotnet add package ...`
* Add [*Moq*](https://github.com/moq/moq4) to test project: `dotnet add package Moq`
* Create [*.editorconfig* file](https://editorconfig.org/) in root of solution and copy [default config](https://docs.microsoft.com/en-us/visualstudio/ide/editorconfig-code-style-settings-reference?view=vs-2019#example-editorconfig-file) from Microsoft docs

## Settings

* Set `<LangVersion>preview</LangVersion>` in test project ([docs](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version))
* Enable [nullable references](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references) in all projects: `<Nullable>enable</Nullable>`
* Set `<AnalysisLevel>preview</AnalysisLevel>` in all projects ([docs](https://devblogs.microsoft.com/dotnet/automatically-find-latent-bugs-in-your-code-with-net-5/))
