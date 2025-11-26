import { repeat as P, html as d, css as w, state as m, property as S, customElement as M } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalToken as T, UMB_MODAL_MANAGER_CONTEXT as N } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT as b } from "@umbraco-cms/backoffice/notification";
import { C as I } from "./shopify-service.model-Nm90ruwK.js";
import { SHOPIFY_CONTEXT_TOKEN as k } from "./shopify.context-CW9f65sx.js";
import { UmbLitElement as A } from "@umbraco-cms/backoffice/lit-element";
const V = new T("Shopify.Modal", {
  modal: {
    type: "sidebar",
    size: "large"
  }
});
var $ = Object.defineProperty, L = Object.getOwnPropertyDescriptor, g = (t) => {
  throw TypeError(t);
}, p = (t, e, s, i) => {
  for (var o = i > 1 ? void 0 : i ? L(e, s) : e, v = t.length - 1, _; v >= 0; v--)
    (_ = t[v]) && (o = (i ? _(e, s, o) : _(o)) || o);
  return i && o && $(e, s, o), o;
}, y = (t, e, s) => e.has(t) || g("Cannot " + s), n = (t, e, s) => (y(t, e, "read from private field"), e.get(t)), u = (t, e, s) => e.has(t) ? g("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, s), f = (t, e, s, i) => (y(t, e, "write to private field"), e.set(t, s), s), O = (t, e, s) => (y(t, e, "access private method"), s), c, l, a, h, C, E;
const D = "shopify-product-picker";
let r = class extends A {
  constructor() {
    super(), u(this, h), u(this, c), u(this, l), u(this, a), this.value = "", this.products = [], this._serviceStatus = {
      isValid: !1,
      type: "",
      description: "",
      useOAuth: !1
    }, this.consumeContext(N, (t) => {
      f(this, l, t);
    }), this.consumeContext(k, (t) => {
      t && (f(this, c, t), this.observe(t.settingsModel, (e) => {
        f(this, a, e);
      }));
    });
  }
  set config(t) {
    this._config = O(this, h, C).call(this, t);
  }
  async connectedCallback() {
    super.connectedCallback(), n(this, a) && (this._serviceStatus = {
      isValid: n(this, a).isValid,
      type: n(this, a).type.value,
      description: "",
      useOAuth: n(this, a).isValid && n(this, a).type.value === "OAuth"
    }, this._serviceStatus.isValid || this._showError(I.none), !(this.value == null || this.value.length == 0) && await O(this, h, E).call(this));
  }
  async _openModal() {
    var s;
    const t = (s = n(this, l)) == null ? void 0 : s.open(this, V, {
      data: {
        headline: "Shopify Products",
        selectedItemIdList: this.products.map((i) => i.id.toString()),
        config: this._config
      }
    }), e = await (t == null ? void 0 : t.onSubmit());
    e && (this.value = JSON.stringify(e.productList.map((i) => i.id)), this.products = e.productList, this.dispatchEvent(new CustomEvent("property-value-change")));
  }
  async _showError(t) {
    const e = await this.getContext(b);
    e == null || e.peek("danger", {
      data: { message: t }
    });
  }
  deleteProduct(t) {
    this.products = this.products.filter((e) => e.id != t), this.value = JSON.stringify(this.products.map((e) => e.id)), this.dispatchEvent(new CustomEvent("property-value-change"));
  }
  render() {
    return d`
            ${this._serviceStatus.isValid ? d`
                    <div>
                        <uui-button
                            class="add-button"
                            @click=${this._openModal}
                            label=${this.localize.term("general_add")}
                            look="placeholder"></uui-button>
                    </div>
                    <div>
                        ${P(
      this.products,
      (t) => d`
                                <uui-ref-node-form name=${t.title} detail=${t.vendor}>
                                    <uui-action-bar slot="actions">
                                        <uui-button label="Remove" @click=${() => this.deleteProduct(t.id)}>Remove</uui-button>
                                    </uui-action-bar>
                                </uui-ref-node-form>
                            `
    )}    
                    </div>
                ` : d`
                    <span></span>
                `}
            `;
  }
};
c = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakMap();
a = /* @__PURE__ */ new WeakMap();
h = /* @__PURE__ */ new WeakSet();
C = function(t) {
  return {
    minItems: t == null ? void 0 : t.getValueByAlias("minItems"),
    maxItems: t == null ? void 0 : t.getValueByAlias("maxItems")
  };
};
E = async function() {
  const t = {
    ids: JSON.parse(JSON.stringify(this.value))
  }, { data: e } = await n(this, c).getListByIds(t);
  e && (this.products = e.result.products);
};
r.styles = [
  w`
            .add-button {
                width: 100%;
            }
        `
];
p([
  m()
], r.prototype, "_config", 2);
p([
  S({ attribute: !1 })
], r.prototype, "config", 1);
p([
  S({ type: String })
], r.prototype, "value", 2);
p([
  m()
], r.prototype, "products", 2);
p([
  m()
], r.prototype, "_serviceStatus", 2);
r = p([
  M(D)
], r);
const R = r;
export {
  r as ShopifyProductPickerPropertyEditor,
  R as default
};
//# sourceMappingURL=shopify-product-picker-property-editor.element-DwRdl7bP.js.map
