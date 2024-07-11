var m = (e) => {
  throw TypeError(e);
};
var y = (e, o, t) => o.has(e) || m("Cannot " + t);
var r = (e, o, t) => (y(e, o, "read from private field"), t ? t.call(e) : o.get(e)), l = (e, o, t) => o.has(e) ? m("Cannot add the same private member more than once") : o instanceof WeakSet ? o.add(e) : o.set(e, t), d = (e, o, t, s) => (y(e, o, "write to private field"), s ? s.call(e, t) : o.set(e, t), t);
import { UmbElementMixin as O } from "@umbraco-cms/backoffice/element-api";
import { LitElement as _, html as h, css as b, property as C, state as g } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalToken as S, UMB_MODAL_MANAGER_CONTEXT as N } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT as T } from "@umbraco-cms/backoffice/notification";
import { SHOPIFY_CONTEXT_TOKEN as k } from "./shopify.context-BMBZ2TCh.js";
const A = new S("Shopify.Modal", {
  modal: {
    type: "sidebar",
    size: "small"
  }
}), M = {
  api: "An access token is configured and will be used to connect to your Shopify account.",
  oauth: "No access token is configured. To connect to your Shopify account using OAuth click 'Connect', select your account and agree to the permissions.",
  none: "No access token or OAuth configuration could be found. Please review your settings before continuing.",
  oauthConnected: "OAuth is configured and an access token is available to connect to your Shopify account. To revoke, click 'Revoke'"
};
var w = Object.defineProperty, p = (e, o, t, s) => {
  for (var n = void 0, u = e.length - 1, f; u >= 0; u--)
    (f = e[u]) && (n = f(o, t, n) || n);
  return n && w(o, t, n), n;
}, a, c;
const v = class v extends O(_) {
  constructor() {
    super();
    l(this, a);
    l(this, c);
    this.value = "", this.products = [], this._serviceStatus = {
      isValid: !1,
      type: "",
      description: "",
      useOAuth: !1
    }, this.consumeContext(N, (t) => {
      d(this, a, t);
    }), this.consumeContext(k, (t) => {
      d(this, c, t);
    });
  }
  async connectedCallback() {
    var n;
    if (super.connectedCallback(), this.value == null || this.value.length == 0) return;
    const { data: t } = await r(this, c).checkConfiguration();
    if (!t || !((n = t.type) != null && n.value)) return;
    this._serviceStatus = {
      isValid: t.isValid,
      type: t.type.value,
      description: "",
      useOAuth: t.isValid && t.type.value === "OAuth"
    }, this._serviceStatus.isValid || this._showError(M.none);
    const s = JSON.parse(JSON.stringify(this.value));
    this.products = s;
  }
  async _openModal() {
    var n;
    const t = (n = r(this, a)) == null ? void 0 : n.open(this, A, {
      data: {
        headline: "HubSpot Forms"
      }
    }), s = await (t == null ? void 0 : t.onSubmit());
    s && (this.value = JSON.stringify(s.products), console.log(this.value), this.dispatchEvent(new CustomEvent("property-value-change")));
  }
  async _showError(t) {
    const s = await this.getContext(T);
    s == null || s.peek("danger", {
      data: { message: t }
    });
  }
  render() {
    return h`
            ${this.value == null || this.value.length == 0 ? h`
                    <uui-button
				        class="add-button"
				        @click=${this._openModal}
				        label=${this.localize.term("general_add")}
				        look="placeholder"></uui-button>
                ` : h`
                    <div></div>
                `}
		`;
  }
};
a = new WeakMap(), c = new WeakMap(), v.styles = [
  b`
            .add-button {
                width: 100%;
            }
        `
];
let i = v;
p([
  C({ type: String })
], i.prototype, "value");
p([
  g()
], i.prototype, "products");
p([
  g()
], i.prototype, "_serviceStatus");
export {
  i as ShopifyProductPickerPropertyEditor,
  i as default
};
//# sourceMappingURL=shopify-product-picker-property-editor.element-Bbzzoqrg.js.map
