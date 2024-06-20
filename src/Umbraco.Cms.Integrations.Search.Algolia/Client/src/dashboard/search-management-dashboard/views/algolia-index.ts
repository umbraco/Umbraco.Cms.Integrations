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
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";

import AlgoliaIndexContext, { ALGOLIA_CONTEXT_TOKEN } from "../../../context/algolia-index.context";
import {
    IndexConfigurationModel,
    ContentTypeDtoModel,
    ResultModel
} from "@umbraco-integrations/algolia/generated";

const elementName = "algolia-index";

@customElement(elementName)
export class AlgoliaIndexElement extends UmbElementMixin(LitElement) {
    #algoliaIndexContext?: AlgoliaIndexContext;

    @property()
    indexId!: string;

    @state()
    private _model: IndexConfigurationModel = {
        id: 0,
        name: '',
        contentData: []
    };

    @state()
    private _contentTypes: Array<ContentTypeDtoModel> = [];

    @state()
    private _showContentTypeProperties: boolean;

    constructor() {
        super();

        this.consumeContext(ALGOLIA_CONTEXT_TOKEN, (_instance) => {
            this.#algoliaIndexContext = _instance;
        });

        this._showContentTypeProperties = false;
    }

    connectedCallback() {
        super.connectedCallback();

        if (this.indexId.length > 0) {
            this.#getContentTypesWithIndex();
            this.#getIndex();
        }
        else {
            this.#getContentTypes();
        }
    }
    
    async #getContentTypes() {
        await this.#algoliaIndexContext?.getContentTypes()
            .then(response => {
                this._contentTypes = response as Array<ContentTypeDtoModel>;
            })
            .catch(error => this.#showError(error.message));
    }

    async #getContentTypesWithIndex() {
        await this.#algoliaIndexContext?.getContentTypesWithIndex(Number(this.indexId))
            .then(response => {
                this._contentTypes = response as Array<ContentTypeDtoModel>;
            })
            .catch((error) => this.#showError(error.message));
    }

    async #getIndex() {
        await this.#algoliaIndexContext?.getIndexById(Number(this.indexId))
            .then(response => {
                this._model = response as IndexConfigurationModel;
            })
            .catch(error => this.#showError(error.message));
    }    

