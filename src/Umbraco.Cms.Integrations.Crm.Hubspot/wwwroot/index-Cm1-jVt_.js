var se = (r) => {
  throw TypeError(r);
};
var ae = (r, e, t) => e.has(r) || se("Cannot " + t);
var S = (r, e, t) => (ae(r, e, "read from private field"), t ? t.call(r) : e.get(r)), D = (r, e, t) => e.has(r) ? se("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), V = (r, e, t, s) => (ae(r, e, "write to private field"), s ? s.call(r, t) : e.set(r, t), t);
import { UMB_AUTH_CONTEXT as Ee } from "@umbraco-cms/backoffice/auth";
import { UmbElementMixin as Oe } from "@umbraco-cms/backoffice/element-api";
import { LitElement as ze, when as oe, html as W, css as xe, state as G, property as Pe, query as Ue, customElement as je } from "@umbraco-cms/backoffice/external/lit";
import { UMB_NOTIFICATION_CONTEXT as Ie } from "@umbraco-cms/backoffice/notification";
import { UmbControllerBase as le } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as He } from "@umbraco-cms/backoffice/context-api";
import { tryExecute as z } from "@umbraco-cms/backoffice/resources";
import { UmbObjectState as $e } from "@umbraco-cms/backoffice/observable-api";
const qe = {
  type: "propertyEditorUi",
  alias: "HubSpot.PropertyEditorUi.FormPicker",
  name: "HubSpot Form Picker Property Editor UI",
  js: () => import("./form-picker-property-editor.element-BlBI3ZKy.js"),
  elementName: "hubspot-form-picker",
  meta: {
    label: "HubSpot Form Picker",
    icon: "icon-handshake",
    group: "pickers",
    propertyEditorSchemaAlias: "HubSpot.FormPicker"
  }
}, Be = {
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
}, Fe = {
  type: "propertyEditorUi",
  alias: "HubSpot.PropertyEditorUi.Authorization",
  name: "HubSpot Authorization Property Editor UI",
  js: () => Promise.resolve().then(() => ft),
  elementName: "hubspot-authorization",
  meta: {
    label: "Authorization",
    icon: "",
    group: ""
  }
}, Re = [
  qe,
  Be,
  Fe
], Ne = {
  type: "globalContext",
  alias: "hubspot-forms.context",
  name: "Hubspot Forms Context",
  js: () => Promise.resolve().then(() => ct)
}, Me = Ne, De = {
  type: "modal",
  alias: "HubspotForms.Modal",
  name: "Hubspot Forms Modal",
  js: () => import("./hubspot-forms-modal.element-CQ-RP_4i.js")
}, Ve = {
  bodySerializer: (r) => JSON.stringify(r, (e, t) => typeof t == "bigint" ? t.toString() : t)
};
function We({
  onRequest: r,
  onSseError: e,
  onSseEvent: t,
  responseTransformer: s,
  responseValidator: a,
  sseDefaultRetryDelay: c,
  sseMaxRetryAttempts: o,
  sseMaxRetryDelay: i,
  sseSleepFn: n,
  url: b,
  ...l
}) {
  let u;
  const A = n ?? ((h) => new Promise((f) => setTimeout(f, h)));
  return { stream: async function* () {
    let h = c ?? 3e3, f = 0;
    const g = l.signal ?? new AbortController().signal;
    for (; !g.aborted; ) {
      f++;
      const F = l.headers instanceof Headers ? l.headers : new Headers(l.headers);
      u !== void 0 && F.set("Last-Event-ID", u);
      try {
        const O = {
          redirect: "follow",
          ...l,
          body: l.serializedBody,
          headers: F,
          signal: g
        };
        let j = new Request(b, O);
        r && (j = await r(b, O));
        const p = await (l.fetch ?? globalThis.fetch)(j);
        if (!p.ok) throw new Error(`SSE failed: ${p.status} ${p.statusText}`);
        if (!p.body) throw new Error("No body in SSE response");
        const k = p.body.pipeThrough(new TextDecoderStream()).getReader();
        let y = "";
        const Y = () => {
          try {
            k.cancel();
          } catch {
          }
        };
        g.addEventListener("abort", Y);
        try {
          for (; ; ) {
            const { done: Se, value: Ce } = await k.read();
            if (Se) break;
            y += Ce, y = y.replace(/\r\n?/g, `
`);
            const Z = y.split(`

`);
            y = Z.pop() ?? "";
            for (const Te of Z) {
              const _e = Te.split(`
`), R = [];
              let ee;
              for (const v of _e)
                if (v.startsWith("data:"))
                  R.push(v.replace(/^data:\s*/, ""));
                else if (v.startsWith("event:"))
                  ee = v.replace(/^event:\s*/, "");
                else if (v.startsWith("id:"))
                  u = v.replace(/^id:\s*/, "");
                else if (v.startsWith("retry:")) {
                  const re = Number.parseInt(v.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(re) || (h = re);
                }
              let I, te = !1;
              if (R.length) {
                const v = R.join(`
`);
                try {
                  I = JSON.parse(v), te = !0;
                } catch {
                  I = v;
                }
              }
              te && (a && await a(I), s && (I = await s(I))), t == null || t({
                data: I,
                event: ee,
                id: u,
                retry: h
              }), R.length && (yield I);
            }
          }
        } finally {
          g.removeEventListener("abort", Y), k.releaseLock();
        }
        break;
      } catch (O) {
        if (e == null || e(O), o !== void 0 && f >= o)
          break;
        const j = Math.min(h * 2 ** (f - 1), i ?? 3e4);
        await A(j);
      }
    }
  }() };
}
const Le = (r) => {
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
}, Je = (r) => {
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
}, Ke = (r) => {
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
}, he = ({
  allowReserved: r,
  explode: e,
  name: t,
  style: s,
  value: a
}) => {
  if (!e) {
    const i = (r ? a : a.map((n) => encodeURIComponent(n))).join(Je(s));
    switch (s) {
      case "label":
        return `.${i}`;
      case "matrix":
        return `;${t}=${i}`;
      case "simple":
        return i;
      default:
        return `${t}=${i}`;
    }
  }
  const c = Le(s), o = a.map((i) => s === "label" || s === "simple" ? r ? i : encodeURIComponent(i) : N({
    allowReserved: r,
    name: t,
    value: i
  })).join(c);
  return s === "label" || s === "matrix" ? c + o : o;
}, N = ({
  allowReserved: r,
  name: e,
  value: t
}) => {
  if (t == null)
    return "";
  if (typeof t == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${e}=${r ? t : encodeURIComponent(t)}`;
}, de = ({
  allowReserved: r,
  explode: e,
  name: t,
  style: s,
  value: a,
  valueOnly: c
}) => {
  if (a instanceof Date)
    return c ? a.toISOString() : `${t}=${a.toISOString()}`;
  if (s !== "deepObject" && !e) {
    let n = [];
    Object.entries(a).forEach(([l, u]) => {
      n = [...n, l, r ? u : encodeURIComponent(u)];
    });
    const b = n.join(",");
    switch (s) {
      case "form":
        return `${t}=${b}`;
      case "label":
        return `.${b}`;
      case "matrix":
        return `;${t}=${b}`;
      default:
        return b;
    }
  }
  const o = Ke(s), i = Object.entries(a).map(
    ([n, b]) => N({
      allowReserved: r,
      name: s === "deepObject" ? `${t}[${n}]` : n,
      value: b
    })
  ).join(o);
  return s === "label" || s === "matrix" ? o + i : i;
}, Ge = /\{[^{}]+\}/g, Xe = ({ path: r, url: e }) => {
  let t = e;
  const s = e.match(Ge);
  if (s)
    for (const a of s) {
      let c = !1, o = a.substring(1, a.length - 1), i = "simple";
      o.endsWith("*") && (c = !0, o = o.substring(0, o.length - 1)), o.startsWith(".") ? (o = o.substring(1), i = "label") : o.startsWith(";") && (o = o.substring(1), i = "matrix");
      const n = r[o];
      if (n == null)
        continue;
      if (Array.isArray(n)) {
        t = t.replace(a, he({ explode: c, name: o, style: i, value: n }));
        continue;
      }
      if (typeof n == "object") {
        t = t.replace(
          a,
          de({
            explode: c,
            name: o,
            style: i,
            value: n,
            valueOnly: !0
          })
        );
        continue;
      }
      if (i === "matrix") {
        t = t.replace(
          a,
          `;${N({
            name: o,
            value: n
          })}`
        );
        continue;
      }
      const b = encodeURIComponent(
        i === "label" ? `.${n}` : n
      );
      t = t.replace(a, b);
    }
  return t;
}, Qe = ({
  baseUrl: r,
  path: e,
  query: t,
  querySerializer: s,
  url: a
}) => {
  const c = a.startsWith("/") ? a : `/${a}`;
  let o = (r ?? "") + c;
  e && (o = Xe({ path: e, url: o }));
  let i = t ? s(t) : "";
  return i.startsWith("?") && (i = i.substring(1)), i && (o += `?${i}`), o;
};
function ie(r) {
  const e = r.body !== void 0;
  if (e && r.bodySerializer)
    return "serializedBody" in r ? r.serializedBody !== void 0 && r.serializedBody !== "" ? r.serializedBody : null : r.body !== "" ? r.body : null;
  if (e)
    return r.body;
}
const Ye = async (r, e) => {
  const t = typeof e == "function" ? await e(r) : e;
  if (t)
    return r.scheme === "bearer" ? `Bearer ${t}` : r.scheme === "basic" ? `Basic ${btoa(t)}` : t;
}, fe = ({
  parameters: r = {},
  ...e
} = {}) => (s) => {
  const a = [];
  if (s && typeof s == "object")
    for (const c in s) {
      const o = s[c];
      if (o == null)
        continue;
      const i = r[c] || e;
      if (Array.isArray(o)) {
        const n = he({
          allowReserved: i.allowReserved,
          explode: !0,
          name: c,
          style: "form",
          value: o,
          ...i.array
        });
        n && a.push(n);
      } else if (typeof o == "object") {
        const n = de({
          allowReserved: i.allowReserved,
          explode: !0,
          name: c,
          style: "deepObject",
          value: o,
          ...i.object
        });
        n && a.push(n);
      } else {
        const n = N({
          allowReserved: i.allowReserved,
          name: c,
          value: o
        });
        n && a.push(n);
      }
    }
  return a.join("&");
}, Ze = (r) => {
  var t;
  if (!r)
    return "stream";
  const e = (t = r.split(";")[0]) == null ? void 0 : t.trim();
  if (e) {
    if (e.startsWith("application/json") || e.endsWith("+json"))
      return "json";
    if (e === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some((s) => e.startsWith(s)))
      return "blob";
    if (e.startsWith("text/"))
      return "text";
  }
}, et = (r, e) => {
  var t, s;
  return e ? !!(r.headers.has(e) || (t = r.query) != null && t[e] || (s = r.headers.get("Cookie")) != null && s.includes(`${e}=`)) : !1;
};
async function tt(r) {
  for (const e of r.security ?? []) {
    if (et(r, e.name))
      continue;
    const t = await Ye(e, r.auth);
    if (!t)
      continue;
    const s = e.name ?? "Authorization";
    switch (e.in) {
      case "query":
        r.query || (r.query = {}), r.query[s] = t;
        break;
      case "cookie":
        r.headers.append("Cookie", `${s}=${t}`);
        break;
      case "header":
      default:
        r.headers.set(s, t);
        break;
    }
  }
}
const ne = (r) => Qe({
  baseUrl: r.baseUrl,
  path: r.path,
  query: r.query,
  querySerializer: typeof r.querySerializer == "function" ? r.querySerializer : fe(r.querySerializer),
  url: r.url
}), ce = (r, e) => {
  var s;
  const t = { ...r, ...e };
  return (s = t.baseUrl) != null && s.endsWith("/") && (t.baseUrl = t.baseUrl.substring(0, t.baseUrl.length - 1)), t.headers = pe(r.headers, e.headers), t;
}, rt = (r) => {
  const e = [];
  return r.forEach((t, s) => {
    e.push([s, t]);
  }), e;
}, pe = (...r) => {
  const e = new Headers();
  for (const t of r) {
    if (!t)
      continue;
    const s = t instanceof Headers ? rt(t) : Object.entries(t);
    for (const [a, c] of s)
      if (c === null)
        e.delete(a);
      else if (Array.isArray(c))
        for (const o of c)
          e.append(a, o);
      else c !== void 0 && e.set(
        a,
        typeof c == "object" ? JSON.stringify(c) : c
      );
  }
  return e;
};
class L {
  constructor() {
    this.fns = [];
  }
  clear() {
    this.fns = [];
  }
  eject(e) {
    const t = this.getInterceptorIndex(e);
    this.fns[t] && (this.fns[t] = null);
  }
  exists(e) {
    const t = this.getInterceptorIndex(e);
    return !!this.fns[t];
  }
  getInterceptorIndex(e) {
    return typeof e == "number" ? this.fns[e] ? e : -1 : this.fns.indexOf(e);
  }
  update(e, t) {
    const s = this.getInterceptorIndex(e);
    return this.fns[s] ? (this.fns[s] = t, e) : !1;
  }
  use(e) {
    return this.fns.push(e), this.fns.length - 1;
  }
}
const st = () => ({
  error: new L(),
  request: new L(),
  response: new L()
}), at = fe({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), ot = {
  "Content-Type": "application/json"
}, ye = (r = {}) => ({
  ...Ve,
  headers: ot,
  parseAs: "auto",
  querySerializer: at,
  ...r
}), it = (r = {}) => {
  let e = ce(ye(), r);
  const t = () => ({ ...e }), s = (l) => (e = ce(e, l), t()), a = st(), c = async (l) => {
    const u = {
      ...e,
      ...l,
      fetch: l.fetch ?? e.fetch ?? globalThis.fetch,
      headers: pe(e.headers, l.headers),
      serializedBody: void 0
    };
    u.security && await tt(u), u.requestValidator && await u.requestValidator(u), u.body !== void 0 && u.bodySerializer && (u.serializedBody = u.bodySerializer(u.body)), (u.body === void 0 || u.serializedBody === "") && u.headers.delete("Content-Type");
    const A = u, m = ne(A);
    return { opts: A, url: m };
  }, o = async (l) => {
    const u = l.throwOnError ?? e.throwOnError, A = l.responseStyle ?? e.responseStyle;
    let m, d;
    try {
      const { opts: h, url: f } = await c(l), g = {
        redirect: "follow",
        ...h,
        body: ie(h)
      };
      m = new Request(f, g);
      for (const p of a.request.fns)
        p && (m = await p(m, h));
      const F = h.fetch;
      d = await F(m);
      for (const p of a.response.fns)
        p && (d = await p(d, m, h));
      const O = {
        request: m,
        response: d
      };
      if (d.ok) {
        const p = (h.parseAs === "auto" ? Ze(d.headers.get("Content-Type")) : h.parseAs) ?? "json";
        if (d.status === 204 || d.headers.get("Content-Length") === "0") {
          let y;
          switch (p) {
            case "arrayBuffer":
            case "blob":
            case "text":
              y = await d[p]();
              break;
            case "formData":
              y = new FormData();
              break;
            case "stream":
              y = d.body;
              break;
            case "json":
            default:
              y = {};
              break;
          }
          return h.responseStyle === "data" ? y : {
            data: y,
            ...O
          };
        }
        let k;
        switch (p) {
          case "arrayBuffer":
          case "blob":
          case "formData":
          case "text":
            k = await d[p]();
            break;
          case "json": {
            const y = await d.text();
            k = y ? JSON.parse(y) : {};
            break;
          }
          case "stream":
            return h.responseStyle === "data" ? d.body : {
              data: d.body,
              ...O
            };
        }
        return p === "json" && (h.responseValidator && await h.responseValidator(k), h.responseTransformer && (k = await h.responseTransformer(k))), h.responseStyle === "data" ? k : {
          data: k,
          ...O
        };
      }
      const j = await d.text();
      let M;
      try {
        M = JSON.parse(j);
      } catch {
      }
      throw M ?? j;
    } catch (h) {
      let f = h;
      for (const g of a.error.fns)
        g && (f = await g(f, d, m, l));
      if (f = f || {}, u)
        throw f;
      return A === "data" ? void 0 : {
        error: f,
        request: m,
        response: d
      };
    }
  }, i = (l) => (u) => o({ ...u, method: l }), n = (l) => async (u) => {
    const { opts: A, url: m } = await c(u);
    return We({
      ...A,
      body: A.body,
      method: l,
      onRequest: async (d, h) => {
        let f = new Request(d, h);
        for (const g of a.request.fns)
          g && (f = await g(f, A));
        return f;
      },
      serializedBody: ie(A),
      url: m
    });
  };
  return {
    buildUrl: (l) => ne({ ...e, ...l }),
    connect: i("CONNECT"),
    delete: i("DELETE"),
    get: i("GET"),
    getConfig: t,
    head: i("HEAD"),
    interceptors: a,
    options: i("OPTIONS"),
    patch: i("PATCH"),
    post: i("POST"),
    put: i("PUT"),
    request: o,
    setConfig: s,
    sse: {
      connect: n("CONNECT"),
      delete: n("DELETE"),
      get: n("GET"),
      head: n("HEAD"),
      options: n("OPTIONS"),
      patch: n("PATCH"),
      post: n("POST"),
      put: n("PUT"),
      trace: n("TRACE")
    },
    trace: i("TRACE")
  };
}, _ = it(ye({ baseUrl: "https://localhost:44370/", throwOnError: !0 }));
class x {
  static postGetAccessToken(e) {
    return (e.client ?? _).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/access-token",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e.headers
      }
    });
  }
  static getAuthorizationUrl(e) {
    return ((e == null ? void 0 : e.client) ?? _).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/authorization-url",
      ...e
    });
  }
  static getCheckConfiguration(e) {
    return ((e == null ? void 0 : e.client) ?? _).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/check-configuration",
      ...e
    });
  }
  static getFormsByApiKey(e) {
    return ((e == null ? void 0 : e.client) ?? _).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/get",
      ...e
    });
  }
  static getFormsOAuth(e) {
    return ((e == null ? void 0 : e.client) ?? _).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/oauth/get",
      ...e
    });
  }
  static postRefreshAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? _).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/refresh",
      ...e
    });
  }
  static postRevokeAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? _).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/revoke",
      ...e
    });
  }
  static getValidateAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? _).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/hubspot-forms/management/api/v1/forms/validate",
      ...e
    });
  }
}
const H = {
  api: "An API key is configured and will be used to connect to your HubSpot account.",
  oauth: "No API key is configured. To connect to your HubSpot account using OAuth click 'Connect', select your account and agree to the permissions.",
  oauthConnected: "OAuth is configured and an access token is available to connect to your HubSpot account. To revoke, click 'Revoke'",
  none: "No API or OAuth configuration could be found. Please review your settings before continuing."
};
class nt extends le {
  constructor(e) {
    super(e);
  }
  async getAuthorizationUrl() {
    const { data: e, error: t } = await z(this, x.getAuthorizationUrl());
    return t || !e ? { error: t } : { data: e };
  }
  async checkApiConfiguration() {
    const { data: e, error: t } = await z(this, x.getCheckConfiguration());
    return t || !e ? { error: t } : { data: e };
  }
  async getAccessToken(e) {
    const { data: t, error: s } = await z(this, x.postGetAccessToken({
      body: e
    }));
    return s || !t ? { error: s } : { data: t };
  }
  async validateAccessToken() {
    const { data: e, error: t } = await z(this, x.getValidateAccessToken());
    return t || !e ? { error: t } : { data: e };
  }
  async refreshAccessToken() {
    const { data: e, error: t } = await z(this, x.postRefreshAccessToken());
    return t || !e ? { error: t } : { data: e };
  }
  async revokeAccessToken() {
    const { data: e, error: t } = await z(this, x.postRevokeAccessToken());
    return t || !e ? { error: t } : { data: e };
  }
  async getFormsByApiKey() {
    const { data: e, error: t } = await z(this, x.getFormsByApiKey());
    return t || !e ? { error: t } : { data: e };
  }
  async getFormsOAuth() {
    const { data: e, error: t } = await z(this, x.getFormsOAuth());
    return t || !e ? { error: t } : { data: e };
  }
}
var w, q;
class K extends le {
  constructor(t) {
    super(t);
    D(this, w);
    D(this, q);
    V(this, q, new $e(void 0)), this.settingsModel = S(this, q).asObservable(), this.provideContext(X, this), V(this, w, new nt(t));
  }
  async hostConnected() {
    super.hostConnected(), this.checkApiConfiguration();
  }
  async getAuthorizationUrl() {
    return await S(this, w).getAuthorizationUrl();
  }
  async checkApiConfiguration() {
    const { data: t } = await S(this, w).checkApiConfiguration();
    S(this, q).setValue(t);
  }
  async getAccessToken(t) {
    return await S(this, w).getAccessToken(t);
  }
  async validateAccessToken() {
    return await S(this, w).validateAccessToken();
  }
  async refreshAccessToken() {
    return await S(this, w).refreshAccessToken();
  }
  async revokeAccessToken() {
    return await S(this, w).revokeAccessToken();
  }
  async getFormsByApiKey() {
    return await S(this, w).getFormsByApiKey();
  }
  async getFormsOAuth() {
    return await S(this, w).getFormsOAuth();
  }
}
w = new WeakMap(), q = new WeakMap();
const X = new He(K.name), ct = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  HUBSPOT_FORMS_CONTEXT_TOKEN: X,
  HubspotFormsContext: K,
  default: K
}, Symbol.toStringTag, { value: "Module" }));
var ut = Object.defineProperty, lt = Object.getOwnPropertyDescriptor, me = (r) => {
  throw TypeError(r);
}, B = (r, e, t, s) => {
  for (var a = s > 1 ? void 0 : s ? lt(e, t) : e, c = r.length - 1, o; c >= 0; c--)
    (o = r[c]) && (a = (s ? o(e, t, a) : o(a)) || a);
  return s && a && ut(e, t, a), a;
}, Q = (r, e, t) => e.has(r) || me("Cannot " + t), C = (r, e, t) => (Q(r, e, "read from private field"), t ? t.call(r) : e.get(r)), J = (r, e, t) => e.has(r) ? me("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), ue = (r, e, t, s) => (Q(r, e, "write to private field"), e.set(r, t), t), $ = (r, e, t) => (Q(r, e, "access private method"), t), U, E, P, be, ge, we, Ae, ke, ve;
const ht = "hubspot-authorization";
let T = class extends Oe(ze) {
  constructor() {
    super(), J(this, P), J(this, U), J(this, E), this._serviceStatus = {
      isValid: !1,
      type: "",
      description: "",
      useOAuth: !1
    }, this._oauthSetup = {
      isConnected: !1,
      isAccessTokenExpired: !1,
      isAccessTokenValid: !1
    }, this.value = "", this.showAuthTokenComponent = !1, this.consumeContext(X, (r) => {
      r && (ue(this, U, r), this.observe(r.settingsModel, (e) => {
        ue(this, E, e);
      }));
    });
  }
  async connectedCallback() {
    super.connectedCallback(), await $(this, P, be).call(this);
  }
  async getAccessToken(r) {
    const { data: e } = await C(this, U).getAccessToken(r);
    e && (this.showAuthTokenComponent = !1, e.startsWith("Error:") ? this._showError(e) : (this._oauthSetup = {
      isConnected: !0
    }, this._serviceStatus.description = H.oauthConnected, this._showSuccess("OAuth Connected"), this.dispatchEvent(new CustomEvent("connect"))));
  }
  async _showSuccess(r) {
    await this._showMessage(r, "positive");
  }
  async _showError(r) {
    await this._showMessage(r, "danger");
  }
  async _showMessage(r, e) {
    const t = await this.getContext(Ie);
    t == null || t.peek(e, {
      data: { message: r }
    });
  }
  render() {
    return W`
            <p>${this._serviceStatus.description}</p>
            ${oe(
      this._serviceStatus.useOAuth,
      () => W`
                    <div id="authConnect">
                        <uui-button look="primary" 
                                    label="Connect"
                                    ?disabled=${this._oauthSetup.isConnected}
                                    @click=${$(this, P, Ae)}></uui-button>
                        <uui-button look="primary"
                                    color="danger"
                                    label="Revoke"
                                    ?disabled=${!this._oauthSetup.isConnected}
                                    @click=${$(this, P, ke)}></uui-button>
                    </div>
                    ${oe(this.showAuthTokenComponent, () => W`
                        <div id="authToken">
                            <uui-input id="auth-code-input" placeholder="Authorization code"></uui-input>
                            <uui-button look="primary"
                                        label="Authorize"
                                        @click=${$(this, P, ve)}></uui-button>
                        </div>
                    `)}
                `
    )}
        `;
  }
};
U = /* @__PURE__ */ new WeakMap();
E = /* @__PURE__ */ new WeakMap();
P = /* @__PURE__ */ new WeakSet();
be = async function() {
  var r, e;
  C(this, E) && (this._serviceStatus = {
    isValid: C(this, E).isValid,
    type: (r = C(this, E).type) == null ? void 0 : r.value,
    description: $(this, P, we).call(this, this._serviceStatus.type),
    useOAuth: C(this, E).isValid && ((e = C(this, E).type) == null ? void 0 : e.value) === "OAuth"
  }, this._serviceStatus.useOAuth && await $(this, P, ge).call(this), C(this, E).isValid || this._showError("Invalid setup. Please review the API/OAuth settings."));
};
ge = async function() {
  const { data: r } = await C(this, U).validateAccessToken();
  r && (this._oauthSetup = {
    isConnected: r.isValid,
    isAccessTokenExpired: r.isExpired,
    isAccessTokenValid: r.isValid
  }, this._oauthSetup.isConnected && this._oauthSetup.isAccessTokenValid && (this._serviceStatus.description = H.oauthConnected), this._oauthSetup.isAccessTokenExpired && await C(this, U).refreshAccessToken());
};
we = function(r) {
  switch (r) {
    case "api":
      return H.api;
    case "oauth":
      return H.oauth;
    case "oauthConnected":
      return H.oauthConnected;
    default:
      return H.none;
  }
};
Ae = async function() {
  const { data: r } = await C(this, U).getAuthorizationUrl();
  if (!r) return;
  var e = document.getElementById("authToken");
  e && (e.style.display = "none");
  const t = window.open(r, "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
  setTimeout(() => {
    t != null && t.closed || (this.showAuthTokenComponent = !0);
  }, 7e3), window.addEventListener("message", async (s) => {
    if (s.data.type === "hubspot:oauth:success") {
      const a = {
        code: s.data.code
      };
      await this.getAccessToken(a);
    }
  }, !1);
};
ke = async function() {
  await C(this, U).revokeAccessToken(), this._oauthSetup = {
    isConnected: !1
  }, this._serviceStatus.description = H.none, this._showSuccess("OAuth connection revoked."), this.dispatchEvent(new CustomEvent("revoke"));
};
ve = async function() {
  if (this._authCodeInput.value.length == 0) {
    this._showError("Incorrect authorization code.");
    return;
  }
  const r = {
    code: this._authCodeInput.value
  };
  await this.getAccessToken(r);
};
T.styles = [
  xe`
            #authToken { 
                margin-top: 20px; 
            }
            #authToken uui-input {
                width: 50%;
                vertical-align: middle;
            }
        `
];
B([
  G()
], T.prototype, "_serviceStatus", 2);
B([
  G()
], T.prototype, "_oauthSetup", 2);
B([
  Pe({ type: String })
], T.prototype, "value", 2);
B([
  G()
], T.prototype, "showAuthTokenComponent", 2);
B([
  Ue("#auth-code-input")
], T.prototype, "_authCodeInput", 2);
T = B([
  je(ht)
], T);
const dt = T, ft = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get HubspotAuthorizationElement() {
    return T;
  },
  default: dt
}, Symbol.toStringTag, { value: "Module" })), St = (r, e) => {
  e.registerMany([
    ...Re,
    Me,
    De
  ]), r.consumeContext(Ee, async (t) => {
    const s = t == null ? void 0 : t.getOpenApiConfiguration();
    _.setConfig({
      baseUrl: (s == null ? void 0 : s.base) ?? "",
      auth: (s == null ? void 0 : s.token) ?? void 0,
      credentials: (s == null ? void 0 : s.credentials) ?? "same-origin"
    });
  });
};
export {
  H as C,
  X as H,
  T as a,
  St as o
};
//# sourceMappingURL=index-Cm1-jVt_.js.map
