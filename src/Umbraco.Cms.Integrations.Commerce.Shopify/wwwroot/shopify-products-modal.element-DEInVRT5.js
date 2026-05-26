import { UmbModalBaseElement as U } from "@umbraco-cms/backoffice/modal";
import { SHOPIFY_CONTEXT_TOKEN as B } from "./shopify.context-Bjllc7Sq.js";
import { UMB_NOTIFICATION_CONTEXT as X } from "@umbraco-cms/backoffice/notification";
import { nothing as O, html as v, css as D, state as h, customElement as F } from "@umbraco-cms/backoffice/external/lit";
import { UMB_COLLECTION_CONTEXT as K } from "@umbraco-cms/backoffice/collection";
import { UmbPaginationManager as G } from "@umbraco-cms/backoffice/utils";
var H = Object.defineProperty, Y = Object.getOwnPropertyDescriptor, w = (e) => {
  throw TypeError(e);
}, c = (e, t, s, i) => {
  for (var o = i > 1 ? void 0 : i ? Y(t, s) : t, r = e.length - 1, d; r >= 0; r--)
    (d = e[r]) && (o = (i ? d(t, s, o) : d(o)) || o);
  return i && o && H(t, s, o), o;
}, I = (e, t, s) => t.has(e) || w("Cannot " + s), u = (e, t, s) => (I(e, t, "read from private field"), s ? s.call(e) : t.get(e)), g = (e, t, s) => t.has(e) ? w("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, s), P = (e, t, s, i) => (I(e, t, "write to private field"), t.set(e, s), s), n = (e, t, s) => (I(e, t, "access private method"), s), f, _, b, y, a, x, S, C, A, E, M, $, N, T, k, V, L;
const q = "shopify-products-modal";
let l = class extends U {
  constructor() {
    super(), g(this, a), g(this, f), g(this, _), g(this, b), g(this, y, new G()), this._modalSelectedProducts = [], this._numberOfSelection = 0, this._maximumItems = 0, this._minimumItems = 0, this._selectionIdList = [], this._currentPageNumber = 1, this._totalPages = 1, this._selection = [], this._tableConfig = {
      allowSelection: !0
    }, this._tableItems = [], this._serviceStatus = {
      isValid: !1,
      type: "",
      description: "",
      useOAuth: !1
    }, this._loading = !1, this._products = [], this._tableColumns = [
      {
        name: "Name",
        alias: "productName"
      },
      {
        name: "Vendor",
        alias: "vendor"
      },
      {
        name: "Status",
        alias: "status"
      },
      {
        name: "Tags",
        alias: "tags"
      },
      {
        name: "SKU",
        alias: "sku"
      },
      {
        name: "Barcode",
        alias: "barcode"
      },
      {
        name: "Price",
        alias: "price"
      },
      {
        name: "",
        alias: "entityActions"
      }
    ], this._selectedItems = [], this._selectedProducts = [], this.consumeContext(B, (e) => {
      e && (P(this, f, e), this.observe(e.settingsModel, (t) => {
        P(this, _, t);
      }));
    }), this.consumeContext(K, (e) => {
      e && (P(this, b, e), this.observe(
        u(this, b).selection.selection,
        (t) => this._selection = t,
        "umbCollectionSelectionObserver"
      ));
    });
  }
  async connectedCallback() {
    super.connectedCallback(), n(this, a, x).call(this);
  }
  saveSelectedItems(e, t) {
    this._selectedItems = this._selectedItems.filter((s) => {
      if (!e.some((i) => i.id == s))
        return s;
    }), t.forEach((s) => {
      this._selectedItems.indexOf(s) == -1 && this._selectedItems.push(s);
    });
  }
  saveSelectedProducts(e) {
    this._selectedProducts = this._selectedProducts.filter((t) => {
      if (!e.some((s) => s.id == t.id.toString()))
        return t;
    }), this._modalSelectedProducts.forEach((t) => {
      this._selectedProducts.some((s) => s.id == t.id) || this._selectedProducts.push(t);
    });
  }
  _onSubmit() {
    this._numberOfSelection == 0 ? this._rejectModal() : this.checkNumberOfSelection() ? (this.value = { productList: this._selectedProducts }, this._submitModal()) : this._showError("Please select the amount of items that has been configured in the setting.");
  }
  checkNumberOfSelection() {
    return this._numberOfSelection >= this._minimumItems && this._numberOfSelection <= this._maximumItems;
  }
  async _showError(e) {
    const t = await this.getContext(X);
    t == null || t.peek("danger", {
      data: { message: e }
    });
  }
  render() {
    return v`
            <umb-body-layout>
                <uui-box headline=${this.data.headline}>
                    ${this._loading ? v`<div class="center loader"><uui-loader></uui-loader></div>` : ""}
                    <umb-table 
                        .config=${this._tableConfig} 
                        .columns=${this._tableColumns} 
                        .items=${this._tableItems}
                        .selection=${this._selection}
                        @selected="${n(this, a, M)}"
                        @deselected="${n(this, a, $)}">
                    </umb-table>
                    ${n(this, a, L).call(this)}
                </uui-box>

                ${this._maximumItems > 0 ? v`
                        <div class="maximum-selection">
                            <span>
                                Add up to ${this._maximumItems} items(s)
                            </span>
                        </div>
                    ` : O}
                <uui-button look="primary"  slot="actions" label="Submit" @click=${this._onSubmit}></uui-button>
                <uui-button slot="actions" label="Close" @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
  }
};
f = /* @__PURE__ */ new WeakMap();
_ = /* @__PURE__ */ new WeakMap();
b = /* @__PURE__ */ new WeakMap();
y = /* @__PURE__ */ new WeakMap();
a = /* @__PURE__ */ new WeakSet();
x = async function() {
  if (!(!u(this, f) || !u(this, _))) {
    if (this._serviceStatus = {
      isValid: u(this, _).isValid,
      type: u(this, _).type.value,
      description: "",
      useOAuth: u(this, _).isValid && u(this, _).type.value === "OAuth"
    }, !this._serviceStatus.isValid) {
      this._showError("Invalid Shopify API Configuration");
      return;
    }
    await n(this, a, C).call(this), await n(this, a, S).call(this, "");
  }
};
S = async function(e) {
  await n(this, a, C).call(this), this._loading = !0;
  const { data: t } = await u(this, f).getList(e);
  if (t) {
    if (!t.isValid) {
      this._showError("Cannot access Shopify API."), this._loading = !1;
      return;
    }
    this._products = t.result.products ?? [], this._loading = !1, (!t.isValid || t.isExpired) && this._showError("Data is invalid or expired."), this._nextPageInfo = t.nextPageInfo, this._previousPageInfo = t.previousPageInfo, n(this, a, A).call(this, this._products), n(this, a, E).call(this);
  }
};
C = async function() {
  const { data: e } = await u(this, f).getTotalPages();
  e && (this._totalPages = Number(e));
};
A = function(e) {
  this._tableItems = e.map((t) => ({
    id: t.id.toString(),
    data: [
      {
        columnAlias: "productName",
        value: t.title
      },
      {
        columnAlias: "vendor",
        value: t.vendor
      },
      {
        columnAlias: "status",
        value: t.status
      },
      {
        columnAlias: "tags",
        value: t.tags
      },
      {
        columnAlias: "sku",
        value: t.variants.map((s) => s.sku).join(",")
      },
      {
        columnAlias: "barcode",
        value: t.variants.map((s) => s.barcode).join(",")
      },
      {
        columnAlias: "price",
        value: t.variants[0].price
      }
    ]
  }));
};
E = async function() {
  var e, t, s, i;
  this._selection = this._selectedItems.length > 0 ? this._selectedItems : this.data.selectedItemIdList, this._maximumItems = ((t = (e = this.data) == null ? void 0 : e.config) == null ? void 0 : t.maxItems) ?? 0, this._minimumItems = ((i = (s = this.data) == null ? void 0 : s.config) == null ? void 0 : i.minItems) ?? 0;
};
M = function(e) {
  n(this, a, N).call(this, e);
};
$ = function(e) {
  n(this, a, N).call(this, e);
};
N = function(e) {
  var o;
  e.stopPropagation();
  const t = e.target, s = t.selection, i = t.items;
  this.saveSelectedItems(i, s), (o = u(this, b)) == null || o.selection.setSelection(s), n(this, a, T).call(this, s, i), this._numberOfSelection = s.length;
};
T = function(e, t) {
  let s = [];
  e.forEach((r) => {
    const d = t.filter((m) => m.id == r);
    d && d.length > 0 && s.push(d);
  });
  let i = s.map((r) => r[0].data), o = s.map((r) => r[0].id);
  this._modalSelectedProducts = n(this, a, k).call(this, i, o), this.saveSelectedProducts(t);
};
k = function(e, t) {
  var i, o, r, d;
  let s = [];
  for (let m = 0; m < e.length; m++) {
    let W = {
      title: (i = e[m].find((p) => p.columnAlias == "productName")) == null ? void 0 : i.value,
      vendor: (o = e[m].find((p) => p.columnAlias == "vendor")) == null ? void 0 : o.value,
      id: Number(t[m]),
      body_html: "",
      status: (r = e[m].find((p) => p.columnAlias == "status")) == null ? void 0 : r.value,
      tags: (d = e[m].find((p) => p.columnAlias == "tags")) == null ? void 0 : d.value,
      variants: [],
      image: {
        src: "",
        alt: ""
      },
      product_type: "",
      published_scope: "",
      handle: ""
    };
    s.push(W);
  }
  return s;
};
V = function(e) {
  var i;
  const t = ((i = e.target) == null ? void 0 : i.current) > this._currentPageNumber, s = t ? this._currentPageNumber + 1 : this._currentPageNumber - 1;
  u(this, y).setCurrentPageNumber(s), this._currentPageNumber = s, n(this, a, S).call(this, t ? this._nextPageInfo : this._previousPageInfo);
};
L = function() {
  return v`
            ${this._totalPages > 1 ? v`
                <div class="shopify-pagination">
                    <uui-pagination
					    class="pagination"
					    .current=${this._currentPageNumber}
					    .total=${this._totalPages}
					    @change=${n(this, a, V)}></uui-pagination>
                </div>
             ` : O}
        `;
};
l.styles = [D`
        .loader {
            display: flex;
            justify-content: center;
        }
        .maximum-selection{
            margin-top: 10px;
            font-weight: bold;
        }
        .shopify-pagination {
            width: 50%;
            margin-top: 10px;
            margin-left: auto;
            margin-right: auto;
        }
    `];
c([
  h()
], l.prototype, "_currentPageNumber", 2);
c([
  h()
], l.prototype, "_totalPages", 2);
c([
  h()
], l.prototype, "_nextPageInfo", 2);
c([
  h()
], l.prototype, "_previousPageInfo", 2);
c([
  h()
], l.prototype, "_selection", 2);
c([
  h()
], l.prototype, "_tableConfig", 2);
c([
  h()
], l.prototype, "_tableItems", 2);
c([
  h()
], l.prototype, "_serviceStatus", 2);
c([
  h()
], l.prototype, "_loading", 2);
c([
  h()
], l.prototype, "_products", 2);
c([
  h()
], l.prototype, "_tableColumns", 2);
l = c([
  F(q)
], l);
export {
  l as default
};
//# sourceMappingURL=shopify-products-modal.element-DEInVRT5.js.map
