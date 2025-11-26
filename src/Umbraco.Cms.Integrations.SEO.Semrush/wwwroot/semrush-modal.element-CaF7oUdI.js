import { UmbModalBaseElement as L } from "@umbraco-cms/backoffice/modal";
import { B as N, T as q, x as C } from "./lit-html-CJZhbK-n.js";
import { SEMRUSH_CONTEXT_TOKEN as B } from "./semrush.context-DpF4KTL0.js";
/**
 * @license
 * Copyright 2019 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const _ = globalThis, b = _.ShadowRoot && (_.ShadyCSS === void 0 || _.ShadyCSS.nativeShadow) && "adoptedStyleSheets" in Document.prototype && "replace" in CSSStyleSheet.prototype, S = Symbol(), A = /* @__PURE__ */ new WeakMap();
let R = class {
  constructor(t, e, i) {
    if (this._$cssResult$ = !0, i !== S) throw Error("CSSResult is not constructable. Use `unsafeCSS` or `css` instead.");
    this.cssText = t, this.t = e;
  }
  get styleSheet() {
    let t = this.o;
    const e = this.t;
    if (b && t === void 0) {
      const i = e !== void 0 && e.length === 1;
      i && (t = A.get(e)), t === void 0 && ((this.o = t = new CSSStyleSheet()).replaceSync(this.cssText), i && A.set(e, t));
    }
    return t;
  }
  toString() {
    return this.cssText;
  }
};
const W = (s) => new R(typeof s == "string" ? s : s + "", void 0, S), V = (s, ...t) => {
  const e = s.length === 1 ? s[0] : t.reduce((i, r, n) => i + ((o) => {
    if (o._$cssResult$ === !0) return o.cssText;
    if (typeof o == "number") return o;
    throw Error("Value passed to 'css' function must be a 'css' function result: " + o + ". Use 'unsafeCSS' to pass non-literal values, but take care to ensure page security.");
  })(r) + s[n + 1], s[0]);
  return new R(e, s, S);
}, F = (s, t) => {
  if (b) s.adoptedStyleSheets = t.map((e) => e instanceof CSSStyleSheet ? e : e.styleSheet);
  else for (const e of t) {
    const i = document.createElement("style"), r = _.litNonce;
    r !== void 0 && i.setAttribute("nonce", r), i.textContent = e.cssText, s.appendChild(i);
  }
}, k = b ? (s) => s : (s) => s instanceof CSSStyleSheet ? ((t) => {
  let e = "";
  for (const i of t.cssRules) e += i.cssText;
  return W(e);
})(s) : s;
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const { is: H, defineProperty: K, getOwnPropertyDescriptor: I, getOwnPropertyNames: J, getOwnPropertySymbols: G, getPrototypeOf: X } = Object, c = globalThis, O = c.trustedTypes, Q = O ? O.emptyScript : "", m = c.reactiveElementPolyfillSupport, u = (s, t) => s, $ = { toAttribute(s, t) {
  switch (t) {
    case Boolean:
      s = s ? Q : null;
      break;
    case Object:
    case Array:
      s = s == null ? s : JSON.stringify(s);
  }
  return s;
}, fromAttribute(s, t) {
  let e = s;
  switch (t) {
    case Boolean:
      e = s !== null;
      break;
    case Number:
      e = s === null ? null : Number(s);
      break;
    case Object:
    case Array:
      try {
        e = JSON.parse(s);
      } catch {
        e = null;
      }
  }
  return e;
} }, w = (s, t) => !H(s, t), U = { attribute: !0, type: String, converter: $, reflect: !1, useDefault: !1, hasChanged: w };
Symbol.metadata ?? (Symbol.metadata = Symbol("metadata")), c.litPropertyMetadata ?? (c.litPropertyMetadata = /* @__PURE__ */ new WeakMap());
class d extends HTMLElement {
  static addInitializer(t) {
    this._$Ei(), (this.l ?? (this.l = [])).push(t);
  }
  static get observedAttributes() {
    return this.finalize(), this._$Eh && [...this._$Eh.keys()];
  }
  static createProperty(t, e = U) {
    if (e.state && (e.attribute = !1), this._$Ei(), this.prototype.hasOwnProperty(t) && ((e = Object.create(e)).wrapped = !0), this.elementProperties.set(t, e), !e.noAccessor) {
      const i = Symbol(), r = this.getPropertyDescriptor(t, i, e);
      r !== void 0 && K(this.prototype, t, r);
    }
  }
  static getPropertyDescriptor(t, e, i) {
    const { get: r, set: n } = I(this.prototype, t) ?? { get() {
      return this[e];
    }, set(o) {
      this[e] = o;
    } };
    return { get: r, set(o) {
      const a = r == null ? void 0 : r.call(this);
      n == null || n.call(this, o), this.requestUpdate(t, a, i);
    }, configurable: !0, enumerable: !0 };
  }
  static getPropertyOptions(t) {
    return this.elementProperties.get(t) ?? U;
  }
  static _$Ei() {
    if (this.hasOwnProperty(u("elementProperties"))) return;
    const t = X(this);
    t.finalize(), t.l !== void 0 && (this.l = [...t.l]), this.elementProperties = new Map(t.elementProperties);
  }
  static finalize() {
    if (this.hasOwnProperty(u("finalized"))) return;
    if (this.finalized = !0, this._$Ei(), this.hasOwnProperty(u("properties"))) {
      const e = this.properties, i = [...J(e), ...G(e)];
      for (const r of i) this.createProperty(r, e[r]);
    }
    const t = this[Symbol.metadata];
    if (t !== null) {
      const e = litPropertyMetadata.get(t);
      if (e !== void 0) for (const [i, r] of e) this.elementProperties.set(i, r);
    }
    this._$Eh = /* @__PURE__ */ new Map();
    for (const [e, i] of this.elementProperties) {
      const r = this._$Eu(e, i);
      r !== void 0 && this._$Eh.set(r, e);
    }
    this.elementStyles = this.finalizeStyles(this.styles);
  }
  static finalizeStyles(t) {
    const e = [];
    if (Array.isArray(t)) {
      const i = new Set(t.flat(1 / 0).reverse());
      for (const r of i) e.unshift(k(r));
    } else t !== void 0 && e.push(k(t));
    return e;
  }
  static _$Eu(t, e) {
    const i = e.attribute;
    return i === !1 ? void 0 : typeof i == "string" ? i : typeof t == "string" ? t.toLowerCase() : void 0;
  }
  constructor() {
    super(), this._$Ep = void 0, this.isUpdatePending = !1, this.hasUpdated = !1, this._$Em = null, this._$Ev();
  }
  _$Ev() {
    var t;
    this._$ES = new Promise((e) => this.enableUpdating = e), this._$AL = /* @__PURE__ */ new Map(), this._$E_(), this.requestUpdate(), (t = this.constructor.l) == null || t.forEach((e) => e(this));
  }
  addController(t) {
    var e;
    (this._$EO ?? (this._$EO = /* @__PURE__ */ new Set())).add(t), this.renderRoot !== void 0 && this.isConnected && ((e = t.hostConnected) == null || e.call(t));
  }
  removeController(t) {
    var e;
    (e = this._$EO) == null || e.delete(t);
  }
  _$E_() {
    const t = /* @__PURE__ */ new Map(), e = this.constructor.elementProperties;
    for (const i of e.keys()) this.hasOwnProperty(i) && (t.set(i, this[i]), delete this[i]);
    t.size > 0 && (this._$Ep = t);
  }
  createRenderRoot() {
    const t = this.shadowRoot ?? this.attachShadow(this.constructor.shadowRootOptions);
    return F(t, this.constructor.elementStyles), t;
  }
  connectedCallback() {
    var t;
    this.renderRoot ?? (this.renderRoot = this.createRenderRoot()), this.enableUpdating(!0), (t = this._$EO) == null || t.forEach((e) => {
      var i;
      return (i = e.hostConnected) == null ? void 0 : i.call(e);
    });
  }
  enableUpdating(t) {
  }
  disconnectedCallback() {
    var t;
    (t = this._$EO) == null || t.forEach((e) => {
      var i;
      return (i = e.hostDisconnected) == null ? void 0 : i.call(e);
    });
  }
  attributeChangedCallback(t, e, i) {
    this._$AK(t, i);
  }
  _$ET(t, e) {
    var n;
    const i = this.constructor.elementProperties.get(t), r = this.constructor._$Eu(t, i);
    if (r !== void 0 && i.reflect === !0) {
      const o = (((n = i.converter) == null ? void 0 : n.toAttribute) !== void 0 ? i.converter : $).toAttribute(e, i.type);
      this._$Em = t, o == null ? this.removeAttribute(r) : this.setAttribute(r, o), this._$Em = null;
    }
  }
  _$AK(t, e) {
    var n, o;
    const i = this.constructor, r = i._$Eh.get(t);
    if (r !== void 0 && this._$Em !== r) {
      const a = i.getPropertyOptions(r), h = typeof a.converter == "function" ? { fromAttribute: a.converter } : ((n = a.converter) == null ? void 0 : n.fromAttribute) !== void 0 ? a.converter : $;
      this._$Em = r, this[r] = h.fromAttribute(e, a.type) ?? ((o = this._$Ej) == null ? void 0 : o.get(r)) ?? null, this._$Em = null;
    }
  }
  requestUpdate(t, e, i) {
    var r;
    if (t !== void 0) {
      const n = this.constructor, o = this[t];
      if (i ?? (i = n.getPropertyOptions(t)), !((i.hasChanged ?? w)(o, e) || i.useDefault && i.reflect && o === ((r = this._$Ej) == null ? void 0 : r.get(t)) && !this.hasAttribute(n._$Eu(t, i)))) return;
      this.C(t, e, i);
    }
    this.isUpdatePending === !1 && (this._$ES = this._$EP());
  }
  C(t, e, { useDefault: i, reflect: r, wrapped: n }, o) {
    i && !(this._$Ej ?? (this._$Ej = /* @__PURE__ */ new Map())).has(t) && (this._$Ej.set(t, o ?? e ?? this[t]), n !== !0 || o !== void 0) || (this._$AL.has(t) || (this.hasUpdated || i || (e = void 0), this._$AL.set(t, e)), r === !0 && this._$Em !== t && (this._$Eq ?? (this._$Eq = /* @__PURE__ */ new Set())).add(t));
  }
  async _$EP() {
    this.isUpdatePending = !0;
    try {
      await this._$ES;
    } catch (e) {
      Promise.reject(e);
    }
    const t = this.scheduleUpdate();
    return t != null && await t, !this.isUpdatePending;
  }
  scheduleUpdate() {
    return this.performUpdate();
  }
  performUpdate() {
    var i;
    if (!this.isUpdatePending) return;
    if (!this.hasUpdated) {
      if (this.renderRoot ?? (this.renderRoot = this.createRenderRoot()), this._$Ep) {
        for (const [n, o] of this._$Ep) this[n] = o;
        this._$Ep = void 0;
      }
      const r = this.constructor.elementProperties;
      if (r.size > 0) for (const [n, o] of r) {
        const { wrapped: a } = o, h = this[n];
        a !== !0 || this._$AL.has(n) || h === void 0 || this.C(n, void 0, o, h);
      }
    }
    let t = !1;
    const e = this._$AL;
    try {
      t = this.shouldUpdate(e), t ? (this.willUpdate(e), (i = this._$EO) == null || i.forEach((r) => {
        var n;
        return (n = r.hostUpdate) == null ? void 0 : n.call(r);
      }), this.update(e)) : this._$EM();
    } catch (r) {
      throw t = !1, this._$EM(), r;
    }
    t && this._$AE(e);
  }
  willUpdate(t) {
  }
  _$AE(t) {
    var e;
    (e = this._$EO) == null || e.forEach((i) => {
      var r;
      return (r = i.hostUpdated) == null ? void 0 : r.call(i);
    }), this.hasUpdated || (this.hasUpdated = !0, this.firstUpdated(t)), this.updated(t);
  }
  _$EM() {
    this._$AL = /* @__PURE__ */ new Map(), this.isUpdatePending = !1;
  }
  get updateComplete() {
    return this.getUpdateComplete();
  }
  getUpdateComplete() {
    return this._$ES;
  }
  shouldUpdate(t) {
    return !0;
  }
  update(t) {
    this._$Eq && (this._$Eq = this._$Eq.forEach((e) => this._$ET(e, this[e]))), this._$EM();
  }
  updated(t) {
  }
  firstUpdated(t) {
  }
}
d.elementStyles = [], d.shadowRootOptions = { mode: "open" }, d[u("elementProperties")] = /* @__PURE__ */ new Map(), d[u("finalized")] = /* @__PURE__ */ new Map(), m == null || m({ ReactiveElement: d }), (c.reactiveElementVersions ?? (c.reactiveElementVersions = [])).push("2.1.0");
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const l = globalThis;
class y extends d {
  constructor() {
    super(...arguments), this.renderOptions = { host: this }, this._$Do = void 0;
  }
  createRenderRoot() {
    var e;
    const t = super.createRenderRoot();
    return (e = this.renderOptions).renderBefore ?? (e.renderBefore = t.firstChild), t;
  }
  update(t) {
    const e = this.render();
    this.hasUpdated || (this.renderOptions.isConnected = this.isConnected), super.update(t), this._$Do = N(e, this.renderRoot, this.renderOptions);
  }
  connectedCallback() {
    var t;
    super.connectedCallback(), (t = this._$Do) == null || t.setConnected(!0);
  }
  disconnectedCallback() {
    var t;
    super.disconnectedCallback(), (t = this._$Do) == null || t.setConnected(!1);
  }
  render() {
    return q;
  }
}
var M;
y._$litElement$ = !0, y.finalized = !0, (M = l.litElementHydrateSupport) == null || M.call(l, { LitElement: y });
const E = l.litElementPolyfillSupport;
E == null || E({ LitElement: y });
(l.litElementVersions ?? (l.litElementVersions = [])).push("4.2.0");
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const Y = (s) => (t, e) => {
  e !== void 0 ? e.addInitializer(() => {
    customElements.define(s, t);
  }) : customElements.define(s, t);
};
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const Z = { attribute: !0, type: String, converter: $, reflect: !1, hasChanged: w }, tt = (s = Z, t, e) => {
  const { kind: i, metadata: r } = e;
  let n = globalThis.litPropertyMetadata.get(r);
  if (n === void 0 && globalThis.litPropertyMetadata.set(r, n = /* @__PURE__ */ new Map()), i === "setter" && ((s = Object.create(s)).wrapped = !0), n.set(e.name, s), i === "accessor") {
    const { name: o } = e;
    return { set(a) {
      const h = t.get.call(this);
      t.set.call(this, a), this.requestUpdate(o, h, s);
    }, init(a) {
      return a !== void 0 && this.C(o, void 0, s, a), a;
    } };
  }
  if (i === "setter") {
    const { name: o } = e;
    return function(a) {
      const h = this[o];
      t.call(this, a), this.requestUpdate(o, h, s);
    };
  }
  throw Error("Unsupported decorator location: " + i);
};
function et(s) {
  return (t, e) => typeof e == "object" ? tt(s, t, e) : ((i, r, n) => {
    const o = r.hasOwnProperty(n);
    return r.constructor.createProperty(n, i), o ? Object.getOwnPropertyDescriptor(r, n) : void 0;
  })(s, t, e);
}
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
function z(s) {
  return et({ ...s, state: !0, attribute: !1 });
}
/**
 * @license
 * Copyright 2021 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
function st(s, t, e) {
  return s ? t(s) : e == null ? void 0 : e(s);
}
var it = Object.defineProperty, rt = Object.getOwnPropertyDescriptor, x = (s) => {
  throw TypeError(s);
}, g = (s, t, e, i) => {
  for (var r = i > 1 ? void 0 : i ? rt(t, e) : t, n = s.length - 1, o; n >= 0; n--)
    (o = s[n]) && (r = (i ? o(t, e, r) : o(r)) || r);
  return i && r && it(t, e, r), r;
}, P = (s, t, e) => t.has(s) || x("Cannot " + e), D = (s, t, e) => (P(s, t, "read from private field"), e ? e.call(s) : t.get(s)), T = (s, t, e) => t.has(s) ? x("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(s) : t.set(s, e), ot = (s, t, e, i) => (P(s, t, "write to private field"), t.set(s, e), e), nt = (s, t, e) => (P(s, t, "access private method"), e), p, v, j;
const at = "shopify-products-modal";
let f = class extends L {
  constructor() {
    super(), T(this, v), T(this, p), this.token = "", this.isTokenAvailable = !1, this.consumeContext(B, (s) => {
      s && ot(this, p, s);
    });
  }
  async connectedCallback() {
    super.connectedCallback(), await nt(this, v, j).call(this);
  }
  isAuthorized() {
    var s, t;
    return (t = (s = this.data) == null ? void 0 : s.authResponse) == null ? void 0 : t.isAuthorized;
  }
  async _revoke() {
    await D(this, p).revokeToken() && (this.value = {
      authResponse: {
        isAuthorized: !1,
        isValid: !1,
        isFreeAccount: !1
      }
    }, this.requestUpdate(), this.dispatchEvent(new CustomEvent("property-value-change")), this._submitModal());
  }
  render() {
    var s, t;
    return C`
            <umb-body-layout>
                <uui-box>
                    <div>
                        <p>
                            <b>Connected: </b>${this.isTokenAvailable && this.isAuthorized()}
                        </p>
                        <p>
                            <b>Account: </b>${this.isAuthorized() ? (t = (s = this.data) == null ? void 0 : s.authResponse) != null && t.isFreeAccount ? "Free" : "Paid" : "N/A"}
                        </p>
                        ${st(this.isAuthorized(), () => C`
                            <p class="semrush-text-wrap">
                                <b>Access Token: </b>
                                <span>${this.token}</span>
                            </p>
                        `)}
                    </div>

                    <uui-button label="Revoke" look="primary" color="danger" ?disabled=${!this.isAuthorized()} @click=${this._revoke}></uui-button>
                </uui-box>

                <uui-button slot="actions" label="Close" @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
  }
};
p = /* @__PURE__ */ new WeakMap();
v = /* @__PURE__ */ new WeakSet();
j = async function() {
  var t, e;
  var s = await D(this, p).getTokenDetails();
  s && (this.token = (t = s.data) == null ? void 0 : t.access_token, this.isTokenAvailable = (e = s.data) == null ? void 0 : e.isAccessTokenAvailable);
};
f.styles = [
  V`
          .semrush-text-wrap{
            word-break: break-all;
            white-space: normal;
          }
      `
];
g([
  z()
], f.prototype, "token", 2);
g([
  z()
], f.prototype, "isTokenAvailable", 2);
f = g([
  Y(at)
], f);
export {
  f as default
};
//# sourceMappingURL=semrush-modal.element-CaF7oUdI.js.map
