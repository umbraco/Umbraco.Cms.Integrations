import { UMB_AUTH_CONTEXT as J } from "@umbraco-cms/backoffice/auth";
const G = {
  type: "propertyEditorUi",
  alias: "ActiveCampaign.PropertyEditorUi.FormPicker",
  name: "ActiveCampaign Form Picker Property Editor UI",
  js: () => import("./form-picker-property-editor.element-CKlPPCzM.js"),
  elementName: "activecampaign-form-picker",
  meta: {
    label: "ActiveCampaign Form Picker",
    icon: "icon-activecampaign",
    group: "pickers",
    propertyEditorSchemaAlias: "ActiveCampaign.FormPicker"
  }
}, Q = {
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
}, X = {
  type: "propertyEditorUi",
  alias: "ActiveCampaign.PropertyEditorUi.Configuration",
  name: "ActiveCampaign Configuration Property Editor UI",
  js: () => import("./configuration-property-editor.element-hThKr8NX.js"),
  elementName: "activecampaign-forms-configuration",
  meta: {
    label: "Configuration",
    icon: "",
    group: ""
  }
}, K = {
  type: "icons",
  name: "ActiveCampaign Forms Icon",
  alias: "ActiveCampaign.PropertyEditorUi.Icon",
  js: () => import("./icons-dictionary-DIDvu1T_.js")
}, Y = [
  G,
  Q,
  X,
  K
], Z = {
  type: "globalContext",
  alias: "activecampaign-forms.context",
  name: "ActiveCampaign Forms Context",
  js: () => import("./activecampaign-forms.context-CtsKAXiI.js")
}, ee = Z, te = {
  type: "modal",
  alias: "ActiveCampaignForms.Modal",
  name: "ActiveCampaign Forms Modal",
  js: () => import("./activecampaign-forms-modal.element-nHGD1sQ7.js")
}, re = {
  bodySerializer: (r) => JSON.stringify(r, (t, e) => typeof e == "bigint" ? e.toString() : e)
};
function ae({
  onRequest: r,
  onSseError: t,
  onSseEvent: e,
  responseTransformer: a,
  responseValidator: o,
  sseDefaultRetryDelay: c,
  sseMaxRetryAttempts: i,
  sseMaxRetryDelay: s,
  sseSleepFn: n,
  url: b,
  ...d
}) {
  let l;
  const w = n ?? ((f) => new Promise((p) => setTimeout(p, f)));
  return { stream: async function* () {
    let f = c ?? 3e3, p = 0;
    const g = d.signal ?? new AbortController().signal;
    for (; !g.aborted; ) {
      p++;
      const v = d.headers instanceof Headers ? d.headers : new Headers(d.headers);
      l !== void 0 && v.set("Last-Event-ID", l);
      try {
        const E = {
          redirect: "follow",
          ...d,
          body: d.serializedBody,
          headers: v,
          signal: g
        };
        let j = new Request(b, E);
        r && (j = await r(b, E));
        const h = await (d.fetch ?? globalThis.fetch)(j);
        if (!h.ok) throw new Error(`SSE failed: ${h.status} ${h.statusText}`);
        if (!h.body) throw new Error("No body in SSE response");
        const C = h.body.pipeThrough(new TextDecoderStream()).getReader();
        let y = "";
        const O = () => {
          try {
            C.cancel();
          } catch {
          }
        };
        g.addEventListener("abort", O);
        try {
          for (; ; ) {
            const { done: M, value: V } = await C.read();
            if (M) break;
            y += V, y = y.replace(/\r\n?/g, `
`);
            const P = y.split(`

`);
            y = P.pop() ?? "";
            for (const L of P) {
              const _ = L.split(`
`), x = [];
              let I;
              for (const A of _)
                if (A.startsWith("data:"))
                  x.push(A.replace(/^data:\s*/, ""));
                else if (A.startsWith("event:"))
                  I = A.replace(/^event:\s*/, "");
                else if (A.startsWith("id:"))
                  l = A.replace(/^id:\s*/, "");
                else if (A.startsWith("retry:")) {
                  const q = Number.parseInt(A.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(q) || (f = q);
                }
              let S, T = !1;
              if (x.length) {
                const A = x.join(`
`);
                try {
                  S = JSON.parse(A), T = !0;
                } catch {
                  S = A;
                }
              }
              T && (o && await o(S), a && (S = await a(S))), e == null || e({
                data: S,
                event: I,
                id: l,
                retry: f
              }), x.length && (yield S);
            }
          }
        } finally {
          g.removeEventListener("abort", O), C.releaseLock();
        }
        break;
      } catch (E) {
        if (t == null || t(E), i !== void 0 && p >= i)
          break;
        const j = Math.min(f * 2 ** (p - 1), s ?? 3e4);
        await w(j);
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
}, ie = (r) => {
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
}, ne = (r) => {
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
}, R = ({
  allowReserved: r,
  explode: t,
  name: e,
  style: a,
  value: o
}) => {
  if (!t) {
    const s = (r ? o : o.map((n) => encodeURIComponent(n))).join(ie(a));
    switch (a) {
      case "label":
        return `.${s}`;
      case "matrix":
        return `;${e}=${s}`;
      case "simple":
        return s;
      default:
        return `${e}=${s}`;
    }
  }
  const c = se(a), i = o.map((s) => a === "label" || a === "simple" ? r ? s : encodeURIComponent(s) : k({
    allowReserved: r,
    name: e,
    value: s
  })).join(c);
  return a === "label" || a === "matrix" ? c + i : i;
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
}, D = ({
  allowReserved: r,
  explode: t,
  name: e,
  style: a,
  value: o,
  valueOnly: c
}) => {
  if (o instanceof Date)
    return c ? o.toISOString() : `${e}=${o.toISOString()}`;
  if (a !== "deepObject" && !t) {
    let n = [];
    Object.entries(o).forEach(([d, l]) => {
      n = [...n, d, r ? l : encodeURIComponent(l)];
    });
    const b = n.join(",");
    switch (a) {
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
  const i = ne(a), s = Object.entries(o).map(
    ([n, b]) => k({
      allowReserved: r,
      name: a === "deepObject" ? `${e}[${n}]` : n,
      value: b
    })
  ).join(i);
  return a === "label" || a === "matrix" ? i + s : s;
}, oe = /\{[^{}]+\}/g, ce = ({ path: r, url: t }) => {
  let e = t;
  const a = t.match(oe);
  if (a)
    for (const o of a) {
      let c = !1, i = o.substring(1, o.length - 1), s = "simple";
      i.endsWith("*") && (c = !0, i = i.substring(0, i.length - 1)), i.startsWith(".") ? (i = i.substring(1), s = "label") : i.startsWith(";") && (i = i.substring(1), s = "matrix");
      const n = r[i];
      if (n == null)
        continue;
      if (Array.isArray(n)) {
        e = e.replace(o, R({ explode: c, name: i, style: s, value: n }));
        continue;
      }
      if (typeof n == "object") {
        e = e.replace(
          o,
          D({
            explode: c,
            name: i,
            style: s,
            value: n,
            valueOnly: !0
          })
        );
        continue;
      }
      if (s === "matrix") {
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
        s === "label" ? `.${n}` : n
      );
      e = e.replace(o, b);
    }
  return e;
}, le = ({
  baseUrl: r,
  path: t,
  query: e,
  querySerializer: a,
  url: o
}) => {
  const c = o.startsWith("/") ? o : `/${o}`;
  let i = (r ?? "") + c;
  t && (i = ce({ path: t, url: i }));
  let s = e ? a(e) : "";
  return s.startsWith("?") && (s = s.substring(1)), s && (i += `?${s}`), i;
};
function $(r) {
  const t = r.body !== void 0;
  if (t && r.bodySerializer)
    return "serializedBody" in r ? r.serializedBody !== void 0 && r.serializedBody !== "" ? r.serializedBody : null : r.body !== "" ? r.body : null;
  if (t)
    return r.body;
}
const de = async (r, t) => {
  const e = typeof t == "function" ? await t(r) : t;
  if (e)
    return r.scheme === "bearer" ? `Bearer ${e}` : r.scheme === "basic" ? `Basic ${btoa(e)}` : e;
}, F = ({
  parameters: r = {},
  ...t
} = {}) => (a) => {
  const o = [];
  if (a && typeof a == "object")
    for (const c in a) {
      const i = a[c];
      if (i == null)
        continue;
      const s = r[c] || t;
      if (Array.isArray(i)) {
        const n = R({
          allowReserved: s.allowReserved,
          explode: !0,
          name: c,
          style: "form",
          value: i,
          ...s.array
        });
        n && o.push(n);
      } else if (typeof i == "object") {
        const n = D({
          allowReserved: s.allowReserved,
          explode: !0,
          name: c,
          style: "deepObject",
          value: i,
          ...s.object
        });
        n && o.push(n);
      } else {
        const n = k({
          allowReserved: s.allowReserved,
          name: c,
          value: i
        });
        n && o.push(n);
      }
    }
  return o.join("&");
}, fe = (r) => {
  var e;
  if (!r)
    return "stream";
  const t = (e = r.split(";")[0]) == null ? void 0 : e.trim();
  if (t) {
    if (t.startsWith("application/json") || t.endsWith("+json"))
      return "json";
    if (t === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some((a) => t.startsWith(a)))
      return "blob";
    if (t.startsWith("text/"))
      return "text";
  }
}, ue = (r, t) => {
  var e, a;
  return t ? !!(r.headers.has(t) || (e = r.query) != null && e[t] || (a = r.headers.get("Cookie")) != null && a.includes(`${t}=`)) : !1;
};
async function pe(r) {
  for (const t of r.security ?? []) {
    if (ue(r, t.name))
      continue;
    const e = await de(t, r.auth);
    if (!e)
      continue;
    const a = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        r.query || (r.query = {}), r.query[a] = e;
        break;
      case "cookie":
        r.headers.append("Cookie", `${a}=${e}`);
        break;
      case "header":
      default:
        r.headers.set(a, e);
        break;
    }
  }
}
const B = (r) => le({
  baseUrl: r.baseUrl,
  path: r.path,
  query: r.query,
  querySerializer: typeof r.querySerializer == "function" ? r.querySerializer : F(r.querySerializer),
  url: r.url
}), N = (r, t) => {
  var a;
  const e = { ...r, ...t };
  return (a = e.baseUrl) != null && a.endsWith("/") && (e.baseUrl = e.baseUrl.substring(0, e.baseUrl.length - 1)), e.headers = H(r.headers, t.headers), e;
}, he = (r) => {
  const t = [];
  return r.forEach((e, a) => {
    t.push([a, e]);
  }), t;
}, H = (...r) => {
  const t = new Headers();
  for (const e of r) {
    if (!e)
      continue;
    const a = e instanceof Headers ? he(e) : Object.entries(e);
    for (const [o, c] of a)
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
class U {
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
    const a = this.getInterceptorIndex(t);
    return this.fns[a] ? (this.fns[a] = e, t) : !1;
  }
  use(t) {
    return this.fns.push(t), this.fns.length - 1;
  }
}
const ye = () => ({
  error: new U(),
  request: new U(),
  response: new U()
}), me = F({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), be = {
  "Content-Type": "application/json"
}, W = (r = {}) => ({
  ...re,
  headers: be,
  parseAs: "auto",
  querySerializer: me,
  ...r
}), ge = (r = {}) => {
  let t = N(W(), r);
  const e = () => ({ ...t }), a = (d) => (t = N(t, d), e()), o = ye(), c = async (d) => {
    const l = {
      ...t,
      ...d,
      fetch: d.fetch ?? t.fetch ?? globalThis.fetch,
      headers: H(t.headers, d.headers),
      serializedBody: void 0
    };
    l.security && await pe(l), l.requestValidator && await l.requestValidator(l), l.body !== void 0 && l.bodySerializer && (l.serializedBody = l.bodySerializer(l.body)), (l.body === void 0 || l.serializedBody === "") && l.headers.delete("Content-Type");
    const w = l, m = B(w);
    return { opts: w, url: m };
  }, i = async (d) => {
    const l = d.throwOnError ?? t.throwOnError, w = d.responseStyle ?? t.responseStyle;
    let m, u;
    try {
      const { opts: f, url: p } = await c(d), g = {
        redirect: "follow",
        ...f,
        body: $(f)
      };
      m = new Request(p, g);
      for (const h of o.request.fns)
        h && (m = await h(m, f));
      const v = f.fetch;
      u = await v(m);
      for (const h of o.response.fns)
        h && (u = await h(u, m, f));
      const E = {
        request: m,
        response: u
      };
      if (u.ok) {
        const h = (f.parseAs === "auto" ? fe(u.headers.get("Content-Type")) : f.parseAs) ?? "json";
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
            ...E
          };
        }
        let C;
        switch (h) {
          case "arrayBuffer":
          case "blob":
          case "formData":
          case "text":
            C = await u[h]();
            break;
          case "json": {
            const y = await u.text();
            C = y ? JSON.parse(y) : {};
            break;
          }
          case "stream":
            return f.responseStyle === "data" ? u.body : {
              data: u.body,
              ...E
            };
        }
        return h === "json" && (f.responseValidator && await f.responseValidator(C), f.responseTransformer && (C = await f.responseTransformer(C))), f.responseStyle === "data" ? C : {
          data: C,
          ...E
        };
      }
      const j = await u.text();
      let z;
      try {
        z = JSON.parse(j);
      } catch {
      }
      throw z ?? j;
    } catch (f) {
      let p = f;
      for (const g of o.error.fns)
        g && (p = await g(p, u, m, d));
      if (p = p || {}, l)
        throw p;
      return w === "data" ? void 0 : {
        error: p,
        request: m,
        response: u
      };
    }
  }, s = (d) => (l) => i({ ...l, method: d }), n = (d) => async (l) => {
    const { opts: w, url: m } = await c(l);
    return ae({
      ...w,
      body: w.body,
      method: d,
      onRequest: async (u, f) => {
        let p = new Request(u, f);
        for (const g of o.request.fns)
          g && (p = await g(p, w));
        return p;
      },
      serializedBody: $(w),
      url: m
    });
  };
  return {
    buildUrl: (d) => B({ ...t, ...d }),
    connect: s("CONNECT"),
    delete: s("DELETE"),
    get: s("GET"),
    getConfig: e,
    head: s("HEAD"),
    interceptors: o,
    options: s("OPTIONS"),
    patch: s("PATCH"),
    post: s("POST"),
    put: s("PUT"),
    request: i,
    setConfig: a,
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
    trace: s("TRACE")
  };
}, we = ge(W({ baseUrl: "https://localhost:44370/", throwOnError: !0 })), Ae = (r, t) => {
  t.registerMany([
    ...Y,
    ee,
    te
  ]), r.consumeContext(J, async (e) => {
    const a = e == null ? void 0 : e.getOpenApiConfiguration();
    we.setConfig({
      baseUrl: (a == null ? void 0 : a.base) ?? "",
      auth: (a == null ? void 0 : a.token) ?? void 0,
      credentials: (a == null ? void 0 : a.credentials) ?? "same-origin"
    });
  });
};
export {
  we as c,
  Ae as o
};
//# sourceMappingURL=index-Cn40Y89p.js.map
