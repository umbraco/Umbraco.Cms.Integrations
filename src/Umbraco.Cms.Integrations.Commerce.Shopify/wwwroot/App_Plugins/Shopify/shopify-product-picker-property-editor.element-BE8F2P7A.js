var f = (e) => {
  throw TypeError(e);
};
var _ = (e, s, t) => s.has(e) || f("Cannot " + t);
var u = (e, s, t) => (_(e, s, "read from private field"), t ? t.call(e) : s.get(e)), h = (e, s, t) => s.has(e) ? f("Cannot add the same private member more than once") : s instanceof WeakSet ? s.add(e) : s.set(e, t), d = (e, s, t, i) => (_(e, s, "write to private field"), i ? i.call(e, t) : s.set(e, t), t);
import { UmbElementMixin as y } from "@umbraco-cms/backoffice/element-api";
import { LitElement as C, html as p, css as b, property as g, state as O } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalToken as M, UMB_MODAL_MANAGER_CONTEXT as N } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT as S } from "@umbraco-cms/backoffice/notification";
import { C as T } from "./shopify-service.model-Nm90ruwK.js";
import { SHOPIFY_CONTEXT_TOKEN as E } from "./shopify.context-DlBtK13t.js";
const w = new M("Shopify.Modal", {
  modal: {
    type: "sidebar",
    size: "small"
  }
});
var A = Object.defineProperty, c = (e, s, t, i) => {
  for (var a = void 0, r = e.length - 1, v; r >= 0; r--)
    (v = e[r]) && (a = v(s, t, a) || a);
  return a && A(s, t, a), a;
}, n, l;
const m = class m extends y(C) {
  constructor() {
    super();
    h(this, n);
    h(this, l);
    this.value = "", this.products = [], this._serviceStatus = {
      isValid: !1,
      type: "",
      description: "",
      useOAuth: !1
    }, this.consumeContext(N, (t) => {
      d(this, n, t);
    }), this.consumeContext(E, (t) => {
      d(this, l, t);
    });
  }
  async connectedCallback() {
    var a;
    if (super.connectedCallback(), this.value == null || this.value.length == 0) return;
    const { data: t } = await u(this, l).checkConfiguration();
    if (!t || !((a = t.type) != null && a.value)) return;
    this._serviceStatus = {
      isValid: t.isValid,
      type: t.type.value,
      description: "",
      useOAuth: t.isValid && t.type.value === "OAuth"
    }, this._serviceStatus.isValid || this._showError(T.none);
    const i = JSON.parse(JSON.stringify(this.value));
    this.products = i;
  }
  async _openModal() {
    var a;
    const t = (a = u(this, n)) == null ? void 0 : a.open(this, w, {
      data: {
        headline: "HubSpot Forms"
      }
    }), i = await (t == null ? void 0 : t.onSubmit());
    i && (this.value = JSON.stringify(i.products), console.log(this.value), this.dispatchEvent(new CustomEvent("property-value-change")));
  }
  async _showError(t) {
    const i = await this.getContext(S);
    i == null || i.peek("danger", {
      data: { message: t }
    });
  }
  render() {
    return p`
            ${this.value == null || this.value.length == 0 ? p`
                    <uui-button
				        class="add-button"
				        @click=${this._openModal}
				        label=${this.localize.term("general_add")}
				        look="placeholder"></uui-button>
                ` : p`
                    <div></div>
                `}
		`;
  }
};
n = new WeakMap(), l = new WeakMap(), m.styles = [
  b`
            .add-button {
                width: 100%;
            }
        `
];
let o = m;
c([
  g({ type: String })
], o.prototype, "value");
c([
  O()
], o.prototype, "products");
c([
  O()
], o.prototype, "_serviceStatus");
export {
  o as ShopifyProductPickerPropertyEditor,
  o as default
};
//# sourceMappingURL=shopify-product-picker-property-editor.element-BE8F2P7A.js.map
