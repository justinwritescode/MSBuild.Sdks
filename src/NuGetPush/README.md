---
author: Justin Chase
author_email: justin@justinwritescode.com
title: README.md
modified: 2022-11-27-05:00:50
created: 2022-11-27-05:00:49
license: MIT
---

# NuGet Push

This is a simple MSBuild task that allows you to push a NuGet package to a NuGet server.

## Usage

```shellscript "Pushing to Azure Artifacts"
dotnet pack MyProject.csproj --target:PushAzure
```

```shellscript "Pushing to GitHub Packages"
dotnet pack MyProject.csproj --target:PushGitHub
```
