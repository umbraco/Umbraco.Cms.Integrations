import { UMB_AUTH_CONTEXT as F } from "@umbraco-cms/backoffice/auth";
const G = {
  bodySerializer: (r) => JSON.stringify(r, (t, e) => typeof e == "bigint" ? e.toString() : e)
};
function Q({
  onRequest: r,
  onSseError: t,
  onSseEvent: e,
  responseTransformer: s,
  responseValidator: i,
  sseDefaultRetryDelay: c,
  sseMaxRetryAttempts: n,
  sseMaxRetryDelay: a,
  sseSleepFn: o,
  url: m,
  ...d
}) {
  let l;
  const g = o ?? ((f) => new Promise((h) => setTimeout(h, f)));
  return { stream: async function* () {
    let f = c ?? 3e3, h = 0;
    const w = d.signal ?? new AbortController().signal;
    for (; !w.aborted; ) {
      h++;
      const C = d.headers instanceof Headers ? d.headers : new Headers(d.headers);
      l !== void 0 && C.set("Last-Event-ID", l);
      try {
        const x = {
          redirect: "follow",
          ...d,
          body: d.serializedBody,
          headers: C,
          signal: w
        };
        let z = new Request(m, x);
        r && (z = await r(m, x));
        const p = await (d.fetch ?? globalThis.fetch)(z);
        if (!p.ok) throw new Error(`SSE failed: ${p.status} ${p.statusText}`);
        if (!p.body) throw new Error("No body in SSE response");
        const S = p.body.pipeThrough(new TextDecoderStream()).getReader();
        let y = "";
        const q = () => {
          try {
            S.cancel();
          } catch {
          }
        };
        w.addEventListener("abort", q);
        try {
          for (; ; ) {
            const { done: L, value: _ } = await S.read();
            if (L) break;
            y += _, y = y.replace(/\r\n?/g, `
`);
            const $ = y.split(`

`);
            y = $.pop() ?? "";
            for (const M of $) {
              const J = M.split(`
`), O = [];
              let U;
              for (const j of J)
                if (j.startsWith("data:"))
                  O.push(j.replace(/^data:\s*/, ""));
                else if (j.startsWith("event:"))
                  U = j.replace(/^event:\s*/, "");
                else if (j.startsWith("id:"))
                  l = j.replace(/^id:\s*/, "");
                else if (j.startsWith("retry:")) {
                  const v = Number.parseInt(j.replace(/^retry:\s*/, ""), 10);
                  Number.isNaN(v) || (f = v);
                }
              let k, I = !1;
              if (O.length) {
                const j = O.join(`
`);
                try {
                  k = JSON.parse(j), I = !0;
                } catch {
                  k = j;
                }
              }
              I && (i && await i(k), s && (k = await s(k))), e == null || e({
                data: k,
                event: U,
                id: l,
                retry: f
              }), O.length && (yield k);
            }
          }
        } finally {
          w.removeEventListener("abort", q), S.releaseLock();
        }
        break;
      } catch (x) {
        if (t == null || t(x), n !== void 0 && h >= n)
          break;
        const z = Math.min(f * 2 ** (h - 1), a ?? 3e4);
        await g(z);
      }
    }
  }() };
}
const X = (r) => {
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
}, K = (r) => {
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
}, Y = (r) => {
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
  value: i
}) => {
  if (!t) {
    const a = (r ? i : i.map((o) => encodeURIComponent(o))).join(K(s));
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
  const c = X(s), n = i.map((a) => s === "label" || s === "simple" ? r ? a : encodeURIComponent(a) : E({
    allowReserved: r,
    name: e,
    value: a
  })).join(c);
  return s === "label" || s === "matrix" ? c + n : n;
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
}, W = ({
  allowReserved: r,
  explode: t,
  name: e,
  style: s,
  value: i,
  valueOnly: c
}) => {
  if (i instanceof Date)
    return c ? i.toISOString() : `${e}=${i.toISOString()}`;
  if (s !== "deepObject" && !t) {
    let o = [];
    Object.entries(i).forEach(([d, l]) => {
      o = [...o, d, r ? l : encodeURIComponent(l)];
    });
    const m = o.join(",");
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
  const n = Y(s), a = Object.entries(i).map(
    ([o, m]) => E({
      allowReserved: r,
      name: s === "deepObject" ? `${e}[${o}]` : o,
      value: m
    })
  ).join(n);
  return s === "label" || s === "matrix" ? n + a : a;
}, Z = /\{[^{}]+\}/g, ee = ({ path: r, url: t }) => {
  let e = t;
  const s = t.match(Z);
  if (s)
    for (const i of s) {
      let c = !1, n = i.substring(1, i.length - 1), a = "simple";
      n.endsWith("*") && (c = !0, n = n.substring(0, n.length - 1)), n.startsWith(".") ? (n = n.substring(1), a = "label") : n.startsWith(";") && (n = n.substring(1), a = "matrix");
      const o = r[n];
      if (o == null)
        continue;
      if (Array.isArray(o)) {
        e = e.replace(i, R({ explode: c, name: n, style: a, value: o }));
        continue;
      }
      if (typeof o == "object") {
        e = e.replace(
          i,
          W({
            explode: c,
            name: n,
            style: a,
            value: o,
            valueOnly: !0
          })
        );
        continue;
      }
      if (a === "matrix") {
        e = e.replace(
          i,
          `;${E({
            name: n,
            value: o
          })}`
        );
        continue;
      }
      const m = encodeURIComponent(
        a === "label" ? `.${o}` : o
      );
      e = e.replace(i, m);
    }
  return e;
}, te = ({
  baseUrl: r,
  path: t,
  query: e,
  querySerializer: s,
  url: i
}) => {
  const c = i.startsWith("/") ? i : `/${i}`;
  let n = (r ?? "") + c;
  t && (n = ee({ path: t, url: n }));
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
const re = async (r, t) => {
  const e = typeof t == "function" ? await t(r) : t;
  if (e)
    return r.scheme === "bearer" ? `Bearer ${e}` : r.scheme === "basic" ? `Basic ${btoa(e)}` : e;
}, P = ({
  parameters: r = {},
  ...t
} = {}) => (s) => {
  const i = [];
  if (s && typeof s == "object")
    for (const c in s) {
      const n = s[c];
      if (n == null)
        continue;
      const a = r[c] || t;
      if (Array.isArray(n)) {
        const o = R({
          allowReserved: a.allowReserved,
          explode: !0,
          name: c,
          style: "form",
          value: n,
          ...a.array
        });
        o && i.push(o);
      } else if (typeof n == "object") {
        const o = W({
          allowReserved: a.allowReserved,
          explode: !0,
          name: c,
          style: "deepObject",
          value: n,
          ...a.object
        });
        o && i.push(o);
      } else {
        const o = E({
          allowReserved: a.allowReserved,
          name: c,
          value: n
        });
        o && i.push(o);
      }
    }
  return i.join("&");
}, se = (r) => {
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
}, ae = (r, t) => {
  var e, s;
  return t ? !!(r.headers.has(t) || (e = r.query) != null && e[t] || (s = r.headers.get("Cookie")) != null && s.includes(`${t}=`)) : !1;
};
async function ne(r) {
  for (const t of r.security ?? []) {
    if (ae(r, t.name))
      continue;
    const e = await re(t, r.auth);
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
const N = (r) => te({
  baseUrl: r.baseUrl,
  path: r.path,
  query: r.query,
  querySerializer: typeof r.querySerializer == "function" ? r.querySerializer : P(r.querySerializer),
  url: r.url
}), D = (r, t) => {
  var s;
  const e = { ...r, ...t };
  return (s = e.baseUrl) != null && s.endsWith("/") && (e.baseUrl = e.baseUrl.substring(0, e.baseUrl.length - 1)), e.headers = H(r.headers, t.headers), e;
}, oe = (r) => {
  const t = [];
  return r.forEach((e, s) => {
    t.push([s, e]);
  }), t;
}, H = (...r) => {
  const t = new Headers();
  for (const e of r) {
    if (!e)
      continue;
    const s = e instanceof Headers ? oe(e) : Object.entries(e);
    for (const [i, c] of s)
      if (c === null)
        t.delete(i);
      else if (Array.isArray(c))
        for (const n of c)
          t.append(i, n);
      else c !== void 0 && t.set(
        i,
        typeof c == "object" ? JSON.stringify(c) : c
      );
  }
  return t;
};
class T {
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
const ie = () => ({
  error: new T(),
  request: new T(),
  response: new T()
}), ce = P({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), le = {
  "Content-Type": "application/json"
}, V = (r = {}) => ({
  ...G,
  headers: le,
  parseAs: "auto",
  querySerializer: ce,
  ...r
}), de = (r = {}) => {
  let t = D(V(), r);
  const e = () => ({ ...t }), s = (d) => (t = D(t, d), e()), i = ie(), c = async (d) => {
    const l = {
      ...t,
      ...d,
      fetch: d.fetch ?? t.fetch ?? globalThis.fetch,
      headers: H(t.headers, d.headers),
      serializedBody: void 0
    };
    l.security && await ne(l), l.requestValidator && await l.requestValidator(l), l.body !== void 0 && l.bodySerializer && (l.serializedBody = l.bodySerializer(l.body)), (l.body === void 0 || l.serializedBody === "") && l.headers.delete("Content-Type");
    const g = l, b = N(g);
    return { opts: g, url: b };
  }, n = async (d) => {
    const l = d.throwOnError ?? t.throwOnError, g = d.responseStyle ?? t.responseStyle;
    let b, u;
    try {
      const { opts: f, url: h } = await c(d), w = {
        redirect: "follow",
        ...f,
        body: B(f)
      };
      b = new Request(h, w);
      for (const p of i.request.fns)
        p && (b = await p(b, f));
      const C = f.fetch;
      u = await C(b);
      for (const p of i.response.fns)
        p && (u = await p(u, b, f));
      const x = {
        request: b,
        response: u
      };
      if (u.ok) {
        const p = (f.parseAs === "auto" ? se(u.headers.get("Content-Type")) : f.parseAs) ?? "json";
        if (u.status === 204 || u.headers.get("Content-Length") === "0") {
          let y;
          switch (p) {
            case "arrayBuffer":
            case "blob":
            case "text":
              y = await u[p]();
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
        switch (p) {
          case "arrayBuffer":
          case "blob":
          case "formData":
          case "text":
            S = await u[p]();
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
        return p === "json" && (f.responseValidator && await f.responseValidator(S), f.responseTransformer && (S = await f.responseTransformer(S))), f.responseStyle === "data" ? S : {
          data: S,
          ...x
        };
      }
      const z = await u.text();
      let A;
      try {
        A = JSON.parse(z);
      } catch {
      }
      throw A ?? z;
    } catch (f) {
      let h = f;
      for (const w of i.error.fns)
        w && (h = await w(h, u, b, d));
      if (h = h || {}, l)
        throw h;
      return g === "data" ? void 0 : {
        error: h,
        request: b,
        response: u
      };
    }
  }, a = (d) => (l) => n({ ...l, method: d }), o = (d) => async (l) => {
    const { opts: g, url: b } = await c(l);
    return Q({
      ...g,
      body: g.body,
      method: d,
      onRequest: async (u, f) => {
        let h = new Request(u, f);
        for (const w of i.request.fns)
          w && (h = await w(h, g));
        return h;
      },
      serializedBody: B(g),
      url: b
    });
  };
  return {
    buildUrl: (d) => N({ ...t, ...d }),
    connect: a("CONNECT"),
    delete: a("DELETE"),
    get: a("GET"),
    getConfig: e,
    head: a("HEAD"),
    interceptors: i,
    options: a("OPTIONS"),
    patch: a("PATCH"),
    post: a("POST"),
    put: a("PUT"),
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
    trace: a("TRACE")
  };
}, fe = de(V({ baseUrl: "https://localhost:44370/", throwOnError: !0 })), ue = {
  type: "globalContext",
  alias: "semrush.context",
  name: "Semrush Context",
  js: () => import("./semrush.context-DrMVUcuj.js")
}, he = ue, pe = [
  {
    type: "workspaceView",
    alias: "Umb.WorkspaceView.Semrush.View",
    name: "Umbraco Integration Workspace for Semrush",
    element: () => import("./semrush-workspace.element-B4daNJm0.js"),
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
], ye = [...pe], be = {
  type: "modal",
  alias: "Semrush.Modal",
  name: "Semrush Modal",
  js: () => import("./semrush-modal.element-ByHcAy0V.js")
}, we = (r, t) => {
  t.registerMany([
    he,
    be,
    ...ye
  ]), r.consumeContext(F, async (e) => {
    const s = e == null ? void 0 : e.getOpenApiConfiguration();
    fe.setConfig({
      baseUrl: (s == null ? void 0 : s.base) ?? "",
      auth: (s == null ? void 0 : s.token) ?? void 0,
      credentials: (s == null ? void 0 : s.credentials) ?? "same-origin"
    });
  });
};
export {
  fe as c,
  we as o
};
//# sourceMappingURL=index-DVRm0NsY.js.map
