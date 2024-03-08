import { 
    LitElement,
    html, 
    css,
    customElement,
    property,
    state,
    nothing
} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import {
    UmbNotificationContext,
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";

import { AlgoliaIndexConfigurationModel } from "../models/AlgoliaIndexConfigurationModel";
import type { AlgoliaContentTypeModel } from "../models/AlgoliaContentTypeModel";
import { AlgoliaIndexResource } from "../services/AlgoliaIndexResource";

@customElement("algolia-index")
export class AlgoliaIndexElement extends UmbElementMixin(LitElement) {
    #notificationContext?: UmbNotificationContext;

    @property()
    indexId!: string;

    @state()
    private _model: AlgoliaIndexConfigurationModel = {
        name: '',
        contentData: []
    };

    @state()
    private _contentTypes: Array<AlgoliaContentTypeModel> = [];

    @state()
    private _showContentTypeProperties: boolean;
    
    connectedCallback() {
        super.connectedCallback();
        if (this.indexId.length > 0) {
            this._getContentTypesWithIndex();
            this._getIndex();
        }
        else
        {
            this._getContentTypes();
        }
      }

    constructor() {
        super();
        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (_instance) => {
            this.#notificationContext = _instance;
          });

        this._showContentTypeProperties = false;
    }

    render() {
        return html`
            <uui-box headline="${this.indexId.length > 0 ? "Create Index Definition" : "Edit Index Definition"}">
                <uui-form>
                    <form id="manageIndexFrm" name="manageIndexFrm" @submit=${this.handleSubmit}>
                        <uui-form-layout-item>
                            <uui-label slot="label" for="inName" required="">Name</uui-label>
                            <span class="alg-description" slot="description">Please enter a name for the index. After save,<br /> you will not be able to change it.</span>
                            <div>
                                ${
                                    this.indexId.length > 0
                                        ? html`<uui-input type="text" name="indexName" label="indexName" disabled .value=${this._model.name} style="width: 17%"></uui-input>` 
                                        : html`<uui-input type="text" name="indexName" label="indexName" .value=${this._model.name} style="width: 17%"></uui-input>`
                                }
                                
                            </div>
                        </uui-form-layout-item>

                        <div class="alg-col-2">
                            <uui-form-layout-item>
                                <uui-label slot="label">Document Types</uui-label>
                                <span class="alg-description" slot="description">Please select the document types you would like to index, and choose the fields to include.</span>
                                <uui-icon-registry-essential>
                                    ${this.renderContentTypes()}
                                </uui-icon-registry-essential>
                            </uui-form-layout-item>
                            ${this.renderContentTypeProperties()}
                        </div>
                        <uui-button type="submit" label="Save" look="primary" color="positive">
                            Save
                        </uui-button>
                    </form>
                </uui-form>

            </uui-box>
        `;
    }

    private async _getContentTypes() {
        await AlgoliaIndexResource.getContentTypes()
            .then(response => {
                this._contentTypes = response;
            })
            .catch((error) => this._showError(error.message));
    }

    private async _getContentTypesWithIndex() {
        await AlgoliaIndexResource.getContentTypesWithIndex(Number(this.indexId))
            .then(response => {
                this._contentTypes = response;
            })
            .catch((error) => this._showError(error.message));
    }

    private async _getIndex() {
        await AlgoliaIndexResource.getIndexById(Number(this.indexId))
            .then(response => this._model = response)
            .catch(error => this._showError(error.message));
    }

    // render
    private renderContentTypes() {
        if (this._contentTypes.length == 0) return nothing;
        return html`
            ${this._contentTypes.map((contentType) => {
                return html`
                    <uui-ref-node id="dc_${contentType.alias}_${contentType.id}"
                                selectable
                                name=${contentType.name}
                                @open=${() => this._contentTypeSelected(contentType.id)}
                                @selected=${() => this._contentTypeSelected(contentType.id)}
                                @deselected=${() => this._contentTypeDeselected(contentType.id)}>
                        <uui-icon slot="icon" name=${contentType.icon}></uui-icon>
                        ${contentType.selected ? html`<uui-tag size="s" slot="tag" color="primary">Selected</uui-tag>` : ''}
                        <uui-action-bar slot="actions">
                            <uui-button label="Remove" color="danger">
                                <uui-icon name="delete"></uui-icon>
                            </uui-button>
                        </uui-action-bar>
                </uui-ref-node>
                `;
            })}
        `;
    }
    private renderContentTypeProperties() {
        if (this._showContentTypeProperties === false) return nothing;
        
        var selectedContentType = this._contentTypes.find( (obj) => obj.selected == true);

        if (selectedContentType === undefined) return nothing;

        return html`
            <uui-form-layout-item>
                <uui-label slot="label">${selectedContentType.name} Properties</uui-label>
                <uui-icon-registry-essential>
                    <div class="alg-col-3">
                        ${selectedContentType.properties.map((property) => {
                            return html`
                                <uui-card-content-node selectable
                                            @open=${this._contentTypePropertySelected(selectedContentType, property.id)}
                                            @selected=${() => this._contentTypePropertySelected(selectedContentType, property.id)}
                                            @deselected=${() => this._contentTypePropertyDeselected(selectedContentType, property.id)}
                                            name=${property.name}>
                                    ${
                                        property.selected? html`<uui-tag size="s" slot="tag" color="primary">Selected</uui-tag>` : ''
                                    }
                                    <ul style="list-style: none; padding-inline-start: 0px; margin: 0;">
                                        <li><span style="font-weight: 700">Group: </span> ${property.group}</li>
                                    </ul>
                                </uui-card-content-node>
                            `;
                        })}
                    </div>
                </uui-icon-registry-essential>

            </uui-form-layout-item>
        `;
    }

    private async _contentTypeSelected(id: number) {
        this._contentTypes = this._contentTypes.map( (obj) => {
            if (obj.id == id) {
                obj.selected = true;
            }
            return obj;
        });
        this._showContentTypeProperties = true;
    }     
    private async _contentTypeDeselected (id: number) {
        this._contentTypes = this._contentTypes.map( (obj) => {
            if (obj.id == id) {
                obj.selected = false;
            }
            return obj;
        });
        this._showContentTypeProperties = false;
    }
    private async _contentTypePropertySelected(contentType: AlgoliaContentTypeModel | undefined, id: number) {
        if (contentType === undefined) return;

        this._contentTypes = this._contentTypes.map( (ctObj) => {
            if (ctObj.id != contentType.id) return ctObj;
            
            ctObj.properties = ctObj.properties.map( (obj) => {
                if (obj.id == id) {
                    obj.selected = true;
                }
                return obj;
            });

            return ctObj;
        });
    }     
    private async _contentTypePropertyDeselected(contentType: AlgoliaContentTypeModel | undefined, id: number) {
        if (contentType == undefined) return;
        this._contentTypes = this._contentTypes.map( (ctObj) => {
            if (ctObj.id != contentType.id) return ctObj;
            
            ctObj.properties = ctObj.properties.map( (obj) => {
                if (obj.id == id) {
                    obj.selected = false;
                }
                return obj;
            });

            return ctObj;
        });
    }

    private async handleSubmit(e: SubmitEvent) {
        e.preventDefault();

        const form = e.target as HTMLFormElement;
        const formData = new FormData(form);

        var indexName = formData.get("indexName") as string;

        if (indexName.length == 0 || this._contentTypes === undefined || this._contentTypes.filter( obj => obj.selected).length == 0) {
            this._showError("Index name and content schema are required.");
            return;
        } 

        var indexConfiguration = new AlgoliaIndexConfigurationModel(indexName);
        if (this.indexId.length > 0) {
            indexConfiguration.id = Number(this.indexId);
        }
        indexConfiguration.contentData = this._contentTypes;

        await AlgoliaIndexResource.saveIndex(indexConfiguration)
            .then(response => {
                if (response.success) {
                    this._showSuccess("Index saved.");

                    const redirectPath = this.indexId.length > 0
                        ? window.location.href.replace(`/index/${this.indexId}`, '')
                        : window.location.href.replace('/index', '');
                    
                    window.history.pushState({}, '', redirectPath); 
                } else
                {
                    this._showError(response.error);
                }
            })
            .catch((error) => this._showError(error.message));
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
          .alg-col-2 {
            display: grid;
            grid-template-columns: 25% 60%;
            gap: 20px;
          }
          .alg-col-3 {
            display: grid;
            grid-template-columns: 33% 33% 33%;
            gap: 10px;
          }
        `,
      ];
}

export default AlgoliaIndexElement;

declare global {
    interface HTMLElementTagNameMap {
        'algolia-index': AlgoliaIndexElement
    }
}