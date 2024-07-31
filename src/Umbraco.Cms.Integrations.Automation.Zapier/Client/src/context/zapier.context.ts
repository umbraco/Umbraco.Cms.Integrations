import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { ZapierRepository } from "../repository/zapier.repository";

export class ZapierContext extends UmbControllerBase{
    #repository: ZapierRepository;

    constructor(host: UmbControllerHost){
        super(host);

        this.provideContext(ZAPIER_CONTEXT_TOKEN, this);
        this.#repository = new ZapierRepository(host);
    }

    async hostConnected() {
        super.hostConnected();
    }

    async getAll(){
        return await this.#repository.getAll();
    }

    async getContentByType(){
        return await this.#repository.getContentByType();
    }

    async getContentTypes(){
        return await this.#repository.getContentTypes();
    }

    async isFormsExtensionInstalled(){
        return await this.#repository.isFormsExtensionInstalled();
    }

    async updatePreferences(){
        return await this.#repository.updatePreferences();
    }

    async validateUser(){
        return await this.#repository.validateUser();
    }
}

export default ZapierContext;

export const ZAPIER_CONTEXT_TOKEN =
    new UmbContextToken<ZapierContext>(ZapierContext.name);