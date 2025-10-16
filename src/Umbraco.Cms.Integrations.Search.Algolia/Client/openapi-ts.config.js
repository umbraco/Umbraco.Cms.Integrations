import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
    logs: {
        level: 'debug',
    },
    input: 'http://localhost:28157/umbraco/swagger/algolia-search-management/swagger.json',
    output: {
        path: 'generated',
    },
    plugins: [
        {
            name: '@hey-api/client-fetch',
            bundle: false,
            exportFromIndex: true,
            throwOnError: true,
        },
        {
            name: '@hey-api/typescript',
            enums: 'typescript',
        },
        {
            name: '@hey-api/sdk',
            asClass: true,
        },
    ],
});


