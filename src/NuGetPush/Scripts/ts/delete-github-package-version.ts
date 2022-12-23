/* 
 * delete-github-package-version.ts
 * 
 *   Created: 2022-11-27-05:39:27
 *   Modified: 2022-12-05-04:15:02
 * 
 *   Author: Justin Chase <justin@justinwritescode.com>
 *   
 *   Copyright © 2022 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 

import process from 'process';
import {deletePackageVersionAsync} from "./github-cli";
// import {Octokit} from "@octokit/rest";
// import {RequestError} from "@octokit/types";

var packageId = process.argv.slice(2)[0];
var version = process.argv.slice(2)[1];
    
(async () => {
    await deletePackageVersionAsync(packageId, version);
})();
