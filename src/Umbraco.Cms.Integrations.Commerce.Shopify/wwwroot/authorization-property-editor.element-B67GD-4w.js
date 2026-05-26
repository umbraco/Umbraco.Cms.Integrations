import { UmbElementMixin as g } from "@umbraco-cms/backoffice/element-api";
import { LitElement as T, when as M, html as w, state as A, property as V, customElement as P } from "@umbraco-cms/backoffice/external/lit";
import { SHOPIFY_CONTEXT_TOKEN as x } from "./shopify.context-Bjllc7Sq.js";
import { C as c } from "./shopify-service.model-Nm90ruwK.js";
import { UMB_NOTIFICATION_CONTEXT as I } from "@umbraco-cms/backoffice/notification";
var $ = Object.defineProperty, b = Object.getOwnPropertyDescriptor, S = (t) => {
  throw TypeError(t);
}, p = (t, e, s, o) => {
  for (var r = o > 1 ? void 0 : o ? b(e, s) : e, l = t.length - 1, _; l >= 0; l--)
    (_ = t[l]) && (r = (o ? _(e, s, r) : _(r)) || r);
  return o && r && $(e, s, r), r;
}, v = (t, e, s) => e.has(t) || S("Cannot " + s), i = (t, e, s) => (v(t, e, "read from private field"), e.get(t)), f = (t, e, s) => e.has(t) ? S("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, s), y = (t, e, s, o) => (v(t, e, "write to private field"), e.set(t, s), s), d = (t, e, s) => (v(t, e, "access private method"), s), a, n, h, C, k, m, E, O;
const z = "shopify-authorization";
let u = class extends g(T) {
  constructor() {
    super(), f(this, h), f(this, a), f(this, n), this._serviceStatus = {
      isValid: !1,
      type: "",
      description: "",
      useOAuth: !1
    }, this._oauthSetup = {
      isConnected: !1,
      isAccessTokenExpired: !1,
      isAccessTokenValid: !1
    }, this.value = "", this.consumeContext(x, (t) => {
      t && (y(this, n, t), this.observe(t.settingsModel, (e) => {
        y(this, a, e);
      }));
    });
  }
  async connectedCallback() {
    super.connectedCallback(), await d(this, h, C).call(this);
  }
  async _showSuccess(t) {
    await this._showMessage(t, "positive");
  }
  async _showError(t) {
    await this._showMessage(t, "danger");
  }
  async _showMessage(t, e) {
    const s = await this.getContext(I);
    s == null || s.peek(e, {
      data: { message: t }
    });
  }
  render() {
    return w`
            <div>
                <p>${this._serviceStatus.description}</p>
            </div>
            ${M(this._serviceStatus.useOAuth, () => w`
                <div>
                    <uui-button 
                        look="primary" 
                        label="Connect" 
                        ?disabled=${this._oauthSetup.isConnected} 
                        @click=${d(this, h, E)}></uui-button>
                    <uui-button 
                        color="danger" 
                        look="secondary" 
                        label="Revoke" 
                        ?disabled=${!this._oauthSetup.isConnected} 
                        @click=${d(this, h, O)}></uui-button>
                </div>
                `)}
            
        `;
  }
};
a = /* @__PURE__ */ new WeakMap();
n = /* @__PURE__ */ new WeakMap();
h = /* @__PURE__ */ new WeakSet();
C = async function() {
  i(this, a) && (this._serviceStatus = {
    isValid: i(this, a).isValid,
    type: i(this, a).type.value,
    description: d(this, h, m).call(this, i(this, a).type.value),
    useOAuth: i(this, a).isValid && i(this, a).type.value === "OAuth"
  }, this._serviceStatus.useOAuth && await d(this, h, k).call(this), i(this, a).isValid || this._showError("Invalid setup. Please review the API/OAuth settings."));
};
k = async function() {
  const { data: t } = await i(this, n).validateAccessToken();
  t && (this._oauthSetup = {
    isConnected: t.isValid,
    isAccessTokenExpired: t.isExpired,
    isAccessTokenValid: t.isValid
  }, this._oauthSetup.isConnected && this._oauthSetup.isAccessTokenValid && (this._serviceStatus.description = c.oauthConnected), this._oauthSetup.isAccessTokenExpired && await i(this, n).refreshAccessToken());
};
m = function(t) {
  switch (t) {
    case "API":
      return c.api;
    case "OAuth":
      return c.oauth;
    case "OAuthConnected":
      return c.oauthConnected;
    default:
      return c.none;
  }
};
E = async function() {
  window.addEventListener("message", async (e) => {
    if (e.data.type === "shopify:oauth:success") {
      const s = {
        code: e.data.code
      }, { data: o } = await i(this, n).getAccessToken(s);
      if (!o) return;
      o.startsWith("Error:") ? this._showError(o) : (this._oauthSetup = {
        isConnected: !0
      }, this._serviceStatus.description = c.oauthConnected, this._showSuccess("OAuth Connected"));
    }
  }, !1);
  const { data: t } = await i(this, n).getAuthorizationUrl();
  t && window.open(t, "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
};
O = async function() {
  await i(this, n).revokeAccessToken(), this._oauthSetup = {
    isConnected: !1
  }, this._serviceStatus.description = c.none, this._showSuccess("OAuth connection revoked."), this.dispatchEvent(new CustomEvent("revoke"));
};
p([
  A()
], u.prototype, "_serviceStatus", 2);
p([
  A()
], u.prototype, "_oauthSetup", 2);
p([
  V({ type: String })
], u.prototype, "value", 2);
u = p([
  P(z)
], u);
const U = u;
export {
  u as ShopifyAuthorizationElement,
  U as default
};
//# sourceMappingURL=authorization-property-editor.element-B67GD-4w.js.map
