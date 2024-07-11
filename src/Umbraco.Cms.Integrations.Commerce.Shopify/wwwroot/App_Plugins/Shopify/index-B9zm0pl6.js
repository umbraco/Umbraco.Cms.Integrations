import { UMB_AUTH_CONTEXT as n } from "@umbraco-cms/backoffice/auth";
const p = {
  type: "globalContext",
  alias: "shopify.context",
  name: "Shopify Context",
  js: () => import("./shopify.context-BMBZ2TCh.js")
}, a = p, c = {
  type: "propertyEditorUi",
  alias: "Shopify.PropertyEditorUi.ProductPicker",
  name: "Shopify Product Picker Property Editor UI",
  js: () => import("./shopify-product-picker-property-editor.element-Bbzzoqrg.js"),
  elementName: "shopify-product-picker",
  meta: {
    label: "Shopify Product Picker",
    icon: "icon-handshake",
    group: "pickers",
    propertyEditorSchemaAlias: "Shopify.ProductPicker"
  }
}, d = {
  type: "propertyEditorSchema",
  name: "Shopify Product Picker",
  alias: "Shopify.ProductPicker",
  meta: {
    defaultPropertyEditorUiAlias: "Shopify.PropertyEditorUi.ProductPicker",
    settings: {
      properties: [
        {
          alias: "Shopify.authorization",
          label: "Authorization",
          description: "Authorization Details",
          propertyEditorUiAlias: "Shopify.PropertyEditorUi.Authorization"
        }
      ]
    }
  }
}, f = [
  c,
  d
], y = {
  type: "modal",
  alias: "Shopify.Modal",
  name: "Shopify Modal",
  js: () => import("./shopify-modal.element-B-BTuxGa.js")
};
class s {
  constructor() {
    this._fns = [];
  }
  eject(t) {
    const i = this._fns.indexOf(t);
    i !== -1 && (this._fns = [...this._fns.slice(0, i), ...this._fns.slice(i + 1)]);
  }
  use(t) {
    this._fns = [...this._fns, t];
  }
}
const o = {
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
    request: new s(),
    response: new s()
  }
}, E = (e, t) => {
  t.registerMany([
    ...f,
    y,
    a
  ]), e.consumeContext(n, async (i) => {
    const r = i.getOpenApiConfiguration();
    o.TOKEN = r.token, o.BASE = r.base, o.WITH_CREDENTIALS = !0;
  });
};
export {
  o as O,
  E as o
};
//# sourceMappingURL=index-B9zm0pl6.js.map
