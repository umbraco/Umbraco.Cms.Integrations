import { LitElement as u, html as l, state as p, customElement as _, nothing as v } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as P } from "@umbraco-cms/backoffice/element-api";
var f = Object.defineProperty, x = Object.getOwnPropertyDescriptor, c = (t) => {
  throw TypeError(t);
}, s = (t, e, a, n) => {
  for (var r = n > 1 ? void 0 : n ? x(e, a) : e, i = t.length - 1, h; i >= 0; i--)
    (h = t[i]) && (r = (n ? h(e, a, r) : h(r)) || r);
  return n && r && f(e, a, r), r;
}, b = (t, e, a) => e.has(t) || c("Cannot " + a), g = (t, e, a) => e.has(t) ? c("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, a), y = (t, e, a) => (b(t, e, "access private method"), a), d, m;
let o = class extends P(u) {
  constructor() {
    super(...arguments), g(this, d), this._routes = [
      {
        path: "/index/:indexId",
        component: () => import("./algolia-index-BgJLZOFO.js"),
        setup: (t, e) => {
          const a = t;
          a.indexId = e.match.params.indexId;
        }
      },
      {
        path: "/index",
        component: () => import("./algolia-index-BgJLZOFO.js"),
        setup: (t) => {
          const e = t;
          e.indexId = "";
        }
      },
      {
        path: "/search/:indexId",
        component: () => import("./algolia-search-CREhygO5.js"),
        setup: (t, e) => {
          const a = t;
          a.indexId = e.match.params.indexId;
        }
      },
      {
        path: "",
        component: () => import("./algolia-dashboard-overview-CNkcX1cx.js")
      }
    ], this._activePath = "";
  }
  render() {
    return l`
        <umb-body-layout header-transparent>
            ${y(this, d, m).call(this)}
            <div id="main">
                <umb-router-slot
                .routes=${this._routes}
                @init=${(t) => {
      this._routerPath = t.target.absoluteRouterPath;
    }}
                @change=${(t) => {
      this._activePath = t.target.localActiveViewPath || "";
    }}></umb-router-slot>
            </div>
        </umb-body-layout>
      `;
  }
};
d = /* @__PURE__ */ new WeakSet();
m = function() {
  return this._routerPath && this._activePath !== "" ? l`
					<div id="header" slot="header">
						<a href=${this._routerPath}> &larr; Back to overview </a>
					</div>
			  ` : v;
};
s([
  p()
], o.prototype, "_routes", 2);
s([
  p()
], o.prototype, "_routerPath", 2);
s([
  p()
], o.prototype, "_activePath", 2);
o = s([
  _("algolia-dashboard-element")
], o);
const w = o;
export {
  o as AlgoliaDashboardElement,
  w as default
};
//# sourceMappingURL=algolia-dashboard.element-nlY3jwgr.js.map
