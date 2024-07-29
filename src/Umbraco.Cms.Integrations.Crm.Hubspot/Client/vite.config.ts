import { defineConfig } from "vite";
import tsconfigPaths from "vite-tsconfig-paths";
import { outputPath } from "./config.outputPath.js";

export default defineConfig({
    build: {
        lib: {
            entry: "src/index.ts",
            formats: ["es"],
        },
        outDir: outputPath,
        emptyOutDir: true,
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco-cms/],
            onwarn: () => { },
        },
    },
    plugins: [tsconfigPaths()],
});
