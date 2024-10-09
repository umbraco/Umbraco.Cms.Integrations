import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { css, html, nothing, customElement, state, when } from '@umbraco-cms/backoffice/external/lit';
import { SEMRUSH_CONTEXT_TOKEN } from '../context/semrush.context';
import type { UmbTableColumn, UmbTableItem } from '@umbraco-cms/backoffice/components';
import { UUIInputEvent, UUIPaginationEvent, UUISelectEvent } from '@umbraco-cms/backoffice/external/uui';
import { UmbPaginationManager } from "@umbraco-cms/backoffice/utils";
import {type Phrase} from "../model/semrush-phrase.model"
import { AuthorizationResponseDtoModel, ContentPropertyDtoModel, DataSourceItemDtoModel, RelatedPhrasesDataDtoModel, RelatedPhrasesDtoModel } from '@umbraco-integrations/semrush/generated';
import { UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { SEMRUSH_MODAL_TOKEN } from '../modal/semrush-modal.token';
import { UMB_CURRENT_USER_CONTEXT } from '@umbraco-cms/backoffice/current-user';
import { UMB_NOTIFICATION_CONTEXT, UmbNotificationColor } from '@umbraco-cms/backoffice/notification';

const elementName = "semrush-workspace-view";
@customElement(elementName)
export class SemrushWorkspaceElement extends UmbLitElement {
    #semrushContext!: typeof SEMRUSH_CONTEXT_TOKEN.TYPE;
    #modalManagerContext!: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;
    #currentUserContext!: typeof UMB_CURRENT_USER_CONTEXT.TYPE;
    #paginationManager = new UmbPaginationManager();

    @state()
    _currentPageNumber = 1;

    @state()
    _totalPages = 1;

    @state()
    _nextPageInfo?: string;

    @state()
    _previousPageInfo?: string;

    @state()
    private authUrl: string = "";

    @state()
    private account: AuthorizationResponseDtoModel = {
        isAuthorized: false,
        isValid: false,
        isFreeAccount: true
    };

    @state()
	private _tableItems: Array<UmbTableItem> = [];

    @state()
	private _tableColumns: Array<UmbTableColumn> = [];

    @state()
    private selectedproperty: string = "";
    @state()
    private propertyList: Array<ContentPropertyDtoModel> = [];

    @state()
    private keywordList: RelatedPhrasesDtoModel | undefined = undefined;

    @state()
    private searchPhrase: string = "";

    @state()
    private selectedDatasource: string = "";
    @state()
    private datasourceList: Array<DataSourceItemDtoModel> | undefined = [];

    @state()
    private selectedMethod: string = "";
    @state()
    private methodList: Array<Phrase> = [
        {
            key: "phrase_fullsearch",
            value: "phrase_fullsearch",
            description: "List of broad matches and alternate search queries, including particular keywords or keyword expressions."
        }, 
        {
            key: "phrase_kdi",
            value: "phrase_kdi",
            description: "Keyword difficulty - an index that helps to estimate how difficult it would be to seize competitor's positions " +
                "in organic search within Google's top 100 with an indicated search term."
        },
        {
            key: "phrase_organic",
            value: "phrase_organic",
            description: "List of domains that are ranking in Google's top 100 organic search results with a requested keyword."
        },
        {
            key: "phrase_related",
            value: "phrase_related",
            description: "Extended list of related keywords, synonyms and variations relevant to a queried term in a chosen database."
        },
        {
            key: "phrase_these",
            value: "phrase_these",
            description: "Summary of up to 100 keywords, including volume, CPC, competition and the number of results in a chosen regional database."
        },
        {
            key: "phrase_this",
            value: "phrase_this",
            description: "Summary of a keyword, including volume, CPC, competition and the number of results in a chosen regional database."
        }
    ];

    constructor() {
        super();
        this.consumeContext(SEMRUSH_CONTEXT_TOKEN, (context) => {
            if (!context) return;
            this.#semrushContext = context;
        });

        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            if (!instance) return;
            this.#modalManagerContext = instance;
        });

        this.consumeContext(UMB_CURRENT_USER_CONTEXT, (instance) => {
            if (!instance) return;
            this.#currentUserContext = instance;
        });
    }

    async connectedCallback() {
        super.connectedCallback();

        await this.#validateToken();
        await this.#getDatasource();
        await this.#getAuthUrl();
        await this.#getContentProperties();
    }

    async #getContentProperties(){
        var contentId = window.location.pathname.split("/")[7];
        var { data } = await this.#semrushContext.getCurrentContentProperties(contentId);
        if (!data) return;

        this.propertyList = data;
    }

    async #getDatasource(){
        var result = await this.#semrushContext.getDataSources();
        if (!result) return;

        this.datasourceList = result.data?.items;
    }

    async #getAuthUrl(){
        var result = await this.#semrushContext.getAuthorizationUrl();
        if (!result) return;

        this.authUrl = result.data!;
    }

    async #validateToken(){
        var result = await this.#semrushContext.validateToken();
        if (!result) return;

        if (result.data?.isAuthorized){
            if (!result.data?.isValid){
                await this.#semrushContext.refreshAccessToken();
                return;
            }

            this.account = result.data!;
            this.requestUpdate();
            this.dispatchEvent(new CustomEvent('property-value-change'));
        }
    }

    async _search(){
        const result = await this.#semrushContext.getRelatedPhrases(this.searchPhrase, this._currentPageNumber, this.selectedDatasource, this.selectedMethod);
        if (!result) return;

        this.keywordList = result.data;
        this._totalPages = result.data?.totalPages!;
        this.#createTableColumns(this.keywordList?.data!);
        this.#createTableItems(this.keywordList?.data!);
    }

    _searchNew(){
        this.searchPhrase = "";
        this.selectedDatasource = "";
        this.selectedMethod = "";
        this.selectedproperty = "";
        this.keywordList = undefined;
    }

    #createTableColumns(phraseList: RelatedPhrasesDataDtoModel) {
        const tableColumns = phraseList.columnNames.map((c) => ({
            name: c,
            alias: c.toLowerCase().trim(),
          }));
    
        this._tableColumns = tableColumns;
      }

    #createTableItems(phraseList: RelatedPhrasesDataDtoModel) {
		this._tableItems = phraseList.rows.map((entry) => {
            const fields = entry.map((f, idx) => ({
                columnAlias: this._tableColumns[idx].alias,
                value: f,
            }));
      
            return {
                id: "",
                data: fields,
            };
        });
	}

    async _onConnect(){
        var authWindow = window.open(this.authUrl, "Semrush_Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
        window.addEventListener("message", async (event: MessageEvent) => {
            if (event.data.type === "semrush:oauth:success") {
                var codeParam = "?code=";
                
                if (authWindow) authWindow.close();

                var code = event.data.url.slice(event.data.url.indexOf(codeParam) + codeParam.length);
                var result = await this.#semrushContext.getAccessToken(code);
                if (!result) return;

                if (result.data !== "error"){
                    await this.#validateToken();

                    this._showSuccess("Access Approved.");
                }else{
                    this._showError("Access Denied.");
                }
            }else{
                this._showError("Access Denied.");
                authWindow!.close();
            }
        }, false);
    }

    #onPropertySelectChange(e: UUISelectEvent) {
        this.selectedproperty = e.target.value.toString();
        this.searchPhrase = this.selectedproperty;
        this.requestUpdate();
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    #onDatasourceSelectChange(e: UUISelectEvent) {
        this.selectedDatasource = e.target.value.toString();
        this.requestUpdate();
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    #onMethodSelectChange(e: UUISelectEvent) {
        this.selectedMethod = e.target.value.toString();
        this.requestUpdate();
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    #onInputChange(e: UUIInputEvent){
        this.searchPhrase = e.target.value.toString();
    }

    #onPageChange(event: UUIPaginationEvent) {
        const currentPageNumber = event.target?.current
        this.#paginationManager.setCurrentPageNumber(currentPageNumber);
        this._currentPageNumber = currentPageNumber;
        this._search();
    }

    private async _showSuccess(message: string) {
        await this._showMessage(message, "positive");
      }
    
    private async _showError(message: string) {
        await this._showMessage(message, "danger");
    }

    private async _showMessage(message: string, color: UmbNotificationColor) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek(color, {
            data: { message },
        });
    }

    private async isAdmin(){
        return await this.#currentUserContext.isCurrentUserAdmin();
    }

    private async _openModal() {
        const pickerContext = this.#modalManagerContext?.open(this, SEMRUSH_MODAL_TOKEN, {
            data: {
                headline: "Authorization",
                authResponse: this.account
            },
        });
    
        const data = await pickerContext?.onSubmit();
        if (!data) return;

        this.account = data.authResponse!;
        this._showSuccess("Access Revoked.");
        this.dispatchEvent(new CustomEvent('property-value-change'));
    }

    #renderPagination() {
        return html`
            ${this._totalPages > 1
             ? html`
                <div class="shopify-pagination">
                    <uui-pagination
					    class="pagination"
					    .current=${this._currentPageNumber}
					    .total=${this._totalPages}
					    @change=${this.#onPageChange}></uui-pagination>
                </div>
             `
             : nothing}
        `;
    }

    render(){
        return html`
            <umb-body-layout>
                <uui-box headline="Content Properties">
                    <div class="semrush-content">
                        <p>
                            Semrush is a marketing SaaS platform that provides online visibility management and content marketing through
                            a single platform, on all key channels.
                        </p>
                        <p>
                            It analyzes the data from the world's largest database of 20 billion keywords, 310 million ads and 17 billion
                            URLs crawled per day, and gives you instant recommendations on SEO, content marketing and advertising that can
                            help you improve your online visibility in days.
                        </p>
                        <p>
                            Keyword search is a powerful tool that runs a full analysis of your keyword and helps you decide whether you
                            should enter into competition for it. As an integral part of any digital marketing strategy, it's importance
                            will never fade away.
                        </p>
                        <p>
                            <uui-icon class="semrush-autofill-icon" name="icon-autofill"></uui-icon>
                            <a href="https://www.semrush.com" target="_blank">
                            read more
                            </a>
                        </p>
                        ${when(this.isAdmin(), () => html`
                            <p>
                                You need to be logged in against Semrush in order take advantage of the tool's full potential. If you cannot do that,
                                please contact one of the administrators.
                            </p> 
                        `)}
                        <p>
                            You can enable the keywords lookup tool by picking one of the content fields or choosing a new blank search from the
                            controls below:
                        </p>
                    </div>
                    <div>
                        <uui-select
                              placeholder="Select content property"
                              @change=${(e : UUISelectEvent) => this.#onPropertySelectChange(e)}
                              .options=${
                                this.propertyList?.map((ft) => ({
                                  name: ft.propertyName,
                                  value: ft.propertyValue,
                                  selected: ft.propertyValue === this.selectedproperty,
                                }))}></uui-select>
                        <span>or</span>
                        <uui-button label="Search new" look="secondary" @click=${this._searchNew}></uui-button>
                    </div>
                </uui-box>

                <br/>

                <uui-box headline="Keyword Search">
                    <uui-button 
                        slot="header-actions" 
                        look=${this.account.isAuthorized ? "secondary" : "primary"} 
                        @click=${this._onConnect} 
                        ?disabled=${this.account.isAuthorized} 
                        class="semrush-connect-button">Connect</uui-button>

                    <uui-button 
                        slot="header-actions" 
                        look=${!this.account.isAuthorized ? "secondary" : "primary"} 
                        @click=${this._openModal}>Status</uui-button>

                    <div>
                        <uui-input .value=${this.searchPhrase} @change=${(e : UUIInputEvent) => this.#onInputChange(e)}></uui-input>
                        <uui-select 
                            placeholder="Please select a data source"
                            @change=${(e : UUISelectEvent) => this.#onDatasourceSelectChange(e)}
                            .options=${
                                this.datasourceList?.map((ft) => ({
                                  name: ft.region,
                                  value: ft.code,
                                  selected: ft.code === this.selectedDatasource,
                                })) ?? []}></uui-select>
                        <uui-select 
                            placeholder="Please select a method"
                            @change=${(e : UUISelectEvent) => this.#onMethodSelectChange(e)}
                            .options=${
                                this.methodList?.map((ft) => ({
                                  name: ft.key,
                                  value: ft.value,
                                  selected: ft.value === this.selectedMethod,
                                }))}></uui-select>
                        <uui-button label="Search keywords" look="primary" @click=${this._search} ?disabled=${!this.account.isAuthorized}></uui-button>
                    </div>

                    ${when(this.keywordList?.data !== undefined, () => html`
                        <div class="semrush-table">
                            <umb-table 
                                .columns=${this._tableColumns} 
                                .items=${this._tableItems}></umb-table>
                        </div>

                        ${(!this.account.isFreeAccount 
                            ? html`
                                <div>
                                    <p>
                                        Because you are using a free account, the number of results is limited to 10 records.
                                        Please upgrade your subscription for enhanced results.
                                    </p>
                                </div> 
                            `   
                            : html`
                                ${this.#renderPagination()}
                            `
                        )}

                        <a href="https://www.semrush.com/analytics/keywordoverview" target="_blank">
                            Get more insights at Semrush
                        </a>
                    `)}
                </uui-box>
            </umb-body-layout>
        `;
    }

    static styles = [
        css`
            .semrush-content p:first-child{
                margin-top: 0 !important;
            }

            .semrush-table{
                margin: 15px 0;
            }

            .semrush-connect-button{
                margin-right: 2px;
            }

            .semrush-autofill-icon{
                margin-bottom: 4px;
            }
        `];
}
export default SemrushWorkspaceElement;

declare global {
	interface HTMLElementTagNameMap {
		[elementName]: SemrushWorkspaceElement;
	}
}