var E = (r) => {
  throw TypeError(r);
};
var S = (r, e, t) => e.has(r) || E("Cannot " + t);
var o = (r, e, t) => (S(r, e, "read from private field"), t ? t.call(r) : e.get(r)), x = (r, e, t) => e.has(r) ? E("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), b = (r, e, t, n) => (S(r, e, "write to private field"), n ? n.call(r, t) : e.set(r, t), t);
import { UmbControllerBase as j } from "@umbraco-cms/backoffice/class-api";
import { tryExecuteAndNotify as f } from "@umbraco-cms/backoffice/resources";
import { O as p } from "./index-BN0CvJk8.js";
import { UmbContextToken as O } from "@umbraco-cms/backoffice/context-api";
class _ extends Error {
  constructor(e, t, n) {
    super(n), this.name = "ApiError", this.url = t.url, this.status = t.status, this.statusText = t.statusText, this.body = t.body, this.request = e;
  }
}
class N extends Error {
  constructor(e) {
    super(e), this.name = "CancelError";
  }
  get isCancelled() {
    return !0;
  }
}
class D {
  constructor(e) {
    this._isResolved = !1, this._isRejected = !1, this._isCancelled = !1, this.cancelHandlers = [], this.promise = new Promise((t, n) => {
      this._resolve = t, this._reject = n;
      const s = (c) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isResolved = !0, this._resolve && this._resolve(c));
      }, a = (c) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isRejected = !0, this._reject && this._reject(c));
      }, i = (c) => {
        this._isResolved || this._isRejected || this._isCancelled || this.cancelHandlers.push(c);
      };
      return Object.defineProperty(i, "isResolved", {
        get: () => this._isResolved
      }), Object.defineProperty(i, "isRejected", {
        get: () => this._isRejected
      }), Object.defineProperty(i, "isCancelled", {
        get: () => this._isCancelled
      }), e(s, a, i);
    });
  }
  get [Symbol.toStringTag]() {
    return "Cancellable Promise";
  }
  then(e, t) {
    return this.promise.then(e, t);
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
      this.cancelHandlers.length = 0, this._reject && this._reject(new N("Request aborted"));
    }
  }
  get isCancelled() {
    return this._isCancelled;
  }
}
const I = (r) => typeof r == "string", C = (r) => I(r) && r !== "", R = (r) => r instanceof Blob, A = (r) => r instanceof FormData, P = (r) => {
  try {
    return btoa(r);
  } catch {
    return Buffer.from(r).toString("base64");
  }
}, H = (r) => {
  const e = [], t = (s, a) => {
    e.push(`${encodeURIComponent(s)}=${encodeURIComponent(String(a))}`);
  }, n = (s, a) => {
    a != null && (a instanceof Date ? t(s, a.toISOString()) : Array.isArray(a) ? a.forEach((i) => n(s, i)) : typeof a == "object" ? Object.entries(a).forEach(([i, c]) => n(`${s}[${i}]`, c)) : t(s, a));
  };
  return Object.entries(r).forEach(([s, a]) => n(s, a)), e.length ? `?${e.join("&")}` : "";
}, U = (r, e) => {
  const t = encodeURI, n = e.url.replace("{api-version}", r.VERSION).replace(/{(.*?)}/g, (a, i) => {
    var c;
    return (c = e.path) != null && c.hasOwnProperty(i) ? t(String(e.path[i])) : a;
  }), s = r.BASE + n;
  return e.query ? s + H(e.query) : s;
}, L = (r) => {
  if (r.formData) {
    const e = new FormData(), t = (n, s) => {
      I(s) || R(s) ? e.append(n, s) : e.append(n, JSON.stringify(s));
    };
    return Object.entries(r.formData).filter(([, n]) => n != null).forEach(([n, s]) => {
      Array.isArray(s) ? s.forEach((a) => t(n, a)) : t(n, s);
    }), e;
  }
}, T = async (r, e) => typeof e == "function" ? e(r) : e, $ = async (r, e) => {
  const [t, n, s, a] = await Promise.all([
    T(e, r.TOKEN),
    T(e, r.USERNAME),
    T(e, r.PASSWORD),
    T(e, r.HEADERS)
  ]), i = Object.entries({
    Accept: "application/json",
    ...a,
    ...e.headers
  }).filter(([, c]) => c != null).reduce((c, [y, h]) => ({
    ...c,
    [y]: String(h)
  }), {});
  if (C(t) && (i.Authorization = `Bearer ${t}`), C(n) && C(s)) {
    const c = P(`${n}:${s}`);
    i.Authorization = `Basic ${c}`;
  }
  return e.body !== void 0 && (e.mediaType ? i["Content-Type"] = e.mediaType : R(e.body) ? i["Content-Type"] = e.body.type || "application/octet-stream" : I(e.body) ? i["Content-Type"] = "text/plain" : A(e.body) || (i["Content-Type"] = "application/json")), new Headers(i);
}, F = (r) => {
  var e, t;
  if (r.body !== void 0)
    return (e = r.mediaType) != null && e.includes("application/json") || (t = r.mediaType) != null && t.includes("+json") ? JSON.stringify(r.body) : I(r.body) || R(r.body) || A(r.body) ? r.body : JSON.stringify(r.body);
}, G = async (r, e, t, n, s, a, i) => {
  const c = new AbortController();
  let y = {
    headers: a,
    body: n ?? s,
    method: e.method,
    signal: c.signal
  };
  r.WITH_CREDENTIALS && (y.credentials = r.CREDENTIALS);
  for (const h of r.interceptors.request._fns)
    y = await h(y);
  return i(() => c.abort()), await fetch(t, y);
}, W = (r, e) => {
  if (e) {
    const t = r.headers.get(e);
    if (I(t))
      return t;
  }
}, M = async (r) => {
  if (r.status !== 204)
    try {
      const e = r.headers.get("Content-Type");
      if (e) {
        const t = ["application/octet-stream", "application/pdf", "application/zip", "audio/", "image/", "video/"];
        if (e.includes("application/json") || e.includes("+json"))
          return await r.json();
        if (t.some((n) => e.includes(n)))
          return await r.blob();
        if (e.includes("multipart/form-data"))
          return await r.formData();
        if (e.includes("text/"))
          return await r.text();
      }
    } catch (e) {
      console.error(e);
    }
}, k = (r, e) => {
  const n = {
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
    ...r.errors
  }[e.status];
  if (n)
    throw new _(r, e, n);
  if (!e.ok) {
    const s = e.status ?? "unknown", a = e.statusText ?? "unknown", i = (() => {
      try {
        return JSON.stringify(e.body, null, 2);
      } catch {
        return;
      }
    })();
    throw new _(
      r,
      e,
      `Generic Error: status: ${s}; status text: ${a}; body: ${i}`
    );
  }
}, m = (r, e) => new D(async (t, n, s) => {
  try {
    const a = U(r, e), i = L(e), c = F(e), y = await $(r, e);
    if (!s.isCancelled) {
      let h = await G(r, e, a, c, i, y, s);
      for (const B of r.interceptors.response._fns)
        h = await B(h);
      const q = await M(h), v = W(h, e.responseHeader), w = {
        url: a,
        ok: h.ok,
        status: h.status,
        statusText: h.statusText,
        body: v ?? q
      };
      k(e, w), t(w.body);
    }
  } catch (a) {
    n(a);
  }
});
class g {
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getContentTypes() {
    return m(p, {
      method: "GET",
      url: "/umbraco/algolia-search/management/api/v1/search/content-type"
    });
  }
  /**
   * @param data The data for the request.
   * @param data.id
   * @returns unknown OK
   * @throws ApiError
   */
  static getContentTypesByIndexId(e) {
    return m(p, {
      method: "GET",
      url: "/umbraco/algolia-search/management/api/v1/search/content-type/index/{id}",
      path: {
        id: e.id
      }
    });
  }
  /**
   * @returns unknown OK
   * @throws ApiError
   */
  static getIndices() {
    return m(p, {
      method: "GET",
      url: "/umbraco/algolia-search/management/api/v1/search/index"
    });
  }
  /**
   * @param data The data for the request.
   * @param data.requestBody
   * @returns unknown OK
   * @throws ApiError
   */
  static saveIndex(e = {}) {
    return m(p, {
      method: "POST",
      url: "/umbraco/algolia-search/management/api/v1/search/index",
      body: e.requestBody,
      mediaType: "application/json"
    });
  }
  /**
   * @param data The data for the request.
   * @param data.id
   * @returns unknown OK
   * @throws ApiError
   */
  static deleteIndex(e) {
    return m(p, {
      method: "DELETE",
      url: "/umbraco/algolia-search/management/api/v1/search/index/{id}",
      path: {
        id: e.id
      }
    });
  }
  /**
   * @param data The data for the request.
   * @param data.id
   * @returns unknown OK
   * @throws ApiError
   */
  static getIndexById(e) {
    return m(p, {
      method: "GET",
      url: "/umbraco/algolia-search/management/api/v1/search/index/{id}",
      path: {
        id: e.id
      }
    });
  }
  /**
   * @param data The data for the request.
   * @param data.indexId
   * @param data.query
   * @returns unknown OK
   * @throws ApiError
   */
  static search(e) {
    return m(p, {
      method: "GET",
      url: "/umbraco/algolia-search/management/api/v1/search/index/{indexId}/search",
      path: {
        indexId: e.indexId
      },
      query: {
        query: e.query
      }
    });
  }
  /**
   * @param data The data for the request.
   * @param data.requestBody
   * @returns unknown OK
   * @throws ApiError
   */
  static buildIndex(e = {}) {
    return m(p, {
      method: "POST",
      url: "/umbraco/algolia-search/management/api/v1/search/index/build",
      body: e.requestBody,
      mediaType: "application/json"
    });
  }
}
var d;
class z {
  constructor(e) {
    x(this, d);
    b(this, d, e);
  }
  async getIndices() {
    const { data: e, error: t } = await f(o(this, d), g.getIndices());
    return e || { error: t };
  }
  async getIndexById(e) {
    const { data: t, error: n } = await f(o(this, d), g.getIndexById({
      id: e
    }));
    return t || { error: n };
  }
  async getContentTypes() {
    const { data: e, error: t } = await f(o(this, d), g.getContentTypes());
    return e || { error: t };
  }
  async getContentTypesWithIndex(e) {
    const { data: t, error: n } = await f(o(this, d), g.getContentTypesByIndexId({
      id: e
    }));
    return t || { error: n };
  }
  async saveIndex(e) {
    const { data: t, error: n } = await f(o(this, d), g.saveIndex({
      requestBody: e
    }));
    return t || { error: n };
  }
  async buildIndex(e) {
    const { data: t, error: n } = await f(o(this, d), g.buildIndex({
      requestBody: e
    }));
    return t || { error: n };
  }
  async deleteIndex(e) {
    const { data: t, error: n } = await f(o(this, d), g.deleteIndex({
      id: e
    }));
    return t || { error: n };
  }
  async searchIndex(e, t) {
    const { data: n, error: s } = await f(o(this, d), g.search({
      indexId: e,
      query: t
    }));
    return n || { error: s };
  }
}
d = new WeakMap();
var l;
class J extends j {
  constructor(t) {
    super(t);
    x(this, l);
    b(this, l, new z(this));
  }
  async getIndices() {
    return o(this, l).getIndices();
  }
  async getIndexById(t) {
    return o(this, l).getIndexById(t);
  }
  async getContentTypes() {
    return o(this, l).getContentTypes();
  }
  async getContentTypesWithIndex(t) {
    return o(this, l).getContentTypesWithIndex(t);
  }
  async saveIndex(t) {
    return o(this, l).saveIndex(t);
  }
  async buildIndex(t) {
    return o(this, l).buildIndex(t);
  }
  async deleteIndex(t) {
    return o(this, l).deleteIndex(t);
  }
  async searchIndex(t, n) {
    return o(this, l).searchIndex(t, n);
  }
}
l = new WeakMap();
var u;
class V extends j {
  constructor(t) {
    super(t);
    x(this, u);
    this.provideContext(K, this), b(this, u, new J(t));
  }
  async getIndices() {
    return await o(this, u).getIndices();
  }
  async getIndexById(t) {
    return o(this, u).getIndexById(t);
  }
  async getContentTypes() {
    return await o(this, u).getContentTypes();
  }
  async getContentTypesWithIndex(t) {
    return o(this, u).getContentTypesWithIndex(t);
  }
  async saveIndex(t) {
    return await o(this, u).saveIndex(t);
  }
  async buildIndex(t) {
    return o(this, u).buildIndex(t);
  }
  async deleteIndex(t) {
    return await o(this, u).deleteIndex(t);
  }
  async searchIndex(t, n) {
    return o(this, u).searchIndex(t, n);
  }
}
u = new WeakMap();
const K = new O(V.name);
export {
  K as ALGOLIA_CONTEXT_TOKEN,
  V as AlgoliaIndexContext,
  V as default
};
//# sourceMappingURL=algolia-index.context-BcCnOnU6.js.map
