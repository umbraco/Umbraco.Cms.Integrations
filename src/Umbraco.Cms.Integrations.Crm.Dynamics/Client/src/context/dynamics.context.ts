import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { DynamicsRepository } from "../repository/dynamics.repository";

export class DynamicsContext extends UmbControllerBase{
    #repository: DynamicsRepository;

    constructor(host: UmbControllerHost){
        super(host);

        this.provideContext(DYNAMICS_CONTEXT_TOKEN, this);
        this.#repository = new DynamicsRepository(host);
    }

    async hostConnected() {
        super.hostConnected();
    }

    async getForms(module: string){
        return await this.#repository.getForms(module);
    }

    async revokeAccessToken(){
        return await this.#repository.revokeAccessToken();
    }

    async getAuthorizationUrl(){
        return await this.#repository.getAuthorizationUrl();
    }

    async checkOauthConfiguration(){
        return await this.#repository.checkOauthConfiguration();
    }

    async getAccessToken(){
        return await this.#repository.getAccessToken();
    }

    async getEmbedCode(formId: string){
        return await this.#repository.getEmbedCode(formId);
    }

    async getSystemUserFullName(){
        return await this.#repository.getSystemUserFullName();
    }

    async oauth(code: string){
        return await this.#repository.oauth(code);
    }
}

export default DynamicsContext;

export const DYNAMICS_CONTEXT_TOKEN =
    new UmbContextToken<DynamicsContext>(DynamicsContext.name);