import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { css, html, nothing, customElement, state } from '@umbraco-cms/backoffice/external/lit';
import { ZAPIER_CONTEXT_TOKEN } from '../context/zapier.context';
import { GetAllResponse, SubscriptionDtoModel } from '@umbraco-integrations/zapier/generated';
import type { UmbTableColumn, UmbTableItem } from '@umbraco-cms/backoffice/components';
import { UUIPaginationEvent } from '@umbraco-cms/backoffice/external/uui';
import { UmbPaginationManager } from "@umbraco-cms/backoffice/utils";

const elementName = "zapier-management-dashboard";
@customElement(elementName)
export class ZapierManagementDashboardElement extends UmbLitElement {
    #zapierContext?: typeof ZAPIER_CONTEXT_TOKEN.TYPE;
    #paginationManager = new UmbPaginationManager();
    #itemList: GetAllResponse | undefined;
    #isFormsExtensionInstalled: boolean | undefined = false;

    @state()
	private _tableItems: Array<UmbTableItem> = [];

    @state()
    _currentPageNumber = 1;

    @state()
    _totalPages = 1;

    @state()
    _nextPageInfo?: string;

    @state()
    _previousPageInfo?: string;

    @state()
	private _tableColumns: Array<UmbTableColumn> = [
		{
			name: 'Identifer',
			alias: 'identifer'
		},
		{
			name: 'Entity Type',
			alias: 'entityType',
		},
		{
			name: 'Hook URL',
			alias: 'hookUrl'
		}
	];

    constructor() {
        super();
        this.consumeContext(ZAPIER_CONTEXT_TOKEN, (instance) => {
            this.#zapierContext = instance;
        });
    }

    async connectedCallback(){
        super.connectedCallback();

        await this.getAll();
        await this.checkFormsExtension();
    }

    async getAll(){
        debugger;
        const data = await this.#zapierContext?.getAll();
        if (!data) return;

        this.#itemList = data.data;

        const lst : Array<SubscriptionDtoModel> = [{
            entityId: "docType1",
            typeName: "Content",
            hookUrl: "wwww.google.com",
            id: 1,
            subscribeHook: true,
            type: 1
        }, {
            entityId: "docType1",
            typeName: "Content",
            hookUrl: "wwww.google.com",
            id: 1,
            subscribeHook: true,
            type: 1
        }, {
            entityId: "docType1",
            typeName: "Content",
            hookUrl: "wwww.google.com",
            id: 1,
            subscribeHook: true,
            type: 1
        }];

        this.#createTableItems(lst);
    }

    async checkFormsExtension() {
        const data = await this.#zapierContext?.checkFormsExtensionInstalled();
        if (!data) return;

        this.#isFormsExtensionInstalled = data.data;
    }

    #createTableItems(products: Array<SubscriptionDtoModel>) {
		this._tableItems = products.map((product) => {
			return {
                id: product.id.toString(),
                data: [{
                    columnAlias: "identifer",
                    value: product.entityId,
                },
                {
                    columnAlias: "entityType",
                    value: product.type,
                },
                {
                    columnAlias: "hookUrl",
                    value: product.hookUrl,
                }
            ]
            }
		});
	}

    override render() {
		return html`
            <div class="zapier-content">
                <umb-body-layout>
                    <uui-box headline="Content Properties">
                        <p>
                            <a href="https://zapier.com/">Zapier</a> is an online platform that helps you automate workflows by connecting your apps and services you use.
                            This allows you to automate tasks without having to build this integration yourself.
                            When an event happens in one app, Zapier can tell another app to perform (or do) a particular action - no code necessary.
                        </p>
                        <p>
                            The heart of any automation boils down to this simple command: <b>WHEN</b> <span>this happens</span> <b>THEN</b> <span>do that</span>.
                        </p>
                        <p>
                            A Zap is an automated workflow that tells your apps to follow this simple command: "When this happens, do that."
                            Every Zap has a trigger and one or more actions. A trigger is an event that starts a Zap, and an action is what your Zap does for you.
                        </p>
                        <p>
                            Zap triggers use webhooks to execute the actions. Webhooks are automated messages sent from apps when something happens.
                        </p>
                        ${!this.#isFormsExtensionInstalled ? html`
                            <p>
                                You can initiate your automation when a content item of a particular document type is published in Umbraco.
                            </p>
                            ` : html`
                            <p>
                                You can initiate your automation when a content item of a particular document type is published or a form is submitted in Umbraco.
                            </p>
                            `
                        }
                        <p>
                            The integration uses Zapier subscription hook triggers, allowing Zapier to set up and remove hook subscriptions when Zaps are created or removed on the platform.
                        </p>
                    </uui-box>
                </umb-body-layout>
            </div>

            <div>
                <umb-body-layout>
                    <uui-box headline="Registed Subscription Hooks">
                        <umb-table 
                            .columns=${this._tableColumns} 
                            .items=${this._tableItems}>
                        </umb-table>
                        ${this.#renderPagination()}
                    </uui-box>
                </umb-body-layout>
            </div>
		`;
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

    #onPageChange(event: UUIPaginationEvent) {
        const forward = event.target?.current > this._currentPageNumber;

        const currentPageNumber = forward ? this._currentPageNumber + 1 : this._currentPageNumber - 1

        this.#paginationManager.setCurrentPageNumber(currentPageNumber);

        this._currentPageNumber = currentPageNumber;
    }

    static styles = [css`
        .zapier-content p:first-child {
            margin-top: 0 !important;
        }
    `];
}

export default ZapierManagementDashboardElement;

declare global {
	interface HTMLElementTagNameMap {
		[elementName]: ZapierManagementDashboardElement;
	}
}