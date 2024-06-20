import {
    LitElement,
    html,
    css,
    customElement,
    state,
    nothing
} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

import { UMB_MODAL_MANAGER_CONTEXT, UMB_CONFIRM_MODAL } from "@umbraco-cms/backoffice/modal";
import {    
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";

import {
    IndexConfigurationModel,
    ResultModel
} from "@umbraco-integrations/algolia/generated";
import AlgoliaIndexContext, { ALGOLIA_CONTEXT_TOKEN } from "../../../context/algolia-index.context";

const elementName = "algolia-dashboard-overview";

@customElement(elementName)
export class AlgoliaDashboardOverviewElement extends UmbElementMixin(LitElement) {
    #algoliaIndexContext?: AlgoliaIndexContext;

    @state()
    private _loading = false;

    @state()
    private _indices: Array<IndexConfigurationModel> = [];

    constructor() {
        super();

        this.consumeContext(ALGOLIA_CONTEXT_TOKEN, (_instance) => {
            this.#algoliaIndexContext = _instance;
        });
    }

    connectedCallback() {
        super.connectedCallback();
        this.#getIndices();
    }

    async #getIndices() {
        this._loading = true;

        await this.#algoliaIndexContext?.getIndices()
            .then(response => {
                this._indices = response as Array<IndexConfigurationModel>;
                this._loading = false;
            })
            .catch(error => this.#showError(error.message));
    }  

    async #onBuildIndex(index: IndexConfigurationModel) {
        const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
        const modalContext = modalManager.open(
            this, UMB_CONFIRM_MODAL,
            {
                data: {
                    headline: `Build Index : ${index.name}`,
                    content: html`
                      <p class="umb-alert umb-alert--warning mb2">
                        This will cause the index to be built.<br />
                        Depending on how much content there is in your site this could take a while.<br />
                        It is not recommended to rebuild an index during times of high website traffic
                        or when editors are editing content.
                      </p>`,
                    color: "danger",
                    confirmLabel: "Ok",
                }
            }
        );
        await modalContext.onSubmit().catch(() => undefined);

        this._loading = true;

        await this.#algoliaIndexContext?.buildIndex(index)
            .then(response => {
                var result = response as ResultModel;
                if (result.success) {
                    this.#showSuccess("Index built.");
                }
                else {
                    this.#showError(result.error);
                }
            })
            .catch(error => this.#showError(error));

        this._loading = false;
    }

    async #onDeleteIndex(index: IndexConfigurationModel) {
        const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
        const modalContext = modalManager.open(
            this, UMB_CONFIRM_MODAL,
            {
                data: {
                    headline: `Delete Index`,
                    content: html`
                      <p class="umb-alert umb-alert--warning mb2">
                        Are you sure you want to delete index <b>${index.name}</b>?
                      </p>`,
                    color: "danger",
                    confirmLabel: "Ok",
                }
            }
        );
        modalContext?.onSubmit().catch((error) => this.#showError(error.message));

        if (index == undefined || index.id == undefined) return;

        this._loading = true;

        await this.#algoliaIndexContext?.deleteIndex(index.id)
            .then(response => {
                var result = response as ResultModel;

                if (result.success) {
                    this.#getIndices();
                    this.#showSuccess("Index deleted");
                }
                else {
                    this.#showError(result.error);
                }

            })
            .catch(error => this.#showError(error));

        this._loading = false;
            
    }

    // notifications
    async #showSuccess(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("positive", {
            data: { message: message },
        });
    }

    async #showError(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("danger", {
            data: { message: message },
        });
    }

    #renderIndicesList() {
        if (this._indices.length == 0) return nothing;

        return html`
          <uui-table aria-label="Indices Table" style="width: 70%">
            <uui-table-column style="width: 20%;"></uui-table-column>
            <uui-table-column style="width: 60%;"></uui-table-column>
            <uui-table-column style="width: 20%;"></uui-table-column>

            <uui-table-head>
                <uui-table-head-cell>Name</uui-table-head-cell>
                <uui-table-head-cell>Definition</uui-table-head-cell>
                <uui-table-head-cell></uui-table-head-cell>
            </uui-table-head>
            ${this._indices.map((index) => {
                return html`
                <uui-table-row>
                  <uui-table-cell>${index.name}</uui-table-cell>
                  <uui-table-cell>
                        ${index.contentData.map((contentData) => {
                            return html`
                                <uui-ref-node name=${contentData.name}
                                            detail=${contentData.properties.map(obj => obj.name).join(', ')}>
                                    <uui-icon slot="icon" name=${contentData.icon}></uui-icon>
                                </uui-ref-node>
                                `;
                })}
                    
                </uui-table-cell>
                <uui-table-cell>
                    <uui-action-bar>
                        <uui-button label="edit" look="default" color="default"
                            @click="${() => window.history.pushState({}, '', window.location.href.replace(/\/+$/, '') + '/index/' + index.id)}">
                            <uui-icon name="edit"></uui-icon>
                        </uui-button>
                        <uui-button label="build" look="default" color="danger" @click="${() => this.#onBuildIndex(index)}">
                            <uui-icon name="sync"></uui-icon>
                        </uui-button>
                        <uui-button label="search" look="default" color="positive" 
                            @click="${() => window.history.pushState({}, '', window.location.href.replace(/\/+$/, '') + '/search/' + index.id)}">
                            <uui-icon name="search"></uui-icon>
                        </uui-button>
                        <uui-button label="delete" look="default" color="default" @click="${() => this.#onDeleteIndex(index)}">
                            <uui-icon name="delete"></uui-icon>
                        </uui-button>
                    </uui-action-bar>
                </uui-table-cell>
                </uui-table-row>
              `;
            })}
            </uui-table>
        `;
    }

    render() {
        return html`
            <uui-box headline="Algolia Indices">
            <div>
                <h5>Manage Algolia Indices</h5>
                <p>
                    Algolia is an AI-powered search and discovery platform allowing you to create cutting-edge customer experiences for their websites or mobile apps.
                    It's like the perfect mediator between your website and customers, making sure the conversation is as smooth and efficient as possible.
                </p>
                <p>
                    The Algolia model provides Search as a Service through an externally hosted search engine, offering web search across the website based
                    on the content payload pushed from the website to Algolia.
                </p>
                <p>
                    To get started, you need to create an index and define the content schema - document types and properties.
                    Then you can build your index, push data to Algolia and run searches across created indices.
                    <br />
                    <a style="text-decoration: underline" target="_blank" href="https://www.algolia.com/doc/guides/getting-started/quick-start/">
                        Read more about integrating Algolia Search
                    </a>
                </p>
            </div>
            <div>
                <uui-button look="primary" color="default" label="Add New Index Definition" 
                @click="${() => window.history.pushState({}, '', window.location.href.replace(/\/+$/, '') + '/index')}"></uui-button>
            </div>
            ${this._loading ? html`<div class="center"><uui-loader></uui-loader></div>` : ''}
            ${this.#renderIndicesList()}
            </uui-box>
        `;
    }

    static styles = [
        css`
      .center {
        display: grid;
        place-items: center;
      }
      .error {
        color: var(--uui-color-danger);
      }
    `,
    ];
}

export default AlgoliaDashboardOverviewElement;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: AlgoliaDashboardOverviewElement
    }
}