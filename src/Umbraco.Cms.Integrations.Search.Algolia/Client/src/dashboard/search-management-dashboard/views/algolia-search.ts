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
import { ALGOLIA_CONTEXT_TOKEN } from '@umbraco-integrations/algolia/context';
import type { IndexConfigurationModel, ResponseModel } from "@umbraco-integrations/algolia/generated";

const elementName = "algolia-search";

@customElement(elementName)
export class AlgoliaSearchElement extends UmbElementMixin(LitElement) {
    #algoliaIndexContext!: typeof ALGOLIA_CONTEXT_TOKEN.TYPE;

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
        this.#getIndex();
    }

    constructor() {
        super();
        
        this.consumeContext(ALGOLIA_CONTEXT_TOKEN, (context) => {
            if (!context) return;

            this.#algoliaIndexContext = context;
        });
    }    

    async #getIndex() {
        const { data } = await this.#algoliaIndexContext.getIndexById(Number(this.indexId));
        if (!data) return;

        this.index = data;
    }

    #onKeyPress(e: KeyboardEvent) {
        e.key == 'Enter' ? this.#onSearch() : undefined;
    }

    async #onSearch() {
        if (!this._searchInput.value.length) return;

        const { data } = await this.#algoliaIndexContext.searchIndex(Number(this.indexId), this._searchInput.value);
        if (!data) return;

        this.indexSearchResult = data;
    }

    render() {
        return html`
            <uui-box headline="Search">
                <small slot="header">Please enter the query you want to search by against index <strong>${this.index.name}</strong></small>
                <div class="flex">
                    <uui-input
                            type="search"
                            id="search-input"
                            placeholder="Type to filter..."
                            label="Type to filter"
                            @keypress=${this.#onKeyPress}>
                    </uui-input>
                    <uui-button color="positive" look="primary" label=${this.localize.term("general_search")} @click=${this.#onSearch}></uui-button>
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
                                                <strong>${entry[0]}</strong> : ${entry[1]}
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
        [elementName]: AlgoliaSearchElement
    }
}