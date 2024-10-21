import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { SemrushRepository } from "../repository/semrush.repository";
import { UmbObjectState } from "@umbraco-cms/backoffice/observable-api";
import { AuthorizationResponseDtoModel } from "@umbraco-integrations/semrush/generated";

export class SemrushContext extends UmbControllerBase{
    #repository: SemrushRepository;
    #settingsModel = new UmbObjectState<AuthorizationResponseDtoModel | undefined>(undefined);
    settingsModel = this.#settingsModel.asObservable();

    constructor(host: UmbControllerHost){
        super(host);

        this.provideContext(SEMRUSH_CONTEXT_TOKEN, this);
        this.#repository = new SemrushRepository(host);
    }

    async hostConnected() {
        super.hostConnected();
    }

    async getTokenDetails(){
        return await this.#repository.getTokenDetails();
    }

    async getAccessToken(code: string){
        return await this.#repository.getAccessToken(code);
    }

    async refreshAccessToken(){
        return await this.#repository.refreshAccessToken();
    }

    async revokeToken(){
        return await this.#repository.revokeToken();
    }

    async validateToken(){
        return await this.#repository.validateToken();
    }

    async oauth(code: string){
        return await this.#repository.oauth(code);
    }

    async getAuthorizationUrl(){
        return await this.#repository.getAuthorizationUrl();
    }

    async getColumns(){
        return await this.#repository.getColumns();
    }

    async getDataSources(){
        return await this.#repository.getDataSources();
    }

    async getRelatedPhrases(phrase: string, pageNumber: number, dataSource: string, method: string){
        return await this.#repository.getRelatedPhrases(phrase, pageNumber, dataSource, method);
    }

    async ping(){
        return await this.#repository.ping();
    }

    async getCurrentContentProperties(contentId: string){
        return await this.#repository.getCurrentContentProperties(contentId);
    }
}

export default SemrushContext;

export const SEMRUSH_CONTEXT_TOKEN =
    new UmbContextToken<SemrushContext>(SemrushContext.name);