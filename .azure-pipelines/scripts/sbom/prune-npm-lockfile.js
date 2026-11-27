#!/usr/bin/env node
// Writes a standalone package-lock.json holding only what one npm workspace ships.
//
// The repo has a single root lockfile shared by every workspace, but each product only
// bundles its own frontend's runtime dependencies. umbraco-sbom scans a lockfile, so this
// builds one per workspace: starting from the workspace's `dependencies` (dev dependencies
// are never followed - they build the bundle but aren't in it), it walks the lockfile using
// Node's resolution rules and keeps every package reached. Links to other workspaces are
// dropped: those are first-party packages loaded at runtime from their own product, so their
// dependencies belong to that product's SBOM.
//
// Usage: node prune-npm-lockfile.js <root-lockfile> <workspace-path> <output-dir>

const fs = require("fs");
const path = require("path");

const [lockfilePath, workspacePath, outputDir] = process.argv.slice(2);
if (!lockfilePath || !workspacePath || !outputDir) {
    console.error("Usage: node prune-npm-lockfile.js <root-lockfile> <workspace-path> <output-dir>");
    process.exit(1);
}

const lock = JSON.parse(fs.readFileSync(lockfilePath, "utf8"));
if (!lock.packages || lock.lockfileVersion < 2) {
    console.error(`${lockfilePath} must be lockfileVersion 2 or 3`);
    process.exit(1);
}

const packages = lock.packages;
const workspaceKey = workspacePath.replace(/\\/g, "/").replace(/\/+$/, "");
const workspace = packages[workspaceKey];
if (!workspace) {
    console.error(`Workspace '${workspaceKey}' not found in ${lockfilePath}`);
    process.exit(1);
}

// Every workspace directory, so links pointing at one can be recognised and skipped.
const workspaceKeys = new Set(
    Object.entries(packages)
        .filter(([key, entry]) => key !== "" && !key.includes("node_modules/") && !entry.link)
        .map(([key]) => key),
);

// Node resolution: look in <dir>/node_modules/<name>, then each parent dir, up to the root.
function resolve(fromKey, name) {
    let dir = fromKey;
    while (true) {
        const candidate = dir ? `${dir}/node_modules/${name}` : `node_modules/${name}`;
        if (packages[candidate]) return candidate;
        if (!dir) return null;
        const slash = dir.lastIndexOf("/");
        dir = slash === -1 ? "" : dir.slice(0, slash);
    }
}

function runtimeDependencyNames(entry) {
    return [
        ...Object.keys(entry.dependencies ?? {}),
        ...Object.keys(entry.optionalDependencies ?? {}),
        ...Object.keys(entry.peerDependencies ?? {}),
    ];
}

const kept = {};
const queue = [workspaceKey];
const visited = new Set();

while (queue.length > 0) {
    const fromKey = queue.shift();
    if (visited.has(fromKey)) continue;
    visited.add(fromKey);

    for (const name of runtimeDependencyNames(packages[fromKey])) {
        const key = resolve(fromKey, name);
        if (!key) continue; // optional or peer dependency that npm didn't install

        let entry = packages[key];
        let packageKey = key;
        if (entry.link) {
            if (workspaceKeys.has(entry.resolved)) continue;
            packageKey = entry.resolved;
            entry = packages[packageKey];
            if (!entry) continue;
        }

        // Reached through runtime dependencies, so it ships: clear any dev marking.
        const { dev, devOptional, ...rest } = entry;
        kept[key] = rest;
        queue.push(packageKey);
    }
}

const pruned = {
    name: workspace.name,
    version: workspace.version,
    lockfileVersion: 3,
    requires: true,
    packages: {
        "": {
            name: workspace.name,
            version: workspace.version,
            license: workspace.license,
            dependencies: workspace.dependencies ?? {},
        },
        ...kept,
    },
};

fs.mkdirSync(outputDir, { recursive: true });
fs.writeFileSync(path.join(outputDir, "package-lock.json"), JSON.stringify(pruned, null, 2));
fs.writeFileSync(
    path.join(outputDir, "package.json"),
    JSON.stringify(
        { name: workspace.name, version: workspace.version, dependencies: workspace.dependencies ?? {} },
        null,
        2,
    ),
);

console.log(`${workspaceKey}: kept ${Object.keys(kept).length} runtime package(s)`);
