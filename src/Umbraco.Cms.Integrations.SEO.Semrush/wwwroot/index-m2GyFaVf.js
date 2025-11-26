var q = Object.defineProperty;
var I = (e, r, t) => r in e ? q(e, r, { enumerable: !0, configurable: !0, writable: !0, value: t }) : e[r] = t;
var v = (e, r, t) => I(e, typeof r != "symbol" ? r + "" : r, t);
import { UMB_AUTH_CONTEXT as R } from "@umbraco-cms/backoffice/auth";
import { umbHttpClient as _ } from "@umbraco-cms/backoffice/http-client";
var T = async (e, r) => {
  let t = typeof r == "function" ? await r(e) : r;
  if (t) return e.scheme === "bearer" ? `Bearer ${t}` : e.scheme === "basic" ? `Basic ${btoa(t)}` : t;
}, A = { bodySerializer: (e) => JSON.stringify(e, (r, t) => typeof t == "bigint" ? t.toString() : t) }, W = (e) => {
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
}, E = (e) => {
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
}, S = ({ allowReserved: e, explode: r, name: t, style: n, value: o }) => {
  if (!r) {
    let a = (e ? o : o.map((i) => encodeURIComponent(i))).join(k(n));
    switch (n) {
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
  let l = W(n), s = o.map((a) => n === "label" || n === "simple" ? e ? a : encodeURIComponent(a) : w({ allowReserved: e, name: t, value: a })).join(l);
  return n === "label" || n === "matrix" ? l + s : s;
}, w = ({ allowReserved: e, name: r, value: t }) => {
  if (t == null) return "";
  if (typeof t == "object") throw new Error("Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these.");
  return `${r}=${e ? t : encodeURIComponent(t)}`;
}, $ = ({ allowReserved: e, explode: r, name: t, style: n, value: o, valueOnly: l }) => {
  if (o instanceof Date) return l ? o.toISOString() : `${t}=${o.toISOString()}`;
  if (n !== "deepObject" && !r) {
    let i = [];
    Object.entries(o).forEach(([p, m]) => {
      i = [...i, p, e ? m : encodeURIComponent(m)];
    });
    let f = i.join(",");
    switch (n) {
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
  let s = E(n), a = Object.entries(o).map(([i, f]) => w({ allowReserved: e, name: n === "deepObject" ? `${t}[${i}]` : i, value: f })).join(s);
  return n === "label" || n === "matrix" ? s + a : a;
}, z = /\{[^{}]+\}/g, D = ({ path: e, url: r }) => {
  let t = r, n = r.match(z);
  if (n) for (let o of n) {
    let l = !1, s = o.substring(1, o.length - 1), a = "simple";
    s.endsWith("*") && (l = !0, s = s.substring(0, s.length - 1)), s.startsWith(".") ? (s = s.substring(1), a = "label") : s.startsWith(";") && (s = s.substring(1), a = "matrix");
    let i = e[s];
    if (i == null) continue;
    if (Array.isArray(i)) {
      t = t.replace(o, S({ explode: l, name: s, style: a, value: i }));
      continue;
    }
    if (typeof i == "object") {
      t = t.replace(o, $({ explode: l, name: s, style: a, value: i, valueOnly: !0 }));
      continue;
    }
    if (a === "matrix") {
      t = t.replace(o, `;${w({ name: s, value: i })}`);
      continue;
    }
    let f = encodeURIComponent(a === "label" ? `.${i}` : i);
    t = t.replace(o, f);
  }
  return t;
}, C = ({ allowReserved: e, array: r, object: t } = {}) => (n) => {
  let o = [];
  if (n && typeof n == "object") for (let l in n) {
    let s = n[l];
    if (s != null) if (Array.isArray(s)) {
      let a = S({ allowReserved: e, explode: !0, name: l, style: "form", value: s, ...r });
      a && o.push(a);
    } else if (typeof s == "object") {
      let a = $({ allowReserved: e, explode: !0, name: l, style: "deepObject", value: s, ...t });
      a && o.push(a);
    } else {
      let a = w({ allowReserved: e, name: l, value: s });
      a && o.push(a);
    }
  }
  return o.join("&");
}, N = (e) => {
  var t;
  if (!e) return "stream";
  let r = (t = e.split(";")[0]) == null ? void 0 : t.trim();
  if (r) {
    if (r.startsWith("application/json") || r.endsWith("+json")) return "json";
    if (r === "multipart/form-data") return "formData";
    if (["application/", "audio/", "image/", "video/"].some((n) => r.startsWith(n))) return "blob";
    if (r.startsWith("text/")) return "text";
  }
}, V = async ({ security: e, ...r }) => {
  for (let t of e) {
    let n = await T(t, r.auth);
    if (!n) continue;
    let o = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        r.query || (r.query = {}), r.query[o] = n;
        break;
      case "cookie":
        r.headers.append("Cookie", `${o}=${n}`);
        break;
      case "header":
      default:
        r.headers.set(o, n);
        break;
    }
    return;
  }
}, x = (e) => H({ baseUrl: e.baseUrl, path: e.path, query: e.query, querySerializer: typeof e.querySerializer == "function" ? e.querySerializer : C(e.querySerializer), url: e.url }), H = ({ baseUrl: e, path: r, query: t, querySerializer: n, url: o }) => {
  let l = o.startsWith("/") ? o : `/${o}`, s = (e ?? "") + l;
  r && (s = D({ path: r, url: s }));
  let a = t ? n(t) : "";
  return a.startsWith("?") && (a = a.substring(1)), a && (s += `?${a}`), s;
}, j = (e, r) => {
  var n;
  let t = { ...e, ...r };
  return (n = t.baseUrl) != null && n.endsWith("/") && (t.baseUrl = t.baseUrl.substring(0, t.baseUrl.length - 1)), t.headers = O(e.headers, r.headers), t;
}, O = (...e) => {
  let r = new Headers();
  for (let t of e) {
    if (!t || typeof t != "object") continue;
    let n = t instanceof Headers ? t.entries() : Object.entries(t);
    for (let [o, l] of n) if (l === null) r.delete(o);
    else if (Array.isArray(l)) for (let s of l) r.append(o, s);
    else l !== void 0 && r.set(o, typeof l == "object" ? JSON.stringify(l) : l);
  }
  return r;
}, g = class {
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
}, M = () => ({ error: new g(), request: new g(), response: new g() }), P = C({ allowReserved: !1, array: { explode: !0, style: "form" }, object: { explode: !0, style: "deepObject" } }), B = { "Content-Type": "application/json" }, U = (e = {}) => ({ ...A, headers: B, parseAs: "auto", querySerializer: P, ...e }), J = (e = {}) => {
  let r = j(U(), e), t = () => ({ ...r }), n = (s) => (r = j(r, s), t()), o = M(), l = async (s) => {
    let a = { ...r, ...s, fetch: s.fetch ?? r.fetch ?? globalThis.fetch, headers: O(r.headers, s.headers) };
    a.security && await V({ ...a, security: a.security }), a.body && a.bodySerializer && (a.body = a.bodySerializer(a.body)), (a.body === void 0 || a.body === "") && a.headers.delete("Content-Type");
    let i = x(a), f = { redirect: "follow", ...a }, p = new Request(i, f);
    for (let c of o.request._fns) c && (p = await c(p, a));
    let m = a.fetch, u = await m(p);
    for (let c of o.response._fns) c && (u = await c(u, p, a));
    let y = { request: p, response: u };
    if (u.ok) {
      if (u.status === 204 || u.headers.get("Content-Length") === "0") return a.responseStyle === "data" ? {} : { data: {}, ...y };
      let c = (a.parseAs === "auto" ? N(u.headers.get("Content-Type")) : a.parseAs) ?? "json";
      if (c === "stream") return a.responseStyle === "data" ? u.body : { data: u.body, ...y };
      let h = await u[c]();
      return c === "json" && (a.responseValidator && await a.responseValidator(h), a.responseTransformer && (h = await a.responseTransformer(h))), a.responseStyle === "data" ? h : { data: h, ...y };
    }
    let b = await u.text();
    try {
      b = JSON.parse(b);
    } catch {
    }
    let d = b;
    for (let c of o.error._fns) c && (d = await c(b, u, p, a));
    if (d = d || {}, a.throwOnError) throw d;
    return a.responseStyle === "data" ? void 0 : { error: d, ...y };
  };
  return { buildUrl: x, connect: (s) => l({ ...s, method: "CONNECT" }), delete: (s) => l({ ...s, method: "DELETE" }), get: (s) => l({ ...s, method: "GET" }), getConfig: t, head: (s) => l({ ...s, method: "HEAD" }), interceptors: o, options: (s) => l({ ...s, method: "OPTIONS" }), patch: (s) => l({ ...s, method: "PATCH" }), post: (s) => l({ ...s, method: "POST" }), put: (s) => l({ ...s, method: "PUT" }), request: l, setConfig: n, trace: (s) => l({ ...s, method: "TRACE" }) };
};
const L = J(U({
  baseUrl: "http://localhost:28157",
  throwOnError: !0
})), G = {
  type: "globalContext",
  alias: "semrush.context",
  name: "Semrush Context",
  js: () => import("./semrush.context-DpF4KTL0.js")
}, F = G, Q = [
  {
    type: "workspaceView",
    alias: "Umb.WorkspaceView.Semrush.View",
    name: "Umbraco Integration Workspace for Semrush",
    element: () => import("./semrush-workspace.element-BfbQQ-l-.js"),
    weight: 30,
    meta: {
      label: "Semrush",
      pathname: "semrush",
      icon: "icon-files"
    },
    conditions: [
      {
        alias: "Umb.Condition.WorkspaceAlias",
        match: "Umb.Workspace.Document"
      }
    ]
  }
], X = [...Q], K = {
  type: "modal",
  alias: "Semrush.Modal",
  name: "Semrush Modal",
  js: () => import("./semrush-modal.element-CaF7oUdI.js")
}, te = (e, r) => {
  r.registerMany([
    F,
    K,
    ...X
  ]), e.consumeContext(R, async (t) => {
    t && L.setConfig(_.getConfig());
  });
};
export {
  L as c,
  te as o
};
//# sourceMappingURL=index-m2GyFaVf.js.map
