import { UMB_AUTH_CONTEXT as s } from "@umbraco-cms/backoffice/auth";
const p = {
  type: "globalContext",
  alias: "shopify.context",
  name: "Shopify Context",
  js: () => import("./shopify.context-DlBtK13t.js")
}, a = p, c = {
  type: "propertyEditorUi",
  alias: "Shopify.PropertyEditorUi.Amount",
  name: "Shopify Product Picker Amount Setting",
  element: () => import("./amount-property-editor.element-eAoDmYox.js"),
  meta: {
    label: "Amount",
    icon: "icon-autofill",
    group: "common"
  }
}, l = {
  type: "propertyEditorUi",
  alias: "Shopify.PropertyEditorUi.Authorization",
  name: "Shopify Product Picker Authorization Setting",
  element: () => import("./authorization-property-editor.element-B4QwE6Ba.js"),
  meta: {
    label: "Authorization",
    icon: "icon-autofill",
    group: "common"
  }
}, m = {
  type: "propertyEditorUi",
  alias: "Shopify.PropertyEditorUi.ProductPicker",
  name: "Shopify Product Picker Property Editor UI",
  js: () => import("./shopify-product-picker-property-editor.element-BE8F2P7A.js"),
  elementName: "shopify-product-picker",
  meta: {
    label: "Shopify Product Picker",
    icon: "icon-shopping-basket-alt",
    group: "pickers",
    propertyEditorSchemaAlias: "Shopify.ProductPicker",
    settings: {
      properties: [
        {
          alias: "authorization",
          label: "Authorization",
          description: "Authorize your Shopify connection.",
          propertyEditorUiAlias: "Shopify.PropertyEditorUi.Authorization"
        },
        {
          alias: "amount",
          label: "Amount",
          description: "Set a required range of items selected.",
          propertyEditorUiAlias: "Shopify.PropertyEditorUi.Amount"
        }
      ]
    }
  }
}, f = [
  m,
  c,
  l
], y = {
  type: "modal",
  alias: "Shopify.Modal",
  name: "Shopify Modal",
  js: () => import("./shopify-modal.element-B-BTuxGa.js")
};
class n {
  constructor() {
    this._fns = [];
  }
  eject(t) {
    const o = this._fns.indexOf(t);
    o !== -1 && (this._fns = [...this._fns.slice(0, o), ...this._fns.slice(o + 1)]);
  }
  use(t) {
    this._fns = [...this._fns, t];
  }
}
const i = {
  BASE: "",
  CREDENTIALS: "include",
  ENCODE_PATH: void 0,
  HEADERS: void 0,
  PASSWORD: void 0,
  TOKEN: void 0,
  USERNAME: void 0,
  VERSION: "Latest",
  WITH_CREDENTIALS: !1,
  interceptors: {
    request: new n(),
    response: new n()
  }
}, u = (e, t) => {
  t.registerMany([
    ...f,
    y,
    a
  ]), e.consumeContext(s, async (o) => {
    const r = o.getOpenApiConfiguration();
    i.TOKEN = r.token, i.BASE = r.base, i.WITH_CREDENTIALS = !0;
  });
};
export {
  i as O,
  u as o
};
//# sourceMappingURL=index-DM1PFbxq.js.map
