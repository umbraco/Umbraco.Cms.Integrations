import type { AlgoliaContentTypePropertyModel } from "./AlgoliaContentTypePropertyModel";

export type AlgoliaContentTypeModel = {
    id: number;
    icon: string;
    alias: string;
    name: string;
    selected: boolean;
    allowRemove: boolean;
    properties: Array<AlgoliaContentTypePropertyModel>
}