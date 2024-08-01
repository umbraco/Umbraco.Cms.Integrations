import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import {
    LitElement,
    css,
    customElement,
    html,
    query
} from "@umbraco-cms/backoffice/external/lit";
import {
    UMB_NOTIFICATION_CONTEXT,
    type UmbNotificationColor,
} from "@umbraco-cms/backoffice/notification";

const elementName = "dynamics-authorization-code";

@customElement(elementName)
export default class DynamicsAuthorizationCodeElement extends UmbElementMixin(LitElement) {

    @query('#auth-code-input')
    private _authCodeInput!: HTMLInputElement;

    async #onAuthorize() {
        if (this._authCodeInput.value.length == 0) {
            this._showError("Incorrect authorization code.");
            return;
        }

        // todo - get access token
    }

    private async _showError(message: string) {
        await this._showMessage(message, "danger");
    }

    private async _showMessage(message: string, color: UmbNotificationColor) {
        const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
        notificationContext?.peek(color, {
            data: { message },
        });
    }

    render() {
        return html`
            <div id="authToken">
                <uui-input id="auth-code-input" placeholder="Authorization code"></uui-input>
                <uui-button look="primary"
                            label="Authorize"
                            @click=${this.#onAuthorize}></uui-button>
            </div>
        `;
    }

    static styles = [
        css`
            #authToken { 
                margin-top: 20px; 
            }
            #authToken uui-input {
                width: 50%;
            }
        `];

}

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: DynamicsAuthorizationCodeElement;
    }
}
