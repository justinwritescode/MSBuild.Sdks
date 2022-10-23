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
