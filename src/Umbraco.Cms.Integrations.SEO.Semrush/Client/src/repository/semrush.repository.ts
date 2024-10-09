import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { AccessTokenService, RelatedPhrasesDtoModel, SemrushService } from "@umbraco-integrations/semrush/generated";

export class SemrushRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getTokenDetails(){
        const { data, error } = await tryExecuteAndNotify(this, AccessTokenService.getTokenDetails());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(code: string){
        const { data, error } = await tryExecuteAndNotify(this, AccessTokenService.getAccessToken({requestBody:{code: code}}));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async refreshAccessToken(){
        const { data, error } = await tryExecuteAndNotify(this, AccessTokenService.refreshAccessToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async revokeToken(){
        const { data, error } = await tryExecuteAndNotify(this, AccessTokenService.revokeToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async validateToken(){
        const { data, error } = await tryExecuteAndNotify(this, AccessTokenService.validateToken());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async oauth(code: string){
        const { data, error } = await tryExecuteAndNotify(this, SemrushService.oauth({code: code}));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAuthorizationUrl(){
        const { data, error } = await tryExecuteAndNotify(this, SemrushService.getAuthorizationUrl());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getColumns(){
        const { data, error } = await tryExecuteAndNotify(this, SemrushService.getColumns());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getDataSources(){
        const { data, error } = await tryExecuteAndNotify(this, SemrushService.getDataSources());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getRelatedPhrases(phrase: string, pageNumber: number, dataSource: string, method: string){
        const { data, error } = await tryExecuteAndNotify(this, SemrushService.getRelatedPhrases({
            phrase: phrase,
            pageNumber: pageNumber,
            dataSource: dataSource,
            method: method
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async ping(){
        const { data, error } = await tryExecuteAndNotify(this, SemrushService.ping());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getCurrentContentProperties(contentId: string){
        const { data, error } = await tryExecuteAndNotify(this, SemrushService.getCurrentContentProperties({contentId: contentId}));

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}