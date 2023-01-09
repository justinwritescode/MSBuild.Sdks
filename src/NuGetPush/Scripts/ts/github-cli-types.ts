/* 
 * github-cli-types.ts
 * 
 *   Created: 2022-11-27-05:39:27
 *   Modified: 2022-12-05-04:14:47
 * 
 *   Author: Justin Chase <justin@justinwritescode.com>
 *   
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 

export interface PackageVersion {
    id: number;
    name: string;
    package_html_url: string;
    url: string;
    created_at: string;
    updated_at: string;
    visibility: string;
    package_type: string;
    downloads_count: number;
    description: string;
    html_url: string;
    license: string;
}

export interface ApiMessage {
    message: string;
    documentation_url: string;
}
