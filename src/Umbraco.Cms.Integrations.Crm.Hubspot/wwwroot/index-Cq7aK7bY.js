var ue = Object.defineProperty;
var V = (t) => {
  throw TypeError(t);
};
var ce = (t, e, r) => e in t ? ue(t, e, { enumerable: !0, configurable: !0, writable: !0, value: r }) : t[e] = r;
var D = (t, e, r) => ce(t, typeof e != "symbol" ? e + "" : e, r), W = (t, e, r) => e.has(t) || V("Cannot " + r);
var p = (t, e, r) => (W(t, e, "read from private field"), r ? r.call(t) : e.get(t)), P = (t, e, r) => e.has(t) ? V("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), $ = (t, e, r, o) => (W(t, e, "write to private field"), o ? o.call(t, r) : e.set(t, r), r);
import { UMB_AUTH_CONTEXT as le } from "@umbraco-cms/backoffice/auth";
import { umbHttpClient as he } from "@umbraco-cms/backoffice/http-client";
import { UmbElementMixin as pe } from "@umbraco-cms/backoffice/element-api";
import { LitElement as de, when as B, html as I, css as fe, state as M, property as me, query as ye, customElement as be } from "@umbraco-cms/backoffice/external/lit";
import { UMB_NOTIFICATION_CONTEXT as ge } from "@umbraco-cms/backoffice/notification";
import { UmbControllerBase as J } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as ve } from "@umbraco-cms/backoffice/context-api";
import { tryExecute as v } from "@umbraco-cms/backoffice/resources";
import { UmbObjectState as Ae } from "@umbraco-cms/backoffice/observable-api";
const we = {
  type: "propertyEditorUi",
  alias: "HubSpot.PropertyEditorUi.FormPicker",
  name: "HubSpot Form Picker Property Editor UI",
  js: () => import("./form-picker-property-editor.element-CD-pNCQw.js"),
  elementName: "hubspot-form-picker",
  meta: {
    label: "HubSpot Form Picker",
    icon: "icon-handshake",
    group: "pickers",
    propertyEditorSchemaAlias: "HubSpot.FormPicker"
  }
}, ke = {
  type: "propertyEditorSchema",
  name: "HubSpot Form Picker",
  alias: "HubSpot.FormPicker",
  meta: {
    defaultPropertyEditorUiAlias: "HubSpot.PropertyEditorUi.FormPicker",
    settings: {
      properties: [
        {
          alias: "hubspot.authorization",
          label: "Authorization",
          description: "Authorization Details",
          propertyEditorUiAlias: "HubSpot.PropertyEditorUi.Authorization"
        }
      ]
    }
  }
}, _e = {
  type: "propertyEditorUi",
  alias: "HubSpot.PropertyEditorUi.Authorization",
  name: "HubSpot Authorization Property Editor UI",
  js: () => Promise.resolve().then(() => Le),
  elementName: "hubspot-authorization",
  meta: {
    label: "Authorization",
    icon: "",
    group: ""
  }
}, Ce = [
  we,
  ke,
  _e
], Se = {
  type: "globalContext",
  alias: "hubspot-forms.context",
  name: "Hubspot Forms Context",
  js: () => Promise.resolve().then(() => De)
}, Te = Se, Ee = {
  type: "modal",
  alias: "HubspotForms.Modal",
  name: "Hubspot Forms Modal",
  js: () => import("./hubspot-forms-modal.element-DxwcZ8li.js")
};
var Oe = async (t, e) => {
  let r = typeof e == "function" ? await e(t) : e;
  if (r) return t.scheme === "bearer" ? `Bearer ${r}` : t.scheme === "basic" ? `Basic ${btoa(r)}` : r;
}, xe = { bodySerializer: (t) => JSON.stringify(t, (e, r) => typeof r == "bigint" ? r.toString() : r) }, Ue = (t) => {
  switch (t) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, ze = (t) => {
  switch (t) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, je = (t) => {
  switch (t) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, X = ({ allowReserved: t, explode: e, name: r, style: o, value: i }) => {
  if (!e) {
    let a = (t ? i : i.map((u) => encodeURIComponent(u))).join(ze(o));
    switch (o) {
      case "label":
        return `.${a}`;
      case "matrix":
        return `;${r}=${a}`;
      case "simple":
        return a;
      default:
        return `${r}=${a}`;
    }
  }
  let n = Ue(o), s = i.map((a) => o === "label" || o === "simple" ? t ? a : encodeURIComponent(a) : j({ allowReserved: t, name: r, value: a })).join(n);
  return o === "label" || o === "matrix" ? n + s : s;
}, j = ({ allowReserved: t, name: e, value: r }) => {
  if (r == null) return "";
  if (typeof r == "object") throw new Error("Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these.");
  return `${e}=${t ? r : encodeURIComponent(r)}`;
}, Q = ({ allowReserved: t, explode: e, name: r, style: o, value: i, valueOnly: n }) => {
  if (i instanceof Date) return n ? i.toISOString() : `${r}=${i.toISOString()}`;
  if (o !== "deepObject" && !e) {
    let u = [];
    Object.entries(i).forEach(([g, x]) => {
      u = [...u, g, t ? x : encodeURIComponent(x)];
    });
    let f = u.join(",");
    switch (o) {
      case "form":
        return `${r}=${f}`;
      case "label":
        return `.${f}`;
      case "matrix":
        return `;${r}=${f}`;
      default:
        return f;
    }
  }
  let s = je(o), a = Object.entries(i).map(([u, f]) => j({ allowReserved: t, name: o === "deepObject" ? `${r}[${u}]` : u, value: f })).join(s);
  return o === "label" || o === "matrix" ? s + a : a;
}, Pe = /\{[^{}]+\}/g, $e = ({ path: t, url: e }) => {
  let r = e, o = e.match(Pe);
  if (o) for (let i of o) {
    let n = !1, s = i.substring(1, i.length - 1), a = "simple";
    s.endsWith("*") && (n = !0, s = s.substring(0, s.length - 1)), s.startsWith(".") ? (s = s.substring(1), a = "label") : s.startsWith(";") && (s = s.substring(1), a = "matrix");
    let u = t[s];
    if (u == null) continue;
    if (Array.isArray(u)) {
      r = r.replace(i, X({ explode: n, name: s, style: a, value: u }));
      continue;
    }
    if (typeof u == "object") {
      r = r.replace(i, Q({ explode: n, name: s, style: a, value: u, valueOnly: !0 }));
      continue;
    }
    if (a === "matrix") {
      r = r.replace(i, `;${j({ name: s, value: u })}`);
      continue;
    }
    let f = encodeURIComponent(a === "label" ? `.${u}` : u);
    r = r.replace(i, f);
  }
  return r;
}, Y = ({ allowReserved: t, array: e, object: r } = {}) => (o) => {
  let i = [];
  if (o && typeof o == "object") for (let n in o) {
    let s = o[n];
    if (s != null) if (Array.isArray(s)) {
      let a = X({ allowReserved: t, explode: !0, name: n, style: "form", value: s, ...e });
      a && i.push(a);
    } else if (typeof s == "object") {
      let a = Q({ allowReserved: t, explode: !0, name: n, style: "deepObject", value: s, ...r });
      a && i.push(a);
    } else {
      let a = j({ allowReserved: t, name: n, value: s });
      a && i.push(a);
    }
  }
  return i.join("&");
}, Ie = (t) => {
  var r;
  if (!t) return "stream";
  let e = (r = t.split(";")[0]) == null ? void 0 : r.trim();
  if (e) {
    if (e.startsWith("application/json") || e.endsWith("+json")) return "json";
    if (e === "multipart/form-data") return "formData";
    if (["application/", "audio/", "image/", "video/"].some((o) => e.startsWith(o))) return "blob";
    if (e.startsWith("text/")) return "text";
  }
}, He = async ({ security: t, ...e }) => {
  for (let r of t) {
    let o = await Oe(r, e.auth);
    if (!o) continue;
    let i = r.name ?? "Authorization";
    switch (r.in) {
      case "query":
        e.query || (e.query = {}), e.query[i] = o;
        break;
      case "cookie":
        e.headers.append("Cookie", `${i}=${o}`);
        break;
      case "header":
      default:
        e.headers.set(i, o);
        break;
    }
    return;
  }
}, K = (t) => Re({ baseUrl: t.baseUrl, path: t.path, query: t.query, querySerializer: typeof t.querySerializer == "function" ? t.querySerializer : Y(t.querySerializer), url: t.url }), Re = ({ baseUrl: t, path: e, query: r, querySerializer: o, url: i }) => {
  let n = i.startsWith("/") ? i : `/${i}`, s = (t ?? "") + n;
  e && (s = $e({ path: e, url: s }));
  let a = r ? o(r) : "";
  return a.startsWith("?") && (a = a.substring(1)), a && (s += `?${a}`), s;
}, G = (t, e) => {
  var o;
  let r = { ...t, ...e };
  return (o = r.baseUrl) != null && o.endsWith("/") && (r.baseUrl = r.baseUrl.substring(0, r.baseUrl.length - 1)), r.headers = Z(t.headers, e.headers), r;
}, Z = (...t) => {
  let e = new Headers();
  for (let r of t) {
    if (!r || typeof r != "object") continue;
    let o = r instanceof Headers ? r.entries() : Object.entries(r);
    for (let [i, n] of o) if (n === null) e.delete(i);
    else if (Array.isArray(n)) for (let s of n) e.append(i, s);
    else n !== void 0 && e.set(i, typeof n == "object" ? JSON.stringify(n) : n);
  }
  return e;
}, H = class {
  constructor() {
    D(this, "_fns");
    this._fns = [];
  }
  clear() {
    this._fns = [];
  }
  getInterceptorIndex(t) {
    return typeof t == "number" ? this._fns[t] ? t : -1 : this._fns.indexOf(t);
  }
  exists(t) {
    let e = this.getInterceptorIndex(t);
    return !!this._fns[e];
  }
  eject(t) {
    let e = this.getInterceptorIndex(t);
    this._fns[e] && (this._fns[e] = null);
  }
  update(t, e) {
    let r = this.getInterceptorIndex(t);
    return this._fns[r] ? (this._fns[r] = e, t) : !1;
  }
  use(t) {
    return this._fns = [...this._fns, t], this._fns.length - 1;
  }
}, Fe = () => ({ error: new H(), request: new H(), response: new H() }), Me = Y({ allowReserved: !1, array: { explode: !0, style: "form" }, object: { explode: !0, style: "deepObject" } }), qe = { "Content-Type": "application/json" }, ee = (t = {}) => ({ ...xe, headers: qe, parseAs: "auto", querySerializer: Me, ...t }), Ne = (t = {}) => {
  let e = G(ee(), t), r = () => ({ ...e }), o = (s) => (e = G(e, s), r()), i = Fe(), n = async (s) => {
    let a = { ...e, ...s, fetch: s.fetch ?? e.fetch ?? globalThis.fetch, headers: Z(e.headers, s.headers) };
    a.security && await He({ ...a, security: a.security }), a.body && a.bodySerializer && (a.body = a.bodySerializer(a.body)), (a.body === void 0 || a.body === "") && a.headers.delete("Content-Type");
    let u = K(a), f = { redirect: "follow", ...a }, g = new Request(u, f);
    for (let l of i.request._fns) l && (g = await l(g, a));
    let x = a.fetch, c = await x(g);
    for (let l of i.response._fns) l && (c = await l(c, g, a));
    let U = { request: g, response: c };
    if (c.ok) {
      if (c.status === 204 || c.headers.get("Content-Length") === "0") return a.responseStyle === "data" ? {} : { data: {}, ...U };
      let l = (a.parseAs === "auto" ? Ie(c.headers.get("Content-Type")) : a.parseAs) ?? "json";
      if (l === "stream") return a.responseStyle === "data" ? c.body : { data: c.body, ...U };
      let O = await c[l]();
      return l === "json" && (a.responseValidator && await a.responseValidator(O), a.responseTransformer && (O = await a.responseTransformer(O))), a.responseStyle === "data" ? O : { data: O, ...U };
    }
    let z = await c.text();
    try {
      z = JSON.parse(z);
    } catch {
    }
    let E = z;
    for (let l of i.error._fns) l && (E = await l(z, c, g, a));
    if (E = E || {}, a.throwOnError) throw E;
    return a.responseStyle === "data" ? void 0 : { error: E, ...U };
  };
  return { buildUrl: K, connect: (s) => n({ ...s, method: "CONNECT" }), delete: (s) => n({ ...s, method: "DELETE" }), get: (s) => n({ ...s, method: "GET" }), getConfig: r, head: (s) => n({ ...s, method: "HEAD" }), interceptors: i, options: (s) => n({ ...s, method: "OPTIONS" }), patch: (s) => n({ ...s, method: "PATCH" }), post: (s) => n({ ...s, method: "POST" }), put: (s) => n({ ...s, method: "PUT" }), request: n, setConfig: o, trace: (s) => n({ ...s, method: "TRACE" }) };
};
const y = Ne(ee({
  baseUrl: "http://localhost:28157",
  throwOnError: !0
}));
class A {
  static postGetAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? y).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/access-token",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e == null ? void 0 : e.headers
      }
    });
  }
  static getAuthorizationUrl(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/authorization-url",
      ...e
    });
  }
  static getCheckConfiguration(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/check-configuration",
      ...e
    });
  }
  static getFormsByApiKey(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/get",
      ...e
    });
  }
  static getFormsOAuth(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/oauth/get",
      ...e
    });
  }
  static postRefreshAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? y).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/refresh",
      ...e
    });
  }
  static postRevokeAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? y).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/revoke",
      ...e
    });
  }
  static getValidateAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/validate",
      ...e
    });
  }
}
const _ = {
  api: "An API key is configured and will be used to connect to your HubSpot account.",
  oauth: "No API key is configured. To connect to your HubSpot account using OAuth click 'Connect', select your account and agree to the permissions.",
  oauthConnected: "OAuth is configured and an access token is available to connect to your HubSpot account. To revoke, click 'Revoke'",
  none: "No API or OAuth configuration could be found. Please review your settings before continuing."
};
class Ve extends J {
  constructor(e) {
    super(e);
  }
  async getAuthorizationUrl() {
    const { data: e, error: r } = await v(this, A.getAuthorizationUrl());
    return r || !e ? { error: r } : { data: e };
  }
  async checkApiConfiguration() {
    const { data: e, error: r } = await v(this, A.getCheckConfiguration());
    return r || !e ? { error: r } : { data: e };
  }
  async getAccessToken(e) {
    const { data: r, error: o } = await v(this, A.postGetAccessToken({
      body: e
    }));
    return o || !r ? { error: o } : { data: r };
  }
  async validateAccessToken() {
    const { data: e, error: r } = await v(this, A.getValidateAccessToken());
    return r || !e ? { error: r } : { data: e };
  }
  async refreshAccessToken() {
    const { data: e, error: r } = await v(this, A.postRefreshAccessToken());
    return r || !e ? { error: r } : { data: e };
  }
  async revokeAccessToken() {
    const { data: e, error: r } = await v(this, A.postRevokeAccessToken());
    return r || !e ? { error: r } : { data: e };
  }
  async getFormsByApiKey() {
    const { data: e, error: r } = await v(this, A.getFormsByApiKey());
    return r || !e ? { error: r } : { data: e };
  }
  async getFormsOAuth() {
    const { data: e, error: r } = await v(this, A.getFormsOAuth());
    return r || !e ? { error: r } : { data: e };
  }
}
var h, S;
class F extends J {
  constructor(r) {
    super(r);
    P(this, h);
    P(this, S);
    $(this, S, new Ae(void 0)), this.settingsModel = p(this, S).asObservable(), this.provideContext(q, this), $(this, h, new Ve(r));
  }
  async hostConnected() {
    super.hostConnected(), this.checkApiConfiguration();
  }
  async getAuthorizationUrl() {
    return await p(this, h).getAuthorizationUrl();
  }
  async checkApiConfiguration() {
    const { data: r } = await p(this, h).checkApiConfiguration();
    p(this, S).setValue(r);
  }
  async getAccessToken(r) {
    return await p(this, h).getAccessToken(r);
  }
  async validateAccessToken() {
    return await p(this, h).validateAccessToken();
  }
  async refreshAccessToken() {
    return await p(this, h).refreshAccessToken();
  }
  async revokeAccessToken() {
    return await p(this, h).revokeAccessToken();
  }
  async getFormsByApiKey() {
    return await p(this, h).getFormsByApiKey();
  }
  async getFormsOAuth() {
    return await p(this, h).getFormsOAuth();
  }
}
h = new WeakMap(), S = new WeakMap();
const q = new ve(F.name), De = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  HUBSPOT_FORMS_CONTEXT_TOKEN: q,
  HubspotFormsContext: F,
  default: F
}, Symbol.toStringTag, { value: "Module" }));
var We = Object.defineProperty, Be = Object.getOwnPropertyDescriptor, te = (t) => {
  throw TypeError(t);
}, T = (t, e, r, o) => {
  for (var i = o > 1 ? void 0 : o ? Be(e, r) : e, n = t.length - 1, s; n >= 0; n--)
    (s = t[n]) && (i = (o ? s(e, r, i) : s(i)) || i);
  return o && i && We(e, r, i), i;
}, N = (t, e, r) => e.has(t) || te("Cannot " + r), d = (t, e, r) => (N(t, e, "read from private field"), r ? r.call(t) : e.get(t)), R = (t, e, r) => e.has(t) ? te("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), L = (t, e, r, o) => (N(t, e, "write to private field"), e.set(t, r), r), C = (t, e, r) => (N(t, e, "access private method"), r), k, b, w, re, se, ae, oe, ie, ne;
const Ke = "hubspot-authorization";
let m = class extends pe(de) {
  constructor() {
    super(), R(this, w), R(this, k), R(this, b), this._serviceStatus = {
      isValid: !1,
      type: "",
      description: "",
      useOAuth: !1
    }, this._oauthSetup = {
      isConnected: !1,
      isAccessTokenExpired: !1,
      isAccessTokenValid: !1
    }, this.value = "", this.showAuthTokenComponent = !1, this.consumeContext(q, (t) => {
      t && (L(this, k, t), this.observe(t.settingsModel, (e) => {
        L(this, b, e);
      }));
    });
  }
  async connectedCallback() {
    super.connectedCallback(), await C(this, w, re).call(this);
  }
  async getAccessToken(t) {
    const { data: e } = await d(this, k).getAccessToken(t);
    e && (this.showAuthTokenComponent = !1, e.startsWith("Error:") ? this._showError(e) : (this._oauthSetup = {
      isConnected: !0
    }, this._serviceStatus.description = _.oauthConnected, this._showSuccess("OAuth Connected"), this.dispatchEvent(new CustomEvent("connect"))));
  }
  async _showSuccess(t) {
    await this._showMessage(t, "positive");
  }
  async _showError(t) {
    await this._showMessage(t, "danger");
  }
  async _showMessage(t, e) {
    const r = await this.getContext(ge);
    r == null || r.peek(e, {
      data: { message: t }
    });
  }
  render() {
    return I`
            <p>${this._serviceStatus.description}</p>
            ${B(
      this._serviceStatus.useOAuth,
      () => I`
                    <div id="authConnect">
                        <uui-button look="primary" 
                                    label="Connect"
                                    ?disabled=${this._oauthSetup.isConnected}
                                    @click=${C(this, w, oe)}></uui-button>
                        <uui-button look="primary"
                                    color="danger"
                                    label="Revoke"
                                    ?disabled=${!this._oauthSetup.isConnected}
                                    @click=${C(this, w, ie)}></uui-button>
                    </div>
                    ${B(this.showAuthTokenComponent, () => I`
                        <div id="authToken">
                            <uui-input id="auth-code-input" placeholder="Authorization code"></uui-input>
                            <uui-button look="primary"
                                        label="Authorize"
                                        @click=${C(this, w, ne)}></uui-button>
                        </div>
                    `)}
                `
    )}
        `;
  }
};
k = /* @__PURE__ */ new WeakMap();
b = /* @__PURE__ */ new WeakMap();
w = /* @__PURE__ */ new WeakSet();
re = async function() {
  var t, e;
  d(this, b) && (this._serviceStatus = {
    isValid: d(this, b).isValid,
    type: (t = d(this, b).type) == null ? void 0 : t.value,
    description: C(this, w, ae).call(this, this._serviceStatus.type),
    useOAuth: d(this, b).isValid && ((e = d(this, b).type) == null ? void 0 : e.value) === "OAuth"
  }, this._serviceStatus.useOAuth && await C(this, w, se).call(this), d(this, b).isValid || this._showError("Invalid setup. Please review the API/OAuth settings."));
};
se = async function() {
  const { data: t } = await d(this, k).validateAccessToken();
  t && (this._oauthSetup = {
    isConnected: t.isValid,
    isAccessTokenExpired: t.isExpired,
    isAccessTokenValid: t.isValid
  }, this._oauthSetup.isConnected && this._oauthSetup.isAccessTokenValid && (this._serviceStatus.description = _.oauthConnected), this._oauthSetup.isAccessTokenExpired && await d(this, k).refreshAccessToken());
};
ae = function(t) {
  switch (t) {
    case "api":
      return _.api;
    case "oauth":
      return _.oauth;
    case "oauthConnected":
      return _.oauthConnected;
    default:
      return _.none;
  }
};
oe = async function() {
  const { data: t } = await d(this, k).getAuthorizationUrl();
  if (!t) return;
  var e = document.getElementById("authToken");
  e && (e.style.display = "none");
  const r = window.open(t, "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
  setTimeout(() => {
    r != null && r.closed || (this.showAuthTokenComponent = !0);
  }, 7e3), window.addEventListener("message", async (o) => {
    if (o.data.type === "hubspot:oauth:success") {
      const i = {
        code: o.data.code
      };
      await this.getAccessToken(i);
    }
  }, !1);
};
ie = async function() {
  await d(this, k).revokeAccessToken(), this._oauthSetup = {
    isConnected: !1
  }, this._serviceStatus.description = _.none, this._showSuccess("OAuth connection revoked."), this.dispatchEvent(new CustomEvent("revoke"));
};
ne = async function() {
  if (this._authCodeInput.value.length == 0) {
    this._showError("Incorrect authorization code.");
    return;
  }
  const t = {
    code: this._authCodeInput.value
  };
  await this.getAccessToken(t);
};
m.styles = [
  fe`
            #authToken { 
                margin-top: 20px; 
            }
            #authToken uui-input {
                width: 50%;
                vertical-align: middle;
            }
        `
];
T([
  M()
], m.prototype, "_serviceStatus", 2);
T([
  M()
], m.prototype, "_oauthSetup", 2);
T([
  me({ type: String })
], m.prototype, "value", 2);
T([
  M()
], m.prototype, "showAuthTokenComponent", 2);
T([
  ye("#auth-code-input")
], m.prototype, "_authCodeInput", 2);
m = T([
  be(Ke)
], m);
const Ge = m, Le = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get HubspotAuthorizationElement() {
    return m;
  },
  default: Ge
}, Symbol.toStringTag, { value: "Module" })), ot = (t, e) => {
  e.registerMany([
    ...Ce,
    Te,
    Ee
  ]), t.consumeContext(le, async (r) => {
    r && y.setConfig(he.getConfig());
  });
};
export {
  _ as C,
  q as H,
  m as a,
  ot as o
};
//# sourceMappingURL=index-Cq7aK7bY.js.map
