var re = (r) => {
  throw TypeError(r);
};
var se = (r, e, t) => e.has(r) || re("Cannot " + t);
var _ = (r, e, t) => (se(r, e, "read from private field"), t ? t.call(r) : e.get(r)), R = (r, e, t) => e.has(r) ? re("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), W = (r, e, t, s) => (se(r, e, "write to private field"), s ? s.call(r, t) : e.set(r, t), t);
import { UMB_AUTH_CONTEXT as ve } from "@umbraco-cms/backoffice/auth";
import { UmbElementMixin as _e } from "@umbraco-cms/backoffice/element-api";
import { LitElement as Ae, html as $, state as G, customElement as Ee } from "@umbraco-cms/backoffice/external/lit";
import { UmbControllerBase as ce } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as Se } from "@umbraco-cms/backoffice/context-api";
import { tryExecute as S } from "@umbraco-cms/backoffice/resources";
import { UmbObjectState as Te } from "@umbraco-cms/backoffice/observable-api";
import { UMB_NOTIFICATION_CONTEXT as ze } from "@umbraco-cms/backoffice/notification";
const Oe = {
  type: "globalContext",
  alias: "dynamics.context",
  name: "Dynamics Context",
  js: () => Promise.resolve().then(() => et)
}, Ue = Oe, xe = {
  type: "propertyEditorUi",
  alias: "Dynamics.PropertyEditorUi.Authorization",
  name: "Dynamics Form Picker Authorization Setting",
  js: () => Promise.resolve().then(() => nt),
  meta: {
    label: "Authorization",
    icon: "icon-autofill",
    group: "common"
  }
}, je = {
  type: "propertyEditorUi",
  alias: "Dynamics.PropertyEditorUi.FormPicker",
  name: "Dynamics Form Picker Property Editor UI",
  js: () => import("./dynamics-form-picker-property-editor.element-B2xMC8sb.js"),
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
}, De = [
  je,
  xe
], Pe = {
  type: "modal",
  alias: "Dynamics.Modal",
  name: "Dynamics Modal",
  js: () => import("./dynamics-form-modal.element-DgdpDQZy.js")
}, $e = {
  bodySerializer: (r) => JSON.stringify(r, (e, t) => typeof t == "bigint" ? t.toString() : t)
};
function Ne({
  onRequest: r,
  onSseError: e,
  onSseEvent: t,
  responseTransformer: s,
  responseValidator: a,
  sseDefaultRetryDelay: c,
  sseMaxRetryAttempts: n,
  sseMaxRetryDelay: i,
  sseSleepFn: o,
  url: b,
  ...l
}) {
  let u;
  const k = o ?? ((d) => new Promise((f) => setTimeout(f, d)));
  return { stream: async function* () {
    let d = c ?? 3e3, f = 0;
    const g = l.signal ?? new AbortController().signal;
    for (; !g.aborted; ) {
      f++;
      const q = l.headers instanceof Headers ? l.headers : new Headers(l.headers);
      u !== void 0 && q.set("Last-Event-ID", u);
      try {
        const E = {
          redirect: "follow",
          ...l,
          body: l.serializedBody,
          headers: q,
          signal: g
        };
        let O = new Request(b, E);
        r && (O = await r(b, E));
        const m = await (l.fetch ?? globalThis.fetch)(O);
        if (!m.ok) throw new Error(`SSE failed: ${m.status} ${m.statusText}`);
        if (!m.body) throw new Error("No body in SSE response");
        const C = m.body.pipeThrough(new TextDecoderStream()).getReader();
        let y = "";
        const K = () => {
          try {
            C.cancel();
          } catch {
          }
        };
        g.addEventListener("abort", K);
        try {
          for (; ; ) {
            const { done: ge, value: we } = await C.read();
            if (ge) break;
            y += we, y = y.replace(/\r\n?/g, `
`);
            const Y = y.split(`

`);
            y = Y.pop() ?? "";
            for (const ke of Y) {
              const Ce = ke.split(`
`), F = [];
              let Z;
              for (const v of Ce)
                if (v.startsWith("data:"))
                  F.push(v.replace(/^data:\s*/, ""));
                else if (v.startsWith("event:"))
                  Z = v.replace(/^event:\s*/, "");
                else if (v.startsWith("id:"))
                  u = v.replace(/^id:\s*/, "");
                else if (v.startsWith("retry:")) {
                  const te = Number.parseInt(v.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(te) || (d = te);
                }
              let U, ee = !1;
              if (F.length) {
                const v = F.join(`
`);
                try {
                  U = JSON.parse(v), ee = !0;
                } catch {
                  U = v;
                }
              }
              ee && (a && await a(U), s && (U = await s(U))), t == null || t({
                data: U,
                event: Z,
                id: u,
                retry: d
              }), F.length && (yield U);
            }
          }
        } finally {
          g.removeEventListener("abort", K), C.releaseLock();
        }
        break;
      } catch (E) {
        if (e == null || e(E), n !== void 0 && f >= n)
          break;
        const O = Math.min(d * 2 ** (f - 1), i ?? 3e4);
        await k(O);
      }
    }
  }() };
}
const qe = (r) => {
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
}, Fe = (r) => {
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
}, Ie = (r) => {
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
}, ue = ({
  allowReserved: r,
  explode: e,
  name: t,
  style: s,
  value: a
}) => {
  if (!e) {
    const i = (r ? a : a.map((o) => encodeURIComponent(o))).join(Fe(s));
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
  const c = qe(s), n = a.map((i) => s === "label" || s === "simple" ? r ? i : encodeURIComponent(i) : I({
    allowReserved: r,
    name: t,
    value: i
  })).join(c);
  return s === "label" || s === "matrix" ? c + n : n;
}, I = ({
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
}, le = ({
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
    let o = [];
    Object.entries(a).forEach(([l, u]) => {
      o = [...o, l, r ? u : encodeURIComponent(u)];
    });
    const b = o.join(",");
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
  const n = Ie(s), i = Object.entries(a).map(
    ([o, b]) => I({
      allowReserved: r,
      name: s === "deepObject" ? `${t}[${o}]` : o,
      value: b
    })
  ).join(n);
  return s === "label" || s === "matrix" ? n + i : i;
}, Be = /\{[^{}]+\}/g, Me = ({ path: r, url: e }) => {
  let t = e;
  const s = e.match(Be);
  if (s)
    for (const a of s) {
      let c = !1, n = a.substring(1, a.length - 1), i = "simple";
      n.endsWith("*") && (c = !0, n = n.substring(0, n.length - 1)), n.startsWith(".") ? (n = n.substring(1), i = "label") : n.startsWith(";") && (n = n.substring(1), i = "matrix");
      const o = r[n];
      if (o == null)
        continue;
      if (Array.isArray(o)) {
        t = t.replace(a, ue({ explode: c, name: n, style: i, value: o }));
        continue;
      }
      if (typeof o == "object") {
        t = t.replace(
          a,
          le({
            explode: c,
            name: n,
            style: i,
            value: o,
            valueOnly: !0
          })
        );
        continue;
      }
      if (i === "matrix") {
        t = t.replace(
          a,
          `;${I({
            name: n,
            value: o
          })}`
        );
        continue;
      }
      const b = encodeURIComponent(
        i === "label" ? `.${o}` : o
      );
      t = t.replace(a, b);
    }
  return t;
}, Re = ({
  baseUrl: r,
  path: e,
  query: t,
  querySerializer: s,
  url: a
}) => {
  const c = a.startsWith("/") ? a : `/${a}`;
  let n = (r ?? "") + c;
  e && (n = Me({ path: e, url: n }));
  let i = t ? s(t) : "";
  return i.startsWith("?") && (i = i.substring(1)), i && (n += `?${i}`), n;
};
function ae(r) {
  const e = r.body !== void 0;
  if (e && r.bodySerializer)
    return "serializedBody" in r ? r.serializedBody !== void 0 && r.serializedBody !== "" ? r.serializedBody : null : r.body !== "" ? r.body : null;
  if (e)
    return r.body;
}
const We = async (r, e) => {
  const t = typeof e == "function" ? await e(r) : e;
  if (t)
    return r.scheme === "bearer" ? `Bearer ${t}` : r.scheme === "basic" ? `Basic ${btoa(t)}` : t;
}, de = ({
  parameters: r = {},
  ...e
} = {}) => (s) => {
  const a = [];
  if (s && typeof s == "object")
    for (const c in s) {
      const n = s[c];
      if (n == null)
        continue;
      const i = r[c] || e;
      if (Array.isArray(n)) {
        const o = ue({
          allowReserved: i.allowReserved,
          explode: !0,
          name: c,
          style: "form",
          value: n,
          ...i.array
        });
        o && a.push(o);
      } else if (typeof n == "object") {
        const o = le({
          allowReserved: i.allowReserved,
          explode: !0,
          name: c,
          style: "deepObject",
          value: n,
          ...i.object
        });
        o && a.push(o);
      } else {
        const o = I({
          allowReserved: i.allowReserved,
          name: c,
          value: n
        });
        o && a.push(o);
      }
    }
  return a.join("&");
}, He = (r) => {
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
}, Ve = (r, e) => {
  var t, s;
  return e ? !!(r.headers.has(e) || (t = r.query) != null && t[e] || (s = r.headers.get("Cookie")) != null && s.includes(`${e}=`)) : !1;
};
async function Le(r) {
  for (const e of r.security ?? []) {
    if (Ve(r, e.name))
      continue;
    const t = await We(e, r.auth);
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
const ne = (r) => Re({
  baseUrl: r.baseUrl,
  path: r.path,
  query: r.query,
  querySerializer: typeof r.querySerializer == "function" ? r.querySerializer : de(r.querySerializer),
  url: r.url
}), ie = (r, e) => {
  var s;
  const t = { ...r, ...e };
  return (s = t.baseUrl) != null && s.endsWith("/") && (t.baseUrl = t.baseUrl.substring(0, t.baseUrl.length - 1)), t.headers = he(r.headers, e.headers), t;
}, Je = (r) => {
  const e = [];
  return r.forEach((t, s) => {
    e.push([s, t]);
  }), e;
}, he = (...r) => {
  const e = new Headers();
  for (const t of r) {
    if (!t)
      continue;
    const s = t instanceof Headers ? Je(t) : Object.entries(t);
    for (const [a, c] of s)
      if (c === null)
        e.delete(a);
      else if (Array.isArray(c))
        for (const n of c)
          e.append(a, n);
      else c !== void 0 && e.set(
        a,
        typeof c == "object" ? JSON.stringify(c) : c
      );
  }
  return e;
};
class H {
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
const Ge = () => ({
  error: new H(),
  request: new H(),
  response: new H()
}), Xe = de({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), Qe = {
  "Content-Type": "application/json"
}, fe = (r = {}) => ({
  ...$e,
  headers: Qe,
  parseAs: "auto",
  querySerializer: Xe,
  ...r
}), Ke = (r = {}) => {
  let e = ie(fe(), r);
  const t = () => ({ ...e }), s = (l) => (e = ie(e, l), t()), a = Ge(), c = async (l) => {
    const u = {
      ...e,
      ...l,
      fetch: l.fetch ?? e.fetch ?? globalThis.fetch,
      headers: he(e.headers, l.headers),
      serializedBody: void 0
    };
    u.security && await Le(u), u.requestValidator && await u.requestValidator(u), u.body !== void 0 && u.bodySerializer && (u.serializedBody = u.bodySerializer(u.body)), (u.body === void 0 || u.serializedBody === "") && u.headers.delete("Content-Type");
    const k = u, p = ne(k);
    return { opts: k, url: p };
  }, n = async (l) => {
    const u = l.throwOnError ?? e.throwOnError, k = l.responseStyle ?? e.responseStyle;
    let p, h;
    try {
      const { opts: d, url: f } = await c(l), g = {
        redirect: "follow",
        ...d,
        body: ae(d)
      };
      p = new Request(f, g);
      for (const m of a.request.fns)
        m && (p = await m(p, d));
      const q = d.fetch;
      h = await q(p);
      for (const m of a.response.fns)
        m && (h = await m(h, p, d));
      const E = {
        request: p,
        response: h
      };
      if (h.ok) {
        const m = (d.parseAs === "auto" ? He(h.headers.get("Content-Type")) : d.parseAs) ?? "json";
        if (h.status === 204 || h.headers.get("Content-Length") === "0") {
          let y;
          switch (m) {
            case "arrayBuffer":
            case "blob":
            case "text":
              y = await h[m]();
              break;
            case "formData":
              y = new FormData();
              break;
            case "stream":
              y = h.body;
              break;
            case "json":
            default:
              y = {};
              break;
          }
          return d.responseStyle === "data" ? y : {
            data: y,
            ...E
          };
        }
        let C;
        switch (m) {
          case "arrayBuffer":
          case "blob":
          case "formData":
          case "text":
            C = await h[m]();
            break;
          case "json": {
            const y = await h.text();
            C = y ? JSON.parse(y) : {};
            break;
          }
          case "stream":
            return d.responseStyle === "data" ? h.body : {
              data: h.body,
              ...E
            };
        }
        return m === "json" && (d.responseValidator && await d.responseValidator(C), d.responseTransformer && (C = await d.responseTransformer(C))), d.responseStyle === "data" ? C : {
          data: C,
          ...E
        };
      }
      const O = await h.text();
      let M;
      try {
        M = JSON.parse(O);
      } catch {
      }
      throw M ?? O;
    } catch (d) {
      let f = d;
      for (const g of a.error.fns)
        g && (f = await g(f, h, p, l));
      if (f = f || {}, u)
        throw f;
      return k === "data" ? void 0 : {
        error: f,
        request: p,
        response: h
      };
    }
  }, i = (l) => (u) => n({ ...u, method: l }), o = (l) => async (u) => {
    const { opts: k, url: p } = await c(u);
    return Ne({
      ...k,
      body: k.body,
      method: l,
      onRequest: async (h, d) => {
        let f = new Request(h, d);
        for (const g of a.request.fns)
          g && (f = await g(f, k));
        return f;
      },
      serializedBody: ae(k),
      url: p
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
    request: n,
    setConfig: s,
    sse: {
      connect: o("CONNECT"),
      delete: o("DELETE"),
      get: o("GET"),
      head: o("HEAD"),
      options: o("OPTIONS"),
      patch: o("PATCH"),
      post: o("POST"),
      put: o("PUT"),
      trace: o("TRACE")
    },
    trace: i("TRACE")
  };
}, A = Ke(fe({ baseUrl: "https://localhost:44370/", throwOnError: !0 }));
class x {
  static getForms(e) {
    return ((e == null ? void 0 : e.client) ?? A).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/dynamics/management/api/v1/forms",
      ...e
    });
  }
  static postFormsAccessToken(e) {
    return (e.client ?? A).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/dynamics/management/api/v1/forms/access-token",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e.headers
      }
    });
  }
  static getFormsAuthorizationUrl(e) {
    return ((e == null ? void 0 : e.client) ?? A).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/dynamics/management/api/v1/forms/authorization-url",
      ...e
    });
  }
  static getFormsEmbedCode(e) {
    return ((e == null ? void 0 : e.client) ?? A).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/dynamics/management/api/v1/forms/embed-code",
      ...e
    });
  }
  static getFormsOauthConfiguration(e) {
    return ((e == null ? void 0 : e.client) ?? A).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/dynamics/management/api/v1/forms/oauth-configuration",
      ...e
    });
  }
  static deleteFormsRevokeAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? A).delete({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/dynamics/management/api/v1/forms/revoke-access-token",
      ...e
    });
  }
  static getFormsSystemUserFullname(e) {
    return ((e == null ? void 0 : e.client) ?? A).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/dynamics/management/api/v1/forms/system-user-fullname",
      ...e
    });
  }
}
class Ye {
  static getUmbracoApiDynamicsAuthorization(e) {
    return ((e == null ? void 0 : e.client) ?? A).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/api/dynamics/authorization",
      ...e
    });
  }
}
class Ze extends ce {
  constructor(e) {
    super(e);
  }
  async getForms(e) {
    const { data: t, error: s } = await S(this, x.getForms({
      query: {
        module: e
      }
    }));
    return s || !t ? { error: s } : { data: t };
  }
  async revokeAccessToken() {
    const { data: e, error: t } = await S(this, x.deleteFormsRevokeAccessToken());
    return t || !e ? { error: t } : { data: e };
  }
  async getAuthorizationUrl() {
    const { data: e, error: t } = await S(this, x.getFormsAuthorizationUrl());
    return t || !e ? { error: t } : { data: e };
  }
  async checkOauthConfiguration() {
    const { data: e, error: t } = await S(this, x.getFormsOauthConfiguration());
    return t || !e ? { error: t } : { data: e };
  }
  async getAccessToken(e) {
    const { data: t, error: s } = await S(this, x.postFormsAccessToken({ body: e }));
    return s || !t ? { error: s } : { data: t };
  }
  async getEmbedCode(e) {
    const { data: t, error: s } = await S(this, x.getFormsEmbedCode({
      query: {
        formId: e
      }
    }));
    return s || !t ? { error: s } : { data: t };
  }
  async getSystemUserFullName() {
    const { data: e, error: t } = await S(this, x.getFormsSystemUserFullname());
    return t || !e ? { error: t } : { data: e };
  }
  async oauth(e) {
    const { data: t, error: s } = await S(this, Ye.getUmbracoApiDynamicsAuthorization({
      query: {
        code: e
      }
    }));
    return s || !t ? { error: s } : { data: t };
  }
}
var w, P;
class J extends ce {
  constructor(t) {
    super(t);
    R(this, w);
    R(this, P);
    W(this, P, new Te(void 0)), this.settingsModel = _(this, P).asObservable(), this.provideContext(X, this), W(this, w, new Ze(t));
  }
  async hostConnected() {
    super.hostConnected(), await this.checkOauthConfiguration();
  }
  async getForms(t) {
    return await _(this, w).getForms(t);
  }
  async revokeAccessToken() {
    return await _(this, w).revokeAccessToken();
  }
  async getAuthorizationUrl() {
    return await _(this, w).getAuthorizationUrl();
  }
  async checkOauthConfiguration() {
    const { data: t } = await _(this, w).checkOauthConfiguration();
    _(this, P).setValue(t);
  }
  async getAccessToken(t) {
    return await _(this, w).getAccessToken(t);
  }
  async getEmbedCode(t) {
    return await _(this, w).getEmbedCode(t);
  }
  async getSystemUserFullName() {
    return await _(this, w).getSystemUserFullName();
  }
  async oauth(t) {
    return await _(this, w).oauth(t);
  }
}
w = new WeakMap(), P = new WeakMap();
const X = new Se(J.name), et = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  DYNAMICS_CONTEXT_TOKEN: X,
  DynamicsContext: J,
  default: J
}, Symbol.toStringTag, { value: "Module" }));
var tt = Object.defineProperty, rt = Object.getOwnPropertyDescriptor, me = (r) => {
  throw TypeError(r);
}, B = (r, e, t, s) => {
  for (var a = s > 1 ? void 0 : s ? rt(e, t) : e, c = r.length - 1, n; c >= 0; c--)
    (n = r[c]) && (a = (s ? n(e, t, a) : n(a)) || a);
  return s && a && tt(e, t, a), a;
}, Q = (r, e, t) => e.has(r) || me("Cannot " + t), z = (r, e, t) => (Q(r, e, "read from private field"), t ? t.call(r) : e.get(r)), V = (r, e, t) => e.has(r) ? me("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), oe = (r, e, t, s) => (Q(r, e, "write to private field"), e.set(r, t), t), L = (r, e, t) => (Q(r, e, "access private method"), t), T, D, N, ye, pe, be;
const st = "dynamics-authorization";
let j = class extends _e(Ae) {
  constructor() {
    super(), V(this, N), V(this, T), V(this, D), this._oauthSetup = {
      isConnected: !1,
      isAccessTokenExpired: !0,
      isAccessTokenValid: !1
    }, this._loading = !0, this._userName = "", this.consumeContext(X, (r) => {
      r && (oe(this, T, r), this.observe(r.settingsModel, (e) => {
        oe(this, D, e);
      }));
    });
  }
  async connectedCallback() {
    super.connectedCallback(), setTimeout(() => {
      L(this, N, ye).call(this);
    }, 3e3);
  }
  async getAccessToken(r) {
    this._loading = !1;
    const { data: e } = await z(this, T).getAccessToken(r);
    if (e)
      if (e.startsWith("Error:"))
        this._showError(e);
      else {
        this._oauthSetup = {
          isConnected: !0
        }, this._showSuccess("OAuth Connected"), await z(this, T).checkOauthConfiguration();
        const { data: t } = await z(this, T).getSystemUserFullName();
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
    const t = await this.getContext(ze);
    t == null || t.peek(e, {
      data: { message: r }
    });
  }
  render() {
    var r, e;
    return $`
            ${this._loading ? $`
                    <div class="center loader"><uui-loader></uui-loader></div>
                ` : $`
                    <div>
                        ${this._oauthSetup.isConnected ? $`
                                <span>
                                    <b>Connected</b>: ${this._userName}
                                </span>
                            ` : $`
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
                            @click=${L(this, N, pe)}></uui-button>
                        <uui-button 
                            color="danger" 
                            look="primary" 
                            label="Revoke"
                            ?disabled=${!((e = this._oauthSetup) != null && e.isConnected)}
                            @click=${L(this, N, be)}></uui-button>
                    </div>
                `}
        `;
  }
};
T = /* @__PURE__ */ new WeakMap();
D = /* @__PURE__ */ new WeakMap();
N = /* @__PURE__ */ new WeakSet();
ye = function() {
  z(this, D) && (z(this, D).isAuthorized ? (this._oauthSetup = {
    isConnected: !0,
    isAccessTokenExpired: !1,
    isAccessTokenValid: !0
  }, this._userName = z(this, D).fullName) : this._showError("Unable to connect to Dynamics. Please review the settings of the form picker property's data type."), this._loading = !1);
};
pe = async function() {
  const { data: r } = await z(this, T).getAuthorizationUrl();
  if (!r) return;
  const e = window.open(r, "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
  this._loading = !0, setTimeout(() => {
    e != null && e.closed && (this._loading = !1);
  }, 3e3), window.addEventListener("message", async (t) => {
    if (t.data.type === "dynamics:oauth:success") {
      const s = {
        code: t.data.code
      };
      await this.getAccessToken(s);
    }
  }, !1);
};
be = async function() {
  await z(this, T).revokeAccessToken(), this._oauthSetup = {
    isConnected: !1
  }, this._showSuccess("OAuth connection revoked."), this.dispatchEvent(new CustomEvent("revoke"));
};
B([
  G()
], j.prototype, "_oauthSetup", 2);
B([
  G()
], j.prototype, "_loading", 2);
B([
  G()
], j.prototype, "_userName", 2);
j = B([
  Ee(st)
], j);
const at = j, nt = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get DynamicsAuthorizationElement() {
    return j;
  },
  default: at
}, Symbol.toStringTag, { value: "Module" })), yt = (r, e) => {
  e.registerMany([
    Ue,
    ...De,
    Pe
  ]), r.consumeContext(ve, async (t) => {
    const s = t == null ? void 0 : t.getOpenApiConfiguration();
    A.setConfig({
      baseUrl: (s == null ? void 0 : s.base) ?? "",
      auth: (s == null ? void 0 : s.token) ?? void 0,
      credentials: (s == null ? void 0 : s.credentials) ?? "same-origin"
    });
  });
};
export {
  X as D,
  j as a,
  yt as o
};
//# sourceMappingURL=index-ya19u6ji.js.map
