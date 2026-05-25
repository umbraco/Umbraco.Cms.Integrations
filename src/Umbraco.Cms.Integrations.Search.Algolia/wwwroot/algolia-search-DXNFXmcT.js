import { LitElement as S, html as c, css as C, property as I, query as E, state as x, customElement as P } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as $ } from "@umbraco-cms/backoffice/element-api";
import { ALGOLIA_CONTEXT_TOKEN as b } from "./algolia-index.context-B5xhfvsN.js";
var w = Object.defineProperty, O = Object.getOwnPropertyDescriptor, f = (e) => {
  throw TypeError(e);
}, l = (e, t, i, n) => {
  for (var r = n > 1 ? void 0 : n ? O(t, i) : t, h = e.length - 1, p; h >= 0; h--)
    (p = e[h]) && (r = (n ? p(t, i, r) : p(r)) || r);
  return n && r && w(t, i, r), r;
}, d = (e, t, i) => t.has(e) || f("Cannot " + i), _ = (e, t, i) => (d(e, t, "read from private field"), i ? i.call(e) : t.get(e)), v = (e, t, i) => t.has(e) ? f("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), R = (e, t, i, n) => (d(e, t, "write to private field"), t.set(e, i), i), u = (e, t, i) => (d(e, t, "access private method"), i), o, s, g, y, m;
const T = "algolia-search";
let a = class extends $(S) {
  constructor() {
    super(), v(this, s), v(this, o), this.index = {
      id: Number(this.indexId),
      name: "",
      contentData: []
    }, this.indexSearchResult = {
      itemsCount: 0,
      pagesCount: 0,
      itemsPerPage: 0,
      hits: []
    }, this.consumeContext(b, (e) => {
      e && R(this, o, e);
    });
  }
  connectedCallback() {
    super.connectedCallback(), u(this, s, g).call(this);
  }
  render() {
    return c`
            <uui-box headline="Search">
                <small slot="header">Please enter the query you want to search by against index <strong>${this.index.name}</strong></small>
                <div class="flex">
                    <uui-input
                            type="search"
                            id="search-input"
                            placeholder="Type to filter..."
                            label="Type to filter"
                            @keypress=${u(this, s, y)}>
                    </uui-input>
                    <uui-button color="positive" look="primary" label=${this.localize.term("general_search")} @click=${u(this, s, m)}></uui-button>
                </div>
                <!--RESULTS -->
                <div>
                    <p>Items Count: ${this.indexSearchResult.itemsCount}</p>
                    <p>Pages Count: ${this.indexSearchResult.pagesCount}</p>
                    <p>Items per Page: ${this.indexSearchResult.itemsPerPage}</p>
                    ${this.indexSearchResult.hits.map((e) => c`
                                <div>
                                    ${Object.entries(e).map((t) => c`
                                            <p>
                                                <strong>${t[0]}</strong> : ${t[1]}
                                            </p>
                                        `)}                                
                                </div>
                            `)}
                </div>
            </uui-box>
        `;
  }
};
o = /* @__PURE__ */ new WeakMap();
s = /* @__PURE__ */ new WeakSet();
g = async function() {
  const { data: e } = await _(this, o).getIndexById(Number(this.indexId));
  e && (this.index = e);
};
y = function(e) {
  e.key == "Enter" && u(this, s, m).call(this);
};
m = async function() {
  if (!this._searchInput.value.length) return;
  const { data: e } = await _(this, o).searchIndex(Number(this.indexId), this._searchInput.value);
  e && (this.indexSearchResult = e);
};
a.styles = [
  C`
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
l([
  I()
], a.prototype, "indexId", 2);
l([
  E("#search-input")
], a.prototype, "_searchInput", 2);
l([
  x()
], a.prototype, "index", 2);
l([
  x()
], a.prototype, "indexSearchResult", 2);
a = l([
  P(T)
], a);
const L = a;
export {
  a as AlgoliaSearchElement,
  L as default
};
//# sourceMappingURL=algolia-search-DXNFXmcT.js.map
