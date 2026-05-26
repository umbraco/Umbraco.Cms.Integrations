import { UMB_AUTH_CONTEXT as F } from "@umbraco-cms/backoffice/auth";
const G = {
  type: "globalContext",
  alias: "shopify.context",
  name: "Shopify Context",
  js: () => import("./shopify.context-Bjllc7Sq.js")
}, Q = G, X = {
  type: "propertyEditorUi",
  alias: "Shopify.PropertyEditorUi.Amount",
  name: "Shopify Product Picker Amount Setting",
  element: () => import("./amount-property-editor.element-CM5qzRlN.js"),
  meta: {
    label: "Amount",
    icon: "icon-autofill",
    group: "common"
  }
}, K = {
  type: "propertyEditorUi",
  alias: "Shopify.PropertyEditorUi.Authorization",
  name: "Shopify Product Picker Authorization Setting",
  element: () => import("./authorization-property-editor.element-B67GD-4w.js"),
  meta: {
    label: "Authorization",
    icon: "icon-autofill",
    group: "common"
  }
}, Y = {
  type: "propertyEditorUi",
  alias: "Shopify.PropertyEditorUi.ProductPicker",
  name: "Shopify Product Picker Property Editor UI",
  element: () => import("./shopify-product-picker-property-editor.element-BVZYJZJD.js"),
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
}, Z = [
  Y,
  X,
  K
], ee = {
  type: "modal",
  alias: "Shopify.Modal",
  name: "Shopify Modal",
  js: () => import("./shopify-products-modal.element-DEInVRT5.js")
}, te = {
  bodySerializer: (r) => JSON.stringify(r, (t, e) => typeof e == "bigint" ? e.toString() : e)
};
function re({
  onRequest: r,
  onSseError: t,
  onSseEvent: e,
  responseTransformer: s,
  responseValidator: o,
  sseDefaultRetryDelay: c,
  sseMaxRetryAttempts: i,
  sseMaxRetryDelay: a,
  sseSleepFn: n,
  url: b,
  ...d
}) {
  let l;
  const g = n ?? ((f) => new Promise((p) => setTimeout(p, f)));
  return { stream: async function* () {
    let f = c ?? 3e3, p = 0;
    const w = d.signal ?? new AbortController().signal;
    for (; !w.aborted; ) {
      p++;
      const j = d.headers instanceof Headers ? d.headers : new Headers(d.headers);
      l !== void 0 && j.set("Last-Event-ID", l);
      try {
        const x = {
          redirect: "follow",
          ...d,
          body: d.serializedBody,
          headers: j,
          signal: w
        };
        let z = new Request(b, x);
        r && (z = await r(b, x));
        const h = await (d.fetch ?? globalThis.fetch)(z);
        if (!h.ok) throw new Error(`SSE failed: ${h.status} ${h.statusText}`);
        if (!h.body) throw new Error("No body in SSE response");
        const S = h.body.pipeThrough(new TextDecoderStream()).getReader();
        let y = "";
        const O = () => {
          try {
            S.cancel();
          } catch {
          }
        };
        w.addEventListener("abort", O);
        try {
          for (; ; ) {
            const { done: V, value: L } = await S.read();
            if (V) break;
            y += L, y = y.replace(/\r\n?/g, `
`);
            const I = y.split(`

`);
            y = I.pop() ?? "";
            for (const _ of I) {
              const J = _.split(`
`), U = [];
              let T;
              for (const E of J)
                if (E.startsWith("data:"))
                  U.push(E.replace(/^data:\s*/, ""));
                else if (E.startsWith("event:"))
                  T = E.replace(/^event:\s*/, "");
                else if (E.startsWith("id:"))
                  l = E.replace(/^id:\s*/, "");
                else if (E.startsWith("retry:")) {
                  const q = Number.parseInt(E.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(q) || (f = q);
                }
              let A, $ = !1;
              if (U.length) {
                const E = U.join(`
`);
                try {
                  A = JSON.parse(E), $ = !0;
                } catch {
                  A = E;
                }
              }
              $ && (o && await o(A), s && (A = await s(A))), e == null || e({
                data: A,
                event: T,
                id: l,
                retry: f
              }), U.length && (yield A);
            }
          }
        } finally {
          w.removeEventListener("abort", O), S.releaseLock();
        }
        break;
      } catch (x) {
        if (t == null || t(x), i !== void 0 && p >= i)
          break;
        const z = Math.min(f * 2 ** (p - 1), a ?? 3e4);
        await g(z);
      }
    }
  }() };
}
const se = (r) => {
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
}, ae = (r) => {
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
}, ie = (r) => {
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
}, D = ({
  allowReserved: r,
  explode: t,
  name: e,
  style: s,
  value: o
}) => {
  if (!t) {
    const a = (r ? o : o.map((n) => encodeURIComponent(n))).join(ae(s));
    switch (s) {
      case "label":
        return `.${a}`;
      case "matrix":
        return `;${e}=${a}`;
      case "simple":
        return a;
      default:
        return `${e}=${a}`;
    }
  }
  const c = se(s), i = o.map((a) => s === "label" || s === "simple" ? r ? a : encodeURIComponent(a) : k({
    allowReserved: r,
    name: e,
    value: a
  })).join(c);
  return s === "label" || s === "matrix" ? c + i : i;
}, k = ({
  allowReserved: r,
  name: t,
  value: e
}) => {
  if (e == null)
    return "";
  if (typeof e == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${t}=${r ? e : encodeURIComponent(e)}`;
}, R = ({
  allowReserved: r,
  explode: t,
  name: e,
  style: s,
  value: o,
  valueOnly: c
}) => {
  if (o instanceof Date)
    return c ? o.toISOString() : `${e}=${o.toISOString()}`;
  if (s !== "deepObject" && !t) {
    let n = [];
    Object.entries(o).forEach(([d, l]) => {
      n = [...n, d, r ? l : encodeURIComponent(l)];
    });
    const b = n.join(",");
    switch (s) {
      case "form":
        return `${e}=${b}`;
      case "label":
        return `.${b}`;
      case "matrix":
        return `;${e}=${b}`;
      default:
        return b;
    }
  }
  const i = ie(s), a = Object.entries(o).map(
    ([n, b]) => k({
      allowReserved: r,
      name: s === "deepObject" ? `${e}[${n}]` : n,
      value: b
    })
  ).join(i);
  return s === "label" || s === "matrix" ? i + a : a;
}, ne = /\{[^{}]+\}/g, oe = ({ path: r, url: t }) => {
  let e = t;
  const s = t.match(ne);
  if (s)
    for (const o of s) {
      let c = !1, i = o.substring(1, o.length - 1), a = "simple";
      i.endsWith("*") && (c = !0, i = i.substring(0, i.length - 1)), i.startsWith(".") ? (i = i.substring(1), a = "label") : i.startsWith(";") && (i = i.substring(1), a = "matrix");
      const n = r[i];
      if (n == null)
        continue;
      if (Array.isArray(n)) {
        e = e.replace(o, D({ explode: c, name: i, style: a, value: n }));
        continue;
      }
      if (typeof n == "object") {
        e = e.replace(
          o,
          R({
            explode: c,
            name: i,
            style: a,
            value: n,
            valueOnly: !0
          })
        );
        continue;
      }
      if (a === "matrix") {
        e = e.replace(
          o,
          `;${k({
            name: i,
            value: n
          })}`
        );
        continue;
      }
      const b = encodeURIComponent(
        a === "label" ? `.${n}` : n
      );
      e = e.replace(o, b);
    }
  return e;
}, ce = ({
  baseUrl: r,
  path: t,
  query: e,
  querySerializer: s,
  url: o
}) => {
  const c = o.startsWith("/") ? o : `/${o}`;
  let i = (r ?? "") + c;
  t && (i = oe({ path: t, url: i }));
  let a = e ? s(e) : "";
  return a.startsWith("?") && (a = a.substring(1)), a && (i += `?${a}`), i;
};
function v(r) {
  const t = r.body !== void 0;
  if (t && r.bodySerializer)
    return "serializedBody" in r ? r.serializedBody !== void 0 && r.serializedBody !== "" ? r.serializedBody : null : r.body !== "" ? r.body : null;
  if (t)
    return r.body;
}
const le = async (r, t) => {
  const e = typeof t == "function" ? await t(r) : t;
  if (e)
    return r.scheme === "bearer" ? `Bearer ${e}` : r.scheme === "basic" ? `Basic ${btoa(e)}` : e;
}, H = ({
  parameters: r = {},
  ...t
} = {}) => (s) => {
  const o = [];
  if (s && typeof s == "object")
    for (const c in s) {
      const i = s[c];
      if (i == null)
        continue;
      const a = r[c] || t;
      if (Array.isArray(i)) {
        const n = D({
          allowReserved: a.allowReserved,
          explode: !0,
          name: c,
          style: "form",
          value: i,
          ...a.array
        });
        n && o.push(n);
      } else if (typeof i == "object") {
        const n = R({
          allowReserved: a.allowReserved,
          explode: !0,
          name: c,
          style: "deepObject",
          value: i,
          ...a.object
        });
        n && o.push(n);
      } else {
        const n = k({
          allowReserved: a.allowReserved,
          name: c,
          value: i
        });
        n && o.push(n);
      }
    }
  return o.join("&");
}, de = (r) => {
  var e;
  if (!r)
    return "stream";
  const t = (e = r.split(";")[0]) == null ? void 0 : e.trim();
  if (t) {
    if (t.startsWith("application/json") || t.endsWith("+json"))
      return "json";
    if (t === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some((s) => t.startsWith(s)))
      return "blob";
    if (t.startsWith("text/"))
      return "text";
  }
}, fe = (r, t) => {
  var e, s;
  return t ? !!(r.headers.has(t) || (e = r.query) != null && e[t] || (s = r.headers.get("Cookie")) != null && s.includes(`${t}=`)) : !1;
};
async function ue(r) {
  for (const t of r.security ?? []) {
    if (fe(r, t.name))
      continue;
    const e = await le(t, r.auth);
    if (!e)
      continue;
    const s = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        r.query || (r.query = {}), r.query[s] = e;
        break;
      case "cookie":
        r.headers.append("Cookie", `${s}=${e}`);
        break;
      case "header":
      default:
        r.headers.set(s, e);
        break;
    }
  }
}
const B = (r) => ce({
  baseUrl: r.baseUrl,
  path: r.path,
  query: r.query,
  querySerializer: typeof r.querySerializer == "function" ? r.querySerializer : H(r.querySerializer),
  url: r.url
}), N = (r, t) => {
  var s;
  const e = { ...r, ...t };
  return (s = e.baseUrl) != null && s.endsWith("/") && (e.baseUrl = e.baseUrl.substring(0, e.baseUrl.length - 1)), e.headers = W(r.headers, t.headers), e;
}, pe = (r) => {
  const t = [];
  return r.forEach((e, s) => {
    t.push([s, e]);
  }), t;
}, W = (...r) => {
  const t = new Headers();
  for (const e of r) {
    if (!e)
      continue;
    const s = e instanceof Headers ? pe(e) : Object.entries(e);
    for (const [o, c] of s)
      if (c === null)
        t.delete(o);
      else if (Array.isArray(c))
        for (const i of c)
          t.append(o, i);
      else c !== void 0 && t.set(
        o,
        typeof c == "object" ? JSON.stringify(c) : c
      );
  }
  return t;
};
class C {
  constructor() {
    this.fns = [];
  }
  clear() {
    this.fns = [];
  }
  eject(t) {
    const e = this.getInterceptorIndex(t);
    this.fns[e] && (this.fns[e] = null);
  }
  exists(t) {
    const e = this.getInterceptorIndex(t);
    return !!this.fns[e];
  }
  getInterceptorIndex(t) {
    return typeof t == "number" ? this.fns[t] ? t : -1 : this.fns.indexOf(t);
  }
  update(t, e) {
    const s = this.getInterceptorIndex(t);
    return this.fns[s] ? (this.fns[s] = e, t) : !1;
  }
  use(t) {
    return this.fns.push(t), this.fns.length - 1;
  }
}
const he = () => ({
  error: new C(),
  request: new C(),
  response: new C()
}), ye = H({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), me = {
  "Content-Type": "application/json"
}, M = (r = {}) => ({
  ...te,
  headers: me,
  parseAs: "auto",
  querySerializer: ye,
  ...r
}), be = (r = {}) => {
  let t = N(M(), r);
  const e = () => ({ ...t }), s = (d) => (t = N(t, d), e()), o = he(), c = async (d) => {
    const l = {
      ...t,
      ...d,
      fetch: d.fetch ?? t.fetch ?? globalThis.fetch,
      headers: W(t.headers, d.headers),
      serializedBody: void 0
    };
    l.security && await ue(l), l.requestValidator && await l.requestValidator(l), l.body !== void 0 && l.bodySerializer && (l.serializedBody = l.bodySerializer(l.body)), (l.body === void 0 || l.serializedBody === "") && l.headers.delete("Content-Type");
    const g = l, m = B(g);
    return { opts: g, url: m };
  }, i = async (d) => {
    const l = d.throwOnError ?? t.throwOnError, g = d.responseStyle ?? t.responseStyle;
    let m, u;
    try {
      const { opts: f, url: p } = await c(d), w = {
        redirect: "follow",
        ...f,
        body: v(f)
      };
      m = new Request(p, w);
      for (const h of o.request.fns)
        h && (m = await h(m, f));
      const j = f.fetch;
      u = await j(m);
      for (const h of o.response.fns)
        h && (u = await h(u, m, f));
      const x = {
        request: m,
        response: u
      };
      if (u.ok) {
        const h = (f.parseAs === "auto" ? de(u.headers.get("Content-Type")) : f.parseAs) ?? "json";
        if (u.status === 204 || u.headers.get("Content-Length") === "0") {
          let y;
          switch (h) {
            case "arrayBuffer":
            case "blob":
            case "text":
              y = await u[h]();
              break;
            case "formData":
              y = new FormData();
              break;
            case "stream":
              y = u.body;
              break;
            case "json":
            default:
              y = {};
              break;
          }
          return f.responseStyle === "data" ? y : {
            data: y,
            ...x
          };
        }
        let S;
        switch (h) {
          case "arrayBuffer":
          case "blob":
          case "formData":
          case "text":
            S = await u[h]();
            break;
          case "json": {
            const y = await u.text();
            S = y ? JSON.parse(y) : {};
            break;
          }
          case "stream":
            return f.responseStyle === "data" ? u.body : {
              data: u.body,
              ...x
            };
        }
        return h === "json" && (f.responseValidator && await f.responseValidator(S), f.responseTransformer && (S = await f.responseTransformer(S))), f.responseStyle === "data" ? S : {
          data: S,
          ...x
        };
      }
      const z = await u.text();
      let P;
      try {
        P = JSON.parse(z);
      } catch {
      }
      throw P ?? z;
    } catch (f) {
      let p = f;
      for (const w of o.error.fns)
        w && (p = await w(p, u, m, d));
      if (p = p || {}, l)
        throw p;
      return g === "data" ? void 0 : {
        error: p,
        request: m,
        response: u
      };
    }
  }, a = (d) => (l) => i({ ...l, method: d }), n = (d) => async (l) => {
    const { opts: g, url: m } = await c(l);
    return re({
      ...g,
      body: g.body,
      method: d,
      onRequest: async (u, f) => {
        let p = new Request(u, f);
        for (const w of o.request.fns)
          w && (p = await w(p, g));
        return p;
      },
      serializedBody: v(g),
      url: m
    });
  };
  return {
    buildUrl: (d) => B({ ...t, ...d }),
    connect: a("CONNECT"),
    delete: a("DELETE"),
    get: a("GET"),
    getConfig: e,
    head: a("HEAD"),
    interceptors: o,
    options: a("OPTIONS"),
    patch: a("PATCH"),
    post: a("POST"),
    put: a("PUT"),
    request: i,
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
    trace: a("TRACE")
  };
}, we = be(M({ baseUrl: "https://localhost:44370/", throwOnError: !0 })), Se = (r, t) => {
  t.registerMany([
    ...Z,
    ee,
    Q
  ]), r.consumeContext(F, async (e) => {
    const s = e == null ? void 0 : e.getOpenApiConfiguration();
    we.setConfig({
      baseUrl: (s == null ? void 0 : s.base) ?? "",
      auth: (s == null ? void 0 : s.token) ?? void 0,
      credentials: (s == null ? void 0 : s.credentials) ?? "same-origin"
    });
  });
};
export {
  we as c,
  Se as o
};
//# sourceMappingURL=index-BjHisZi7.js.map
