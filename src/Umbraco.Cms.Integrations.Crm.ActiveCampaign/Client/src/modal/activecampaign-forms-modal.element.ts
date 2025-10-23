import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { ActiveCampaignFormPickerModalData, ActiveCampaignFormPickerModalValue } from "./activecampaign.modal-token";
import { css, customElement, html, nothing, repeat, state } from "@umbraco-cms/backoffice/external/lit";
import { ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN } from "../context/activecampaign-forms.context";
import { ApiAccessDtoModel, FormDtoModel } from "@umbraco-integrations/activecampaign-forms/generated";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import { UUIInputEvent, UUIPaginationEvent } from "@umbraco-cms/backoffice/external/uui";

const elementName = "activecampaign-forms-modal";

@customElement(elementName)
export default class ActiveCampaignFormsModalElement
    extends UmbModalBaseElement<ActiveCampaignFormPickerModalData, ActiveCampaignFormPickerModalValue> {
    #activecampaignFormsContext!: typeof ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN.TYPE;
    #configurationModel?: ApiAccessDtoModel;

    @state()
    private _loading = false;

    @state()
    private _forms: Array<FormDtoModel> = [];

    @state()
    private _filteredForms: Array<FormDtoModel> = [];

    @state()
    _currentPageNumber = 1;

    @state()
    _totalPages = 1;

    @state()
    _searchQuery = "";

    #filterTimeout?: NodeJS.Timeout;

    constructor() {
        super();

        this.consumeContext(ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN, (context) => {
            if (!context) return;

            this.#activecampaignFormsContext = context;
            this.observe(context.configurationModel, (configurationModel) => {
                this.#configurationModel = configurationModel;
            });
        });
    }

    async connectedCallback() {
        super.connectedCallback();
        this.#checkApiAccess();
    }

    async #checkApiAccess() {
        if (!this.#activecampaignFormsContext || !this.#configurationModel) return;

        if (!this.#configurationModel.isApiConfigurationValid) {
            this._showError("Invalid API configuration.");
            return;
        }

        await this.#loadForms();
    }

    async #loadForms(page?: number, searchQuery?: string) {
        this._loading = true;

        const { data } = await this.#activecampaignFormsContext.getForms(page, searchQuery);
        if (!data) {
            this._loading = false;
            return;
        }

        this._totalPages = data.meta.totalPages;

        this._forms = data.forms ?? [];
        this._filteredForms = this._forms;

        this._loading = false;
    }

    async #handleFilterInput(event: UUIInputEvent) {
        let query = (event.target.value as string) || '';
        query = query.toLowerCase();
        this._searchQuery = query;

        // Clear existing timeout
        if (this.#filterTimeout) {
            clearTimeout(this.#filterTimeout);
        }

        this.#filterTimeout = setTimeout(async () => {
            this._currentPageNumber = 1;
            await this.#loadForms(this._currentPageNumber, this._searchQuery);
        }, 2000);
    }

    async #onPageChange(event: UUIPaginationEvent) {
        this._currentPageNumber = event.target?.current;

        await this.#loadForms(this._currentPageNumber, this._searchQuery);
    }

    #renderPagination() {
        return html`
            ${this._totalPages > 1
                ? html`
                    <div class="activecampaign-pagination">
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

    private _renderFilter() {
        return html` <uui-input
			type="search"
			id="filter"
			@input="${this.#handleFilterInput}"
			placeholder="Type to filter..."
			label="Type to filter forms">
			<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
		</uui-input>`;
    }

    private _onSelect(form: FormDtoModel) {
        this.value = { form };
        this._submitModal();
    }

    private async _showError(message: string) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek("danger", {
            data: { message },
        });
    }

    private _renderForm(form: FormDtoModel) {
        return html`
            <uui-ref-node-form
                selectable
                name=${form.name ?? ""}
                @selected=${() => this._onSelect(form)}>
            </uui-ref-node-form>
        `;
    }

    render() {
        return html`
            <umb-body-layout>
                <uui-box headline=${this.data!.headline}>
                    ${this._renderFilter()}
                    ${this._loading ? html`<div class="center"><uui-loader></uui-loader></div>` : ""}
                    ${repeat(this._filteredForms, (form) => this._renderForm(form))}
                    ${this.#renderPagination()}
                </uui-box>

                <uui-button slot="actions" label=${this.localize.term("general_close")} @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }

    static styles = [
        css`
            uui-box {
                margin-bottom: var(--uui-size-8);
            }

            #filter {
                width: 100%;
                margin-bottom: var(--uui-size-3);
            }

            uui-icon {
                margin: auto;
                margin-left: var(--uui-size-2);
            }

            .center {
                display: grid;
                place-items: center;
            }

            .activecampaign-pagination {
                width: 50%;
                margin-top: 10px;
                margin-left: auto;
                margin-right: auto;
            }
        `];
}