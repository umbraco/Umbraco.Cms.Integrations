import { defineConfig } from 'vite';

export default defineConfig({
    build: {
        lib: {
            entry: "src/hubspot-property-editor-ui.element.ts",
            formats: ["es"],
        },
        outDir: "dist",
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/],
        },

    },
});
