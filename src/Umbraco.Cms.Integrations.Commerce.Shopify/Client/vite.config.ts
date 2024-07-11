import { defineConfig } from "vite";
import tsconfigPaths from "vite-tsconfig-paths";

export default defineConfig({
    build: {
        lib: {
            entry: "src/index.ts",
            formats: ["es"],
        },
        outDir: "../wwwroot/App_Plugins/Shopify",
        emptyOutDir: true,
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco-cms/],
            onwarn: () => { },
        },
    },
    plugins: [tsconfigPaths()],
});