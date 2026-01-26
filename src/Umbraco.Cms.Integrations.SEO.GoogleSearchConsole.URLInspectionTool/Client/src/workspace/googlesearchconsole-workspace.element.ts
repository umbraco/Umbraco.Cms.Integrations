import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { css, html, customElement, state, when } from '@umbraco-cms/backoffice/external/lit';
import type { UmbNotificationContext } from '@umbraco-cms/backoffice/notification';
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";
import { GoogleSearchConsoleRepository } from "../repository/googlesearchconsole.repository";
import type { InspectionResultDtoModel } from "@umbraco-integrations/googlesearchconsole/generated";

import { UUISelectEvent } from "@umbraco-cms/backoffice/external/uui";

import "./inspectresult-box.element";

const elementName = "googlesearchconsole-workspace-view";

@customElement(elementName)
export class GoogleSearchConsoleWorkspaceElement extends UmbLitElement {
    #repository = new GoogleSearchConsoleRepository(this);
    #notificationContext?: UmbNotificationContext;
    #workspaceContext?: typeof UMB_DOCUMENT_WORKSPACE_CONTEXT.TYPE;

    @state()
    private _loading = false;

    @state()
    private _isConnected = false;

    @state()
    private _showResults = false;

    @state()
    private _authorizationUrl = "";

    @state()
    inspectionObj = {
        urls: [] as Array<string>,
        inspectionUrl: "",
        siteUrl: window.location.origin,
        languageCode: "",
        multipleUrls: false,
        enabled: false
    };

    @state()
    inspectionResult: InspectionResultDtoModel | null = null;