    async #contentTypeSelected(id: number) {
        this._contentTypes = this._contentTypes.map((obj) => {
            if (obj.id == id) {
                obj.selected = true;
            }
            return obj;
        });
        this._showContentTypeProperties = true;
    }

    async #contentTypeDeselected(id: number) {
        this._contentTypes = this._contentTypes.map((obj) => {
            if (obj.id == id) {
                obj.selected = false;
            }
            return obj;
        });
        this._showContentTypeProperties = false;
    }

    async #contentTypePropertySelected(contentType: ContentTypeDtoModel | undefined, id: number) {
        if (contentType === undefined) return;

        this._contentTypes = this._contentTypes.map((ctObj) => {
            if (ctObj.id != contentType.id) return ctObj;

            ctObj.properties = ctObj.properties.map((obj) => {
                if (obj.id == id) {
                    obj.selected = true;
                }
                return obj;
            });

            return ctObj;
        });
    }

    async #contentTypePropertyDeselected(contentType: ContentTypeDtoModel | undefined, id: number) {
        if (contentType == undefined) return;
        this._contentTypes = this._contentTypes.map((ctObj) => {
            if (ctObj.id != contentType.id) return ctObj;

            ctObj.properties = ctObj.properties.map((obj) => {
                if (obj.id == id) {
                    obj.selected = false;
                }
                return obj;
            });

            return ctObj;
        });
    }

    async #handleSubmit(e: SubmitEvent) {
        e.preventDefault();

        const form = e.target as HTMLFormElement;
        const formData = new FormData(form);

        var indexName = this.indexId.length > 0
            ? this._model.name
            : formData.get("indexName") as string;

        if (indexName.length == 0 || this._contentTypes === undefined || this._contentTypes.filter(obj => obj.selected).length == 0) {
            this.#showError("Index name and content schema are required.");
            return;
        }

        var indexConfiguration: IndexConfigurationModel = {
            id: 0,
            name: indexName,
            contentData: []
        };

        if (this.indexId.length > 0) {
            indexConfiguration.id = Number(this.indexId);
        }
        indexConfiguration.contentData = this._contentTypes;

        await this.#algoliaIndexContext?.saveIndex(indexConfiguration)
            .then(response => {
                var resultModel = response as ResultModel;
                if (resultModel.success) {
                    this.#showSuccess("Index saved.");

                    const redirectPath = this.indexId.length > 0
                        ? window.location.href.replace(`/index/${this.indexId}`, '')
                        : window.location.href.replace('/index', '');

                    window.history.pushState({}, '', redirectPath);
                } else {
                    this.#showError(resultModel.error);
                }
            })
            .catch((error) => this.#showError(error.message));
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

    // render
    private renderContentTypes() {
        if (this._contentTypes.length == 0) return nothing;
        return html`
            ${this._contentTypes.map((contentType) => {
                return html`
                    <uui-ref-node id="dc_${contentType.alias}_${contentType.id}"
                                selectable
                                name=${contentType.name}
                                @selected=${() => this.#contentTypeSelected(contentType.id)}
                                @deselected=${() => this.#contentTypeDeselected(contentType.id)}>
                        <uui-icon slot="icon" name=${contentType.icon}></uui-icon>
                        ${contentType.selected ? html`<uui-tag size="s" slot="tag" color="positive">Selected</uui-tag>` : ''}
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

        var selectedContentType = this._contentTypes.find((obj) => obj.selected == true);

        if (selectedContentType === undefined) return nothing;

        return html`
            <uui-form-layout-item>
                <uui-label slot="label">${selectedContentType.name} Properties</uui-label>
                    <div class="alg-col-3">
                        ${selectedContentType.properties.map((property) => {
                            return html`
                                <uui-card-content-node selectable
                                            @selected=${() => this.#contentTypePropertySelected(selectedContentType, property.id)}
                                            @deselected=${() => this.#contentTypePropertyDeselected(selectedContentType, property.id)}
                                            name=${property.name}>
                                    ${property.selected ? html`<uui-tag size="s" slot="tag" color="positive">Selected</uui-tag>` : ''}
                                    <ul style="list-style: none; padding-inline-start: 0px; margin: 0;">
                                        <li><span style="font-weight: 700">Group: </span> ${property.group}</li>
                                    </ul>
                                </uui-card-content-node>
                            `;
                        })}
                    </div>
            </uui-form-layout-item>
        `;
    }

    render() {
        return html`
            <uui-box headline=${this.indexId.length > 0 ? "Create Index Definition" : "Edit Index Definition"}>
                <uui-form>
                    <form id="manageIndexFrm" name="manageIndexFrm" @submit=${this.#handleSubmit}>
                        <uui-form-layout-item>
                            <uui-label slot="label" for="inName" required="">Name</uui-label>
                            <span class="alg-description" slot="description">Please enter a name for the index. After save,<br /> you will not be able to change it.</span>
                            <div>
                                <uui-input type="text" name="indexName" label="indexName" ?disabled=${this.indexId.length > 0} .value=${this._model.name} style="width: 17%"></uui-input>                                
                            </div>
                        </uui-form-layout-item>

                        <div class="alg-col-2">
                            <uui-form-layout-item>
                                <uui-label slot="label">Document Types</uui-label>
                                <span class="alg-description" slot="description">Please select the document types you would like to index, and choose the fields to include.</span>
                                ${this.renderContentTypes()}
                            </uui-form-layout-item>
                            ${this.renderContentTypeProperties()}
                        </div>
                        <uui-button type="submit" label=${this.localize.term("buttons_save")} look="primary" color="positive"></uui-button>
                    </form>
                </uui-form>
            </uui-box>
        `;
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
        [elementName]: AlgoliaIndexElement
    }
}