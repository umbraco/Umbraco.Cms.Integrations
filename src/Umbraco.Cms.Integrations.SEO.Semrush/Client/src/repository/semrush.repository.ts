import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { AccessTokenService, RelatedPhrasesDtoModelReadable, SemrushService } from "@umbraco-integrations/semrush/generated";

export class SemrushRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getTokenDetails(){
        const { data, error } = await tryExecute(this, AccessTokenService.getTokenDetails());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(code: string) {
        const { data, error } = await tryExecute(this, AccessTokenService.postTokenGet({ body: { code: code } }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async refreshAccessToken() {
        const { data, error } = await tryExecute(this, AccessTokenService.postTokenRefresh());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async revokeToken() {
        const { data, error } = await tryExecute(this, AccessTokenService.postTokenRevoke());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async validateToken() {
        const { data, error } = await tryExecute(this, AccessTokenService.getTokenValidate());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async oauth(code: string) {
        const { data, error } = await tryExecute(this, SemrushService.getAuth({ query: { code: code } }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAuthorizationUrl() {
        const { data, error } = await tryExecute(this, SemrushService.getAuthUrl());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getColumns(){
        const { data, error } = await tryExecute(this, SemrushService.getColumns());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getDataSources(){
        const { data, error } = await tryExecute(this, SemrushService.getDataSources());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getRelatedPhrases(phrase: string, pageNumber: number, dataSource: string, method: string){
        const { data, error } = await tryExecute(this, SemrushService.getRelatedPhrases({
            query: {
                phrase: phrase,
                pageNumber: pageNumber,
                dataSource: dataSource,
                method: method
            }
        }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async ping() {
        const { data, error } = await tryExecute(this, SemrushService.getPing());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getCurrentContentProperties(contentId: string) {
        const { data, error } = await tryExecute(this, SemrushService.getContentProperties({ query: { contentId: contentId } }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}