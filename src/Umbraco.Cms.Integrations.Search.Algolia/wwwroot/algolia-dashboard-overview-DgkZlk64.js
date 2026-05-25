import { LitElement as C, html as n, nothing as I, css as $, state as g, customElement as k } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as O } from "@umbraco-cms/backoffice/element-api";
import { UMB_MODAL_MANAGER_CONTEXT as w, UMB_CONFIRM_MODAL as _ } from "@umbraco-cms/backoffice/modal";
import { ALGOLIA_CONTEXT_TOKEN as E } from "./algolia-index.context-B5xhfvsN.js";
var M = Object.defineProperty, S = Object.getOwnPropertyDescriptor, v = (e) => {
  throw TypeError(e);
}, h = (e, t, i, a) => {
  for (var o = a > 1 ? void 0 : a ? S(t, i) : t, c = e.length - 1, d; c >= 0; c--)
    (d = e[c]) && (o = (a ? d(t, i, o) : d(o)) || o);
  return a && o && M(t, i, o), o;
}, b = (e, t, i) => t.has(e) || v("Cannot " + i), m = (e, t, i) => (b(e, t, "read from private field"), i ? i.call(e) : t.get(e)), f = (e, t, i) => t.has(e) ? v("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), T = (e, t, i, a) => (b(e, t, "write to private field"), t.set(e, i), i), s = (e, t, i) => (b(e, t, "access private method"), i), r, l, p, y, x, A;
const D = "algolia-dashboard-overview";
let u = class extends O(C) {
  constructor() {
    super(), f(this, l), f(this, r), this._loading = !1, this._indices = [], this.consumeContext(E, (e) => {
      e && T(this, r, e);
    });
  }
  connectedCallback() {
    super.connectedCallback(), s(this, l, p).call(this);
  }
  render() {
    return n`
            <uui-box headline="Algolia Indices">
            <div>
                <h5>Manage Algolia Indices</h5>
                <p>
                    Algolia is an AI-powered search and discovery platform allowing you to create cutting-edge customer experiences for their websites or mobile apps.
                    It's like the perfect mediator between your website and customers, making sure the conversation is as smooth and efficient as possible.
                </p>
                <p>
                    The Algolia model provides Search as a Service through an externally hosted search engine, offering web search across the website based
                    on the content payload pushed from the website to Algolia.
                </p>
                <p>
                    To get started, you need to create an index and define the content schema - document types and properties.
                    Then you can build your index, push data to Algolia and run searches across created indices.
                    <br />
                    <a style="text-decoration: underline" target="_blank" href="https://www.algolia.com/doc/guides/getting-started/quick-start/">
                        Read more about integrating Algolia Search
                    </a>
                </p>
            </div>
            <div>
                <uui-button look="primary" color="default" label="Add New Index Definition" 
                @click="${() => window.history.pushState({}, "", window.location.href.replace(/\/+$/, "") + "/index")}"></uui-button>
            </div>
            ${this._loading ? n`<div class="center"><uui-loader></uui-loader></div>` : ""}
            ${s(this, l, A).call(this)}
            </uui-box>
        `;
  }
};
r = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakSet();
p = async function() {
  this._loading = !0;
  const { data: e } = await m(this, r).getIndices();
  e && (this._indices = e), this._loading = !1;
};
y = async function(e) {
  var a;
  const t = await this.getContext(w);
  if (!t) return;
  await t.open(
    this,
    _,
    {
      data: {
        headline: `Build Index : ${e.name}`,
        content: n`
                      <p class="umb-alert umb-alert--warning mb2">
                        This will cause the index to be built.<br />
                        Depending on how much content there is in your site this could take a while.<br />
                        It is not recommended to rebuild an index during times of high website traffic
                        or when editors are editing content.
                      </p>`,
        color: "danger",
        confirmLabel: "Ok"
      }
    }
  ).onSubmit().catch(() => {
  }), this._loading = !0, await ((a = m(this, r)) == null ? void 0 : a.buildIndex(e)), this._loading = !1;
};
x = async function(e) {
  var a;
  const t = await this.getContext(w);
  if (!t) return;
  await t.open(
    this,
    _,
    {
      data: {
        headline: "Delete Index",
        content: n`
                      <p class="umb-alert umb-alert--warning mb2">
                        Are you sure you want to delete index <b>${e.name}</b>?
                      </p>`,
        color: "danger",
        confirmLabel: "Ok"
      }
    }
  ).onSubmit().catch(() => {
  }), this._loading = !0, await ((a = m(this, r)) == null ? void 0 : a.deleteIndex(Number(e.id))), s(this, l, p).call(this), this._loading = !1;
};
A = function() {
  return this._indices.length == 0 ? I : n`
          <uui-table aria-label="Indices Table" style="width: 70%">
            <uui-table-column style="width: 20%;"></uui-table-column>
            <uui-table-column style="width: 60%;"></uui-table-column>
            <uui-table-column style="width: 20%;"></uui-table-column>

            <uui-table-head>
                <uui-table-head-cell>Name</uui-table-head-cell>
                <uui-table-head-cell>Definition</uui-table-head-cell>
                <uui-table-head-cell></uui-table-head-cell>
            </uui-table-head>
            ${this._indices.map((e) => n`
                <uui-table-row>
                  <uui-table-cell>${e.name}</uui-table-cell>
                  <uui-table-cell>
                        ${e.contentData.map((t) => {
    if (t.properties)
      return n`
                                <uui-ref-node name=${t.name}
                                            detail=${t.properties.map((i) => i.name).join(", ")}>
                                    <uui-icon slot="icon" name=${t.icon}></uui-icon>
                                </uui-ref-node>
                                `;
  })}
                    
                </uui-table-cell>
                <uui-table-cell>
                    <uui-action-bar>
                        <uui-button label="edit" look="default" color="default"
                            @click="${() => window.history.pushState({}, "", window.location.href.replace(/\/+$/, "") + "/index/" + e.id)}">
                            <uui-icon name="edit"></uui-icon>
                        </uui-button>
                        <uui-button label="build" look="default" color="danger" @click="${() => s(this, l, y).call(this, e)}">
                            <uui-icon name="sync"></uui-icon>
                        </uui-button>
                        <uui-button label="search" look="default" color="positive" 
                            @click="${() => window.history.pushState({}, "", window.location.href.replace(/\/+$/, "") + "/search/" + e.id)}">
                            <uui-icon name="search"></uui-icon>
                        </uui-button>
                        <uui-button label="delete" look="default" color="default" @click="${() => s(this, l, x).call(this, e)}">
                            <uui-icon name="delete"></uui-icon>
                        </uui-button>
                    </uui-action-bar>
                </uui-table-cell>
                </uui-table-row>
              `)}
            </uui-table>
        `;
};
u.styles = [
  $`
      .center {
        display: grid;
        place-items: center;
      }
      .error {
        color: var(--uui-color-danger);
      }
    `
];
h([
  g()
], u.prototype, "_loading", 2);
h([
  g()
], u.prototype, "_indices", 2);
u = h([
  k(D)
], u);
const G = u;
export {
  u as AlgoliaDashboardOverviewElement,
  G as default
};
//# sourceMappingURL=algolia-dashboard-overview-DgkZlk64.js.map
