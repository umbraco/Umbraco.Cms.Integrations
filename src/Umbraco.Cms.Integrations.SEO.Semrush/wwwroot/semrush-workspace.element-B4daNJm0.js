import { UmbLitElement as q } from "@umbraco-cms/backoffice/lit-element";
import { when as P, html as u, nothing as H, css as X, state as c, customElement as F } from "@umbraco-cms/backoffice/external/lit";
import { SEMRUSH_CONTEXT_TOKEN as Y } from "./semrush.context-DrMVUcuj.js";
import { UmbPaginationManager as Q } from "@umbraco-cms/backoffice/utils";
import { UmbModalToken as J, UMB_MODAL_MANAGER_CONTEXT as Z } from "@umbraco-cms/backoffice/modal";
import { UMB_CURRENT_USER_CONTEXT as j } from "@umbraco-cms/backoffice/current-user";
import { UMB_NOTIFICATION_CONTEXT as ee } from "@umbraco-cms/backoffice/notification";
import { E as N } from "./lit-html-CJZhbK-n.js";
const te = new J("Semrush.Modal", {
  modal: {
    type: "sidebar",
    size: "small"
  }
});
/**
 * @license
 * Copyright 2020 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const se = (e) => e.strings === void 0;
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const ie = { CHILD: 2 }, oe = (e) => (...t) => ({ _$litDirective$: e, values: t });
class ae {
  constructor(t) {
  }
  get _$AU() {
    return this._$AM._$AU;
  }
  _$AT(t, s, i) {
    this._$Ct = t, this._$AM = s, this._$Ci = i;
  }
  _$AS(t, s) {
    return this.update(t, s);
  }
  update(t, s) {
    return this.render(...s);
  }
}
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const f = (e, t) => {
  var i;
  const s = e._$AN;
  if (s === void 0) return !1;
  for (const a of s) (i = a._$AO) == null || i.call(a, t, !1), f(a, t);
  return !0;
}, b = (e) => {
  let t, s;
  do {
    if ((t = e._$AM) === void 0) break;
    s = t._$AN, s.delete(e), e = t;
  } while ((s == null ? void 0 : s.size) === 0);
}, D = (e) => {
  for (let t; t = e._$AM; e = t) {
    let s = t._$AN;
    if (s === void 0) t._$AN = s = /* @__PURE__ */ new Set();
    else if (s.has(e)) break;
    s.add(e), he(t);
  }
};
function re(e) {
  this._$AN !== void 0 ? (b(this), this._$AM = e, D(this)) : this._$AM = e;
}
function ne(e, t = !1, s = 0) {
  const i = this._$AH, a = this._$AN;
  if (a !== void 0 && a.size !== 0) if (t) if (Array.isArray(i)) for (let o = s; o < i.length; o++) f(i[o], !1), b(i[o]);
  else i != null && (f(i, !1), b(i));
  else f(this, e);
}
const he = (e) => {
  e.type == ie.CHILD && (e._$AP ?? (e._$AP = ne), e._$AQ ?? (e._$AQ = re));
};
class ce extends ae {
  constructor() {
    super(...arguments), this._$AN = void 0;
  }
  _$AT(t, s, i) {
    super._$AT(t, s, i), D(this), this.isConnected = t._$AU;
  }
  _$AO(t, s = !0) {
    var i, a;
    t !== this.isConnected && (this.isConnected = t, t ? (i = this.reconnected) == null || i.call(this) : (a = this.disconnected) == null || a.call(this)), s && (f(this, t), b(this));
  }
  setValue(t) {
    if (se(this._$Ct)) this._$Ct._$AI(t, this);
    else {
      const s = [...this._$Ct._$AH];
      s[this._$Ci] = t, this._$Ct._$AI(s, this, 0);
    }
  }
  disconnected() {
  }
  reconnected() {
  }
}
/**
 * @license
 * Copyright 2020 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const A = () => new ue();
class ue {
}
const C = /* @__PURE__ */ new WeakMap(), S = oe(class extends ce {
  render(e) {
    return N;
  }
  update(e, [t]) {
    var i;
    const s = t !== this.G;
    return s && this.G !== void 0 && this.rt(void 0), (s || this.lt !== this.ct) && (this.G = t, this.ht = (i = e.options) == null ? void 0 : i.host, this.rt(this.ct = e.element)), N;
  }
  rt(e) {
    if (this.isConnected || (e = void 0), typeof this.G == "function") {
      const t = this.ht ?? globalThis;
      let s = C.get(t);
      s === void 0 && (s = /* @__PURE__ */ new WeakMap(), C.set(t, s)), s.get(this.G) !== void 0 && this.G.call(this.ht, void 0), s.set(this.G, e), e !== void 0 && this.G.call(this.ht, e);
    } else this.G.value = e;
  }
  get lt() {
    var e, t;
    return typeof this.G == "function" ? (e = C.get(this.ht ?? globalThis)) == null ? void 0 : e.get(this.G) : (t = this.G) == null ? void 0 : t.value;
  }
  disconnected() {
    this.lt === this.ct && this.rt(void 0);
  }
  reconnected() {
    this.rt(this.ct);
  }
});
var le = Object.defineProperty, de = Object.getOwnPropertyDescriptor, U = (e) => {
  throw TypeError(e);
}, h = (e, t, s, i) => {
  for (var a = i > 1 ? void 0 : i ? de(t, s) : t, o = e.length - 1, n; o >= 0; o--)
    (n = e[o]) && (a = (i ? n(t, s, a) : n(a)) || a);
  return i && a && le(t, s, a), a;
}, L = (e, t, s) => t.has(e) || U("Cannot " + s), m = (e, t, s) => (L(e, t, "read from private field"), s ? s.call(e) : t.get(e)), g = (e, t, s) => t.has(e) ? U("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, s), E = (e, t, s, i) => (L(e, t, "write to private field"), t.set(e, s), s), d = (e, t, s) => (L(e, t, "access private method"), s), v, w, $, T, l, R, x, O, M, G, I, z, B, W, K, V;
const pe = "semrush-workspace-view";
let r = class extends q {
  constructor() {
    super(), g(this, l), g(this, v), g(this, w), g(this, $), g(this, T, new Q()), this.dsRef = A(), this.methodRef = A(), this.columnRef = A(), this._searchKeywordsBoxVisible = !1, this.dsSearchDomainTooltip = "", this.dsSearchTypeTooltip = "", this.methodTooltip = "", this._columns = [], this._loading = !1, this._searchLoading = !1, this._currentPageNumber = 1, this._totalPages = 1, this.authUrl = "", this.account = {
      isAuthorized: !1,
      isValid: !1,
      isFreeAccount: !0
    }, this.keywordList = void 0, this.searchPhrase = "", this.selectedproperty = "", this.propertyList = [], this.selectedDatasource = "", this.datasourceList = [], this.selectedMethod = "", this.methodList = [
      {
        key: "phrase_fullsearch",
        value: "phrase_fullsearch",
        description: "List of broad matches and alternate search queries, including particular keywords or keyword expressions."
      },
      {
        key: "phrase_kdi",
        value: "phrase_kdi",
        description: "Keyword difficulty - an index that helps to estimate how difficult it would be to seize competitor's positions in organic search within Google's top 100 with an indicated search term."
      },
      {
        key: "phrase_organic",
        value: "phrase_organic",
        description: "List of domains that are ranking in Google's top 100 organic search results with a requested keyword."
      },
      {
        key: "phrase_related",
        value: "phrase_related",
        description: "Extended list of related keywords, synonyms and variations relevant to a queried term in a chosen database."
      },
      {
        key: "phrase_these",
        value: "phrase_these",
        description: "Summary of up to 100 keywords, including volume, CPC, competition and the number of results in a chosen regional database."
      },
      {
        key: "phrase_this",
        value: "phrase_this",
        description: "Summary of a keyword, including volume, CPC, competition and the number of results in a chosen regional database."
      }
    ], this.consumeContext(Y, (e) => {
      e && E(this, v, e);
    }), this.consumeContext(Z, (e) => {
      e && E(this, w, e);
    }), this.consumeContext(j, (e) => {
      e && E(this, $, e);
    });
  }
  async connectedCallback() {
    super.connectedCallback(), this._loading = !0, await d(this, l, M).call(this), await d(this, l, x).call(this), await d(this, l, O).call(this), await d(this, l, R).call(this), await d(this, l, G).call(this), this._loading = !1;
  }
  _getColumnDescription(e) {
    var t;
    return (t = this._columns.find((s) => s.name == e)) == null ? void 0 : t.description;
  }
  async _search() {
    this._searchLoading = !0;
    const { data: e } = await m(this, v).getRelatedPhrases(this.searchPhrase, this._currentPageNumber, this.selectedDatasource, this.selectedMethod);
    e && (e.isSuccessful ? (this.keywordList = e, this._totalPages = Number(e.totalPages)) : this._showError(e.error), this._searchLoading = !1);
  }
  _searchNew() {
    this.searchPhrase = "", this.selectedDatasource = "", this.selectedMethod = "", this.selectedproperty = "", this.dsSearchDomainTooltip = "", this.dsSearchTypeTooltip = "", this.methodTooltip = "", this.keywordList = void 0, this._searchKeywordsBoxVisible = !0;
  }
  async _onConnect() {
    this._loading = !0;
    var e = window.open(this.authUrl, "Semrush_Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
    window.addEventListener("message", async (t) => {
      if (t.data.type === "semrush:oauth:success") {
        var s = "?code=";
        e && e.close();
        var i = t.data.url.slice(t.data.url.indexOf(s) + s.length), { data: a } = await m(this, v).getAccessToken(i);
        if (!a) return;
        a !== "error" ? (await d(this, l, M).call(this), this._showSuccess("Access Approved.")) : this._showError("Access Denied.");
      } else
        this._showError("Access Denied."), e.close();
    }, !1), this._loading = !1;
  }
  async _showSuccess(e) {
    await this._showMessage(e, "positive");
  }
  async _showError(e) {
    await this._showMessage(e, "danger");
  }
  async _showMessage(e, t) {
    const s = await this.getContext(ee);
    s == null || s.peek(t, {
      data: { message: e }
    });
  }
  async isAdmin() {
    return await m(this, $).isCurrentUserAdmin();
  }
  async _openModal() {
    var s;
    const e = (s = m(this, w)) == null ? void 0 : s.open(this, te, {
      data: {
        headline: "Authorization",
        authResponse: this.account
      }
    }), t = await (e == null ? void 0 : e.onSubmit());
    t && (this.account = t.authResponse, this._showSuccess("Access Revoked."), this.dispatchEvent(new CustomEvent("property-value-change")));
  }
  _onDataSourceMouseOver(e) {
    var s, i, a, o, n, p;
    var t = e.target.value.toString();
    if (t !== void 0 && t !== "") {
      this.dsSearchDomainTooltip = (i = (s = this.datasourceList) == null ? void 0 : s.find((k) => k.code == t)) == null ? void 0 : i.googleSearchDomain, this.dsSearchTypeTooltip = (o = (a = this.datasourceList) == null ? void 0 : a.find((k) => k.code == t)) == null ? void 0 : o.researchTypes;
      const _ = (n = this.shadowRoot) == null ? void 0 : n.getElementById("tooltip-toggle"), y = (p = this.shadowRoot) == null ? void 0 : p.getElementById("tooltip-popover");
      _ == null || _.addEventListener("mouseenter", () => y == null ? void 0 : y.showPopover()), _ == null || _.addEventListener("mouseleave", () => y == null ? void 0 : y.hidePopover());
    }
  }
  _onMethodMouseOver(e) {
    var s, i, a, o;
    var t = e.target.value.toString();
    if (t !== void 0 && t !== "") {
      this.methodTooltip = (i = (s = this.methodList) == null ? void 0 : s.find((_) => _.value == t)) == null ? void 0 : i.description;
      const n = (a = this.shadowRoot) == null ? void 0 : a.getElementById("method-tooltip-toggle"), p = (o = this.shadowRoot) == null ? void 0 : o.getElementById("method-tooltip-popover");
      n == null || n.addEventListener("mouseenter", () => p == null ? void 0 : p.showPopover()), n == null || n.addEventListener("mouseleave", () => p == null ? void 0 : p.hidePopover());
    }
  }
  _onColumnMouseOver(e, t) {
    var a, o;
    const s = (a = this.shadowRoot) == null ? void 0 : a.getElementById("column-" + t), i = (o = this.shadowRoot) == null ? void 0 : o.getElementById("column-tooltip-popover-" + t);
    s == null || s.addEventListener("mouseenter", () => i == null ? void 0 : i.showPopover()), s == null || s.addEventListener("mouseleave", () => i == null ? void 0 : i.hidePopover());
  }
  render() {
    var e;
    return u`
            <umb-body-layout>
                ${this._loading ? u`<div class="semrush-loader"><uui-loader></uui-loader></div>` : ""}
                <uui-box headline="Content Properties">
                    <div class="semrush-content">
                        <p>
                            Semrush is a marketing SaaS platform that provides online visibility management and content marketing through
                            a single platform, on all key channels.
                        </p>
                        <p>
                            It analyzes the data from the world's largest database of 20 billion keywords, 310 million ads and 17 billion
                            URLs crawled per day, and gives you instant recommendations on SEO, content marketing and advertising that can
                            help you improve your online visibility in days.
                        </p>
                        <p>
                            Keyword search is a powerful tool that runs a full analysis of your keyword and helps you decide whether you
                            should enter into competition for it. As an integral part of any digital marketing strategy, it's importance
                            will never fade away.
                        </p>
                        <p>
                            <uui-icon class="semrush-autofill-icon" name="icon-autofill"></uui-icon>
                            <a href="https://www.semrush.com" target="_blank">
                            read more
                            </a>
                        </p>
                        ${P(this.isAdmin(), () => u`
                            <p>
                                You need to be logged in against Semrush in order take advantage of the tool's full potential. If you cannot do that,
                                please contact one of the administrators.
                            </p> 
                        `)}
                        <p>
                            You can enable the keywords lookup tool by picking one of the content fields or choosing a new blank search from the
                            controls below:
                        </p>
                    </div>
                    <div>
                        <uui-select
                              placeholder="Select content property"
                              @change=${(t) => d(this, l, I).call(this, t)}
                              .options=${(e = this.propertyList) == null ? void 0 : e.map((t) => ({
      name: t.propertyName,
      value: t.propertyValue,
      selected: t.propertyValue === this.selectedproperty,
      group: t.propertyGroup
    }))}>
                        </uui-select>
                        <span>or</span>
                        <uui-button label="Search new" look="secondary" @click=${this._searchNew}></uui-button>
                    </div>
                </uui-box>

                <br/>

                ${P(this._searchKeywordsBoxVisible, () => {
      var t, s, i, a;
      return u`
                    <uui-box headline="Keyword Search">
                        <uui-button 
                            slot="header-actions" 
                            look=${this.account.isAuthorized ? "secondary" : "primary"} 
                            @click=${this._onConnect} 
                            ?disabled=${this.account.isAuthorized} 
                            class="semrush-connect-button">Connect</uui-button>

                        <uui-button 
                            slot="header-actions" 
                            look=${this.account.isAuthorized ? "primary" : "secondary"} 
                            @click=${this._openModal}>Status</uui-button>

                        <div>
                            <uui-input .value=${this.searchPhrase} @change=${(o) => d(this, l, W).call(this, o)} class="semrush-input"></uui-input>

                            <uui-select id="tooltip-toggle" popovertarget="tooltip-popover"
                                @mouseover=${(o) => this._onDataSourceMouseOver(o)} ${S(this.dsRef)}
                                placeholder="Please select a data source"
                                class="semrush-select"
                                @change=${(o) => d(this, l, z).call(this, o)}
                                .options=${((t = this.datasourceList) == null ? void 0 : t.map((o) => ({
        name: o.region,
        value: o.code,
        selected: o.code === this.selectedDatasource
      }))) ?? []}>
                            </uui-select>
                            <uui-popover-container placement="bottom-start" id="tooltip-popover">
                                <div class="semrush-tooltip">
                                    <span><b>Research Types:</b> ${this.dsSearchTypeTooltip}</span><br />
                                    <span><b>Google Search Domain:</b> ${this.dsSearchDomainTooltip}</span>
                                </div>
                            </uui-popover-container>

                            <uui-select id="method-tooltip-toggle" popovertarget="method-tooltip-popover"
                                @mouseover=${(o) => this._onMethodMouseOver(o)} ${S(this.methodRef)}
                                placeholder="Please select a method"
                                class="semrush-select"
                                @change=${(o) => d(this, l, B).call(this, o)}
                                .options=${(s = this.methodList) == null ? void 0 : s.map((o) => ({
        name: o.key,
        value: o.value,
        selected: o.value === this.selectedMethod
      }))}>
                            </uui-select>
                            <uui-popover-container placement="bottom-start" id="method-tooltip-popover">
                                <div class="semrush-tooltip">
                                    <span>${this.methodTooltip}</span>
                                </div>
                            </uui-popover-container>

                            <uui-button label="Search keywords" look="primary" @click=${this._search} ?disabled=${!this.account.isAuthorized}></uui-button>
                        </div>

                        ${((i = this.keywordList) == null ? void 0 : i.data) !== void 0 && ((a = this.keywordList) != null && a.data) ? u`
                                ${this._searchLoading ? u`<div class="semrush-loader"><uui-loader></uui-loader></div>` : ""}
                                <div class="semrush-table">
                                    <uui-table>
                                        <uui-table-head style="background-color: ; color: ">
                                            ${this.keywordList.data.columnNames.map((o, n) => u`
                                                <uui-table-head-cell>
                                                    <span id="column-${n}" popovertarget="column-tooltip-popover-${n}" @mouseover=${(p) => this._onColumnMouseOver(p, n)} ${S(this.columnRef)}>${o}</span>
                                                    <uui-popover-container placement="bottom-start" id="column-tooltip-popover-${n}">
                                                        <div class="semrush-tooltip">
                                                            ${this._getColumnDescription(o)}
                                                        </div>
                                                    </uui-popover-container>
                                                </uui-table-head-cell>
                                            `)}
                                        </uui-table-head>
                                        ${this.keywordList.data.rows.map((o) => u`
                                            <uui-table-row>
                                                ${o.map((n) => u`
                                                    <uui-table-cell>
                                                        <span>${n}</span>
                                                    </uui-table-cell>
                                                `)}
                                            </uui-table-row>
                                        `)}
                                    </uui-table>
                                </div>

                                ${this.account.isFreeAccount ? u`
                                        <div>
                                            <p>
                                                Because you are using a free account, the number of results is limited to 10 records.
                                                Please upgrade your subscription for enhanced results.
                                            </p>
                                        </div> 
                                    ` : u`
                                        ${d(this, l, V).call(this)}
                                    `}

                                <a href="https://www.semrush.com/analytics/keywordoverview" target="_blank">
                                    Get more insights at Semrush
                                </a>
                            ` : u``}
                    </uui-box>    
                `;
    })}
            </umb-body-layout>
        `;
  }
};
v = /* @__PURE__ */ new WeakMap();
w = /* @__PURE__ */ new WeakMap();
$ = /* @__PURE__ */ new WeakMap();
T = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakSet();
R = async function() {
  var e = window.location.pathname.split("/")[7], { data: t } = await m(this, v).getCurrentContentProperties(e);
  t && (this.propertyList = t);
};
x = async function() {
  var { data: e } = await m(this, v).getDataSources();
  e && (this.datasourceList = e.items);
};
O = async function() {
  var { data: e } = await m(this, v).getAuthorizationUrl();
  e && (this.authUrl = e);
};
M = async function() {
  var { data: e } = await m(this, v).validateToken();
  if (e && e.isAuthorized) {
    if (!e.isValid) {
      await m(this, v).refreshAccessToken();
      return;
    }
    this.account = e, this.requestUpdate(), this.dispatchEvent(new CustomEvent("property-value-change"));
  }
};
G = async function() {
  var { data: e } = await m(this, v).getColumns();
  e && (this._columns = e);
};
I = function(e) {
  this._searchKeywordsBoxVisible = !0, this.selectedproperty = e.target.value.toString(), this.searchPhrase = this.selectedproperty, this.requestUpdate(), this.dispatchEvent(new CustomEvent("property-value-change"));
};
z = function(e) {
  this.selectedDatasource = e.target.value.toString(), this.requestUpdate(), this.dispatchEvent(new CustomEvent("property-value-change"));
};
B = function(e) {
  this.selectedMethod = e.target.value.toString(), this.requestUpdate(), this.dispatchEvent(new CustomEvent("property-value-change"));
};
W = function(e) {
  this.searchPhrase = e.target.value.toString();
};
K = function(e) {
  var s;
  const t = (s = e.target) == null ? void 0 : s.current;
  m(this, T).setCurrentPageNumber(t), this._currentPageNumber = t, this._search();
};
V = function() {
  return u`
            ${this._totalPages > 1 ? u`
                <div class="shopify-pagination">
                    <uui-pagination
					    class="pagination"
					    .current=${this._currentPageNumber}
					    .total=${this._totalPages}
					    @change=${d(this, l, K)}></uui-pagination>
                </div>
             ` : H}
        `;
};
r.styles = [
  X`
            .semrush-content p:first-child {
                margin-top: 0 !important;
            }

            .semrush-table {
                margin: 15px 0;
            }

            .semrush-connect-button {
                margin-right: 2px;
            }

            .semrush-autofill-icon {
                margin-bottom: 4px;
            }

            .semrush-tooltip {
                background-color: var(--uui-color-surface); 
                max-width: 150px; 
                box-shadow: var(--uui-shadow-depth-4); 
                padding: var(--uui-size-space-4); 
                border-radius: var(--uui-border-radius); 
                font-size: 0.9rem;
            }

            .semrush-loader {
                text-align: center;
            }

            .semrush-input, .semrush-select { 
                vertical-align: middle; 
            }
        `
];
h([
  c()
], r.prototype, "_searchKeywordsBoxVisible", 2);
h([
  c()
], r.prototype, "dsSearchDomainTooltip", 2);
h([
  c()
], r.prototype, "dsSearchTypeTooltip", 2);
h([
  c()
], r.prototype, "methodTooltip", 2);
h([
  c()
], r.prototype, "_columns", 2);
h([
  c()
], r.prototype, "_loading", 2);
h([
  c()
], r.prototype, "_searchLoading", 2);
h([
  c()
], r.prototype, "_currentPageNumber", 2);
h([
  c()
], r.prototype, "_totalPages", 2);
h([
  c()
], r.prototype, "authUrl", 2);
h([
  c()
], r.prototype, "account", 2);
h([
  c()
], r.prototype, "keywordList", 2);
h([
  c()
], r.prototype, "searchPhrase", 2);
h([
  c()
], r.prototype, "selectedproperty", 2);
h([
  c()
], r.prototype, "propertyList", 2);
h([
  c()
], r.prototype, "selectedDatasource", 2);
h([
  c()
], r.prototype, "datasourceList", 2);
h([
  c()
], r.prototype, "selectedMethod", 2);
h([
  c()
], r.prototype, "methodList", 2);
r = h([
  F(pe)
], r);
const be = r;
export {
  r as SemrushWorkspaceElement,
  be as default
};
//# sourceMappingURL=semrush-workspace.element-B4daNJm0.js.map
