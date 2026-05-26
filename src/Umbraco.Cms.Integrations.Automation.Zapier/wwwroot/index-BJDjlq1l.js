import { UMB_AUTH_CONTEXT as F } from "@umbraco-cms/backoffice/auth";
const Z = {
  type: "globalContext",
  alias: "zapier.context",
  name: "Zapier Context",
  js: () => import("./zapier.context-X4E0FeNc.js")
}, G = Z, Q = [
  {
    type: "dashboard",
    alias: "Zapier.Management.Dashboard",
    name: "Zapier Management Dashboard",
    element: () => import("./zapier-management-dashboard.element-DZFmtCWl.js"),
    weight: 5,
    meta: {
      label: "Zapier Integrations",
      pathname: "zapier-management"
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content"
      }
    ]
  }
], X = [...Q], K = {
  bodySerializer: (r) => JSON.stringify(r, (t, e) => typeof e == "bigint" ? e.toString() : e)
};
function Y({
  onRequest: r,
  onSseError: t,
  onSseEvent: e,
  responseTransformer: s,
  responseValidator: o,
  sseDefaultRetryDelay: c,
  sseMaxRetryAttempts: n,
  sseMaxRetryDelay: a,
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
      const O = d.headers instanceof Headers ? d.headers : new Headers(d.headers);
      l !== void 0 && O.set("Last-Event-ID", l);
      try {
        const j = {
          redirect: "follow",
          ...d,
          body: d.serializedBody,
          headers: O,
          signal: w
        };
        let z = new Request(m, j);
        r && (z = await r(m, j));
        const p = await (d.fetch ?? globalThis.fetch)(z);
        if (!p.ok) throw new Error(`SSE failed: ${p.status} ${p.statusText}`);
        if (!p.body) throw new Error("No body in SSE response");
        const S = p.body.pipeThrough(new TextDecoderStream()).getReader();
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
            const { done: L, value: _ } = await S.read();
            if (L) break;
            b += _, b = b.replace(/\r\n?/g, `
`);
            const $ = b.split(`

`);
            b = $.pop() ?? "";
            for (const M of $) {
              const J = M.split(`
`), E = [];
              let I;
              for (const x of J)
                if (x.startsWith("data:"))
                  E.push(x.replace(/^data:\s*/, ""));
                else if (x.startsWith("event:"))
                  I = x.replace(/^event:\s*/, "");
                else if (x.startsWith("id:"))
                  l = x.replace(/^id:\s*/, "");
                else if (x.startsWith("retry:")) {
                  const v = Number.parseInt(x.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(v) || (f = v);
                }
              let C, U = !1;
              if (E.length) {
                const x = E.join(`
`);
                try {
                  C = JSON.parse(x), U = !0;
                } catch {
                  C = x;
                }
              }
              U && (o && await o(C), s && (C = await s(C))), e == null || e({
                data: C,
                event: I,
                id: l,
                retry: f
              }), E.length && (yield C);
            }
          }
        } finally {
          w.removeEventListener("abort", k), S.releaseLock();
        }
        break;
      } catch (j) {
        if (t == null || t(j), n !== void 0 && h >= n)
          break;
        const z = Math.min(f * 2 ** (h - 1), a ?? 3e4);
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
  style: s,
  value: o
}) => {
  if (!t) {
    const a = (r ? o : o.map((i) => encodeURIComponent(i))).join(te(s));
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
  const c = ee(s), n = o.map((a) => s === "label" || s === "simple" ? r ? a : encodeURIComponent(a) : A({
    allowReserved: r,
    name: e,
    value: a
  })).join(c);
  return s === "label" || s === "matrix" ? c + n : n;
}, A = ({
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
  style: s,
  value: o,
  valueOnly: c
}) => {
  if (o instanceof Date)
    return c ? o.toISOString() : `${e}=${o.toISOString()}`;
  if (s !== "deepObject" && !t) {
    let i = [];
    Object.entries(o).forEach(([d, l]) => {
      i = [...i, d, r ? l : encodeURIComponent(l)];
    });
    const m = i.join(",");
    switch (s) {
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
  const n = re(s), a = Object.entries(o).map(
    ([i, m]) => A({
      allowReserved: r,
      name: s === "deepObject" ? `${e}[${i}]` : i,
      value: m
    })
  ).join(n);
  return s === "label" || s === "matrix" ? n + a : a;
}, se = /\{[^{}]+\}/g, ae = ({ path: r, url: t }) => {
  let e = t;
  const s = t.match(se);
  if (s)
    for (const o of s) {
      let c = !1, n = o.substring(1, o.length - 1), a = "simple";
      n.endsWith("*") && (c = !0, n = n.substring(0, n.length - 1)), n.startsWith(".") ? (n = n.substring(1), a = "label") : n.startsWith(";") && (n = n.substring(1), a = "matrix");
      const i = r[n];
      if (i == null)
        continue;
      if (Array.isArray(i)) {
        e = e.replace(o, R({ explode: c, name: n, style: a, value: i }));
        continue;
      }
      if (typeof i == "object") {
        e = e.replace(
          o,
          P({
            explode: c,
            name: n,
            style: a,
            value: i,
            valueOnly: !0
          })
        );
        continue;
      }
      if (a === "matrix") {
        e = e.replace(
          o,
          `;${A({
            name: n,
            value: i
          })}`
        );
        continue;
      }
      const m = encodeURIComponent(
        a === "label" ? `.${i}` : i
      );
      e = e.replace(o, m);
    }
  return e;
}, ne = ({
  baseUrl: r,
  path: t,
  query: e,
  querySerializer: s,
  url: o
}) => {
  const c = o.startsWith("/") ? o : `/${o}`;
  let n = (r ?? "") + c;
  t && (n = ae({ path: t, url: n }));
  let a = e ? s(e) : "";
  return a.startsWith("?") && (a = a.substring(1)), a && (n += `?${a}`), n;
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
} = {}) => (s) => {
  const o = [];
  if (s && typeof s == "object")
    for (const c in s) {
      const n = s[c];
      if (n == null)
        continue;
      const a = r[c] || t;
      if (Array.isArray(n)) {
        const i = R({
          allowReserved: a.allowReserved,
          explode: !0,
          name: c,
          style: "form",
          value: n,
          ...a.array
        });
        i && o.push(i);
      } else if (typeof n == "object") {
        const i = P({
          allowReserved: a.allowReserved,
          explode: !0,
          name: c,
          style: "deepObject",
          value: n,
          ...a.object
        });
        i && o.push(i);
      } else {
        const i = A({
          allowReserved: a.allowReserved,
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
    if (["application/", "audio/", "image/", "video/"].some((s) => t.startsWith(s)))
      return "blob";
    if (t.startsWith("text/"))
      return "text";
  }
}, ce = (r, t) => {
  var e, s;
  return t ? !!(r.headers.has(t) || (e = r.query) != null && e[t] || (s = r.headers.get("Cookie")) != null && s.includes(`${t}=`)) : !1;
};
async function le(r) {
  for (const t of r.security ?? []) {
    if (ce(r, t.name))
      continue;
    const e = await ie(t, r.auth);
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
const D = (r) => ne({
  baseUrl: r.baseUrl,
  path: r.path,
  query: r.query,
  querySerializer: typeof r.querySerializer == "function" ? r.querySerializer : H(r.querySerializer),
  url: r.url
}), N = (r, t) => {
  var s;
  const e = { ...r, ...t };
  return (s = e.baseUrl) != null && s.endsWith("/") && (e.baseUrl = e.baseUrl.substring(0, e.baseUrl.length - 1)), e.headers = W(r.headers, t.headers), e;
}, de = (r) => {
  const t = [];
  return r.forEach((e, s) => {
    t.push([s, e]);
  }), t;
}, W = (...r) => {
  const t = new Headers();
  for (const e of r) {
    if (!e)
      continue;
    const s = e instanceof Headers ? de(e) : Object.entries(e);
    for (const [o, c] of s)
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
    const s = this.getInterceptorIndex(t);
    return this.fns[s] ? (this.fns[s] = e, t) : !1;
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
  ...K,
  headers: he,
  parseAs: "auto",
  querySerializer: ue,
  ...r
}), pe = (r = {}) => {
  let t = N(V(), r);
  const e = () => ({ ...t }), s = (d) => (t = N(t, d), e()), o = fe(), c = async (d) => {
    const l = {
      ...t,
      ...d,
      fetch: d.fetch ?? t.fetch ?? globalThis.fetch,
      headers: W(t.headers, d.headers),
      serializedBody: void 0
    };
    l.security && await le(l), l.requestValidator && await l.requestValidator(l), l.body !== void 0 && l.bodySerializer && (l.serializedBody = l.bodySerializer(l.body)), (l.body === void 0 || l.serializedBody === "") && l.headers.delete("Content-Type");
    const g = l, y = D(g);
    return { opts: g, url: y };
  }, n = async (d) => {
    const l = d.throwOnError ?? t.throwOnError, g = d.responseStyle ?? t.responseStyle;
    let y, u;
    try {
      const { opts: f, url: h } = await c(d), w = {
        redirect: "follow",
        ...f,
        body: B(f)
      };
      y = new Request(h, w);
      for (const p of o.request.fns)
        p && (y = await p(y, f));
      const O = f.fetch;
      u = await O(y);
      for (const p of o.response.fns)
        p && (u = await p(u, y, f));
      const j = {
        request: y,
        response: u
      };
      if (u.ok) {
        const p = (f.parseAs === "auto" ? oe(u.headers.get("Content-Type")) : f.parseAs) ?? "json";
        if (u.status === 204 || u.headers.get("Content-Length") === "0") {
          let b;
          switch (p) {
            case "arrayBuffer":
            case "blob":
            case "text":
              b = await u[p]();
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
        switch (p) {
          case "arrayBuffer":
          case "blob":
          case "formData":
          case "text":
            S = await u[p]();
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
        return p === "json" && (f.responseValidator && await f.responseValidator(S), f.responseTransformer && (S = await f.responseTransformer(S))), f.responseStyle === "data" ? S : {
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
        w && (h = await w(h, u, y, d));
      if (h = h || {}, l)
        throw h;
      return g === "data" ? void 0 : {
        error: h,
        request: y,
        response: u
      };
    }
  }, a = (d) => (l) => n({ ...l, method: d }), i = (d) => async (l) => {
    const { opts: g, url: y } = await c(l);
    return Y({
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
      url: y
    });
  };
  return {
    buildUrl: (d) => D({ ...t, ...d }),
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
    request: n,
    setConfig: s,
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
    trace: a("TRACE")
  };
}, be = pe(V({ baseUrl: "https://localhost:44370/", throwOnError: !0 })), me = (r, t) => {
  t.registerMany([
    G,
    ...X
  ]), r.consumeContext(F, async (e) => {
    const s = e == null ? void 0 : e.getOpenApiConfiguration();
    be.setConfig({
      baseUrl: (s == null ? void 0 : s.base) ?? "",
      auth: (s == null ? void 0 : s.token) ?? void 0,
      credentials: (s == null ? void 0 : s.credentials) ?? "same-origin"
    });
  });
};
export {
  be as c,
  me as o
};
//# sourceMappingURL=index-BJDjlq1l.js.map
