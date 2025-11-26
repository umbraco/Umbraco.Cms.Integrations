var A = Object.defineProperty;
var U = (e, r, t) => r in e ? A(e, r, { enumerable: !0, configurable: !0, writable: !0, value: t }) : e[r] = t;
var v = (e, r, t) => U(e, typeof r != "symbol" ? r + "" : r, t);
import { UMB_AUTH_CONTEXT as R } from "@umbraco-cms/backoffice/auth";
import { umbHttpClient as _ } from "@umbraco-cms/backoffice/http-client";
const I = {
  type: "dashboard",
  alias: "Algolia.Dashboard",
  name: "Algolia Search Management",
  element: () => import("./algolia-dashboard.element-D4kmJ3GI.js"),
  meta: {
    label: "Algolia Search Management",
    pathname: "algolia-search-management"
  },
  conditions: [
    {
      alias: "Umb.Condition.SectionAlias",
      match: "Umb.Section.Settings"
    }
  ]
}, T = I, E = {
  type: "globalContext",
  alias: "algolia.context",
  name: "Algolia Context",
  js: () => import("./algolia-index.context-FyjSKGue.js")
}, z = E;
var W = async (e, r) => {
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
}, k = (e) => {
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
}, M = (e) => {
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
}, S = ({ allowReserved: e, explode: r, name: t, style: s, value: o }) => {
  if (!r) {
    let a = (e ? o : o.map((i) => encodeURIComponent(i))).join(k(s));
    switch (s) {
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
  let l = N(s), n = o.map((a) => s === "label" || s === "simple" ? e ? a : encodeURIComponent(a) : g({ allowReserved: e, name: t, value: a })).join(l);
  return s === "label" || s === "matrix" ? l + n : n;
}, g = ({ allowReserved: e, name: r, value: t }) => {
  if (t == null) return "";
  if (typeof t == "object") throw new Error("Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these.");
  return `${r}=${e ? t : encodeURIComponent(t)}`;
}, $ = ({ allowReserved: e, explode: r, name: t, style: s, value: o, valueOnly: l }) => {
  if (o instanceof Date) return l ? o.toISOString() : `${t}=${o.toISOString()}`;
  if (s !== "deepObject" && !r) {
    let i = [];
    Object.entries(o).forEach(([d, m]) => {
      i = [...i, d, e ? m : encodeURIComponent(m)];
    });
    let f = i.join(",");
    switch (s) {
      case "form":
        return `${t}=${f}`;
      case "label":
        return `.${f}`;
      case "matrix":
        return `;${t}=${f}`;
      default:
        return f;
    }
  }
  let n = M(s), a = Object.entries(o).map(([i, f]) => g({ allowReserved: e, name: s === "deepObject" ? `${t}[${i}]` : i, value: f })).join(n);
  return s === "label" || s === "matrix" ? n + a : a;
}, H = /\{[^{}]+\}/g, P = ({ path: e, url: r }) => {
  let t = r, s = r.match(H);
  if (s) for (let o of s) {
    let l = !1, n = o.substring(1, o.length - 1), a = "simple";
    n.endsWith("*") && (l = !0, n = n.substring(0, n.length - 1)), n.startsWith(".") ? (n = n.substring(1), a = "label") : n.startsWith(";") && (n = n.substring(1), a = "matrix");
    let i = e[n];
    if (i == null) continue;
    if (Array.isArray(i)) {
      t = t.replace(o, S({ explode: l, name: n, style: a, value: i }));
      continue;
    }
    if (typeof i == "object") {
      t = t.replace(o, $({ explode: l, name: n, style: a, value: i, valueOnly: !0 }));
      continue;
    }
    if (a === "matrix") {
      t = t.replace(o, `;${g({ name: n, value: i })}`);
      continue;
    }
    let f = encodeURIComponent(a === "label" ? `.${i}` : i);
    t = t.replace(o, f);
  }
  return t;
}, C = ({ allowReserved: e, array: r, object: t } = {}) => (s) => {
  let o = [];
  if (s && typeof s == "object") for (let l in s) {
    let n = s[l];
    if (n != null) if (Array.isArray(n)) {
      let a = S({ allowReserved: e, explode: !0, name: l, style: "form", value: n, ...r });
      a && o.push(a);
    } else if (typeof n == "object") {
      let a = $({ allowReserved: e, explode: !0, name: l, style: "deepObject", value: n, ...t });
      a && o.push(a);
    } else {
      let a = g({ allowReserved: e, name: l, value: n });
      a && o.push(a);
    }
  }
  return o.join("&");
}, B = (e) => {
  var t;
  if (!e) return "stream";
  let r = (t = e.split(";")[0]) == null ? void 0 : t.trim();
  if (r) {
    if (r.startsWith("application/json") || r.endsWith("+json")) return "json";
    if (r === "multipart/form-data") return "formData";
    if (["application/", "audio/", "image/", "video/"].some((s) => r.startsWith(s))) return "blob";
    if (r.startsWith("text/")) return "text";
  }
}, J = async ({ security: e, ...r }) => {
  for (let t of e) {
    let s = await W(t, r.auth);
    if (!s) continue;
    let o = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        r.query || (r.query = {}), r.query[o] = s;
        break;
      case "cookie":
        r.headers.append("Cookie", `${o}=${s}`);
        break;
      case "header":
      default:
        r.headers.set(o, s);
        break;
    }
    return;
  }
}, x = (e) => L({ baseUrl: e.baseUrl, path: e.path, query: e.query, querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : C(e.querySerializer), url: e.url }), L = ({ baseUrl: e, path: r, query: t, querySerializer: s, url: o }) => {
  let l = o.startsWith("/") ? o : `/${o}`, n = (e ?? "") + l;
  r && (n = P({ path: r, url: n }));
  let a = t ? s(t) : "";
  return a.startsWith("?") && (a = a.substring(1)), a && (n += `?${a}`), n;
}, j = (e, r) => {
  var s;
  let t = { ...e, ...r };
  return (s = t.baseUrl) != null && s.endsWith("/") && (t.baseUrl = t.baseUrl.substring(0, t.baseUrl.length - 1)), t.headers = O(e.headers, r.headers), t;
}, O = (...e) => {
  let r = new Headers();
  for (let t of e) {
    if (!t || typeof t != "object") continue;
    let s = t instanceof Headers ? t.entries() : Object.entries(t);
    for (let [o, l] of s) if (l === null) r.delete(o);
    else if (Array.isArray(l)) for (let n of l) r.append(o, n);
    else l !== void 0 && r.set(o, typeof l == "object" ? JSON.stringify(l) : l);
  }
  return r;
}, w = class {
  constructor() {
    v(this, "_fns");
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
}, V = () => ({ error: new w(), request: new w(), response: new w() }), G = C({ allowReserved: !1, array: { explode: !0, style: "form" }, object: { explode: !0, style: "deepObject" } }), F = { "Content-Type": "application/json" }, q = (e = {}) => ({ ...D, headers: F, parseAs: "auto", querySerializer: G, ...e }), Q = (e = {}) => {
  let r = j(q(), e), t = () => ({ ...r }), s = (n) => (r = j(r, n), t()), o = V(), l = async (n) => {
    let a = { ...r, ...n, fetch: n.fetch ?? r.fetch ?? globalThis.fetch, headers: O(r.headers, n.headers) };
    a.security && await J({ ...a, security: a.security }), a.body && a.bodySerializer && (a.body = a.bodySerializer(a.body)), (a.body === void 0 || a.body === "") && a.headers.delete("Content-Type");
    let i = x(a), f = { redirect: "follow", ...a }, d = new Request(i, f);
    for (let c of o.request._fns) c && (d = await c(d, a));
    let m = a.fetch, u = await m(d);
    for (let c of o.response._fns) c && (u = await c(u, d, a));
    let y = { request: d, response: u };
    if (u.ok) {
      if (u.status === 204 || u.headers.get("Content-Length") === "0") return a.responseStyle === "data" ? {} : { data: {}, ...y };
      let c = (a.parseAs === "auto" ? B(u.headers.get("Content-Type")) : a.parseAs) ?? "json";
      if (c === "stream") return a.responseStyle === "data" ? u.body : { data: u.body, ...y };
      let h = await u[c]();
      return c === "json" && (a.responseValidator && await a.responseValidator(h), a.responseTransformer && (h = await a.responseTransformer(h))), a.responseStyle === "data" ? h : { data: h, ...y };
    }
    let b = await u.text();
    try {
      b = JSON.parse(b);
    } catch {
    }
    let p = b;
    for (let c of o.error._fns) c && (p = await c(b, u, d, a));
    if (p = p || {}, a.throwOnError) throw p;
    return a.responseStyle === "data" ? void 0 : { error: p, ...y };
  };
  return { buildUrl: x, connect: (n) => l({ ...n, method: "CONNECT" }), delete: (n) => l({ ...n, method: "DELETE" }), get: (n) => l({ ...n, method: "GET" }), getConfig: t, head: (n) => l({ ...n, method: "HEAD" }), interceptors: o, options: (n) => l({ ...n, method: "OPTIONS" }), patch: (n) => l({ ...n, method: "PATCH" }), post: (n) => l({ ...n, method: "POST" }), put: (n) => l({ ...n, method: "PUT" }), request: l, setConfig: s, trace: (n) => l({ ...n, method: "TRACE" }) };
};
const X = Q(q({
  baseUrl: "http://localhost:28157",
  throwOnError: !0
})), ee = (e, r) => {
  r.registerMany([T, z]), e.consumeContext(R, async (t) => {
    t && X.setConfig(_.getConfig());
  });
};
export {
  X as c,
  ee as o
};
//# sourceMappingURL=index-DHm3D8am.js.map
