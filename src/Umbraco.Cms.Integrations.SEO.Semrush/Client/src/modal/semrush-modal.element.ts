import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { css, html } from "lit";
import { customElement, state } from "lit/decorators.js";
import { SemrushModalData, SemrushModalValue } from "./semrush-modal.token";
import { when } from "lit/directives/when.js";
import { SEMRUSH_CONTEXT_TOKEN } from "../context/semrush.context";

const elementName = "shopify-products-modal";

@customElement(elementName)
export default class ShopifyProductsModalElement extends UmbModalBaseElement<SemrushModalData, SemrushModalValue>{
    #semrushContext!: typeof SEMRUSH_CONTEXT_TOKEN.TYPE;

    @state()
    private token: string = "";

    @state()
    private isTokenAvailable: boolean = false;

    constructor() {
        super();
        this.consumeContext(SEMRUSH_CONTEXT_TOKEN, (context) => {
            if (!context) return;
            this.#semrushContext = context;
        });
    }

    async connectedCallback() {
        super.connectedCallback();

        await this.#getTokenDetail();
    }

    private isAuthorized(){
        return this.data?.authResponse?.isAuthorized;
    }

    async #getTokenDetail(){
        var result = await this.#semrushContext.getTokenDetails();
        if (!result) return;

        this.token = result.data?.access_token!;
        this.isTokenAvailable = result.data?.isAccessTokenAvailable!;
    }

    async _revoke(){
        const result = await this.#semrushContext.revokeToken();
        if (!result) return;

        this.value = {
            authResponse:{
                isAuthorized: false,
                isValid: false,
                isFreeAccount: false
            }
        }

        this.requestUpdate();
        this.dispatchEvent(new CustomEvent('property-value-change'));
        this._submitModal();
    }

    render(){
        return html`
            <umb-body-layout>
                <uui-box>
                    <div>
                        <p>
                            <b>Connected: </b>${this.isTokenAvailable && this.isAuthorized()}
                        </p>
                        <p>
                            <b>Account: </b>${this.isAuthorized() ? (this.data?.authResponse?.isFreeAccount ? "Free" : "Paid") : "N/A"}
                        </p>
                        ${when(this.isAuthorized(), () => html`
                            <p class="semrush-text-wrap">
                                <b>Access Token: </b>
                                <span>${this.token}</span>
                            </p>
                        `)}
                    </div>

                    <uui-button label="Revoke" look="primary" color="danger" ?disabled=${!this.isAuthorized()} @click=${this._revoke}></uui-button>
                </uui-box>

                <uui-button slot="actions" label="Close" @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
    }

    static styles = [
        css`
          .semrush-text-wrap{
            word-break: break-all;
            white-space: normal;
          }
      `];
}