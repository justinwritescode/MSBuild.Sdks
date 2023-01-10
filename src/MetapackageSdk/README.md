---
author: Justin Chase
author_email: justin@justinwritescode.com
title: README.md
modified: 2023-01-09-03:26:54
created: 2023-01-09-03:26:53
license: MIT
---

# Metapackage SDK

This SDK provides a way to create a NuGet package that contains only references other packages.  This is useful if you want to create a package that can be used as a dependency, but doesn't actually contain any files.

## Usage

Simply create a project file with the `PackageReference` items you want to include and target the `MetapackageSdk` as shown below.  Then, package the project as a NuGet package and reference it in your other projects.

### Example

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <Sdk Name="MetapackageSdk" />
    <PropertyGroup>
        <TargetFramework>net7.0</TargetFramework>
    </PropertyGroup>
    <ItemGroup>
        <ProjectReference Include="Project.Reference.One" />
        <ProjectReference Include="Project.Reference.Two" />
        <ProjectReference Include="Project.Reference.Three" />
    </ItemGroup>
</Project>
```
