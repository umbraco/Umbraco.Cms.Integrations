import { 
  LitElement,
  html, 
  css,
  customElement,
  state,
  nothing
} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

import type { UmbModalManagerContext } from "@umbraco-cms/backoffice/modal";
import { UMB_MODAL_MANAGER_CONTEXT, UMB_CONFIRM_MODAL } from "@umbraco-cms/backoffice/modal";

import {
  UmbNotificationContext,
  UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";

import type { AlgoliaIndexConfigurationModel } from "../models/AlgoliaIndexConfigurationModel";
import { AlgoliaIndexResource } from "../services/AlgoliaIndexResource";

@customElement("algolia-dashboard-overview")
export class AlgoliaDashboardOverviewElement extends UmbElementMixin(LitElement) {
  #modalManagerContext?: UmbModalManagerContext;
  #notificationContext?: UmbNotificationContext;

  @state()
  private _loading = false;

  @state()
  private _indices: Array<AlgoliaIndexConfigurationModel> = [];
  
  constructor() {
      super();
      this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
          this.#modalManagerContext = instance;
      });
      this.consumeContext(UMB_NOTIFICATION_CONTEXT, (_instance) => {
        this.#notificationContext = _instance;
      });
  }

  connectedCallback() {
    super.connectedCallback();
    this._getIndices();
  }

  private async _getIndices() {
    this._loading = true;
    await AlgoliaIndexResource.getIndices()
      .then(response => {
        this._indices = response;
        this._loading = false;
      })
      .catch(error => this._showError(error.message));
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
            @click="${() => window.history.pushState({}, '', window.location.href.replace(/\/+$/, '') + '/index') }"></uui-button>
        </div>
        ${this._loading ? html`<div class="center"><uui-loader></uui-loader></div>` : ''}
        ${this.renderIndicesList()}
      </uui-box>
    `;
  }

  private renderIndicesList() {
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
                <uui-icon-registry-essential>
                    ${index.contentData.map((contentData) => {
                      return html`
                        <uui-ref-node name=${contentData.name}
                                  detail=${contentData.properties.map(obj => obj.name).join(', ')}>
                          <uui-icon slot="icon" name=${contentData.icon}></uui-icon>
                        </uui-ref-node>
                      `;
                    })}
                    
                </uui-icon-registry-essential>
            </uui-table-cell>
            <uui-table-cell>
                <uui-icon-registry-essential>
                    <uui-action-bar>
                        <uui-button label="edit" look="default" color="default"
                            @click="${() =>  window.history.pushState({}, '', window.location.href.replace(/\/+$/, '') + '/index/' + index.id) }">
                            <uui-icon name="edit"></uui-icon>
                        </uui-button>
                        <uui-button label="build" look="default" color="danger" @click="${() => this._onBuildIndex(index)}">
                            <uui-icon name="sync"></uui-icon>
                        </uui-button>
                        <uui-button label="search" look="default" color="positive" 
                            @click="${() =>  window.history.pushState({}, '', window.location.href.replace(/\/+$/, '') + '/search/' + index.id) }">
                            <uui-icon name="search"></uui-icon>
                        </uui-button>
                        <uui-button label="delete" look="default" color="default" @click="${() => this._onDeleteIndex(index)}">
                            <uui-icon name="delete"></uui-icon>
                        </uui-button>
                    </uui-action-bar>
                </uui-icon-registry-essential>
            </uui-table-cell>
            </uui-table-row>
          `;
        })}
        </uui-table>
    `;
  }

  private async _onBuildIndex(index: AlgoliaIndexConfigurationModel) {
    const modalContext = this.#modalManagerContext?.open(UMB_CONFIRM_MODAL, {
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
    modalContext?.onSubmit()
        .then(async () => {
            this._loading = true;
            
            await AlgoliaIndexResource.buildIndex(index)
              .then(response => {
                if (response.success) {
                  this._showSuccess("Index built.");
                }
                else {
                  this._showError(response.error);
                }
              })
              .catch(error => this._showError(error));
            
            this._loading = false;
        })
        .catch(error => this._showError(error));
  }

  private async _onDeleteIndex(index: AlgoliaIndexConfigurationModel) {
    const modalContext = this.#modalManagerContext?.open(UMB_CONFIRM_MODAL, {
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
    modalContext?.onSubmit()
        .then(async () => {
            if (index == undefined || index.id == undefined) return;

            this._loading = true;
            
            var response = await AlgoliaIndexResource.deleteIndex(index.id);            
            
            this._loading = false;

            if (response.success) {
              this._getIndices();
              this._showSuccess("Index deleted");
            }
            else
            {
              this._showError(response.error);
            }

            this._loading = false;
        })
        .catch((error) => {
          this._showError(error.message);
        });
  }

  // notifications
  private _showSuccess(message: string) {
    this.#notificationContext?.peek("positive", {
      data: { message: message },
    });
  }

  private _showError(message: string) {
    this.#notificationContext?.peek("danger", {
      data: { message: message },
    });
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
    'algolia-dashboard-overview': AlgoliaDashboardOverviewElement
  }
}