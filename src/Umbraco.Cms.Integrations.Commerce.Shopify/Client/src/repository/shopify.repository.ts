import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { ShopifyService, type OAuthRequestDtoModel } from "../generated";

export class ShopifyRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async checkConfiguration(){
        const { data, error } = await tryExecuteAndNotify(this, ShopifyService.checkConfiguration());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(oAuthRequestDtoModel: OAuthRequestDtoModel){
        const { data, error } = await tryExecuteAndNotify(this, ShopifyService.getAccessToken({requestBody: oAuthRequestDtoModel}));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async validateAccessToken(){
        const { data, error } = await tryExecuteAndNotify(this, ShopifyService.validateAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async revokeAccessToken(){
        const { data, error } = await tryExecuteAndNotify(this, ShopifyService.revokeAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getList(){
        const { data, error } = await tryExecuteAndNotify(this, ShopifyService.getList());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getListByIds(){
        const { data, error } = await tryExecuteAndNotify(this, ShopifyService.getListByIds());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getTotalPages(){
        const { data, error } = await tryExecuteAndNotify(this, ShopifyService.getTotalPages());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAuthorizationUrl(){
        const { data, error } = await tryExecuteAndNotify(this, ShopifyService.getAuthorizationUrl());

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}

export default ShopifyRepository;