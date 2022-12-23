/* 
 * github-cli.ts
 * 
 *   Created: 2022-11-27-05:39:27
 *   Modified: 2022-12-05-04:14:53
 * 
 *   Author: Justin Chase <justin@justinwritescode.com>
 *   
 *   Copyright © 2022 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 

import {execSync} from "child_process";
import * as fs from "fs";
import * as process from "process";
import {PackageVersion, ApiMessage} from "./github-cli-types";

export async function deletePackageVersionAsync(packageId: string, version: string) : Promise<void> {
    return new Promise<void>(async (resolve, reject) => {
        var versionsResult:string|undefined|null|PackageVersion[]|ApiMessage = undefined;
        try {
            execSync(`curl -H "Authorization: Bearer ${process.env.GIT_TOKEN}" -H "Accept: application/vnd.github+json" "https://api.github.com/user/packages/nuget/${packageId}/versions" 1> ${packageId}.versions.json 2> /dev/null`, {encoding: 'utf8', stdio: 'pipe'});
            versionsResult = fs.readFileSync(` ${packageId}.versions.json`, 'utf8');
            versionsResult = JSON.parse(versionsResult) as  PackageVersion[]|ApiMessage;
        }
        catch(ex) {
            // console.log(ex);
            //reject(ex);
            resolve();
        }

        versionsResult = JSON.parse(fs.readFileSync(`${packageId}.versions.json`, 'utf8')) as PackageVersion[]|ApiMessage;
        if(versionsResult instanceof Array)
        {
            const versionId = versionsResult.find((v: any) => v.name === version)?.id;

            if (versionId && versionId != undefined) {
                try {
                    var jsonOutput = execSync(`curl -X DELETE -H "Authorization: Bearer ${process.env.GIT_TOKEN}" -H "Accept: application/vnd.github+json" "https://api.github.com/user/packages/nuget/${packageId}/versions/${versionId}" 1> ${packageId}.delete-result.json 2> /dev/null`, {encoding: 'utf8', stdio: 'pipe'});
                    try { 
                        const deleteVersionResult = JSON.parse(jsonOutput) as ApiMessage|null;
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
        else if(versionsResult.message.includes("not found")) //"Not Found" || versionsResult.message == "Package not found.")
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
        var deletePackageResultJsonString = "";
        console.log(`Deleting package ${packageId}...`);
        try {
            deletePackageResultJsonString = execSync(`curl -X DELETE -H "Authorization: Bearer ${process.env.GIT_TOKEN}" -H "Accept: application/vnd.github+json" "https://api.github.com/user/packages/nuget/${packageId}" &> ${packageId}.delete-package.result.json`, {encoding: 'utf8'});
            console.log("The package was deleted successfully.")
        }
        catch(ex) {
            resolve();
        }
        var deletePackageResult = JSON.parse(deletePackageResultJsonString) as ApiMessage;
        if(deletePackageResult.message.includes("not found")) //"Not Found" || versionsResult.message == "Package not found.")
        {
            console.log(`Package ${packageId} not found.  Skipping...`);
        }
        if(fs.existsSync(`${packageId}.delete-package.result.json`))
            fs.unlinkSync(`${packageId}.delete-package.result.json`);
        resolve();
    });
}
