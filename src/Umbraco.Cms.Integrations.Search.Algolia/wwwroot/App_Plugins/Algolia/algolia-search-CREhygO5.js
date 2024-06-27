import { LitElement as f, html as l, css as y, property as g, query as C, state as _, customElement as I } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as S } from "@umbraco-cms/backoffice/element-api";
import { UMB_NOTIFICATION_CONTEXT as E } from "@umbraco-cms/backoffice/notification";
import { ALGOLIA_CONTEXT_TOKEN as b } from "./algolia-index.context-BcCnOnU6.js";
var w = Object.defineProperty, P = Object.getOwnPropertyDescriptor, m = (e) => {
  throw TypeError(e);
}, o = (e, t, i, s) => {
  for (var r = s > 1 ? void 0 : s ? P(t, i) : t, p = e.length - 1, u; p >= 0; p--)
    (u = e[p]) && (r = (s ? u(t, i, r) : u(r)) || r);
  return s && r && w(t, i, r), r;
}, v = (e, t, i) => t.has(e) || m("Cannot " + i), c = (e, t, i) => (v(e, t, "read from private field"), i ? i.call(e) : t.get(e)), d = (e, t, i) => t.has(e) ? m("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), x = (e, t, i, s) => (v(e, t, "write to private field"), t.set(e, i), i), h, n;
let a = class extends S(f) {
  constructor() {
    super(), d(this, h), d(this, n), this.index = {
      id: Number(this.indexId),
      name: "",
      contentData: []
    }, this.indexSearchResult = {
      itemsCount: 0,
      pagesCount: 0,
      itemsPerPage: 0,
      hits: []
    }, this.consumeContext(E, (e) => {
      x(this, h, e);
    }), this.consumeContext(b, (e) => {
      x(this, n, e);
    });
  }
  connectedCallback() {
    super.connectedCallback(), this._getIndex();
  }
  render() {
    return l`
            <uui-box headline="Search">
                <small slot="header">Please enter the query you want to search by against index <b>${this.index.name}</b></small>
                <div class="flex">
                    <uui-input
                            type="search"
                            id="search-input"
                            placeholder="Type to filter..."
                            label="Type to filter"
                            @keypress=${this._onKeyPress}>
                    </uui-input>
                    <uui-button color="positive" look="primary" label="Search" @click="${this._onSearch}"> Search </uui-button>
                </div>
                <!--RESULTS -->
                <div>
                    <p>Items Count: ${this.indexSearchResult.itemsCount}</p>
                    <p>Pages Count: ${this.indexSearchResult.pagesCount}</p>
                    <p>Items per Page: ${this.indexSearchResult.itemsPerPage}</p>
                    ${this.indexSearchResult.hits.map((e) => l`
                                <div>
                                    ${Object.entries(e).map((t) => l`
                                            <p>
                                                <b>${t[0]}</b> : ${t[1]}
                                            </p>
                                        `)}                                
                                </div>
                            `)}
                </div>
            </uui-box>
        `;
  }
  async _getIndex() {
    var e;
    await ((e = c(this, n)) == null ? void 0 : e.getIndexById(Number(this.indexId)).then((t) => this.index = t).catch((t) => this._showError(t.message)));
  }
  _onKeyPress(e) {
    e.key == "Enter" && this._onSearch();
  }
  async _onSearch() {
    var e;
    this._searchInput.value.length && await ((e = c(this, n)) == null ? void 0 : e.searchIndex(Number(this.indexId), this._searchInput.value).then((t) => {
      this.indexSearchResult = t;
    }).catch((t) => this._showError(t)));
  }
  // notifications
  _showError(e) {
    var t;
    (t = c(this, h)) == null || t.peek("danger", {
      data: { message: e }
    });
  }
};
h = /* @__PURE__ */ new WeakMap();
n = /* @__PURE__ */ new WeakMap();
a.styles = [
  y`
            uui-box p {
                margin-top: 0;
            }
            div.flex {
                display: flex;
            }
            div.flex > uui-button {
                padding-left: var(--uui-size-space-4);
                height: 0;
            }
            uui-input {
                width: 100%;
                margin-bottom: var(--uui-size-space-5);
            }
        `
];
o([
  g()
], a.prototype, "indexId", 2);
o([
  C("#search-input")
], a.prototype, "_searchInput", 2);
o([
  _()
], a.prototype, "index", 2);
o([
  _()
], a.prototype, "indexSearchResult", 2);
a = o([
  I("algolia-search")
], a);
const R = a;
export {
  a as AlgoliaSearchElement,
  R as default
};
//# sourceMappingURL=algolia-search-CREhygO5.js.map
