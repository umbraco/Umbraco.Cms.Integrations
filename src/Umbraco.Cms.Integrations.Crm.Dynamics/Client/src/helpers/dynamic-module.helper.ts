import { DynamicsModuleModel } from "@umbraco-integrations/dynamics/generated";

export function parseModule(module: string) {
    switch (module) {
        case "1": return DynamicsModuleModel.OUTBOUND;
        case "2": return DynamicsModuleModel.REAL_TIME;
    }
}