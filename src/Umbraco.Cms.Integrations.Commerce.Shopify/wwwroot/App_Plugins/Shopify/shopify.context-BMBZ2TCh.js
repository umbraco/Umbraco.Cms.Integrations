var R = (t) => {
  throw TypeError(t);
};
var A = (t, e, r) => e.has(t) || R("Cannot " + r);
var u = (t, e, r) => (A(t, e, "read from private field"), r ? r.call(t) : e.get(t)), k = (t, e, r) => e.has(t) ? R("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), C = (t, e, r, s) => (A(t, e, "write to private field"), s ? s.call(t, r) : e.set(t, r), r);
import { UmbControllerBase as S } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as P } from "@umbraco-cms/backoffice/context-api";
import { tryExecuteAndNotify as h } from "@umbraco-cms/backoffice/resources";
import { O as y } from "./index-B9zm0pl6.js";
class E extends Error {
  constructor(e, r, s) {
    super(s), this.name = "ApiError", this.url = r.url, this.status = r.status, this.statusText = r.statusText, this.body = r.body, this.request = e;
  }
}
class O extends Error {
  constructor(e) {
    super(e), this.name = "CancelError";
  }
  get isCancelled() {
    return !0;
  }
}
class x {
  constructor(e) {
    this._isResolved = !1, this._isRejected = !1, this._isCancelled = !1, this.cancelHandlers = [], this.promise = new Promise((r, s) => {
      this._resolve = r, this._reject = s;
      const n = (o) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isResolved = !0, this._resolve && this._resolve(o));
      }, a = (o) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isRejected = !0, this._reject && this._reject(o));
      }, i = (o) => {
        this._isResolved || this._isRejected || this._isCancelled || this.cancelHandlers.push(o);
      };
      return Object.defineProperty(i, "isResolved", {
        get: () => this._isResolved
      }), Object.defineProperty(i, "isRejected", {
        get: () => this._isRejected
      }), Object.defineProperty(i, "isCancelled", {
        get: () => this._isCancelled
      }), e(n, a, i);
    });
  }
  get [Symbol.toStringTag]() {
    return "Cancellable Promise";
  }
  then(e, r) {
    return this.promise.then(e, r);
  }
  catch(e) {
    return this.promise.catch(e);
  }
  finally(e) {
    return this.promise.finally(e);
  }
  cancel() {
    if (!(this._isResolved || this._isRejected || this._isCancelled)) {
      if (this._isCancelled = !0, this.cancelHandlers.length)
        try {
          for (const e of this.cancelHandlers)
            e();
        } catch (e) {
          console.warn("Cancellation threw an error", e);
          return;
        }
      this.cancelHandlers.length = 0, this._reject && this._reject(new O("Request aborted"));
    }
  }
  get isCancelled() {
    return this._isCancelled;
  }
}
const m = (t) => typeof t == "string", T = (t) => m(t) && t !== "", b = (t) => t instanceof Blob, _ = (t) => t instanceof FormData, I = (t) => {
  try {
    return btoa(t);
  } catch {
    return Buffer.from(t).toString("base64");
  }
}, N = (t) => {
  const e = [], r = (n, a) => {
    e.push(`${encodeURIComponent(n)}=${encodeURIComponent(String(a))}`);
  }, s = (n, a) => {
    a != null && (a instanceof Date ? r(n, a.toISOString()) : Array.isArray(a) ? a.forEach((i) => s(n, i)) : typeof a == "object" ? Object.entries(a).forEach(([i, o]) => s(`${n}[${i}]`, o)) : r(n, a));
  };
  return Object.entries(t).forEach(([n, a]) => s(n, a)), e.length ? `?${e.join("&")}` : "";
}, B = (t, e) => {
  const r = encodeURI, s = e.url.replace("{api-version}", t.VERSION).replace(/{(.*?)}/g, (a, i) => {
    var o;
    return (o = e.path) != null && o.hasOwnProperty(i) ? r(String(e.path[i])) : a;
  }), n = t.BASE + s;
  return e.query ? n + N(e.query) : n;
}, U = (t) => {
  if (t.formData) {
    const e = new FormData(), r = (s, n) => {
      m(n) || b(n) ? e.append(s, n) : e.append(s, JSON.stringify(n));
    };
    return Object.entries(t.formData).filter(([, s]) => s != null).forEach(([s, n]) => {
      Array.isArray(n) ? n.forEach((a) => r(s, a)) : r(s, n);
    }), e;
  }
}, g = async (t, e) => typeof e == "function" ? e(t) : e, L = async (t, e) => {
  const [r, s, n, a] = await Promise.all([
    g(e, t.TOKEN),
    g(e, t.USERNAME),
    g(e, t.PASSWORD),
    g(e, t.HEADERS)
  ]), i = Object.entries({
    Accept: "application/json",
    ...a,
    ...e.headers
  }).filter(([, o]) => o != null).reduce((o, [l, d]) => ({
    ...o,
    [l]: String(d)
  }), {});
  if (T(r) && (i.Authorization = `Bearer ${r}`), T(s) && T(n)) {
    const o = I(`${s}:${n}`);
    i.Authorization = `Basic ${o}`;
  }
  return e.body !== void 0 && (e.mediaType ? i["Content-Type"] = e.mediaType : b(e.body) ? i["Content-Type"] = e.body.type || "application/octet-stream" : m(e.body) ? i["Content-Type"] = "text/plain" : _(e.body) || (i["Content-Type"] = "application/json")), new Headers(i);
}, H = (t) => {
  var e, r;
  if (t.body !== void 0)
    return (e = t.mediaType) != null && e.includes("application/json") || (r = t.mediaType) != null && r.includes("+json") ? JSON.stringify(t.body) : m(t.body) || b(t.body) || _(t.body) ? t.body : JSON.stringify(t.body);
}, D = async (t, e, r, s, n, a, i) => {
  const o = new AbortController();
  let l = {
    headers: a,
    body: s ?? n,
    method: e.method,
    signal: o.signal
  };
  t.WITH_CREDENTIALS && (l.credentials = t.CREDENTIALS);
  for (const d of t.interceptors.request._fns)
    l = await d(l);
  return i(() => o.abort()), await fetch(r, l);
}, F = (t, e) => {
  if (e) {
    const r = t.headers.get(e);
    if (m(r))
      return r;
  }
}, $ = async (t) => {
  if (t.status !== 204)
    try {
      const e = t.headers.get("Content-Type");
      if (e) {
        const r = ["application/octet-stream", "application/pdf", "application/zip", "audio/", "image/", "video/"];
        if (e.includes("application/json") || e.includes("+json"))
          return await t.json();
        if (r.some((s) => e.includes(s)))
          return await t.blob();
        if (e.includes("multipart/form-data"))
          return await t.formData();
        if (e.includes("text/"))
          return await t.text();
      }
    } catch (e) {
      console.error(e);
    }
}, z = (t, e) => {
  const s = {
    400: "Bad Request",
    401: "Unauthorized",
    402: "Payment Required",
    403: "Forbidden",
    404: "Not Found",
    405: "Method Not Allowed",
    406: "Not Acceptable",
    407: "Proxy Authentication Required",
    408: "Request Timeout",
    409: "Conflict",
    410: "Gone",
    411: "Length Required",
    412: "Precondition Failed",
    413: "Payload Too Large",
    414: "URI Too Long",
    415: "Unsupported Media Type",
    416: "Range Not Satisfiable",
    417: "Expectation Failed",
    418: "Im a teapot",
    421: "Misdirected Request",
    422: "Unprocessable Content",
    423: "Locked",
    424: "Failed Dependency",
    425: "Too Early",
    426: "Upgrade Required",
    428: "Precondition Required",
    429: "Too Many Requests",
    431: "Request Header Fields Too Large",
    451: "Unavailable For Legal Reasons",
    500: "Internal Server Error",
    501: "Not Implemented",
    502: "Bad Gateway",
    503: "Service Unavailable",
    504: "Gateway Timeout",
    505: "HTTP Version Not Supported",
    506: "Variant Also Negotiates",
    507: "Insufficient Storage",
    508: "Loop Detected",
    510: "Not Extended",
    511: "Network Authentication Required",
    ...t.errors
  }[e.status];
  if (s)
    throw new E(t, e, s);
  if (!e.ok) {
    const n = e.status ?? "unknown", a = e.statusText ?? "unknown", i = (() => {
      try {
        return JSON.stringify(e.body, null, 2);
      } catch {
        return;
      }
    })();
    throw new E(
      t,
      e,
      `Generic Error: status: ${n}; status text: ${a}; body: ${i}`
    );
  }
}, f = (t, e) => new x(async (r, s, n) => {
  try {
    const a = B(t, e), i = U(e), o = H(e), l = await L(t, e);
    if (!n.isCancelled) {
      let d = await D(t, e, a, o, i, l, n);
      for (const j of t.interceptors.response._fns)
        d = await j(d);
      const q = await $(d), v = F(d, e.responseHeader), w = {
        url: a,
        ok: d.ok,
        status: d.status,
        statusText: d.statusText,
        body: v ?? q
      };
      z(e, w), r(w.body);
    }
  } catch (a) {
    s(a);
  }
});
class p {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static checkConfiguration() {
    return f(y, {
      method: "GET",
      url: "/umbraco/shopify/management/api/v1/check-configuration",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @param data The data for the request.
   * @param data.requestBody
   * @returns string OK
   * @throws ApiError
   */
  static getAccessToken(e = {}) {
    return f(y, {
      method: "POST",
      url: "/umbraco/shopify/management/api/v1/get-access-token",
      body: e.requestBody,
      mediaType: "application/json",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static getAuthorizationUrl() {
    return f(y, {
      method: "GET",
      url: "/umbraco/shopify/management/api/v1/get-authorization-url",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @param data The data for the request.
   * @param data.pageInfo
   * @returns unknown OK
   * @throws ApiError
   */
  static getList(e = {}) {
    return f(y, {
      method: "GET",
      url: "/umbraco/shopify/management/api/v1/get-list",
      query: {
        pageInfo: e.pageInfo
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @param data The data for the request.
   * @param data.requestBody
   * @returns unknown OK
   * @throws ApiError
   */
  static getListByIds(e = {}) {
    return f(y, {
      method: "GET",
      url: "/umbraco/shopify/management/api/v1/get-list-by-ids",
      body: e.requestBody,
      mediaType: "application/json",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns string OK
   * @throws ApiError
   */
  static revokeAccessToken() {
    return f(y, {
      method: "POST",
      url: "/umbraco/shopify/management/api/v1/revoke-access-token",
      responseHeader: "Umb-Notifications",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns number OK
   * @throws ApiError
   */
  static getTotalPages() {
    return f(y, {
      method: "GET",
      url: "/umbraco/shopify/management/api/v1/total-pages",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static validateAccessToken() {
    return f(y, {
      method: "GET",
      url: "/umbraco/shopify/management/api/v1/validate-access-token",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class G extends S {
  constructor(e) {
    super(e);
  }
  async checkConfiguration() {
    const { data: e, error: r } = await h(this, p.checkConfiguration());
    return r || !e ? { error: r } : { data: e };
  }
  async getAccessToken(e) {
    const { data: r, error: s } = await h(this, p.getAccessToken({ requestBody: e }));
    return s || !r ? { error: s } : { data: r };
  }
  async validateAccessToken() {
    const { data: e, error: r } = await h(this, p.validateAccessToken());
    return r || !e ? { error: r } : { data: e };
  }
  async revokeAccessToken() {
    const { data: e, error: r } = await h(this, p.revokeAccessToken());
    return r || !e ? { error: r } : { data: e };
  }
  async getList() {
    const { data: e, error: r } = await h(this, p.getList());
    return r || !e ? { error: r } : { data: e };
  }
  async getListByIds() {
    const { data: e, error: r } = await h(this, p.getListByIds());
    return r || !e ? { error: r } : { data: e };
  }
  async getTotalPages() {
    const { data: e, error: r } = await h(this, p.getTotalPages());
    return r || !e ? { error: r } : { data: e };
  }
  async getAuthorizationUrl() {
    const { data: e, error: r } = await h(this, p.getAuthorizationUrl());
    return r || !e ? { error: r } : { data: e };
  }
}
var c;
class M extends S {
  constructor(r) {
    super(r);
    k(this, c);
    this.provideContext(J, this), C(this, c, new G(r));
  }
  async checkConfiguration() {
    return await u(this, c).checkConfiguration();
  }
  async getAccessToken(r) {
    return await u(this, c).getAccessToken(r);
  }
  async validateAccessToken() {
    return await u(this, c).validateAccessToken();
  }
  async revokeAccessToken() {
    return await u(this, c).revokeAccessToken();
  }
  async getList() {
    return await u(this, c).getList();
  }
  async getListByIds() {
    return await u(this, c).getListByIds();
  }
  async getTotalPages() {
    return await u(this, c).getTotalPages();
  }
  async getAuthorizationUrl() {
    return await u(this, c).getAuthorizationUrl();
  }
}
c = new WeakMap();
const J = new P(M.name);
export {
  J as SHOPIFY_CONTEXT_TOKEN,
  M as ShopifyContext,
  M as default
};
//# sourceMappingURL=shopify.context-BMBZ2TCh.js.map
