import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { ShopifyRepository } from "../repository/shopify.repository";
import { EditorSettingsModelReadable, RequestDtoModel, type OAuthRequestDtoModel } from "@umbraco-integrations/shopify/generated";
import { UmbObjectState } from "@umbraco-cms/backoffice/observable-api";

export class ShopifyContext extends UmbControllerBase{
    #repository: ShopifyRepository;
    #data = new UmbObjectState<string | undefined>(undefined);
    #settingsModel = new UmbObjectState<EditorSettingsModelReadable | undefined>(undefined);
    settingsModel = this.#settingsModel.asObservable();

    constructor(host: UmbControllerHost){
        super(host);

        this.provideContext(SHOPIFY_CONTEXT_TOKEN, this);
        this.#repository = new ShopifyRepository(host);
    }

    async hostConnected() {
        super.hostConnected();
        this.checkConfiguration();
    }

    async checkConfiguration(){
        const { data } = await this.#repository.checkConfiguration();
        this.#settingsModel.setValue(data);
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

    async getList(pageInfo?: string){
        return await this.#repository.getList(pageInfo);
    }

    async getListByIds(model: RequestDtoModel){
        return await this.#repository.getListByIds(model);
    }

    async getTotalPages(){
        return await this.#repository.getTotalPages();
    }

    async getAuthorizationUrl(){
        return await this.#repository.getAuthorizationUrl();
    }

    async refreshAccessToken(){
        return await this.#repository.refreshAccessToken();
    }

    getData() {
        return this.#data.getValue();
      }
}

export default ShopifyContext;

export const SHOPIFY_CONTEXT_TOKEN =
    new UmbContextToken<ShopifyContext>(ShopifyContext.name);