    constructor() {
        super();
        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
            this.#notificationContext = instance;
        });
        this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (context) => {
            if (!context) return;

            this.#workspaceContext = context;
            this.observe(context.unique, async (id) => {
                if (!id) return;
                var data = await this.#repository.getUrls(id);
                if (data?.data?.length > 0) {
                    this.inspectionObj.multipleUrls = data?.data[0].urlInfos.length > 1;
                    data?.data[0].urlInfos.forEach((urlInfo, index) => {
                        if (index == 0 && urlInfo.culture) {
                            this.inspectionObj.languageCode = urlInfo.culture;
                        }

                        if (urlInfo.url) {
                            const url = this._isRelativeUrl(urlInfo.url)
                                ? `${window.location.origin}${urlInfo.url}`
                                : urlInfo.url;
                            this.inspectionObj.urls.push(url);
                        }
                    });
                    this.inspectionObj.inspectionUrl = this.inspectionObj.urls[0];
                }
            });
        });
    }

    async connectedCallback() {
        super.connectedCallback();

        this._loading = true;

        const oauthConfig = await this.#repository.getOAuthConfiguration();
        this._isConnected = oauthConfig.data?.isConnected ?? false;
        this._authorizationUrl = oauthConfig.data?.authorizationUrl ?? "";

        this._loading = false;
    }

    #onConnect() {
        window.addEventListener("message", async (event: MessageEvent) => {
            if (event.data.type == "google:oauth:success") {
                var codeParam = "?code=";
                var scopeParam = "&scope=";
                var code = event.data.url.slice(event.data.url.indexOf(codeParam) + codeParam.length, event.data.url.indexOf(scopeParam));

                const data = await this.#repository.getAccessToken(code);
                const isError = data?.data && !data?.data.success;
                const notification = {
                    data: {
                        title: "Google Search Console Authorization",
                        message: isError
                            ? data?.data.errorMessage ?? "Access Denied"
                            : "Access Approved"
                    }
                };
                this.#notificationContext?.peek(isError ? "danger" : "positive", notification);
                if (!isError) {
                    this._isConnected = true;
                }
            } else if (event.data.type == "google:oauth:denied") {
                const notification = { data: { title: "Google Search Console Authorization", message: "Access Denied" } };
                this.#notificationContext?.peek("danger", notification);
                this._isConnected = false;
            }
        }, false);

        window.open(this._authorizationUrl,
            "GoogleSearchConsole_Authorize",
            "width=900,height=700,modal=yes,alwaysRaised=yes");
    }

    async #onRevoke() {
        var result = await this.#repository.revokeAccessToken();
        if (!result) return;

        this._isConnected = false;
    }

    async #onInspect() {
        this._loading = true;

        const { data } = await this.#repository.inspect(this.inspectionObj.inspectionUrl, this.inspectionObj.siteUrl, this.inspectionObj.languageCode);
        if (data) {
            this._showResults = true;
            this.inspectionResult = data;
        }

        this._loading = false;
    }

    #onEdit() {
        this.inspectionObj = { ...this.inspectionObj, multipleUrls: false, enabled: true };
    }

    async #onChangeInspectionUrl(e: UUISelectEvent) {
        const inspectUrl = e.target.value as string;

        const documentId = this.#workspaceContext!.getUnique();
        const documentUrls = await this.#repository.getUrls(documentId!);

        if (documentUrls?.data?.length > 0) {
            documentUrls?.data[0].urlInfos.forEach((urlInfo) => {
                if (inspectUrl === urlInfo.url) {
                    this.inspectionObj = {
                        ...this.inspectionObj,
                        languageCode: urlInfo.culture || ""
                    }; 
                }
            });
        }
    }

    #renderHeaderActions() {
        return html`
            <div slot="header-actions">
                 <a href="javascript:void(0)" class="signin"
                    ?disabled=${this._isConnected}"
                    @click=${this.#onConnect}>
                    <img src="${this._isConnected ? "/App_Plugins/GoogleSearchConsole/images/btn_google_signin_dark_disabled_web.png" : "/App_Plugins/GoogleSearchConsole/images/btn_google_signin_dark_normal_web.png"}" />
                </a>
                <uui-button label="Revoke" look="primary" color="danger"
                    ?disabled=${!this._isConnected} 
                    @click=${this.#onRevoke}></uui-button> 
            </div>`;
    }

    #renderInspect() {
        return html`
            <div class="row">
                <div class="field">
                    <uui-label>Inspection URL</uui-label>
                        ${this.inspectionObj.multipleUrls
                            ? html`<uui-select
                                    style="width: 100%"
                                    @change=${this.#onChangeInspectionUrl}
                                    .options=${this.inspectionObj.urls.map(url => ({
                                        name: url,
                                        value: url,
                                        selected: url === this.inspectionObj.inspectionUrl
                                    }))}></uui-select>`
                            : html`<uui-input 
                                    style="width: 100%"
                                    ?disabled=${!this.inspectionObj.enabled} 
                                    .value=${this.inspectionObj.inspectionUrl}></uui-input>`}
                        
                </div>
                <div class="field">
                    <uui-label>Site URL</uui-label>
                    <uui-input
                        style="width: 100%"
                        ?disabled=${!this.inspectionObj.enabled}
                        .value=${this.inspectionObj.siteUrl}></uui-input>
                </div>
            </div>
            <div style="row">
                <uui-button
                    look="primary"
                    label="Inspect"
                    ?disabled=${!this._isConnected}
                    @click=${this.#onInspect}></uui-button>
                <uui-button
                    look="primary"
                    color="warning"
                    label="Edit"
                    ?disabled=${!this._isConnected}
                    @click=${this.#onEdit}></uui-button>
            </div>
        `;
    }

    #renderResults() {
        return html`
            <uui-box>
                <div>
                    <inspectresult-box 
                        headline="Inspection Result Link"
                        headlineSlot="Link to Search Console URL inspection."
                        .link=${this.inspectionResult?.inspectionResultLink}></inspectresult-box>
                    ${when(this.inspectionResult?.indexStatusResult, () => html`
                        <inspectresult-box
                            headline="Index Status Result"
                            headlineSlot="Result of the index status analysis."
                            .data=${this.inspectionResult?.indexStatusResult}></inspectresult-box>
                    `)}
                    ${when(this.inspectionResult?.ampResult, () => html`
                        <inspectresult-box
                            headline="AMP Result"
                            headlineSlot="Result of the AMP analysis. Absent if the page is not an AMP page."
                            .data=${this.inspectionResult?.ampResult}></inspectresult-box>
                    `)}
                    ${when(this.inspectionResult?.mobileUsabilityResult, () => html`
                        <inspectresult-box
                            headline="Mobile Usability Result"
                            headlineSlot="Result of the Mobile usability analysis."
                            .data=${this.inspectionResult?.mobileUsabilityResult}></inspectresult-box>
                    `)}
                    ${when(this.inspectionResult?.richResultsResult, () => html`
                        <inspectresult-box
                            headline="Rich Results Result"
                            headlineSlot="Result of the Rich Results analysis. Absent if there are no rich results found."
                            .data=${this.inspectionResult?.richResultsResult}></inspectresult-box>
                    `)}
                </div>
            </uui-box>
        `;
    }

    render() {
        return html`
            <umb-body-layout>
                <uui-box headline=${this.localize.term("urlInspectionTool_title")}>
                    ${this.#renderHeaderActions()}
                    <div>
                        <h5>About Google Search Console - URL Inspection API</h5>
                        <p>
                            The Search Console APIs are a way to access data outside of Search Console, through external applications and products.
                        </p>
                        <p>
                            You can request the data Search Console has about the indexed version of the current node, and the API will return the indexed information.
                        </p>
                        <p>
                            The request parameters include the URL you'd like to inspect and the URL of the property as defined in Search Console.
                        </p>
                        <p>
                            The response includes analysis results containing information from Search Console, including index status, AMP, rich results and mobile usability.
                        </p>
                        <p>
                            Usage limits - the quote is enforced per Search Console website property: 2000 queries per day / 600 queries per minute.
                        </p>
                    </div>
                    ${this.#renderInspect()}
                    ${this._loading ? html`<div class="loader"><uui-loader></uui-loader></div>` : ""}
                    <br/>
                    ${when(this._showResults, () => this.#renderResults())}
                </uui-box>
            </umb-body-layout>
        `;
    }

    _isRelativeUrl(url: string) {
        var regExp = new RegExp('^(?:[a-z]+:)?//', 'i');
        return !regExp.test(url);
    }

    static styles = [
        css`
            .loader {
                display: flex;
                justify-content: center;
            }
            .signin {
                display: inline-flex;
                vertical-align: middle;
                height: 37px;
            }
            .row {
              display: flex;
              gap: var(--uui-size-space-5);
            }
            .field {
              flex: 0 0 auto;
              min-width: 30rem;
            }
        `];
}
export default GoogleSearchConsoleWorkspaceElement;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: GoogleSearchConsoleWorkspaceElement;
    }
}