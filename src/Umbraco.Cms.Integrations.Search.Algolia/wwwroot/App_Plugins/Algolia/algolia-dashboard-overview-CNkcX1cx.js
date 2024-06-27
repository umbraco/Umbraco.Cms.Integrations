import { LitElement as $, html as r, css as C, state as _, customElement as M, nothing as T } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as E } from "@umbraco-cms/backoffice/element-api";
import { UMB_MODAL_MANAGER_CONTEXT as v, UMB_CONFIRM_MODAL as y } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT as I } from "@umbraco-cms/backoffice/notification";
import { ALGOLIA_CONTEXT_TOKEN as S } from "./algolia-index.context-BcCnOnU6.js";
var D = Object.defineProperty, N = Object.getOwnPropertyDescriptor, x = (e) => {
  throw TypeError(e);
}, b = (e, t, i, o) => {
  for (var a = o > 1 ? void 0 : o ? N(t, i) : t, s = e.length - 1, h; s >= 0; s--)
    (h = e[s]) && (a = (o ? h(t, i, a) : h(a)) || a);
  return o && a && D(t, i, a), a;
}, p = (e, t, i) => t.has(e) || x("Cannot " + i), m = (e, t, i) => (p(e, t, "read from private field"), i ? i.call(e) : t.get(e)), w = (e, t, i) => t.has(e) ? x("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), L = (e, t, i, o) => (p(e, t, "write to private field"), t.set(e, i), i), n = (e, t, i) => (p(e, t, "access private method"), i), c, l, g, A, k, f, u, O;
let d = class extends E($) {
  constructor() {
    super(), w(this, l), w(this, c), this._loading = !1, this._indices = [], this.consumeContext(S, (e) => {
      L(this, c, e);
    });
  }
  connectedCallback() {
    super.connectedCallback(), n(this, l, g).call(this);
  }
  render() {
    return r`
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
            ${this._loading ? r`<div class="center"><uui-loader></uui-loader></div>` : ""}
            ${n(this, l, O).call(this)}
            </uui-box>
        `;
  }
};
c = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakSet();
g = async function() {
  var e;
  this._loading = !0, await ((e = m(this, c)) == null ? void 0 : e.getIndices().then((t) => {
    this._indices = t, this._loading = !1;
  }).catch((t) => n(this, l, u).call(this, t.message)));
};
A = async function(e) {
  var o;
  await (await this.getContext(v)).open(
    this,
    y,
    {
      data: {
        headline: `Build Index : ${e.name}`,
        content: r`
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
  }), this._loading = !0, await ((o = m(this, c)) == null ? void 0 : o.buildIndex(e).then((a) => {
    var s = a;
    s.success ? n(this, l, f).call(this, "Index built.") : n(this, l, u).call(this, s.error);
  }).catch((a) => n(this, l, u).call(this, a))), this._loading = !1;
};
k = async function(e) {
  var o;
  const i = (await this.getContext(v)).open(
    this,
    y,
    {
      data: {
        headline: "Delete Index",
        content: r`
                      <p class="umb-alert umb-alert--warning mb2">
                        Are you sure you want to delete index <b>${e.name}</b>?
                      </p>`,
        color: "danger",
        confirmLabel: "Ok"
      }
    }
  );
  i == null || i.onSubmit().catch((a) => n(this, l, u).call(this, a.message)), !(e == null || e.id == null) && (this._loading = !0, await ((o = m(this, c)) == null ? void 0 : o.deleteIndex(e.id).then((a) => {
    var s = a;
    s.success ? (n(this, l, g).call(this), n(this, l, f).call(this, "Index deleted")) : n(this, l, u).call(this, s.error);
  }).catch((a) => n(this, l, u).call(this, a))), this._loading = !1);
};
f = async function(e) {
  const t = await this.getContext(I);
  t == null || t.peek("positive", {
    data: { message: e }
  });
};
u = async function(e) {
  const t = await this.getContext(I);
  t == null || t.peek("danger", {
    data: { message: e }
  });
};
O = function() {
  return this._indices.length == 0 ? T : r`
          <uui-table aria-label="Indices Table" style="width: 70%">
            <uui-table-column style="width: 20%;"></uui-table-column>
            <uui-table-column style="width: 60%;"></uui-table-column>
            <uui-table-column style="width: 20%;"></uui-table-column>

            <uui-table-head>
                <uui-table-head-cell>Name</uui-table-head-cell>
                <uui-table-head-cell>Definition</uui-table-head-cell>
                <uui-table-head-cell></uui-table-head-cell>
            </uui-table-head>
            ${this._indices.map((e) => r`
                <uui-table-row>
                  <uui-table-cell>${e.name}</uui-table-cell>
                  <uui-table-cell>
                        ${e.contentData.map((t) => r`
                                <uui-ref-node name=${t.name}
                                            detail=${t.properties.map((i) => i.name).join(", ")}>
                                    <uui-icon slot="icon" name=${t.icon}></uui-icon>
                                </uui-ref-node>
                                `)}
                    
                </uui-table-cell>
                <uui-table-cell>
                    <uui-action-bar>
                        <uui-button label="edit" look="default" color="default"
                            @click="${() => window.history.pushState({}, "", window.location.href.replace(/\/+$/, "") + "/index/" + e.id)}">
                            <uui-icon name="edit"></uui-icon>
                        </uui-button>
                        <uui-button label="build" look="default" color="danger" @click="${() => n(this, l, A).call(this, e)}">
                            <uui-icon name="sync"></uui-icon>
                        </uui-button>
                        <uui-button label="search" look="default" color="positive" 
                            @click="${() => window.history.pushState({}, "", window.location.href.replace(/\/+$/, "") + "/search/" + e.id)}">
                            <uui-icon name="search"></uui-icon>
                        </uui-button>
                        <uui-button label="delete" look="default" color="default" @click="${() => n(this, l, k).call(this, e)}">
                            <uui-icon name="delete"></uui-icon>
                        </uui-button>
                    </uui-action-bar>
                </uui-table-cell>
                </uui-table-row>
              `)}
            </uui-table>
        `;
};
d.styles = [
  C`
      .center {
        display: grid;
        place-items: center;
      }
      .error {
        color: var(--uui-color-danger);
      }
    `
];
b([
  _()
], d.prototype, "_loading", 2);
b([
  _()
], d.prototype, "_indices", 2);
d = b([
  M("algolia-dashboard-overview")
], d);
const W = d;
export {
  d as AlgoliaDashboardOverviewElement,
  W as default
};
//# sourceMappingURL=algolia-dashboard-overview-CNkcX1cx.js.map
