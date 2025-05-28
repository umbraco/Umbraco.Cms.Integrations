import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { ShopifyService, type OAuthRequestDtoModel, RequestDtoModel } from "@umbraco-integrations/shopify/generated";

export class ShopifyRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async checkConfiguration() {
        const { data, error } = await tryExecute(this, ShopifyService.getCheckConfiguration());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(oAuthRequestDtoModel: OAuthRequestDtoModel) {
        const { data, error } = await tryExecute(this, ShopifyService.postAccessToken({ body: oAuthRequestDtoModel }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async validateAccessToken() {
        const { data, error } = await tryExecute(this, ShopifyService.getValidateAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async revokeAccessToken() {
        const { data, error } = await tryExecute(this, ShopifyService.postRevokeAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getList(pageInfo?: string) {
        const { data, error } = await tryExecute(this, ShopifyService.getList({ query: { pageInfo: pageInfo } }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getListByIds(model: RequestDtoModel) {
        const { data, error } = await tryExecute(this, ShopifyService.postListByIds({
            body: model
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getTotalPages(){
        const { data, error } = await tryExecute(this, ShopifyService.getTotalPages());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAuthorizationUrl(){
        const { data, error } = await tryExecute(this, ShopifyService.getAuthorizationUrl());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async refreshAccessToken() {
        const { data, error } = await tryExecute(this, ShopifyService.postRefreshAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}

export default ShopifyRepository;