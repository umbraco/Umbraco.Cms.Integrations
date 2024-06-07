import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";

import { AlgoliaIndexDataSource } from "./algolia-index.data-source";
import { AlgoliaIndexConfigurationModel } from "../models/AlgoliaIndexConfigurationModel";

export class AlgoliaIndexRepository extends UmbControllerBase {

    #algoliaIndexDataSource: AlgoliaIndexDataSource;

    constructor(host: UmbControllerHost) {
        super(host);

        this.#algoliaIndexDataSource = new AlgoliaIndexDataSource(this);
    }

    async getIndices() {
        return this.#algoliaIndexDataSource.getIndices();
    }

    async getIndexById(id: Number) {
        return this.#algoliaIndexDataSource.getIndexById(id);
    }

    async getContentTypes() {
        return this.#algoliaIndexDataSource.getContentTypes();
    }

    async getContentTypesWithIndex(id: Number) {
        return this.#algoliaIndexDataSource.getContentTypesWithIndex(id);
    }

    async saveIndex(indexConfiguration: AlgoliaIndexConfigurationModel) {
        return this.#algoliaIndexDataSource.saveIndex(indexConfiguration);
    }

    async buildIndex(indexConfiguration: AlgoliaIndexConfigurationModel) {
        return this.#algoliaIndexDataSource.buildIndex(indexConfiguration);
    }

    async deleteIndex(id: Number) {
        return this.#algoliaIndexDataSource.deleteIndex(id);
    }

    async searchIndex(id: Number, query: string) {
        return this.#algoliaIndexDataSource.searchIndex(id, query);
    }

}
