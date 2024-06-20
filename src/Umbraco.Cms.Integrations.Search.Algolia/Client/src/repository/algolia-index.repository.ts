import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import {
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";

import { IndexConfigurationModel, AlgoliaSearchService } from "@umbraco-integrations/algolia/generated";

export class AlgoliaIndexRepository extends UmbControllerBase {

    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getIndices() {
        const { data, error } = await tryExecuteAndNotify(this, AlgoliaSearchService.getIndices());

        if (error || !data) {
            return { error }
        }

        return { data }
    }

    async getIndexById(id: number) {
        const { data, error } = await tryExecuteAndNotify(this, AlgoliaSearchService.getIndexById({
            id: id
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getContentTypes() {
        const { data, error } = await tryExecuteAndNotify(this, AlgoliaSearchService.getContentTypes());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getContentTypesWithIndex(id: number) {
        const { data, error } = await tryExecuteAndNotify(this, AlgoliaSearchService.getContentTypesByIndexId({
            id: id
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async saveIndex(indexConfiguration: IndexConfigurationModel) {
        const { data, error } = await tryExecuteAndNotify(this, AlgoliaSearchService.saveIndex({
            requestBody: indexConfiguration
        }));

        if (error || !data) {
            return { error };
        }

        this.#showSuccess("Index saved.");

        const redirectPath = indexConfiguration.id != 0
            ? window.location.href.replace(`/index/${indexConfiguration.id}`, '')
            : window.location.href.replace('/index', '');

        window.history.pushState({}, '', redirectPath);

        return { data };
    }

    async buildIndex(indexConfiguration: IndexConfigurationModel) {
        const { data, error } = await tryExecuteAndNotify(this, AlgoliaSearchService.buildIndex({
            requestBody: indexConfiguration
        }));

        if (error || !data) {
            return { error };
        }

        this.#showSuccess("Index built.");

        return { data };
    }

    async deleteIndex(id: number) {
        const { data, error } = await tryExecuteAndNotify(this, AlgoliaSearchService.deleteIndex({
            id: id
        }));

        if (error || !data) {
            return { error };
        }

        this.#showSuccess("Index deleted");

        return { data };
    }

    async searchIndex(id: number, query: string) {
        const { data, error } = await tryExecuteAndNotify(this, AlgoliaSearchService.search({
            indexId: id,
            query: query
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    // notifications
    async #showSuccess(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("positive", {
            data: { message: message },
        });
    }
}
