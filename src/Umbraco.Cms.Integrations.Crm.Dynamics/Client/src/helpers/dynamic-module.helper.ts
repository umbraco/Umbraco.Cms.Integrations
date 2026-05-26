export const DynamicsModule = {
    OUTBOUND: 1,
    REAL_TIME: 2,
    BOTH: 3,
} as const;

export function parseModule(module: string) {
    switch (module) {
        case "1": return DynamicsModule.OUTBOUND;
        case "2": return DynamicsModule.REAL_TIME;
    }
}
