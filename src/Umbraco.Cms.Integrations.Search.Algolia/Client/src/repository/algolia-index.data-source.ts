import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { AlgoliaSearchService } from "../api/services";
import { IndexConfigurationModel } from "../api/models";
/*import { AlgoliaIndexConfigurationModel } from "../models/AlgoliaIndexConfigurationModel";*/

export class AlgoliaIndexDataSource {

    #host: UmbControllerHost;

    constructor(host: UmbControllerHost) {
        this.#host = host;
    }

    async getIndices() {

        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaSearchService.getIndices());

        if (data) {
            return data;
        }

        return { error };
    }

    async getIndexById(id: number) {

        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaSearchService.getIndexById({
            id: id
        }));

        if (data) {
            return data;
        }

        return { error };
    }

    async getContentTypes() {

        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaSearchService.getContentTypes());

        if (data) {
            return data;
        }

        return { error };
    }

    async getContentTypesWithIndex(id: number) {
        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaSearchService.getContentTypesByIndexId({
            id: id
        }));

        if (data) {
            return data;
        }

        return { error };
    }

    async saveIndex(indexConfiguration: IndexConfigurationModel) {
        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaSearchService.saveIndex({
            requestBody: indexConfiguration
        }));

        if (data) {
            return data;
        }

        return { error };
    }

    async buildIndex(indexConfiguration: IndexConfigurationModel) {
        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaSearchService.buildIndex({
            requestBody: indexConfiguration
        }));

        if (data) {
            return data;
        }

        return { error };
    }

    async deleteIndex(id: number) {
        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaSearchService.deleteIndex({
            id: id
        }));

        if (data) {
            return data;
        }

        return { error };
    }

    async searchIndex(id: number, query: string) {
        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaSearchService.search({
            indexId: id,
            query: query
        }));

        if (data) {
            return data;
        }

        return { error };
    }
}