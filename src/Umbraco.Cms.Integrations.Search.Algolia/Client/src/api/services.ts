import { OpenAPI } from "./core/OpenAPI";
import type { CancelablePromise } from "./core/CancelablePromise";
import { request as __request } from "./core/request";

import { AlgoliaIndexConfigurationModel } from "../models/AlgoliaIndexConfigurationModel";
import { AlgoliaContentTypeModel } from "../models/AlgoliaContentTypeModel";
import { AlgoliaResultModel } from "../models/AlgoliaResultModel";
import { AlgoliaSearchResultModel } from "../models/AlgoliaSearchResultModel";

export class AlgoliaIndexService {

    private static apiPath: string = "/umbraco/algolia-search/management/api/v1/search";

    /**
     * @returns Array<AlgoliaIndexConfigurationModel> Success
     * @throws ApiError
     */
    public static getIndices(): CancelablePromise<Array<AlgoliaIndexConfigurationModel>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: `${this.apiPath}/index`,
        });
    }

    /**
     * @param id
     * @returns AlgoliaIndexConfigurationModel Success
     * @throws ApiError
     */
    public static getIndexById(id: Number): CancelablePromise<AlgoliaIndexConfigurationModel> {
        return __request(OpenAPI, {
            method: 'GET',
            url: `${this.apiPath}/index/{id}`,
            path: {
                id
            }
        });
    }

    /**
     * @returns Array<AlgoliaContentTypeModel> Success
     * @throws ApiError
     */
    public static getContentTypes(): CancelablePromise<Array<AlgoliaContentTypeModel>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: `${this.apiPath}/content-type`,
        });
    }

    /**
     * @param id
     * @returns Array<AlgoliaContentTypeModel> Success
     * @throws ApiError
     */
    public static getContentTypesWithIndex(id: Number): CancelablePromise<Array<AlgoliaContentTypeModel>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: `${this.apiPath}/content-type/index/{id}`,
            path: {
                id
            }
        });
    }

    /**
     * @param indexConfiguration
     * @returns AlgoliaResultModel Success
     * @throws ApiError
     */
    public static saveIndex(indexConfiguration: AlgoliaIndexConfigurationModel): CancelablePromise<AlgoliaResultModel> {
        return __request(OpenAPI, {
            method: 'POST',
            url: `${this.apiPath}/index`,
            body: indexConfiguration,
            headers: {
                'Content-Type': 'application/json',
                Accept: 'application/json',
            }
        });
    }

    /**
     * @param indexConfiguration
     * @returns AlgoliaResultModel Success
     * @throws ApiError
     */
    public static buildIndex(indexConfiguration: AlgoliaIndexConfigurationModel): CancelablePromise<AlgoliaResultModel> {
        return __request(OpenAPI, {
            method: 'POST',
            url: `${this.apiPath}/index/build`,
            body: indexConfiguration,
            headers: {
                'Content-Type': 'application/json',
                Accept: 'application/json',
            }
        });
    }

    /**
     * @param id
     * @returns AlgoliaResultModel Success
     * @throws ApiError
     */
    public static deleteIndex(id: Number): CancelablePromise<AlgoliaResultModel> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: `${this.apiPath}/index/{id}`,
            path: {
                id
            }
        });
    }

    /**
     * @param id
     * @param query
     * @returns AlgoliaSearchResultModel Success
     * @throws ApiError
     */
    public static searchIndex(id: Number, query: string): CancelablePromise<AlgoliaSearchResultModel> {
        return __request(OpenAPI, {
            method: 'GET',
            url: `${this.apiPath}/index/{id}/search?query={query}`,
            path: {
                id, query
            }
        });
    }
}