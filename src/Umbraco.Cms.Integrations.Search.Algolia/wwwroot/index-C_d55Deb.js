import { UMB_AUTH_CONTEXT as F } from "@umbraco-cms/backoffice/auth";
const G = {
  type: "dashboard",
  alias: "Algolia.Dashboard",
  name: "Algolia Search Management",
  element: () => import("./algolia-dashboard.element-CKFConJB.js"),
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
}, Q = G, X = {
  type: "globalContext",
  alias: "algolia.context",
  name: "Algolia Context",
  js: () => import("./algolia-index.context-B5xhfvsN.js")
}, K = X, Y = {
  bodySerializer: (r) => JSON.stringify(r, (t, e) => typeof e == "bigint" ? e.toString() : e)
};
function Z({
  onRequest: r,
  onSseError: t,
  onSseEvent: e,
  responseTransformer: a,
  responseValidator: o,
  sseDefaultRetryDelay: c,
  sseMaxRetryAttempts: n,
  sseMaxRetryDelay: s,
  sseSleepFn: i,
  url: m,
  ...d
}) {
  let l;
  const g = i ?? ((f) => new Promise((h) => setTimeout(h, f)));
  return { stream: async function* () {
    let f = c ?? 3e3, h = 0;
    const w = d.signal ?? new AbortController().signal;
    for (; !w.aborted; ) {
      h++;
      const C = d.headers instanceof Headers ? d.headers : new Headers(d.headers);
      l !== void 0 && C.set("Last-Event-ID", l);
      try {
        const j = {
          redirect: "follow",
          ...d,
          body: d.serializedBody,
          headers: C,
          signal: w
        };
        let z = new Request(m, j);
        r && (z = await r(m, j));
        const y = await (d.fetch ?? globalThis.fetch)(z);
        if (!y.ok) throw new Error(`SSE failed: ${y.status} ${y.statusText}`);
        if (!y.body) throw new Error("No body in SSE response");
        const S = y.body.pipeThrough(new TextDecoderStream()).getReader();
        let b = "";
        const k = () => {
          try {
            S.cancel();
          } catch {
          }
        };
        w.addEventListener("abort", k);
        try {
          for (; ; ) {
            const { done: L, value: M } = await S.read();
            if (L) break;
            b += M, b = b.replace(/\r\n?/g, `
`);
            const $ = b.split(`

`);
            b = $.pop() ?? "";
            for (const _ of $) {
              const J = _.split(`
`), O = [];
              let I;
              for (const x of J)
                if (x.startsWith("data:"))
                  O.push(x.replace(/^data:\s*/, ""));
                else if (x.startsWith("event:"))
                  I = x.replace(/^event:\s*/, "");
                else if (x.startsWith("id:"))
                  l = x.replace(/^id:\s*/, "");
                else if (x.startsWith("retry:")) {
                  const v = Number.parseInt(x.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(v) || (f = v);
                }
              let A, U = !1;
              if (O.length) {
                const x = O.join(`
`);
                try {
                  A = JSON.parse(x), U = !0;
                } catch {
                  A = x;
                }
              }
              U && (o && await o(A), a && (A = await a(A))), e == null || e({
                data: A,
                event: I,
                id: l,
                retry: f
              }), O.length && (yield A);
            }
          }
        } finally {
          w.removeEventListener("abort", k), S.releaseLock();
        }
        break;
      } catch (j) {
        if (t == null || t(j), n !== void 0 && h >= n)
          break;
        const z = Math.min(f * 2 ** (h - 1), s ?? 3e4);
        await g(z);
      }
    }
  }() };
}
const ee = (r) => {
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
}, te = (r) => {
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
}, re = (r) => {
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
    const s = (r ? o : o.map((i) => encodeURIComponent(i))).join(te(a));
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
  const c = ee(a), n = o.map((s) => a === "label" || a === "simple" ? r ? s : encodeURIComponent(s) : E({
    allowReserved: r,
    name: e,
    value: s
  })).join(c);
  return a === "label" || a === "matrix" ? c + n : n;
}, E = ({
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
}, P = ({
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
    let i = [];
    Object.entries(o).forEach(([d, l]) => {
      i = [...i, d, r ? l : encodeURIComponent(l)];
    });
    const m = i.join(",");
    switch (a) {
      case "form":
        return `${e}=${m}`;
      case "label":
        return `.${m}`;
      case "matrix":
        return `;${e}=${m}`;
      default:
        return m;
    }
  }
  const n = re(a), s = Object.entries(o).map(
    ([i, m]) => E({
      allowReserved: r,
      name: a === "deepObject" ? `${e}[${i}]` : i,
      value: m
    })
  ).join(n);
  return a === "label" || a === "matrix" ? n + s : s;
}, ae = /\{[^{}]+\}/g, se = ({ path: r, url: t }) => {
  let e = t;
  const a = t.match(ae);
  if (a)
    for (const o of a) {
      let c = !1, n = o.substring(1, o.length - 1), s = "simple";
      n.endsWith("*") && (c = !0, n = n.substring(0, n.length - 1)), n.startsWith(".") ? (n = n.substring(1), s = "label") : n.startsWith(";") && (n = n.substring(1), s = "matrix");
      const i = r[n];
      if (i == null)
        continue;
      if (Array.isArray(i)) {
        e = e.replace(o, R({ explode: c, name: n, style: s, value: i }));
        continue;
      }
      if (typeof i == "object") {
        e = e.replace(
          o,
          P({
            explode: c,
            name: n,
            style: s,
            value: i,
            valueOnly: !0
          })
        );
        continue;
      }
      if (s === "matrix") {
        e = e.replace(
          o,
          `;${E({
            name: n,
            value: i
          })}`
        );
        continue;
      }
      const m = encodeURIComponent(
        s === "label" ? `.${i}` : i
      );
      e = e.replace(o, m);
    }
  return e;
}, ne = ({
  baseUrl: r,
  path: t,
  query: e,
  querySerializer: a,
  url: o
}) => {
  const c = o.startsWith("/") ? o : `/${o}`;
  let n = (r ?? "") + c;
  t && (n = se({ path: t, url: n }));
  let s = e ? a(e) : "";
  return s.startsWith("?") && (s = s.substring(1)), s && (n += `?${s}`), n;
};
function B(r) {
  const t = r.body !== void 0;
  if (t && r.bodySerializer)
    return "serializedBody" in r ? r.serializedBody !== void 0 && r.serializedBody !== "" ? r.serializedBody : null : r.body !== "" ? r.body : null;
  if (t)
    return r.body;
}
const ie = async (r, t) => {
  const e = typeof t == "function" ? await t(r) : t;
  if (e)
    return r.scheme === "bearer" ? `Bearer ${e}` : r.scheme === "basic" ? `Basic ${btoa(e)}` : e;
}, H = ({
  parameters: r = {},
  ...t
} = {}) => (a) => {
  const o = [];
  if (a && typeof a == "object")
    for (const c in a) {
      const n = a[c];
      if (n == null)
        continue;
      const s = r[c] || t;
      if (Array.isArray(n)) {
        const i = R({
          allowReserved: s.allowReserved,
          explode: !0,
          name: c,
          style: "form",
          value: n,
          ...s.array
        });
        i && o.push(i);
      } else if (typeof n == "object") {
        const i = P({
          allowReserved: s.allowReserved,
          explode: !0,
          name: c,
          style: "deepObject",
          value: n,
          ...s.object
        });
        i && o.push(i);
      } else {
        const i = E({
          allowReserved: s.allowReserved,
          name: c,
          value: n
        });
        i && o.push(i);
      }
    }
  return o.join("&");
}, oe = (r) => {
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
}, ce = (r, t) => {
  var e, a;
  return t ? !!(r.headers.has(t) || (e = r.query) != null && e[t] || (a = r.headers.get("Cookie")) != null && a.includes(`${t}=`)) : !1;
};
async function le(r) {
  for (const t of r.security ?? []) {
    if (ce(r, t.name))
      continue;
    const e = await ie(t, r.auth);
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
const N = (r) => ne({
  baseUrl: r.baseUrl,
  path: r.path,
  query: r.query,
  querySerializer: typeof r.querySerializer == "function" ? r.querySerializer : H(r.querySerializer),
  url: r.url
}), D = (r, t) => {
  var a;
  const e = { ...r, ...t };
  return (a = e.baseUrl) != null && a.endsWith("/") && (e.baseUrl = e.baseUrl.substring(0, e.baseUrl.length - 1)), e.headers = W(r.headers, t.headers), e;
}, de = (r) => {
  const t = [];
  return r.forEach((e, a) => {
    t.push([a, e]);
  }), t;
}, W = (...r) => {
  const t = new Headers();
  for (const e of r) {
    if (!e)
      continue;
    const a = e instanceof Headers ? de(e) : Object.entries(e);
    for (const [o, c] of a)
      if (c === null)
        t.delete(o);
      else if (Array.isArray(c))
        for (const n of c)
          t.append(o, n);
      else c !== void 0 && t.set(
        o,
        typeof c == "object" ? JSON.stringify(c) : c
      );
  }
  return t;
};
class q {
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
const fe = () => ({
  error: new q(),
  request: new q(),
  response: new q()
}), ue = H({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), he = {
  "Content-Type": "application/json"
}, V = (r = {}) => ({
  ...Y,
  headers: he,
  parseAs: "auto",
  querySerializer: ue,
  ...r
}), ye = (r = {}) => {
  let t = D(V(), r);
  const e = () => ({ ...t }), a = (d) => (t = D(t, d), e()), o = fe(), c = async (d) => {
    const l = {
      ...t,
      ...d,
      fetch: d.fetch ?? t.fetch ?? globalThis.fetch,
      headers: W(t.headers, d.headers),
      serializedBody: void 0
    };
    l.security && await le(l), l.requestValidator && await l.requestValidator(l), l.body !== void 0 && l.bodySerializer && (l.serializedBody = l.bodySerializer(l.body)), (l.body === void 0 || l.serializedBody === "") && l.headers.delete("Content-Type");
    const g = l, p = N(g);
    return { opts: g, url: p };
  }, n = async (d) => {
    const l = d.throwOnError ?? t.throwOnError, g = d.responseStyle ?? t.responseStyle;
    let p, u;
    try {
      const { opts: f, url: h } = await c(d), w = {
        redirect: "follow",
        ...f,
        body: B(f)
      };
      p = new Request(h, w);
      for (const y of o.request.fns)
        y && (p = await y(p, f));
      const C = f.fetch;
      u = await C(p);
      for (const y of o.response.fns)
        y && (u = await y(u, p, f));
      const j = {
        request: p,
        response: u
      };
      if (u.ok) {
        const y = (f.parseAs === "auto" ? oe(u.headers.get("Content-Type")) : f.parseAs) ?? "json";
        if (u.status === 204 || u.headers.get("Content-Length") === "0") {
          let b;
          switch (y) {
            case "arrayBuffer":
            case "blob":
            case "text":
              b = await u[y]();
              break;
            case "formData":
              b = new FormData();
              break;
            case "stream":
              b = u.body;
              break;
            case "json":
            default:
              b = {};
              break;
          }
          return f.responseStyle === "data" ? b : {
            data: b,
            ...j
          };
        }
        let S;
        switch (y) {
          case "arrayBuffer":
          case "blob":
          case "formData":
          case "text":
            S = await u[y]();
            break;
          case "json": {
            const b = await u.text();
            S = b ? JSON.parse(b) : {};
            break;
          }
          case "stream":
            return f.responseStyle === "data" ? u.body : {
              data: u.body,
              ...j
            };
        }
        return y === "json" && (f.responseValidator && await f.responseValidator(S), f.responseTransformer && (S = await f.responseTransformer(S))), f.responseStyle === "data" ? S : {
          data: S,
          ...j
        };
      }
      const z = await u.text();
      let T;
      try {
        T = JSON.parse(z);
      } catch {
      }
      throw T ?? z;
    } catch (f) {
      let h = f;
      for (const w of o.error.fns)
        w && (h = await w(h, u, p, d));
      if (h = h || {}, l)
        throw h;
      return g === "data" ? void 0 : {
        error: h,
        request: p,
        response: u
      };
    }
  }, s = (d) => (l) => n({ ...l, method: d }), i = (d) => async (l) => {
    const { opts: g, url: p } = await c(l);
    return Z({
      ...g,
      body: g.body,
      method: d,
      onRequest: async (u, f) => {
        let h = new Request(u, f);
        for (const w of o.request.fns)
          w && (h = await w(h, g));
        return h;
      },
      serializedBody: B(g),
      url: p
    });
  };
  return {
    buildUrl: (d) => N({ ...t, ...d }),
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
    request: n,
    setConfig: a,
    sse: {
      connect: i("CONNECT"),
      delete: i("DELETE"),
      get: i("GET"),
      head: i("HEAD"),
      options: i("OPTIONS"),
      patch: i("PATCH"),
      post: i("POST"),
      put: i("PUT"),
      trace: i("TRACE")
    },
    trace: s("TRACE")
  };
}, be = ye(V({ baseUrl: "https://localhost:44370/", throwOnError: !0 })), me = (r, t) => {
  t.registerMany([Q, K]), r.consumeContext(F, async (e) => {
    const a = e == null ? void 0 : e.getOpenApiConfiguration();
    be.setConfig({
      baseUrl: (a == null ? void 0 : a.base) ?? "",
      auth: (a == null ? void 0 : a.token) ?? void 0,
      credentials: (a == null ? void 0 : a.credentials) ?? "same-origin"
    });
  });
};
export {
  be as c,
  me as o
};
//# sourceMappingURL=index-C_d55Deb.js.map
