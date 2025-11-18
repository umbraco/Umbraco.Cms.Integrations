import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { AccessToken, RelatedPhrasesDtoModel, Semrush } from "@umbraco-integrations/semrush/generated";

export class SemrushRepository extends UmbControllerBase {
    constructor(host: UmbControllerHost) {
        super(host);
    }

    async getTokenDetails(){
        const { data, error } = await tryExecute(this, AccessToken.getTokenDetails());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(code: string) {
        const { data, error } = await tryExecute(this, AccessToken.postTokenGet({ body: { code: code } }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async refreshAccessToken() {
        const { data, error } = await tryExecute(this, AccessToken.postTokenRefresh());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async revokeToken() {
        const { data, error } = await tryExecute(this, AccessToken.postTokenRevoke());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async validateToken() {
        const { data, error } = await tryExecute(this, AccessToken.getTokenValidate());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async oauth(code: string) {
        const { data, error } = await tryExecute(this, Semrush.getAuth({ query: { code: code } }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAuthorizationUrl() {
        const { data, error } = await tryExecute(this, Semrush.getAuthUrl());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getColumns(){
        const { data, error } = await tryExecute(this, Semrush.getColumns());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getDataSources(){
        const { data, error } = await tryExecute(this, Semrush.getDataSources());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getRelatedPhrases(phrase: string, pageNumber: number, dataSource: string, method: string){
        const { data, error } = await tryExecute(this, Semrush.getRelatedPhrases({
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
        const { data, error } = await tryExecute(this, Semrush.getPing());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getCurrentContentProperties(contentId: string) {
        const { data, error } = await tryExecute(this, Semrush.getContentProperties({ query: { contentId: contentId } }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}