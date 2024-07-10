import type { CancelablePromise } from './core/CancelablePromise';
import { OpenAPI } from './core/OpenAPI';
import { request as __request } from './core/request';
import type { ShopifyData } from './models';

export class ShopifyService {

	/**
	 * @returns unknown OK
	 * @throws ApiError
	 */
	public static checkConfiguration(): CancelablePromise<ShopifyData['responses']['CheckConfiguration']> {
		
		return __request(OpenAPI, {
			method: 'GET',
			url: '/umbraco/shopify/management/api/v1/check-configuration',
		});
	}

	/**
	 * @returns string OK
	 * @throws ApiError
	 */
	public static getAccessToken(data: ShopifyData['payloads']['GetAccessToken'] = {}): CancelablePromise<ShopifyData['responses']['GetAccessToken']> {
		const {
                    
                    requestBody
                } = data;
		return __request(OpenAPI, {
			method: 'POST',
			url: '/umbraco/shopify/management/api/v1/get-access-token',
			body: requestBody,
			mediaType: 'application/json',
		});
	}

	/**
	 * @returns string OK
	 * @throws ApiError
	 */
	public static getAuthorizationUrl(): CancelablePromise<ShopifyData['responses']['GetAuthorizationUrl']> {
		
		return __request(OpenAPI, {
			method: 'GET',
			url: '/umbraco/shopify/management/api/v1/get-authorization-url',
		});
	}

	/**
	 * @returns unknown OK
	 * @throws ApiError
	 */
	public static getList(data: ShopifyData['payloads']['GetList'] = {}): CancelablePromise<ShopifyData['responses']['GetList']> {
		const {
                    
                    pageInfo
                } = data;
		return __request(OpenAPI, {
			method: 'GET',
			url: '/umbraco/shopify/management/api/v1/get-list',
			query: {
				pageInfo
			},
		});
	}

	/**
	 * @returns unknown OK
	 * @throws ApiError
	 */
	public static getListByIds(data: ShopifyData['payloads']['GetListByIds'] = {}): CancelablePromise<ShopifyData['responses']['GetListByIds']> {
		const {
                    
                    requestBody
                } = data;
		return __request(OpenAPI, {
			method: 'GET',
			url: '/umbraco/shopify/management/api/v1/get-list-by-ids',
			body: requestBody,
			mediaType: 'application/json',
		});
	}

	/**
	 * @returns string OK
	 * @throws ApiError
	 */
	public static revokeAccessToken(): CancelablePromise<ShopifyData['responses']['RevokeAccessToken']> {
		
		return __request(OpenAPI, {
			method: 'POST',
			url: '/umbraco/shopify/management/api/v1/revoke-access-token',
			responseHeader: 'Umb-Notifications',
		});
	}

	/**
	 * @returns number OK
	 * @throws ApiError
	 */
	public static getTotalPages(): CancelablePromise<ShopifyData['responses']['GetTotalPages']> {
		
		return __request(OpenAPI, {
			method: 'GET',
			url: '/umbraco/shopify/management/api/v1/total-pages',
		});
	}

	/**
	 * @returns unknown OK
	 * @throws ApiError
	 */
	public static validateAccessToken(): CancelablePromise<ShopifyData['responses']['ValidateAccessToken']> {
		
		return __request(OpenAPI, {
			method: 'GET',
			url: '/umbraco/shopify/management/api/v1/validate-access-token',
		});
	}

}