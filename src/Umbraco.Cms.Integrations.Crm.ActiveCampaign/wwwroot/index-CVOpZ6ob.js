var $ = Object.defineProperty;
var O = (e, r, t) => r in e ? $(e, r, { enumerable: !0, configurable: !0, writable: !0, value: t }) : e[r] = t;
var C = (e, r, t) => O(e, typeof r != "symbol" ? r + "" : r, t);
import { UMB_AUTH_CONTEXT as I } from "@umbraco-cms/backoffice/auth";
const P = {
  type: "propertyEditorUi",
  alias: "ActiveCampaign.PropertyEditorUi.FormPicker",
  name: "ActiveCampaign Form Picker Property Editor UI",
  js: () => import("./form-picker-property-editor.element-B-6gwbTk.js"),
  elementName: "activecampaign-form-picker",
  meta: {
    label: "ActiveCampaign Form Picker",
    icon: "icon-activecampaign",
    group: "pickers",
    propertyEditorSchemaAlias: "ActiveCampaign.FormPicker"
  }
}, q = {
  type: "propertyEditorSchema",
  name: "ActiveCampaign Form Picker",
  alias: "ActiveCampaign.FormPicker",
  meta: {
    defaultPropertyEditorUiAlias: "ActiveCampaign.PropertyEditorUi.FormPicker",
    settings: {
      properties: [
        {
          alias: "activecampaign.configuration",
          label: "Configuration",
          description: "API Access",
          propertyEditorUiAlias: "ActiveCampaign.PropertyEditorUi.Configuration"
        }
      ]
    }
  }
}, R = {
  type: "propertyEditorUi",
  alias: "ActiveCampaign.PropertyEditorUi.Configuration",
  name: "ActiveCampaign Configuration Property Editor UI",
  js: () => import("./configuration-property-editor.element-C1jwmW2n.js"),
  elementName: "activecampaign-forms-configuration",
  meta: {
    label: "Configuration",
    icon: "",
    group: ""
  }
}, _ = {
  type: "icons",
  name: "ActiveCampaign Forms Icon",
  alias: "ActiveCampaign.PropertyEditorUi.Icon",
  js: () => import("./icons-dictionary-DIDvu1T_.js")
}, k = [
  P,
  q,
  R,
  _
], T = {
  type: "globalContext",
  alias: "activecampaign-forms.context",
  name: "ActiveCampaign Forms Context",
  js: () => import("./activecampaign-forms.context-DO6P_YQW.js")
}, F = T, z = {
  type: "modal",
  alias: "ActiveCampaignForms.Modal",
  name: "ActiveCampaign Forms Modal",
  js: () => import("./activecampaign-forms-modal.element-BR8GWDK5.js")
};
var N = async (e, r) => {
  let t = typeof r == "function" ? await r(e) : r;
  if (t) return e.scheme === "bearer" ? `Bearer ${t}` : e.scheme === "basic" ? `Basic ${btoa(t)}` : t;
}, W = { bodySerializer: (e) => JSON.stringify(e, (r, t) => typeof t == "bigint" ? t.toString() : t) }, M = (e) => {
  switch (e) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, D = (e) => {
  switch (e) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, H = (e) => {
  switch (e) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, j = ({ allowReserved: e, explode: r, name: t, style: o, value: n }) => {
  if (!r) {
    let a = (e ? n : n.map((l) => encodeURIComponent(l))).join(D(o));
    switch (o) {
      case "label":
        return `.${a}`;
      case "matrix":
        return `;${t}=${a}`;
      case "simple":
        return a;
      default:
        return `${t}=${a}`;
    }
  }
  let s = M(o), i = n.map((a) => o === "label" || o === "simple" ? e ? a : encodeURIComponent(a) : g({ allowReserved: e, name: t, value: a })).join(s);
  return o === "label" || o === "matrix" ? s + i : i;
}, g = ({ allowReserved: e, name: r, value: t }) => {
  if (t == null) return "";
  if (typeof t == "object") throw new Error("Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these.");
  return `${r}=${e ? t : encodeURIComponent(t)}`;
}, x = ({ allowReserved: e, explode: r, name: t, style: o, value: n, valueOnly: s }) => {
  if (n instanceof Date) return s ? n.toISOString() : `${t}=${n.toISOString()}`;
  if (o !== "deepObject" && !r) {
    let l = [];
    Object.entries(n).forEach(([d, y]) => {
      l = [...l, d, e ? y : encodeURIComponent(y)];
    });
    let u = l.join(",");
    switch (o) {
      case "form":
        return `${t}=${u}`;
      case "label":
        return `.${u}`;
      case "matrix":
        return `;${t}=${u}`;
      default:
        return u;
    }
  }
  let i = H(o), a = Object.entries(n).map(([l, u]) => g({ allowReserved: e, name: o === "deepObject" ? `${t}[${l}]` : l, value: u })).join(i);
  return o === "label" || o === "matrix" ? i + a : a;
}, B = /\{[^{}]+\}/g, J = ({ path: e, url: r }) => {
  let t = r, o = r.match(B);
  if (o) for (let n of o) {
    let s = !1, i = n.substring(1, n.length - 1), a = "simple";
    i.endsWith("*") && (s = !0, i = i.substring(0, i.length - 1)), i.startsWith(".") ? (i = i.substring(1), a = "label") : i.startsWith(";") && (i = i.substring(1), a = "matrix");
    let l = e[i];
    if (l == null) continue;
    if (Array.isArray(l)) {
      t = t.replace(n, j({ explode: s, name: i, style: a, value: l }));
      continue;
    }
    if (typeof l == "object") {
      t = t.replace(n, x({ explode: s, name: i, style: a, value: l, valueOnly: !0 }));
      continue;
    }
    if (a === "matrix") {
      t = t.replace(n, `;${g({ name: i, value: l })}`);
      continue;
    }
    let u = encodeURIComponent(a === "label" ? `.${l}` : l);
    t = t.replace(n, u);
  }
  return t;
}, U = ({ allowReserved: e, array: r, object: t } = {}) => (o) => {
  let n = [];
  if (o && typeof o == "object") for (let s in o) {
    let i = o[s];
    if (i != null) if (Array.isArray(i)) {
      let a = j({ allowReserved: e, explode: !0, name: s, style: "form", value: i, ...r });
      a && n.push(a);
    } else if (typeof i == "object") {
      let a = x({ allowReserved: e, explode: !0, name: s, style: "deepObject", value: i, ...t });
      a && n.push(a);
    } else {
      let a = g({ allowReserved: e, name: s, value: i });
      a && n.push(a);
    }
  }
  return n.join("&");
}, L = (e) => {
  var t;
  if (!e) return "stream";
  let r = (t = e.split(";")[0]) == null ? void 0 : t.trim();
  if (r) {
    if (r.startsWith("application/json") || r.endsWith("+json")) return "json";
    if (r === "multipart/form-data") return "formData";
    if (["application/", "audio/", "image/", "video/"].some((o) => r.startsWith(o))) return "blob";
    if (r.startsWith("text/")) return "text";
  }
}, V = async ({ security: e, ...r }) => {
  for (let t of e) {
    let o = await N(t, r.auth);
    if (!o) continue;
    let n = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        r.query || (r.query = {}), r.query[n] = o;
        break;
      case "cookie":
        r.headers.append("Cookie", `${n}=${o}`);
        break;
      case "header":
      default:
        r.headers.set(n, o);
        break;
    }
    return;
  }
}, w = (e) => G({ baseUrl: e.baseUrl, path: e.path, query: e.query, querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : U(e.querySerializer), url: e.url }), G = ({ baseUrl: e, path: r, query: t, querySerializer: o, url: n }) => {
  let s = n.startsWith("/") ? n : `/${n}`, i = (e ?? "") + s;
  r && (i = J({ path: r, url: i }));
  let a = t ? o(t) : "";
  return a.startsWith("?") && (a = a.substring(1)), a && (i += `?${a}`), i;
}, A = (e, r) => {
  var o;
  let t = { ...e, ...r };
  return (o = t.baseUrl) != null && o.endsWith("/") && (t.baseUrl = t.baseUrl.substring(0, t.baseUrl.length - 1)), t.headers = E(e.headers, r.headers), t;
}, E = (...e) => {
  let r = new Headers();
  for (let t of e) {
    if (!t || typeof t != "object") continue;
    let o = t instanceof Headers ? t.entries() : Object.entries(t);
    for (let [n, s] of o) if (s === null) r.delete(n);
    else if (Array.isArray(s)) for (let i of s) r.append(n, i);
    else s !== void 0 && r.set(n, typeof s == "object" ? JSON.stringify(s) : s);
  }
  return r;
}, v = class {
  constructor() {
    C(this, "_fns");
    this._fns = [];
  }
  clear() {
    this._fns = [];
  }
  getInterceptorIndex(e) {
    return typeof e == "number" ? this._fns[e] ? e : -1 : this._fns.indexOf(e);
  }
  exists(e) {
    let r = this.getInterceptorIndex(e);
    return !!this._fns[r];
  }
  eject(e) {
    let r = this.getInterceptorIndex(e);
    this._fns[r] && (this._fns[r] = null);
  }
  update(e, r) {
    let t = this.getInterceptorIndex(e);
    return this._fns[t] ? (this._fns[t] = r, e) : !1;
  }
  use(e) {
    return this._fns = [...this._fns, e], this._fns.length - 1;
  }
}, Q = () => ({ error: new v(), request: new v(), response: new v() }), X = U({ allowReserved: !1, array: { explode: !0, style: "form" }, object: { explode: !0, style: "deepObject" } }), K = { "Content-Type": "application/json" }, S = (e = {}) => ({ ...W, headers: K, parseAs: "auto", querySerializer: X, ...e }), Y = (e = {}) => {
  let r = A(S(), e), t = () => ({ ...r }), o = (i) => (r = A(r, i), t()), n = Q(), s = async (i) => {
    let a = { ...r, ...i, fetch: i.fetch ?? r.fetch ?? globalThis.fetch, headers: E(r.headers, i.headers) };
    a.security && await V({ ...a, security: a.security }), a.body && a.bodySerializer && (a.body = a.bodySerializer(a.body)), (a.body === void 0 || a.body === "") && a.headers.delete("Content-Type");
    let l = w(a), u = { redirect: "follow", ...a }, d = new Request(l, u);
    for (let p of n.request._fns) p && (d = await p(d, a));
    let y = a.fetch, c = await y(d);
    for (let p of n.response._fns) p && (c = await p(c, d, a));
    let h = { request: d, response: c };
    if (c.ok) {
      if (c.status === 204 || c.headers.get("Content-Length") === "0") return a.responseStyle === "data" ? {} : { data: {}, ...h };
      let p = (a.parseAs === "auto" ? L(c.headers.get("Content-Type")) : a.parseAs) ?? "json";
      if (p === "stream") return a.responseStyle === "data" ? c.body : { data: c.body, ...h };
      let f = await c[p]();
      return p === "json" && (a.responseValidator && await a.responseValidator(f), a.responseTransformer && (f = await a.responseTransformer(f))), a.responseStyle === "data" ? f : { data: f, ...h };
    }
    let b = await c.text();
    try {
      b = JSON.parse(b);
    } catch {
    }
    let m = b;
    for (let p of n.error._fns) p && (m = await p(b, c, d, a));
    if (m = m || {}, a.throwOnError) throw m;
    return a.responseStyle === "data" ? void 0 : { error: m, ...h };
  };
  return { buildUrl: w, connect: (i) => s({ ...i, method: "CONNECT" }), delete: (i) => s({ ...i, method: "DELETE" }), get: (i) => s({ ...i, method: "GET" }), getConfig: t, head: (i) => s({ ...i, method: "HEAD" }), interceptors: n, options: (i) => s({ ...i, method: "OPTIONS" }), patch: (i) => s({ ...i, method: "PATCH" }), post: (i) => s({ ...i, method: "POST" }), put: (i) => s({ ...i, method: "PUT" }), request: s, setConfig: o, trace: (i) => s({ ...i, method: "TRACE" }) };
};
const Z = Y(S({
  baseUrl: "http://localhost:28157",
  throwOnError: !0
})), re = (e, r) => {
  r.registerMany([
    ...k,
    F,
    z
  ]), e.consumeContext(I, async (t) => {
    const o = t == null ? void 0 : t.getOpenApiConfiguration();
    Z.setConfig({
      baseUrl: (o == null ? void 0 : o.base) ?? "",
      auth: (o == null ? void 0 : o.token) ?? void 0,
      credentials: (o == null ? void 0 : o.credentials) ?? "same-origin"
    });
  });
};
export {
  Z as c,
  re as o
};
//# sourceMappingURL=index-CVOpZ6ob.js.map
