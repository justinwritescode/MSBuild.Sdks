// 
// github-cli.ts
// 
//   Created: 2022-10-29-08:02:03
//   Modified: 2022-11-01-06:05:02
// 
//   Author: Justin Chase <justin@justinwritescode.com>
//   
//   Copyright © 2022 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
// 

import {execSync} from "child_process";
import * as fs from "fs";
import * as process from "process";
import {PackageVersion, ApiMessage} from "./github-cli-types";

export async function deletePackageVersionAsync(packageId: string, version: string) : Promise<void> {
    return new Promise<void>(async (resolve, reject) => {
        try {
            execSync(`curl -H "Authorization: Bearer ${process.env.GIT_TOKEN}" -H "Accept: application/vnd.github+json" "https://api.github.com/user/packages/nuget/${packageId}/versions" 1> ${packageId}.versions.json 2> /dev/null`, {encoding: 'utf8'});
        }
        catch(ex) {
            // console.log(ex);
            //reject(ex);
            resolve();
        }

        const versionsResult = JSON.parse(fs.readFileSync(`${packageId}.versions.json`, 'utf8')) as PackageVersion[]|ApiMessage;
        if(versionsResult instanceof Array)
        {
            const versionId = versionsResult.find((v: any) => v.name === version)?.id;

            if (versionId && versionId != undefined) {
                try {
                    execSync(`curl -X DELETE -H "Authorization: Bearer ${process.env.GIT_TOKEN}" -H "Accept: application/vnd.github+json" "https://api.github.com/user/packages/nuget/${packageId}/versions/${versionId}" 1> ${packageId}.delete-result.json 2> /dev/null`, {encoding: 'utf8'});
                    try { 
                        const deleteVersionResult = JSON.parse(fs.readFileSync(`${packageId}.delete-result.json`, 'utf8')) as ApiMessage|null;
                        if(deleteVersionResult?.message == "You cannot delete the last version of a package. You must delete the package instead.") {
                            await deletePackageAsync(packageId);
                        }
                        console.log(`deleteVersionResult: ${deleteVersionResult}: deleteVersionResult`);
                    }
                    catch(ex){
                        // console.log(ex);
                        // reject(ex);
                        resolve();
                    }
                    resolve();
                }
                catch(ex) {
                    // console.log(ex);
                    resolve();
                }
            }
        }
        else if(versionsResult.message == "Not Found" || versionsResult.message == "Package not found.")
        {
            console.log(`Package ${packageId} with version number ${version} not found.  Skipping...`);
            resolve();
        }
        if(fs.existsSync(`${packageId}.versions.json`))
            fs.unlinkSync(`${packageId}.versions.json`);
        if(fs.existsSync(`${packageId}.delete-result.json`))
            fs.unlinkSync(`${packageId}.delete-result.json`);
    })
}

export function deletePackageAsync(packageId: string) : Promise<void> {
    return new Promise<void>((resolve, reject) => {
        console.log(`Deleting package ${packageId}...`);
        try {
            execSync(`curl -X DELETE -H "Authorization: Bearer ${process.env.GIT_TOKEN}" -H "Accept: application/vnd.github+json" "https://api.github.com/user/packages/nuget/${packageId}" &> ${packageId}.delete-package.result.json`, {encoding: 'utf8'});
        }
        catch(ex) {
            resolve();
        }
        var deletePackageResult = JSON.parse(fs.readFileSync(`${packageId}.delete-package.result.json`, 'utf8')) as ApiMessage;
        if(deletePackageResult.message == "Not Found" || deletePackageResult.message == "Package not found.")
        {
            console.log(`Package ${packageId} not found.  Skipping...`);
        }
        if(fs.existsSync(`${packageId}.delete-package.result.json`))
            fs.unlinkSync(`${packageId}.delete-package.result.json`);
        resolve();
    });
}
