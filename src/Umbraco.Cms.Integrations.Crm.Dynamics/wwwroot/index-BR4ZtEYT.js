var ae = Object.defineProperty;
var I = (r) => {
  throw TypeError(r);
};
var se = (r, e, t) => e in r ? ae(r, e, { enumerable: !0, configurable: !0, writable: !0, value: t }) : r[e] = t;
var W = (r, e, t) => se(r, typeof e != "symbol" ? e + "" : e, t), B = (r, e, t) => e.has(r) || I("Cannot " + t);
var d = (r, e, t) => (B(r, e, "read from private field"), t ? t.call(r) : e.get(r)), z = (r, e, t) => e.has(r) ? I("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), $ = (r, e, t, n) => (B(r, e, "write to private field"), n ? n.call(r, t) : e.set(r, t), t);
import { UMB_AUTH_CONTEXT as ne } from "@umbraco-cms/backoffice/auth";
import { umbHttpClient as ie } from "@umbraco-cms/backoffice/http-client";
import { UmbElementMixin as oe } from "@umbraco-cms/backoffice/element-api";
import { LitElement as ce, html as E, state as N, customElement as le } from "@umbraco-cms/backoffice/external/lit";
import { UmbControllerBase as J } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as ue } from "@umbraco-cms/backoffice/context-api";
import { tryExecute as p } from "@umbraco-cms/backoffice/resources";
import { UmbObjectState as he } from "@umbraco-cms/backoffice/observable-api";
import { UMB_NOTIFICATION_CONTEXT as de } from "@umbraco-cms/backoffice/notification";
const me = {
  type: "globalContext",
  alias: "dynamics.context",
  name: "Dynamics Context",
  js: () => Promise.resolve().then(() => Fe)
}, ye = me, fe = {
  type: "propertyEditorUi",
  alias: "Dynamics.PropertyEditorUi.Authorization",
  name: "Dynamics Form Picker Authorization Setting",
  js: () => Promise.resolve().then(() => qe),
  meta: {
    label: "Authorization",
    icon: "icon-autofill",
    group: "common"
  }
}, pe = {
  type: "propertyEditorUi",
  alias: "Dynamics.PropertyEditorUi.FormPicker",
  name: "Dynamics Form Picker Property Editor UI",
  js: () => import("./dynamics-form-picker-property-editor.element-D-SutgyU.js"),
  meta: {
    label: "Dynamics Form Picker",
    icon: "icon-handshake",
    group: "pickers",
    propertyEditorSchemaAlias: "Umbraco.Cms.Integrations.Crm.Dynamics.FormPicker",
    settings: {
      properties: [
        {
          alias: "configuration",
          label: "Configuration",
          description: "Connect with your Microsoft account.",
          propertyEditorUiAlias: "Dynamics.PropertyEditorUi.Authorization"
        },
        {
          alias: "modules",
          label: "Modules",
          description: "Select the Microsoft Dynamics modules you want to use.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.RadioButtonList",
          config: [{ alias: "items", value: ["Outbound", "RealTime", "Both"] }]
        }
      ]
    }
  }
}, ge = [
  pe,
  fe
], be = {
  type: "modal",
  alias: "Dynamics.Modal",
  name: "Dynamics Modal",
  js: () => import("./dynamics-form-modal.element-BdAAWIYo.js")
};
var we = async (r, e) => {
  let t = typeof e == "function" ? await e(r) : e;
  if (t) return r.scheme === "bearer" ? `Bearer ${t}` : r.scheme === "basic" ? `Basic ${btoa(t)}` : t;
}, ve = { bodySerializer: (r) => JSON.stringify(r, (e, t) => typeof t == "bigint" ? t.toString() : t) }, _e = (r) => {
  switch (r) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, Ce = (r) => {
  switch (r) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, ke = (r) => {
  switch (r) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, G = ({ allowReserved: r, explode: e, name: t, style: n, value: i }) => {
  if (!e) {
    let s = (r ? i : i.map((c) => encodeURIComponent(c))).join(Ce(n));
    switch (n) {
      case "label":
        return `.${s}`;
      case "matrix":
        return `;${t}=${s}`;
      case "simple":
        return s;
      default:
        return `${t}=${s}`;
    }
  }
  let o = _e(n), a = i.map((s) => n === "label" || n === "simple" ? r ? s : encodeURIComponent(s) : x({ allowReserved: r, name: t, value: s })).join(o);
  return n === "label" || n === "matrix" ? o + a : a;
}, x = ({ allowReserved: r, name: e, value: t }) => {
  if (t == null) return "";
  if (typeof t == "object") throw new Error("Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these.");
  return `${e}=${r ? t : encodeURIComponent(t)}`;
}, X = ({ allowReserved: r, explode: e, name: t, style: n, value: i, valueOnly: o }) => {
  if (i instanceof Date) return o ? i.toISOString() : `${t}=${i.toISOString()}`;
  if (n !== "deepObject" && !e) {
    let c = [];
    Object.entries(i).forEach(([f, T]) => {
      c = [...c, f, r ? T : encodeURIComponent(T)];
    });
    let m = c.join(",");
    switch (n) {
      case "form":
        return `${t}=${m}`;
      case "label":
        return `.${m}`;
      case "matrix":
        return `;${t}=${m}`;
      default:
        return m;
    }
  }
  let a = ke(n), s = Object.entries(i).map(([c, m]) => x({ allowReserved: r, name: n === "deepObject" ? `${t}[${c}]` : c, value: m })).join(a);
  return n === "label" || n === "matrix" ? a + s : s;
}, Ae = /\{[^{}]+\}/g, Ee = ({ path: r, url: e }) => {
  let t = e, n = e.match(Ae);
  if (n) for (let i of n) {
    let o = !1, a = i.substring(1, i.length - 1), s = "simple";
    a.endsWith("*") && (o = !0, a = a.substring(0, a.length - 1)), a.startsWith(".") ? (a = a.substring(1), s = "label") : a.startsWith(";") && (a = a.substring(1), s = "matrix");
    let c = r[a];
    if (c == null) continue;
    if (Array.isArray(c)) {
      t = t.replace(i, G({ explode: o, name: a, style: s, value: c }));
      continue;
    }
    if (typeof c == "object") {
      t = t.replace(i, X({ explode: o, name: a, style: s, value: c, valueOnly: !0 }));
      continue;
    }
    if (s === "matrix") {
      t = t.replace(i, `;${x({ name: a, value: c })}`);
      continue;
    }
    let m = encodeURIComponent(s === "label" ? `.${c}` : c);
    t = t.replace(i, m);
  }
  return t;
}, K = ({ allowReserved: r, array: e, object: t } = {}) => (n) => {
  let i = [];
  if (n && typeof n == "object") for (let o in n) {
    let a = n[o];
    if (a != null) if (Array.isArray(a)) {
      let s = G({ allowReserved: r, explode: !0, name: o, style: "form", value: a, ...e });
      s && i.push(s);
    } else if (typeof a == "object") {
      let s = X({ allowReserved: r, explode: !0, name: o, style: "deepObject", value: a, ...t });
      s && i.push(s);
    } else {
      let s = x({ allowReserved: r, name: o, value: a });
      s && i.push(s);
    }
  }
  return i.join("&");
}, Se = (r) => {
  var t;
  if (!r) return "stream";
  let e = (t = r.split(";")[0]) == null ? void 0 : t.trim();
  if (e) {
    if (e.startsWith("application/json") || e.endsWith("+json")) return "json";
    if (e === "multipart/form-data") return "formData";
    if (["application/", "audio/", "image/", "video/"].some((n) => e.startsWith(n))) return "blob";
    if (e.startsWith("text/")) return "text";
  }
}, Te = async ({ security: r, ...e }) => {
  for (let t of r) {
    let n = await we(t, e.auth);
    if (!n) continue;
    let i = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        e.query || (e.query = {}), e.query[i] = n;
        break;
      case "cookie":
        e.headers.append("Cookie", `${i}=${n}`);
        break;
      case "header":
      default:
        e.headers.set(i, n);
        break;
    }
    return;
  }
}, V = (r) => Ue({ baseUrl: r.baseUrl, path: r.path, query: r.query, querySerializer: typeof r.querySerializer == "function" ? r.querySerializer : K(r.querySerializer), url: r.url }), Ue = ({ baseUrl: r, path: e, query: t, querySerializer: n, url: i }) => {
  let o = i.startsWith("/") ? i : `/${i}`, a = (r ?? "") + o;
  e && (a = Ee({ path: e, url: a }));
  let s = t ? n(t) : "";
  return s.startsWith("?") && (s = s.substring(1)), s && (a += `?${s}`), a;
}, H = (r, e) => {
  var n;
  let t = { ...r, ...e };
  return (n = t.baseUrl) != null && n.endsWith("/") && (t.baseUrl = t.baseUrl.substring(0, t.baseUrl.length - 1)), t.headers = Q(r.headers, e.headers), t;
}, Q = (...r) => {
  let e = new Headers();
  for (let t of r) {
    if (!t || typeof t != "object") continue;
    let n = t instanceof Headers ? t.entries() : Object.entries(t);
    for (let [i, o] of n) if (o === null) e.delete(i);
    else if (Array.isArray(o)) for (let a of o) e.append(i, a);
    else o !== void 0 && e.set(i, typeof o == "object" ? JSON.stringify(o) : o);
  }
  return e;
}, D = class {
  constructor() {
    W(this, "_fns");
    this._fns = [];
  }
  clear() {
    this._fns = [];
  }
  getInterceptorIndex(r) {
    return typeof r == "number" ? this._fns[r] ? r : -1 : this._fns.indexOf(r);
  }
  exists(r) {
    let e = this.getInterceptorIndex(r);
    return !!this._fns[e];
  }
  eject(r) {
    let e = this.getInterceptorIndex(r);
    this._fns[e] && (this._fns[e] = null);
  }
  update(r, e) {
    let t = this.getInterceptorIndex(r);
    return this._fns[t] ? (this._fns[t] = e, r) : !1;
  }
  use(r) {
    return this._fns = [...this._fns, r], this._fns.length - 1;
  }
}, Oe = () => ({ error: new D(), request: new D(), response: new D() }), xe = K({ allowReserved: !1, array: { explode: !0, style: "form" }, object: { explode: !0, style: "deepObject" } }), je = { "Content-Type": "application/json" }, Y = (r = {}) => ({ ...ve, headers: je, parseAs: "auto", querySerializer: xe, ...r }), ze = (r = {}) => {
  let e = H(Y(), r), t = () => ({ ...e }), n = (a) => (e = H(e, a), t()), i = Oe(), o = async (a) => {
    let s = { ...e, ...a, fetch: a.fetch ?? e.fetch ?? globalThis.fetch, headers: Q(e.headers, a.headers) };
    s.security && await Te({ ...s, security: s.security }), s.body && s.bodySerializer && (s.body = s.bodySerializer(s.body)), (s.body === void 0 || s.body === "") && s.headers.delete("Content-Type");
    let c = V(s), m = { redirect: "follow", ...s }, f = new Request(c, m);
    for (let u of i.request._fns) u && (f = await u(f, s));
    let T = s.fetch, l = await T(f);
    for (let u of i.response._fns) u && (l = await u(l, f, s));
    let U = { request: f, response: l };
    if (l.ok) {
      if (l.status === 204 || l.headers.get("Content-Length") === "0") return s.responseStyle === "data" ? {} : { data: {}, ...U };
      let u = (s.parseAs === "auto" ? Se(l.headers.get("Content-Type")) : s.parseAs) ?? "json";
      if (u === "stream") return s.responseStyle === "data" ? l.body : { data: l.body, ...U };
      let A = await l[u]();
      return u === "json" && (s.responseValidator && await s.responseValidator(A), s.responseTransformer && (A = await s.responseTransformer(A))), s.responseStyle === "data" ? A : { data: A, ...U };
    }
    let O = await l.text();
    try {
      O = JSON.parse(O);
    } catch {
    }
    let k = O;
    for (let u of i.error._fns) u && (k = await u(O, l, f, s));
    if (k = k || {}, s.throwOnError) throw k;
    return s.responseStyle === "data" ? void 0 : { error: k, ...U };
  };
  return { buildUrl: V, connect: (a) => o({ ...a, method: "CONNECT" }), delete: (a) => o({ ...a, method: "DELETE" }), get: (a) => o({ ...a, method: "GET" }), getConfig: t, head: (a) => o({ ...a, method: "HEAD" }), interceptors: i, options: (a) => o({ ...a, method: "OPTIONS" }), patch: (a) => o({ ...a, method: "PATCH" }), post: (a) => o({ ...a, method: "POST" }), put: (a) => o({ ...a, method: "PUT" }), request: o, setConfig: n, trace: (a) => o({ ...a, method: "TRACE" }) };
};
const y = ze(Y({
  baseUrl: "http://localhost:28157",
  throwOnError: !0
}));
class w {
  static getForms(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dynamics/management/api/v1/forms",
      ...e
    });
  }
  static postFormsAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? y).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dynamics/management/api/v1/forms/access-token",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e == null ? void 0 : e.headers
      }
    });
  }
  static getFormsAuthorizationUrl(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dynamics/management/api/v1/forms/authorization-url",
      ...e
    });
  }
  static getFormsEmbedCode(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dynamics/management/api/v1/forms/embed-code",
      ...e
    });
  }
  static getFormsOauthConfiguration(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dynamics/management/api/v1/forms/oauth-configuration",
      ...e
    });
  }
  static deleteFormsRevokeAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? y).delete({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dynamics/management/api/v1/forms/revoke-access-token",
      ...e
    });
  }
  static getFormsSystemUserFullname(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dynamics/management/api/v1/forms/system-user-fullname",
      ...e
    });
  }
}
class $e {
  static getUmbracoApiDynamicsAuthorization(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/api/dynamics/authorization",
      ...e
    });
  }
}
class De extends J {
  constructor(e) {
    super(e);
  }
  async getForms(e) {
    const { data: t, error: n } = await p(this, w.getForms({
      query: {
        module: e
      }
    }));
    return n || !t ? { error: n } : { data: t };
  }
  async revokeAccessToken() {
    const { data: e, error: t } = await p(this, w.deleteFormsRevokeAccessToken());
    return t || !e ? { error: t } : { data: e };
  }
  async getAuthorizationUrl() {
    const { data: e, error: t } = await p(this, w.getFormsAuthorizationUrl());
    return t || !e ? { error: t } : { data: e };
  }
  async checkOauthConfiguration() {
    const { data: e, error: t } = await p(this, w.getFormsOauthConfiguration());
    return t || !e ? { error: t } : { data: e };
  }
  async getAccessToken(e) {
    const { data: t, error: n } = await p(this, w.postFormsAccessToken({ body: e }));
    return n || !t ? { error: n } : { data: t };
  }
  async getEmbedCode(e) {
    const { data: t, error: n } = await p(this, w.getFormsEmbedCode({
      query: {
        formId: e
      }
    }));
    return n || !t ? { error: n } : { data: t };
  }
  async getSystemUserFullName() {
    const { data: e, error: t } = await p(this, w.getFormsSystemUserFullname());
    return t || !e ? { error: t } : { data: e };
  }
  async oauth(e) {
    const { data: t, error: n } = await p(this, $e.getUmbracoApiDynamicsAuthorization({
      query: {
        code: e
      }
    }));
    return n || !t ? { error: n } : { data: t };
  }
}
var h, C;
class R extends J {
  constructor(t) {
    super(t);
    z(this, h);
    z(this, C);
    $(this, C, new he(void 0)), this.settingsModel = d(this, C).asObservable(), this.provideContext(P, this), $(this, h, new De(t));
  }
  async hostConnected() {
    super.hostConnected(), await this.checkOauthConfiguration();
  }
  async getForms(t) {
    return await d(this, h).getForms(t);
  }
  async revokeAccessToken() {
    return await d(this, h).revokeAccessToken();
  }
  async getAuthorizationUrl() {
    return await d(this, h).getAuthorizationUrl();
  }
  async checkOauthConfiguration() {
    const { data: t } = await d(this, h).checkOauthConfiguration();
    d(this, C).setValue(t);
  }
  async getAccessToken(t) {
    return await d(this, h).getAccessToken(t);
  }
  async getEmbedCode(t) {
    return await d(this, h).getEmbedCode(t);
  }
  async getSystemUserFullName() {
    return await d(this, h).getSystemUserFullName();
  }
  async oauth(t) {
    return await d(this, h).oauth(t);
  }
}
h = new WeakMap(), C = new WeakMap();
const P = new ue(R.name), Fe = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  DYNAMICS_CONTEXT_TOKEN: P,
  DynamicsContext: R,
  default: R
}, Symbol.toStringTag, { value: "Module" }));
var Me = Object.defineProperty, Re = Object.getOwnPropertyDescriptor, Z = (r) => {
  throw TypeError(r);
}, j = (r, e, t, n) => {
  for (var i = n > 1 ? void 0 : n ? Re(e, t) : e, o = r.length - 1, a; o >= 0; o--)
    (a = r[o]) && (i = (n ? a(e, t, i) : a(i)) || i);
  return n && i && Me(e, t, i), i;
}, q = (r, e, t) => e.has(r) || Z("Cannot " + t), b = (r, e, t) => (q(r, e, "read from private field"), t ? t.call(r) : e.get(r)), F = (r, e, t) => e.has(r) ? Z("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), L = (r, e, t, n) => (q(r, e, "write to private field"), e.set(r, t), t), M = (r, e, t) => (q(r, e, "access private method"), t), g, _, S, ee, te, re;
const Ne = "dynamics-authorization";
let v = class extends oe(ce) {
  constructor() {
    super(), F(this, S), F(this, g), F(this, _), this._oauthSetup = {
      isConnected: !1,
      isAccessTokenExpired: !0,
      isAccessTokenValid: !1
    }, this._loading = !0, this._userName = "", this.consumeContext(P, (r) => {
      r && (L(this, g, r), this.observe(r.settingsModel, (e) => {
        L(this, _, e);
      }));
    });
  }
  async connectedCallback() {
    super.connectedCallback(), setTimeout(() => {
      M(this, S, ee).call(this);
    }, 3e3);
  }
  async getAccessToken(r) {
    this._loading = !1;
    const { data: e } = await b(this, g).getAccessToken(r);
    if (e)
      if (e.startsWith("Error:"))
        this._showError(e);
      else {
        this._oauthSetup = {
          isConnected: !0
        }, this._showSuccess("OAuth Connected"), await b(this, g).checkOauthConfiguration();
        const { data: t } = await b(this, g).getSystemUserFullName();
        this._userName = t, this.dispatchEvent(new CustomEvent("connect"));
      }
  }
  async _showSuccess(r) {
    await this._showMessage(r, "positive");
  }
  async _showError(r) {
    await this._showMessage(r, "danger");
  }
  async _showMessage(r, e) {
    const t = await this.getContext(de);
    t == null || t.peek(e, {
      data: { message: r }
    });
  }
  render() {
    var r, e;
    return E`
            ${this._loading ? E`
                    <div class="center loader"><uui-loader></uui-loader></div>
                ` : E`
                    <div>
                        ${this._oauthSetup.isConnected ? E`
                                <span>
                                    <b>Connected</b>: ${this._userName}
                                </span>
                            ` : E`
                                <span>
                                    <b>Disconnected</b>
                                </span>
                            `}
                        
                    </div>
                    <div>
                        <uui-button 
                            look="primary" 
                            label="Connect"
                            ?disabled=${(r = this._oauthSetup) == null ? void 0 : r.isConnected}
                            @click=${M(this, S, te)}></uui-button>
                        <uui-button 
                            color="danger" 
                            look="primary" 
                            label="Revoke"
                            ?disabled=${!((e = this._oauthSetup) != null && e.isConnected)}
                            @click=${M(this, S, re)}></uui-button>
                    </div>
                `}
        `;
  }
};
g = /* @__PURE__ */ new WeakMap();
_ = /* @__PURE__ */ new WeakMap();
S = /* @__PURE__ */ new WeakSet();
ee = function() {
  b(this, _) && (b(this, _).isAuthorized ? (this._oauthSetup = {
    isConnected: !0,
    isAccessTokenExpired: !1,
    isAccessTokenValid: !0
  }, this._userName = b(this, _).fullName) : this._showError("Unable to connect to Dynamics. Please review the settings of the form picker property's data type."), this._loading = !1);
};
te = async function() {
  const { data: r } = await b(this, g).getAuthorizationUrl();
  if (!r) return;
  const e = window.open(r, "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
  this._loading = !0, setTimeout(() => {
    e != null && e.closed && (this._loading = !1);
  }, 3e3), window.addEventListener("message", async (t) => {
    if (t.data.type === "dynamics:oauth:success") {
      const n = {
        code: t.data.code
      };
      await this.getAccessToken(n);
    }
  }, !1);
};
re = async function() {
  await b(this, g).revokeAccessToken(), this._oauthSetup = {
    isConnected: !1
  }, this._showSuccess("OAuth connection revoked."), this.dispatchEvent(new CustomEvent("revoke"));
};
j([
  N()
], v.prototype, "_oauthSetup", 2);
j([
  N()
], v.prototype, "_loading", 2);
j([
  N()
], v.prototype, "_userName", 2);
v = j([
  le(Ne)
], v);
const Pe = v, qe = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get DynamicsAuthorizationElement() {
    return v;
  },
  default: Pe
}, Symbol.toStringTag, { value: "Module" })), Qe = (r, e) => {
  e.registerMany([
    ye,
    ...ge,
    be
  ]), r.consumeContext(ne, async (t) => {
    t && y.setConfig(ie.getConfig());
  });
};
export {
  P as D,
  v as a,
  Qe as o
};
//# sourceMappingURL=index-BR4ZtEYT.js.map
