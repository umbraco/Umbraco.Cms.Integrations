import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";

import { AlgoliaIndexDataSource } from './algolia-index.data-source.js';
import type { IndexConfigurationModel } from "@umbraco-integrations/algolia/generated";

export class AlgoliaIndexRepository extends UmbControllerBase {

    #algoliaIndexDataSource: AlgoliaIndexDataSource;

    constructor(host: UmbControllerHost) {
        super(host);

        this.#algoliaIndexDataSource = new AlgoliaIndexDataSource(this);
    }

    async getIndices() {
        return this.#algoliaIndexDataSource.getIndices();
    }

    async getIndexById(id: number) {
        return this.#algoliaIndexDataSource.getIndexById(id);
    }

    async getContentTypes() {
        return this.#algoliaIndexDataSource.getContentTypes();
    }

    async getContentTypesWithIndex(id: number) {
        return this.#algoliaIndexDataSource.getContentTypesWithIndex(id);
    }

    async saveIndex(indexConfiguration: IndexConfigurationModel) {
        return this.#algoliaIndexDataSource.saveIndex(indexConfiguration);
    }

    async buildIndex(indexConfiguration: IndexConfigurationModel) {
        return this.#algoliaIndexDataSource.buildIndex(indexConfiguration);
    }

    async deleteIndex(id: number) {
        return this.#algoliaIndexDataSource.deleteIndex(id);
    }

    async searchIndex(id: number, query: string) {
        return this.#algoliaIndexDataSource.searchIndex(id, query);
    }

}
