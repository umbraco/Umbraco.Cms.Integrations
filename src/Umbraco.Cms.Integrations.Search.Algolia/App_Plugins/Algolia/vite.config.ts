import { defineConfig } from "vite";

export default defineConfig({
    build: {
        lib: {
            entry: "src/algolia-dashboard.element.ts",
            formats: ["es"],
        },
        outDir: "dist",
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/],
        },
    },
});