import {
    LitElement,
    html,
    css,
    customElement,
    property,
    state,
    query
} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import {
    UmbNotificationContext,
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";

import { IndexConfigurationModel, ResponseModel } from "@umbraco-integrations/algolia/generated";
import AlgoliaIndexContext, { ALGOLIA_CONTEXT_TOKEN } from "../../../context/algolia-index.context";

@customElement("algolia-search")
export class AlgoliaSearchElement extends UmbElementMixin(LitElement) {
    #notificationContext?: UmbNotificationContext;
    #algoliaIndexContext?: AlgoliaIndexContext;

    @property()
    indexId!: string;

    @query('#search-input')
    private _searchInput!: HTMLInputElement;

    @state()
    index: IndexConfigurationModel = {
        id: Number(this.indexId),
        name: "",
        contentData: []
    };

    @state()
    indexSearchResult: ResponseModel = {
        itemsCount: 0,
        pagesCount: 0,
        itemsPerPage: 0,
        hits: []
    };

    connectedCallback() {
        super.connectedCallback();
        this._getIndex();
    }

    constructor() {
        super();
        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (_instance) => {
            this.#notificationContext = _instance;
        });
        this.consumeContext(ALGOLIA_CONTEXT_TOKEN, (_instance) => {
            this.#algoliaIndexContext = _instance;
        });
    }

    render() {
        return html`
            <uui-box headline="Search">
                <small slot="header">Please enter the query you want to search by against index <b>${this.index.name}</b></small>
                <div class="flex">
                    <uui-input
                            type="search"
                            id="search-input"
                            placeholder="Type to filter..."
                            label="Type to filter"
                            @keypress=${this._onKeyPress}>
                    </uui-input>
                    <uui-button color="positive" look="primary" label="Search" @click="${this._onSearch}"> Search </uui-button>
                </div>
                <!--RESULTS -->
                <div>
                    <p>Items Count: ${this.indexSearchResult.itemsCount}</p>
                    <p>Pages Count: ${this.indexSearchResult.pagesCount}</p>
                    <p>Items per Page: ${this.indexSearchResult.itemsPerPage}</p>
                    ${this.indexSearchResult.hits.map((obj) => {
                            return html`
                                <div>
                                    ${Object.entries(obj).map((entry) => {
                                        return html`
                                            <p>
                                                <b>${entry[0]}</b> : ${entry[1]}
                                            </p>
                                        `;
                                    })}                                
                                </div>
                            `;
                    })}
                </div>
            </uui-box>
        `;
    }

    private async _getIndex() {
        await this.#algoliaIndexContext?.getIndexById(Number(this.indexId))
            .then(response => this.index = response as IndexConfigurationModel)
            .catch(error => this._showError(error.message));
    }

    private _onKeyPress(e: KeyboardEvent) {
        e.key == 'Enter' ? this._onSearch() : undefined;
    }

    private async _onSearch() {
        if (!this._searchInput.value.length) return;

        await this.#algoliaIndexContext?.searchIndex(Number(this.indexId), this._searchInput.value)
            .then(response => {
                this.indexSearchResult = response as ResponseModel;
            })
            .catch((error) => this._showError(error));
    }

    // notifications
    private _showError(message: string) {
        this.#notificationContext?.peek("danger", {
            data: { message: message },
        });
    }

    static styles = [
        css`
            uui-box p {
                margin-top: 0;
            }
            div.flex {
                display: flex;
            }
            div.flex > uui-button {
                padding-left: var(--uui-size-space-4);
                height: 0;
            }
            uui-input {
                width: 100%;
                margin-bottom: var(--uui-size-space-5);
            }
        `,
    ];
}

export default AlgoliaSearchElement;

declare global {
    interface HTMLElementTagNameMap {
        'algolia-search': AlgoliaSearchElement
    }
}