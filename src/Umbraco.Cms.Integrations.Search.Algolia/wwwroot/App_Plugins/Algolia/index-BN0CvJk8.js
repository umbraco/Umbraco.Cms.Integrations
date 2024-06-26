import { UMB_AUTH_CONTEXT as i } from "@umbraco-cms/backoffice/auth";
class o {
  constructor() {
    this._fns = [];
  }
  eject(e) {
    const t = this._fns.indexOf(e);
    t !== -1 && (this._fns = [...this._fns.slice(0, t), ...this._fns.slice(t + 1)]);
  }
  use(e) {
    this._fns = [...this._fns, e];
  }
}
const n = {
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
    request: new o(),
    response: new o()
  }
}, l = {
  type: "dashboard",
  name: "Algolia Search Management",
  alias: "Algolia.Dashboard",
  elementName: "algolia-dashboard-element",
  js: () => import("./algolia-dashboard.element-nlY3jwgr.js"),
  meta: {
    label: "Algolia Search Management",
    pathname: "algolia-search-management"
  },
  conditions: [
    {
      alias: "Umb.Condition.SectionAlias",
      match: "Umb.Section.Settings"
    }
  ]
}, c = l, r = {
  type: "globalContext",
  alias: "algolia.context",
  name: "Algolia Context",
  js: () => import("./algolia-index.context-BcCnOnU6.js")
}, m = r, E = (s, e) => {
  e.registerMany([c, m]), s.consumeContext(i, async (t) => {
    const a = t.getOpenApiConfiguration();
    n.TOKEN = a.token, n.BASE = a.base, n.WITH_CREDENTIALS = !0;
  });
};
export {
  n as O,
  E as o
};
//# sourceMappingURL=index-BN0CvJk8.js.map
