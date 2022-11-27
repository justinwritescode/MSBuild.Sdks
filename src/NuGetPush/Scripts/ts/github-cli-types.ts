// 
// github-cli-types.ts
// 
//   Created: 2022-10-29-08:00:28
//   Modified: 2022-11-01-06:04:54
// 
//   Author: Justin Chase <justin@justinwritescode.com>
//   
//   Copyright © 2022 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
// 

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
