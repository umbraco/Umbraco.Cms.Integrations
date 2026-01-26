import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { GoogleSearchConsole } from "@umbraco-integrations/googlesearchconsole/generated";
import { GoogleSearchConsoleDocumentDataSource } from "./googlesearchconsole-document.data-source"; 

export class GoogleSearchConsoleRepository extends UmbControllerBase {
    #documentDataSource: GoogleSearchConsoleDocumentDataSource;

    constructor(host: UmbControllerHost) {
        super(host);
        this.#documentDataSource = new GoogleSearchConsoleDocumentDataSource(this);
    }

    async getOAuthConfiguration() {
        const { data, error } = await tryExecute(this, GoogleSearchConsole.getOauthConfiguration());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getAccessToken(code: string) {
        const { data, error } = await tryExecute(this, GoogleSearchConsole.postOauthAccessToken({ body: { code } }));

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async revokeAccessToken() {
        const { data, error } = await tryExecute(this, GoogleSearchConsole.postOauthRevoke());

        if (error || !data) {
            return { error };
        }

        return { data };
    }

    async getUrls(documentId: string) {
        return this.#documentDataSource.getUrls(documentId);
    }

    async inspect(inspectionUrl: string, siteUrl: string, languageCode: string) {
        const { data, error } = await tryExecute(this, GoogleSearchConsole.postInspect({
            body: {
                inspectionUrl,
                siteUrl,
                languageCode
            }}));

        if (error || !data) {
            return { error };
        }

        return { data };
    }
}