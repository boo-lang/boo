# Boo.Sdk

An MSBuild SDK for [Boo](https://github.com/boo-lang/boo) projects. It brings in
the .NET SDK, the Boo build targets, and a `booc` to compile with, for the benefit
of `dotnet build`, Visual Studio, or other IDEs.

```xml
<Project Sdk="Boo.Sdk/0.9.7">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

</Project>
```

Every `.boo` file under the project directory is compiled. `Boo.Lang` and
`Boo.Lang.Extensions` are referenced for you. `Boo.Lang.Useful` and
`Boo.Lang.PatternMatching` ship in the package too, under `$(BooToolsDirectory)`,
and can be referenced explicitly:

```xml
<ItemGroup>
  <Reference Include="Boo.Lang.Useful" HintPath="$(BooToolsDirectory)Boo.Lang.Useful.dll" />
</ItemGroup>
```
