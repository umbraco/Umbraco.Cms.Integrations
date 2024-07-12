var k = (t) => {
  throw TypeError(t);
};
var A = (t, e, r) => e.has(t) || k("Cannot " + r);
var u = (t, e, r) => (A(t, e, "read from private field"), r ? r.call(t) : e.get(t)), R = (t, e, r) => e.has(t) ? k("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), C = (t, e, r, s) => (A(t, e, "write to private field"), s ? s.call(t, r) : e.set(t, r), r);
import { UmbControllerBase as E } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as P } from "@umbraco-cms/backoffice/context-api";
import { tryExecuteAndNotify as l } from "@umbraco-cms/backoffice/resources";
import { O as h } from "./index-DM1PFbxq.js";
class S extends Error {
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
      const n = (i) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isResolved = !0, this._resolve && this._resolve(i));
      }, a = (i) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isRejected = !0, this._reject && this._reject(i));
      }, o = (i) => {
        this._isResolved || this._isRejected || this._isCancelled || this.cancelHandlers.push(i);
      };
      return Object.defineProperty(o, "isResolved", {
        get: () => this._isResolved
      }), Object.defineProperty(o, "isRejected", {
        get: () => this._isRejected
      }), Object.defineProperty(o, "isCancelled", {
        get: () => this._isCancelled
      }), e(n, a, o);
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
    a != null && (a instanceof Date ? r(n, a.toISOString()) : Array.isArray(a) ? a.forEach((o) => s(n, o)) : typeof a == "object" ? Object.entries(a).forEach(([o, i]) => s(`${n}[${o}]`, i)) : r(n, a));
  };
  return Object.entries(t).forEach(([n, a]) => s(n, a)), e.length ? `?${e.join("&")}` : "";
}, B = (t, e) => {
  const r = encodeURI, s = e.url.replace("{api-version}", t.VERSION).replace(/{(.*?)}/g, (a, o) => {
    var i;
    return (i = e.path) != null && i.hasOwnProperty(o) ? r(String(e.path[o])) : a;
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
  ]), o = Object.entries({
    Accept: "application/json",
    ...a,
    ...e.headers
  }).filter(([, i]) => i != null).reduce((i, [p, d]) => ({
    ...i,
    [p]: String(d)
  }), {});
  if (T(r) && (o.Authorization = `Bearer ${r}`), T(s) && T(n)) {
    const i = I(`${s}:${n}`);
    o.Authorization = `Basic ${i}`;
  }
  return e.body !== void 0 && (e.mediaType ? o["Content-Type"] = e.mediaType : b(e.body) ? o["Content-Type"] = e.body.type || "application/octet-stream" : m(e.body) ? o["Content-Type"] = "text/plain" : _(e.body) || (o["Content-Type"] = "application/json")), new Headers(o);
}, H = (t) => {
  var e, r;
  if (t.body !== void 0)
    return (e = t.mediaType) != null && e.includes("application/json") || (r = t.mediaType) != null && r.includes("+json") ? JSON.stringify(t.body) : m(t.body) || b(t.body) || _(t.body) ? t.body : JSON.stringify(t.body);
}, D = async (t, e, r, s, n, a, o) => {
  const i = new AbortController();
  let p = {
    headers: a,
    body: s ?? n,
    method: e.method,
    signal: i.signal
  };
  t.WITH_CREDENTIALS && (p.credentials = t.CREDENTIALS);
  for (const d of t.interceptors.request._fns)
    p = await d(p);
  return o(() => i.abort()), await fetch(r, p);
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
    throw new S(t, e, s);
  if (!e.ok) {
    const n = e.status ?? "unknown", a = e.statusText ?? "unknown", o = (() => {
      try {
        return JSON.stringify(e.body, null, 2);
      } catch {
        return;
      }
    })();
    throw new S(
      t,
      e,
      `Generic Error: status: ${n}; status text: ${a}; body: ${o}`
    );
  }
}, f = (t, e) => new x(async (r, s, n) => {
  try {
    const a = B(t, e), o = U(e), i = H(e), p = await L(t, e);
    if (!n.isCancelled) {
      let d = await D(t, e, a, i, o, p, n);
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
class y {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static checkConfiguration() {
    return f(h, {
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
    return f(h, {
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
    return f(h, {
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
    return f(h, {
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
    return f(h, {
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
  static refreshAccessToken() {
    return f(h, {
      method: "POST",
      url: "/umbraco/shopify/management/api/v1/refresh",
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
    return f(h, {
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
    return f(h, {
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
    return f(h, {
      method: "GET",
      url: "/umbraco/shopify/management/api/v1/validate-access-token",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
class G extends E {
  constructor(e) {
    super(e);
  }
  async checkConfiguration() {
    const { data: e, error: r } = await l(this, y.checkConfiguration());
    return r || !e ? { error: r } : { data: e };
  }
  async getAccessToken(e) {
    const { data: r, error: s } = await l(this, y.getAccessToken({ requestBody: e }));
    return s || !r ? { error: s } : { data: r };
  }
  async validateAccessToken() {
    const { data: e, error: r } = await l(this, y.validateAccessToken());
    return r || !e ? { error: r } : { data: e };
  }
  async revokeAccessToken() {
    const { data: e, error: r } = await l(this, y.revokeAccessToken());
    return r || !e ? { error: r } : { data: e };
  }
  async getList() {
    const { data: e, error: r } = await l(this, y.getList());
    return r || !e ? { error: r } : { data: e };
  }
  async getListByIds() {
    const { data: e, error: r } = await l(this, y.getListByIds());
    return r || !e ? { error: r } : { data: e };
  }
  async getTotalPages() {
    const { data: e, error: r } = await l(this, y.getTotalPages());
    return r || !e ? { error: r } : { data: e };
  }
  async getAuthorizationUrl() {
    const { data: e, error: r } = await l(this, y.getAuthorizationUrl());
    return r || !e ? { error: r } : { data: e };
  }
  async refreshAccessToken() {
    const { data: e, error: r } = await l(this, y.refreshAccessToken());
    return r || !e ? { error: r } : { data: e };
  }
}
var c;
class M extends E {
  constructor(r) {
    super(r);
    R(this, c);
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
  async refreshAccessToken() {
    return await u(this, c).refreshAccessToken();
  }
}
c = new WeakMap();
const J = new P(M.name);
export {
  J as SHOPIFY_CONTEXT_TOKEN,
  M as ShopifyContext,
  M as default
};
//# sourceMappingURL=shopify.context-DlBtK13t.js.map
