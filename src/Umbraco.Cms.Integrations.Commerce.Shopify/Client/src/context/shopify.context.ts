import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { ShopifyRepository } from "../repository/shopify.repository";
import { type OAuthRequestDtoModel } from "@umbraco-integrations/shopify/generated";

export class ShopifyContext extends UmbControllerBase{
    #repository: ShopifyRepository;

    constructor(host: UmbControllerHost){
        super(host);

        this.provideContext(SHOPIFY_CONTEXT_TOKEN, this);
        this.#repository = new ShopifyRepository(host);
    }

    async checkConfiguration(){
        return await this.#repository.checkConfiguration();
    }

    async getAccessToken(oAuthRequestDtoModel: OAuthRequestDtoModel){
        return await this.#repository.getAccessToken(oAuthRequestDtoModel);
    }

    async validateAccessToken(){
        return await this.#repository.validateAccessToken();
    }

    async revokeAccessToken(){
        return await this.#repository.revokeAccessToken();
    }

    async getList(){
        return await this.#repository.getList();
    }

    async getListByIds(){
        return await this.#repository.getListByIds();
    }

    async getTotalPages(){
        return await this.#repository.getTotalPages();
    }

    async getAuthorizationUrl(){
        return await this.#repository.getAuthorizationUrl();
    }
}

export default ShopifyContext;

export const SHOPIFY_CONTEXT_TOKEN =
    new UmbContextToken<ShopifyContext>(ShopifyContext.name);