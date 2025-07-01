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
import type { UUIInputEvent } from "@umbraco-cms/backoffice/external/uui";
import { ALGOLIA_CONTEXT_TOKEN } from '@umbraco-integrations/algolia/context';
import type {
    IndexConfigurationModel,
    ContentTypeDtoModel
} from "@umbraco-integrations/algolia/generated";

const elementName = "algolia-index";

@customElement(elementName)
export class AlgoliaIndexElement extends UmbElementMixin(LitElement) {
    #algoliaIndexContext!: typeof ALGOLIA_CONTEXT_TOKEN.TYPE;

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
    private _showContentTypeProperties = false;

    constructor() {
        super();

        this.consumeContext(ALGOLIA_CONTEXT_TOKEN, (context) => {
            if (!context) return;

            this.#algoliaIndexContext = context;
        });
    }

    async connectedCallback() {
        super.connectedCallback();

        if (this.indexId.length > 0) {
            await this.#getContentTypesWithIndex();
            this.#getIndex();
        }
        else {
            this.#getContentTypes();
        }
    }
    
    async #getContentTypes() {
        const { data } = await this.#algoliaIndexContext.getContentTypes();
        if (!data) return;

        this._contentTypes = data;
    }

    async #getContentTypesWithIndex() {
        const { data } = await this.#algoliaIndexContext.getContentTypesWithIndex(Number(this.indexId));
        if (!data) return;

        this._contentTypes = data;
    }

    async #getIndex() {
        const { data } = await this.#algoliaIndexContext.getIndexById(Number(this.indexId));
        if (!data) return;

        this._model = data;

        if (this._model.contentData.length) {
            this._showContentTypeProperties = true;
        }
    }    

    #handleNameChange(e: UUIInputEvent) {
        this._model.name = e.target.value.toString();
    }

    async #contentTypeSelected(id: number) {
        this._contentTypes = this._contentTypes.map((obj) => {
            if (obj.id === id) {
                obj.selected = true;
            }
            return obj;
        });

        this._showContentTypeProperties = true;
    }

    async #contentTypeDeselected(id: number) {
        this._contentTypes = this._contentTypes.map((obj) => {
            if (obj.id === id) {
                obj.selected = false;
            }
            return obj;
        });

        this._showContentTypeProperties = this._contentTypes.filter(x => x.selected).length !== 0;
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

        if (this._model.name.length == 0 || this._contentTypes === undefined || this._contentTypes?.filter(obj => obj.selected).length == 0) {
            this.#showError("Index name and content schema are required.");
            return;
        }

        const indexConfiguration: IndexConfigurationModel = {
            id: 0,
            name: this._model.name,
            contentData: []
        };

        if (this.indexId.length > 0) {
            indexConfiguration.id = Number(this.indexId);
        }

        indexConfiguration.contentData = this._contentTypes;

        await this.#algoliaIndexContext?.saveIndex(indexConfiguration);
    }

    // notifications
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
                    <uui-ref-node 
                        selectable
                        ?selected=${contentType.selected}
                        name=${contentType.name}
                        @selected=${() => this.#contentTypeSelected(contentType.id)}
                        @deselected=${() => this.#contentTypeDeselected(contentType.id)}>
                        <umb-icon slot="icon" name=${contentType.icon}></umb-icon>
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

        const selectedContentTypes = this._contentTypes.filter((obj) => obj.selected == true);

        if (!selectedContentTypes?.length) return nothing;

        return html`
            ${selectedContentTypes.map(selectedContentType => html`
                <uui-form-layout-item>
                    <uui-label slot="label">${selectedContentType.name} Properties</uui-label>
                        <div id="grid">
                            ${selectedContentType.properties.map((property) => {
                                return html`
                                    <uui-card-content-node 
                                        selectable
                                        ?selected=${property.selected}
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
            `)}
        `;
    }

    render() {
        return html`
            <uui-box headline=${this.indexId.length > 0 ? "Create Index Definition" : "Edit Index Definition"}>
                <uui-form>
                    <form id="manageIndexFrm" name="manageIndexFrm" @submit=${this.#handleSubmit}>
                        <umb-property-layout 
                            label="Name" 
                            description="Please enter a name for the index. After save, you will not be able to change it."> 
                            <uui-input 
                                slot="editor" 
                                ?disabled=${this.indexId.length > 0} 
                                .value=${this._model.name}
                                @change=${this.#handleNameChange}></uui-input>                                
                        </umb-property-layout>

                        <umb-property-layout 
                            label="Document Types" 
                            description="Please select the document types you would like to index, and choose the fields to include.">
                            <div slot="editor">
                                ${this.renderContentTypes()}
                                ${this.renderContentTypeProperties()}
                            </div>
                        </umb-property-layout>                        
          
                        <uui-button type="submit" label=${this.localize.term("buttons_save")} look="primary" color="positive"></uui-button>
                    </form>
                </uui-form>
            </uui-box>
        `;
    }

    static styles = [
        css`
          #grid {
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