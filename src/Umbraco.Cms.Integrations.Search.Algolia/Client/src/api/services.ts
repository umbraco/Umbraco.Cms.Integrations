import type { CancelablePromise } from './core/CancelablePromise';
import { OpenAPI } from './core/OpenAPI';
import { request as __request } from './core/request';
import type { AlgoliaSearchData } from './models';

export class AlgoliaSearchService {

    /**
     * @returns unknown OK
     * @throws ApiError
     */
    public static getContentTypes(): CancelablePromise<AlgoliaSearchData['responses']['GetContentTypes']> {

        return __request(OpenAPI, {
            method: 'GET',
            url: '/umbraco/algolia-search/management/api/v1/search/content-type',
        });
    }

    /**
     * @returns unknown OK
     * @throws ApiError
     */
    public static getContentTypesByIndexId(data: AlgoliaSearchData['payloads']['GetContentTypesByIndexId']): CancelablePromise<AlgoliaSearchData['responses']['GetContentTypesByIndexId']> {
        const {

            id
        } = data;
        return __request(OpenAPI, {
            method: 'GET',
            url: '/umbraco/algolia-search/management/api/v1/search/content-type/index/{id}',
            path: {
                id
            },
        });
    }

    /**
     * @returns unknown OK
     * @throws ApiError
     */
    public static getIndices(): CancelablePromise<AlgoliaSearchData['responses']['GetIndices']> {

        return __request(OpenAPI, {
            method: 'GET',
            url: '/umbraco/algolia-search/management/api/v1/search/index',
        });
    }

    /**
     * @returns unknown OK
     * @throws ApiError
     */
    public static saveIndex(data: AlgoliaSearchData['payloads']['SaveIndex'] = {}): CancelablePromise<AlgoliaSearchData['responses']['SaveIndex']> {
        const {

            requestBody
        } = data;
        return __request(OpenAPI, {
            method: 'POST',
            url: '/umbraco/algolia-search/management/api/v1/search/index',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

    /**
     * @returns unknown OK
     * @throws ApiError
     */
    public static deleteIndex(data: AlgoliaSearchData['payloads']['DeleteIndex']): CancelablePromise<AlgoliaSearchData['responses']['DeleteIndex']> {
        const {

            id
        } = data;
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/umbraco/algolia-search/management/api/v1/search/index/{id}',
            path: {
                id
            },
        });
    }

    /**
     * @returns unknown OK
     * @throws ApiError
     */
    public static getIndexById(data: AlgoliaSearchData['payloads']['GetIndexById']): CancelablePromise<AlgoliaSearchData['responses']['GetIndexById']> {
        const {

            id
        } = data;
        return __request(OpenAPI, {
            method: 'GET',
            url: '/umbraco/algolia-search/management/api/v1/search/index/{id}',
            path: {
                id
            },
        });
    }

    /**
     * @returns unknown OK
     * @throws ApiError
     */
    public static search(data: AlgoliaSearchData['payloads']['Search']): CancelablePromise<AlgoliaSearchData['responses']['Search']> {
        const {

            indexId,
            query
        } = data;
        return __request(OpenAPI, {
            method: 'GET',
            url: '/umbraco/algolia-search/management/api/v1/search/index/{indexId}/search',
            path: {
                indexId
            },
            query: {
                query
            },
        });
    }

    /**
     * @returns unknown OK
     * @throws ApiError
     */
    public static buildIndex(data: AlgoliaSearchData['payloads']['BuildIndex'] = {}): CancelablePromise<AlgoliaSearchData['responses']['BuildIndex']> {
        const {

            requestBody
        } = data;
        return __request(OpenAPI, {
            method: 'POST',
            url: '/umbraco/algolia-search/management/api/v1/search/index/build',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

}