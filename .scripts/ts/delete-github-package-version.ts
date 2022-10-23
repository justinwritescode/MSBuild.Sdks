#!/usr/bin/env ts-node

//
// delete-github-package-version.ts
// 
// This script deletes a package version from GitHub Packages.
// It's needed because the ```dotnet nuget delete``` command doesn't work with GitHub Packages.
// It calls the GitHub API to delete the package version.
//
//

import process from 'process';
import * as fs from "fs";
import path from "path";
import {execSync} from "child_process";
import {deletePackageVersionAsync} from "./github-cli";
import {PackageVersion} from "./github-cli-types";
// import {Octokit} from "@octokit/rest";
// import {RequestError} from "@octokit/types";

var packageId = process.argv.slice(2)[0];
var version = process.argv.slice(2)[1];
    
console.log(`Deleting package version ${version} for package ${packageId}...`);

(async () => {
    await deletePackageVersionAsync(packageId, version);
})();
