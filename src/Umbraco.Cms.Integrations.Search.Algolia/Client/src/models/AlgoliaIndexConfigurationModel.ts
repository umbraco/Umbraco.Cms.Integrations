import { AlgoliaContentTypeModel } from "./AlgoliaContentTypeModel";

export class AlgoliaIndexConfigurationModel {
    public id?: number;
    public name: string;
    public contentData: Array<AlgoliaContentTypeModel> = [];

    constructor(name: string) {
        this.name = name;
    }
}