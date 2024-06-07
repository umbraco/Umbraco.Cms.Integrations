import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { AlgoliaIndexRepository } from "../repository/algolia-index.repository";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { AlgoliaIndexConfigurationModel } from "../models/AlgoliaIndexConfigurationModel";

export class AlgoliaIndexContext extends UmbControllerBase {

    #repository: AlgoliaIndexRepository;

    constructor(host: UmbControllerHost) {
        super(host);

        this.provideContext(ALGOLIA_CONTEXT_TOKEN, this);
        this.#repository = new AlgoliaIndexRepository(host);
    }

    async getIndices() {
        return await this.#repository.getIndices();
    }

    async getIndexById(id: Number) {
        return this.#repository.getIndexById(id);
    }

    async getContentTypes() {
        return await this.#repository.getContentTypes();
    }

    async getContentTypesWithIndex(id: Number) {
        return this.#repository.getContentTypesWithIndex(id);
    }

    async saveIndex(indexConfiguration: AlgoliaIndexConfigurationModel) {
        return await this.#repository.saveIndex(indexConfiguration);
    }

    async buildIndex(indexConfiguration: AlgoliaIndexConfigurationModel) {
        return this.#repository.buildIndex(indexConfiguration);
    }

    async deleteIndex(id: number) {
        return await this.#repository.deleteIndex(id);
    }

    async searchIndex(id: Number, query: string) {
        return this.#repository.searchIndex(id, query);
    }
}

export default AlgoliaIndexContext;

export const ALGOLIA_CONTEXT_TOKEN =
    new UmbContextToken<AlgoliaIndexContext>(AlgoliaIndexContext.name);
