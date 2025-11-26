var C = Object.defineProperty;
var I = (e, r, t) => r in e ? C(e, r, { enumerable: !0, configurable: !0, writable: !0, value: t }) : e[r] = t;
var w = (e, r, t) => I(e, typeof r != "symbol" ? r + "" : r, t);
import { UMB_AUTH_CONTEXT as P } from "@umbraco-cms/backoffice/auth";
import { umbHttpClient as O } from "@umbraco-cms/backoffice/http-client";
const q = {
  type: "globalContext",
  alias: "shopify.context",
  name: "Shopify Context",
  js: () => import("./shopify.context-CW9f65sx.js")
}, z = q, R = {
  type: "propertyEditorUi",
  alias: "Shopify.PropertyEditorUi.Amount",
  name: "Shopify Product Picker Amount Setting",
  element: () => import("./amount-property-editor.element-D2I8ny88.js"),
  meta: {
    label: "Amount",
    icon: "icon-autofill",
    group: "common"
  }
}, _ = {
  type: "propertyEditorUi",
  alias: "Shopify.PropertyEditorUi.Authorization",
  name: "Shopify Product Picker Authorization Setting",
  element: () => import("./authorization-property-editor.element-KLvidTXJ.js"),
  meta: {
    label: "Authorization",
    icon: "icon-autofill",
    group: "common"
  }
}, T = {
  type: "propertyEditorUi",
  alias: "Shopify.PropertyEditorUi.ProductPicker",
  name: "Shopify Product Picker Property Editor UI",
  element: () => import("./shopify-product-picker-property-editor.element-DwRdl7bP.js"),
  meta: {
    label: "Shopify Product Picker",
    icon: "icon-shopping-basket-alt",
    group: "pickers",
    propertyEditorSchemaAlias: "Umbraco.Cms.Integrations.Commerce.Shopify.ProductPicker",
    settings: {
      properties: [
        {
          alias: "authorization",
          label: "Authorization",
          description: "Authorize your Shopify connection.",
          propertyEditorUiAlias: "Shopify.PropertyEditorUi.Authorization"
        },
        {
          alias: "minItems",
          label: "Minimum number of items",
          description: "Set a minimum number of items selected.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.Integer"
        },
        {
          alias: "maxItems",
          label: "Maximum number of items",
          description: "Set a maximum number of items selected.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.Integer"
        }
      ],
      defaultData: [
        { alias: "minItems", value: 0 },
        { alias: "maxItems", value: 2 }
      ]
    }
  }
}, k = [
  T,
  R,
  _
], W = {
  type: "modal",
  alias: "Shopify.Modal",
  name: "Shopify Modal",
  js: () => import("./shopify-products-modal.element-lOtPZy7y.js")
};
var M = async (e, r) => {
  let t = typeof r == "function" ? await r(e) : r;
  if (t) return e.scheme === "bearer" ? `Bearer ${t}` : e.scheme === "basic" ? `Basic ${btoa(t)}` : t;
}, D = { bodySerializer: (e) => JSON.stringify(e, (r, t) => typeof t == "bigint" ? t.toString() : t) }, N = (e) => {
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
}, H = (e) => {
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
}, B = (e) => {
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
}, U = ({ allowReserved: e, explode: r, name: t, style: i, value: n }) => {
  if (!r) {
    let o = (e ? n : n.map((l) => encodeURIComponent(l))).join(H(i));
    switch (i) {
      case "label":
        return `.${o}`;
      case "matrix":
        return `;${t}=${o}`;
      case "simple":
        return o;
      default:
        return `${t}=${o}`;
    }
  }
  let s = N(i), a = n.map((o) => i === "label" || i === "simple" ? e ? o : encodeURIComponent(o) : g({ allowReserved: e, name: t, value: o })).join(s);
  return i === "label" || i === "matrix" ? s + a : a;
}, g = ({ allowReserved: e, name: r, value: t }) => {
  if (t == null) return "";
  if (typeof t == "object") throw new Error("Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these.");
  return `${r}=${e ? t : encodeURIComponent(t)}`;
}, j = ({ allowReserved: e, explode: r, name: t, style: i, value: n, valueOnly: s }) => {
  if (n instanceof Date) return s ? n.toISOString() : `${t}=${n.toISOString()}`;
  if (i !== "deepObject" && !r) {
    let l = [];
    Object.entries(n).forEach(([f, h]) => {
      l = [...l, f, e ? h : encodeURIComponent(h)];
    });
    let c = l.join(",");
    switch (i) {
      case "form":
        return `${t}=${c}`;
      case "label":
        return `.${c}`;
      case "matrix":
        return `;${t}=${c}`;
      default:
        return c;
    }
  }
  let a = B(i), o = Object.entries(n).map(([l, c]) => g({ allowReserved: e, name: i === "deepObject" ? `${t}[${l}]` : l, value: c })).join(a);
  return i === "label" || i === "matrix" ? a + o : o;
}, J = /\{[^{}]+\}/g, L = ({ path: e, url: r }) => {
  let t = r, i = r.match(J);
  if (i) for (let n of i) {
    let s = !1, a = n.substring(1, n.length - 1), o = "simple";
    a.endsWith("*") && (s = !0, a = a.substring(0, a.length - 1)), a.startsWith(".") ? (a = a.substring(1), o = "label") : a.startsWith(";") && (a = a.substring(1), o = "matrix");
    let l = e[a];
    if (l == null) continue;
    if (Array.isArray(l)) {
      t = t.replace(n, U({ explode: s, name: a, style: o, value: l }));
      continue;
    }
    if (typeof l == "object") {
      t = t.replace(n, j({ explode: s, name: a, style: o, value: l, valueOnly: !0 }));
      continue;
    }
    if (o === "matrix") {
      t = t.replace(n, `;${g({ name: a, value: l })}`);
      continue;
    }
    let c = encodeURIComponent(o === "label" ? `.${l}` : l);
    t = t.replace(n, c);
  }
  return t;
}, A = ({ allowReserved: e, array: r, object: t } = {}) => (i) => {
  let n = [];
  if (i && typeof i == "object") for (let s in i) {
    let a = i[s];
    if (a != null) if (Array.isArray(a)) {
      let o = U({ allowReserved: e, explode: !0, name: s, style: "form", value: a, ...r });
      o && n.push(o);
    } else if (typeof a == "object") {
      let o = j({ allowReserved: e, explode: !0, name: s, style: "deepObject", value: a, ...t });
      o && n.push(o);
    } else {
      let o = g({ allowReserved: e, name: s, value: a });
      o && n.push(o);
    }
  }
  return n.join("&");
}, V = (e) => {
  var t;
  if (!e) return "stream";
  let r = (t = e.split(";")[0]) == null ? void 0 : t.trim();
  if (r) {
    if (r.startsWith("application/json") || r.endsWith("+json")) return "json";
    if (r === "multipart/form-data") return "formData";
    if (["application/", "audio/", "image/", "video/"].some((i) => r.startsWith(i))) return "blob";
    if (r.startsWith("text/")) return "text";
  }
}, G = async ({ security: e, ...r }) => {
  for (let t of e) {
    let i = await M(t, r.auth);
    if (!i) continue;
    let n = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        r.query || (r.query = {}), r.query[n] = i;
        break;
      case "cookie":
        r.headers.append("Cookie", `${n}=${i}`);
        break;
      case "header":
      default:
        r.headers.set(n, i);
        break;
    }
    return;
  }
}, x = (e) => F({ baseUrl: e.baseUrl, path: e.path, query: e.query, querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : A(e.querySerializer), url: e.url }), F = ({ baseUrl: e, path: r, query: t, querySerializer: i, url: n }) => {
  let s = n.startsWith("/") ? n : `/${n}`, a = (e ?? "") + s;
  r && (a = L({ path: r, url: a }));
  let o = t ? i(t) : "";
  return o.startsWith("?") && (o = o.substring(1)), o && (a += `?${o}`), a;
}, v = (e, r) => {
  var i;
  let t = { ...e, ...r };
  return (i = t.baseUrl) != null && i.endsWith("/") && (t.baseUrl = t.baseUrl.substring(0, t.baseUrl.length - 1)), t.headers = E(e.headers, r.headers), t;
}, E = (...e) => {
  let r = new Headers();
  for (let t of e) {
    if (!t || typeof t != "object") continue;
    let i = t instanceof Headers ? t.entries() : Object.entries(t);
    for (let [n, s] of i) if (s === null) r.delete(n);
    else if (Array.isArray(s)) for (let a of s) r.append(n, a);
    else s !== void 0 && r.set(n, typeof s == "object" ? JSON.stringify(s) : s);
  }
  return r;
}, S = class {
  constructor() {
    w(this, "_fns");
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
}, Q = () => ({ error: new S(), request: new S(), response: new S() }), X = A({ allowReserved: !1, array: { explode: !0, style: "form" }, object: { explode: !0, style: "deepObject" } }), K = { "Content-Type": "application/json" }, $ = (e = {}) => ({ ...D, headers: K, parseAs: "auto", querySerializer: X, ...e }), Y = (e = {}) => {
  let r = v($(), e), t = () => ({ ...r }), i = (a) => (r = v(r, a), t()), n = Q(), s = async (a) => {
    let o = { ...r, ...a, fetch: a.fetch ?? r.fetch ?? globalThis.fetch, headers: E(r.headers, a.headers) };
    o.security && await G({ ...o, security: o.security }), o.body && o.bodySerializer && (o.body = o.bodySerializer(o.body)), (o.body === void 0 || o.body === "") && o.headers.delete("Content-Type");
    let l = x(o), c = { redirect: "follow", ...o }, f = new Request(l, c);
    for (let p of n.request._fns) p && (f = await p(f, o));
    let h = o.fetch, u = await h(f);
    for (let p of n.response._fns) p && (u = await p(u, f, o));
    let y = { request: f, response: u };
    if (u.ok) {
      if (u.status === 204 || u.headers.get("Content-Length") === "0") return o.responseStyle === "data" ? {} : { data: {}, ...y };
      let p = (o.parseAs === "auto" ? V(u.headers.get("Content-Type")) : o.parseAs) ?? "json";
      if (p === "stream") return o.responseStyle === "data" ? u.body : { data: u.body, ...y };
      let m = await u[p]();
      return p === "json" && (o.responseValidator && await o.responseValidator(m), o.responseTransformer && (m = await o.responseTransformer(m))), o.responseStyle === "data" ? m : { data: m, ...y };
    }
    let b = await u.text();
    try {
      b = JSON.parse(b);
    } catch {
    }
    let d = b;
    for (let p of n.error._fns) p && (d = await p(b, u, f, o));
    if (d = d || {}, o.throwOnError) throw d;
    return o.responseStyle === "data" ? void 0 : { error: d, ...y };
  };
  return { buildUrl: x, connect: (a) => s({ ...a, method: "CONNECT" }), delete: (a) => s({ ...a, method: "DELETE" }), get: (a) => s({ ...a, method: "GET" }), getConfig: t, head: (a) => s({ ...a, method: "HEAD" }), interceptors: n, options: (a) => s({ ...a, method: "OPTIONS" }), patch: (a) => s({ ...a, method: "PATCH" }), post: (a) => s({ ...a, method: "POST" }), put: (a) => s({ ...a, method: "PUT" }), request: s, setConfig: i, trace: (a) => s({ ...a, method: "TRACE" }) };
};
const Z = Y($({
  baseUrl: "http://localhost:28157",
  throwOnError: !0
})), oe = (e, r) => {
  r.registerMany([
    ...k,
    W,
    z
  ]), e.consumeContext(P, async (t) => {
    t && Z.setConfig(O.getConfig());
  });
};
export {
  Z as c,
  oe as o
};
//# sourceMappingURL=index-gYX5oh2M.js.map
