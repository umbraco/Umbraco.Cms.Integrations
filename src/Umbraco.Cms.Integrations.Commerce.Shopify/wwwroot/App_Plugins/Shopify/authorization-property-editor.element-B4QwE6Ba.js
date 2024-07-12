import { UmbElementMixin as O } from "@umbraco-cms/backoffice/element-api";
import { LitElement as E, html as f, when as T, state as w, property as g, customElement as V } from "@umbraco-cms/backoffice/external/lit";
import { SHOPIFY_CONTEXT_TOKEN as x } from "./shopify.context-DlBtK13t.js";
import { C as n } from "./shopify-service.model-Nm90ruwK.js";
import { UMB_NOTIFICATION_CONTEXT as M } from "@umbraco-cms/backoffice/notification";
var P = Object.defineProperty, I = Object.getOwnPropertyDescriptor, y = (t) => {
  throw TypeError(t);
}, d = (t, e, s, i) => {
  for (var o = i > 1 ? void 0 : i ? I(e, s) : e, p = t.length - 1, l; p >= 0; p--)
    (l = t[p]) && (o = (i ? l(e, s, o) : l(o)) || o);
  return i && o && P(e, s, o), o;
}, _ = (t, e, s) => e.has(t) || y("Cannot " + s), r = (t, e, s) => (_(t, e, "read from private field"), s ? s.call(t) : e.get(t)), v = (t, e, s) => e.has(t) ? y("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, s), $ = (t, e, s, i) => (_(t, e, "write to private field"), e.set(t, s), s), u = (t, e, s) => (_(t, e, "access private method"), s), a, c, A, S, C, k, m;
const z = "shopify-authorization";
let h = class extends O(E) {
  constructor() {
    super(), v(this, c), v(this, a), this._serviceStatus = {
      isValid: !1,
      type: "",
      description: "",
      useOAuth: !1
    }, this._oauthSetup = {
      isConnected: !1,
      isAccessTokenExpired: !1,
      isAccessTokenValid: !1
    }, this.value = "", this.consumeContext(x, (t) => {
      $(this, a, t);
    });
  }
  async connectedCallback() {
    super.connectedCallback(), await u(this, c, A).call(this);
  }
  async _showSuccess(t) {
    await this._showMessage(t, "positive");
  }
  async _showError(t) {
    await this._showMessage(t, "danger");
  }
  async _showMessage(t, e) {
    const s = await this.getContext(M);
    s == null || s.peek(e, {
      data: { message: t }
    });
  }
  render() {
    return f`
            <div>
                <p>${this._serviceStatus.description}</p>
            </div>
            ${T(this._serviceStatus.useOAuth, () => f`
                <div>
                    <uui-button 
                        look="primary" 
                        label="Connect" 
                        ?disabled=${this._oauthSetup.isConnected} 
                        .onclick=${u(this, c, k).call(this)}></uui-button>
                    <uui-button 
                        color="danger" 
                        look="secondary" 
                        label="Revoke" 
                        ?disabled=${!this._oauthSetup.isConnected} 
                        .onclick=${u(this, c, m).call(this)}></uui-button>
                </div>
                `)}
            
        `;
  }
};
a = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakSet();
A = async function() {
  var e;
  const { data: t } = await r(this, a).checkConfiguration();
  !t || !((e = t.type) != null && e.value) || (this._serviceStatus = {
    isValid: t.isValid,
    type: t.type.value,
    description: u(this, c, C).call(this, t.type.value),
    useOAuth: t.isValid && t.type.value === "OAuth"
  }, this._serviceStatus.useOAuth && await u(this, c, S).call(this), t.isValid || this._showError("Invalid setup. Please review the API/OAuth settings."));
};
S = async function() {
  const { data: t } = await r(this, a).validateAccessToken();
  t && (this._oauthSetup = {
    isConnected: t.isValid,
    isAccessTokenExpired: t.isExpired,
    isAccessTokenValid: t.isValid
  }, this._oauthSetup.isConnected && this._oauthSetup.isAccessTokenValid && (this._serviceStatus.description = n.oauthConnected), this._oauthSetup.isAccessTokenExpired && await r(this, a).refreshAccessToken());
};
C = function(t) {
  switch (t) {
    case "API":
      return n.api;
    case "OAuth":
      return n.oauth;
    case "OAuthConnected":
      return n.oauthConnected;
    default:
      return n.none;
  }
};
k = async function() {
  window.addEventListener("message", async (e) => {
    if (e.data.type === "hubspot:oauth:success") {
      const s = {
        code: e.data.code
      }, { data: i } = await r(this, a).getAccessToken(s);
      if (!i) return;
      i.startsWith("Error:") ? this._showError(i) : (this._oauthSetup = {
        isConnected: !0
      }, this._serviceStatus.description = n.oauthConnected, this._showSuccess("OAuth Connected"));
    }
  }, !1);
  const { data: t } = await r(this, a).getAuthorizationUrl();
  t && window.open(t, "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
};
m = async function() {
  await r(this, a).revokeAccessToken(), this._oauthSetup = {
    isConnected: !1
  }, this._serviceStatus.description = n.none, this._showSuccess("OAuth connection revoked.");
};
d([
  w()
], h.prototype, "_serviceStatus", 2);
d([
  w()
], h.prototype, "_oauthSetup", 2);
d([
  g({ type: String })
], h.prototype, "value", 2);
h = d([
  V(z)
], h);
const R = h;
export {
  h as ShopifyAuthorizationElement,
  R as default
};
//# sourceMappingURL=authorization-property-editor.element-B4QwE6Ba.js.map
