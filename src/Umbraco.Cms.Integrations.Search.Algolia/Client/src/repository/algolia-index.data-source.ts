import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { AlgoliaIndexService } from "../api/services";
import { AlgoliaIndexConfigurationModel } from "../models/AlgoliaIndexConfigurationModel";

export class AlgoliaIndexDataSource {

    #host: UmbControllerHost;

    constructor(host: UmbControllerHost) {
        this.#host = host;
    }

    async getIndices() {

        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaIndexService.getIndices());

        if (data) {
            return data;
        }

        return { error };
    }

    async getIndexById(id: Number) {

        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaIndexService.getIndexById(id));

        if (data) {
            return data;
        }

        return { error };
    }

    async getContentTypes() {

        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaIndexService.getContentTypes());

        if (data) {
            return data;
        }

        return { error };
    }

    async getContentTypesWithIndex(id: Number) {
        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaIndexService.getContentTypesWithIndex(id));

        if (data) {
            return data;
        }

        return { error };
    }

    async saveIndex(indexConfiguration: AlgoliaIndexConfigurationModel) {
        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaIndexService.saveIndex(indexConfiguration));

        if (data) {
            return data;
        }

        return { error };
    }

    async buildIndex(indexConfiguration: AlgoliaIndexConfigurationModel) {
        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaIndexService.buildIndex(indexConfiguration));

        if (data) {
            return data;
        }

        return { error };
    }

    async deleteIndex(id: Number) {
        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaIndexService.deleteIndex(id));

        if (data) {
            return data;
        }

        return { error };
    }

    async searchIndex(id: Number, query: string) {
        const { data, error } = await tryExecuteAndNotify(this.#host, AlgoliaIndexService.searchIndex(id, query));

        if (data) {
            return data;
        }

        return { error };
    }
}