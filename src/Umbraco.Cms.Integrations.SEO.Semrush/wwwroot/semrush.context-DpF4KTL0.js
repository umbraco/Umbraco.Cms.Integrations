var b = (r) => {
  throw TypeError(r);
};
var T = (r, e, t) => e.has(r) || b("Cannot " + t);
var n = (r, e, t) => (T(r, e, "read from private field"), t ? t.call(r) : e.get(r)), y = (r, e, t) => e.has(r) ? b("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), d = (r, e, t, s) => (T(r, e, "write to private field"), s ? s.call(r, t) : e.set(r, t), t);
import { UmbControllerBase as w } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as v } from "@umbraco-cms/backoffice/context-api";
import { tryExecute as u } from "@umbraco-cms/backoffice/resources";
import { c } from "./index-m2GyFaVf.js";
import { UmbObjectState as f } from "@umbraco-cms/backoffice/observable-api";
class i {
  static getTokenDetails(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/token/details",
      ...e
    });
  }
  static postTokenGet(e) {
    return ((e == null ? void 0 : e.client) ?? c).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/token/get",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e == null ? void 0 : e.headers
      }
    });
  }
  static postTokenRefresh(e) {
    return ((e == null ? void 0 : e.client) ?? c).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/token/refresh",
      ...e
    });
  }
  static postTokenRevoke(e) {
    return ((e == null ? void 0 : e.client) ?? c).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/token/revoke",
      ...e
    });
  }
  static getTokenValidate(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/token/validate",
      ...e
    });
  }
}
class h {
  static getAuth(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/auth",
      ...e
    });
  }
  static getAuthUrl(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/auth/url",
      ...e
    });
  }
  static getColumns(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/columns",
      ...e
    });
  }
  static getContentProperties(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/content-properties",
      ...e
    });
  }
  static getDataSources(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/data-sources",
      ...e
    });
  }
  static getPing(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/ping",
      ...e
    });
  }
  static getRelatedPhrases(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/semrush/management/api/v1/related-phrases",
      ...e
    });
  }
}
class o extends w {
  constructor(e) {
    super(e);
  }
  async getTokenDetails() {
    const { data: e, error: t } = await u(this, i.getTokenDetails());
    return t || !e ? { error: t } : { data: e };
  }
  async getAccessToken(e) {
    const { data: t, error: s } = await u(this, i.postTokenGet({ body: { code: e } }));
    return s || !t ? { error: s } : { data: t };
  }
  async refreshAccessToken() {
    const { data: e, error: t } = await u(this, i.postTokenRefresh());
    return t || !e ? { error: t } : { data: e };
  }
  async revokeToken() {
    const { data: e, error: t } = await u(this, i.postTokenRevoke());
    return t || !e ? { error: t } : { data: e };
  }
  async validateToken() {
    const { data: e, error: t } = await u(this, i.getTokenValidate());
    return t || !e ? { error: t } : { data: e };
  }
  async oauth(e) {
    const { data: t, error: s } = await u(this, h.getAuth({ query: { code: e } }));
    return s || !t ? { error: s } : { data: t };
  }
  async getAuthorizationUrl() {
    const { data: e, error: t } = await u(this, h.getAuthUrl());
    return t || !e ? { error: t } : { data: e };
  }
  async getColumns() {
    const { data: e, error: t } = await u(this, h.getColumns());
    return t || !e ? { error: t } : { data: e };
  }
  async getDataSources() {
    const { data: e, error: t } = await u(this, h.getDataSources());
    return t || !e ? { error: t } : { data: e };
  }
  async getRelatedPhrases(e, t, s, l) {
    const { data: g, error: k } = await u(this, h.getRelatedPhrases({
      query: {
        phrase: e,
        pageNumber: t,
        dataSource: s,
        method: l
      }
    }));
    return k || !g ? { error: k } : { data: g };
  }
  async ping() {
    const { data: e, error: t } = await u(this, h.getPing());
    return t || !e ? { error: t } : { data: e };
  }
  async getCurrentContentProperties(e) {
    const { data: t, error: s } = await u(this, h.getContentProperties({ query: { contentId: e } }));
    return s || !t ? { error: s } : { data: t };
  }
}
var a, m;
class C extends w {
  constructor(t) {
    super(t);
    y(this, a);
    y(this, m);
    d(this, m, new f(void 0)), this.settingsModel = n(this, m).asObservable(), this.provideContext(A, this), d(this, a, new o(t));
  }
  async hostConnected() {
    super.hostConnected();
  }
  async getTokenDetails() {
    return await n(this, a).getTokenDetails();
  }
  async getAccessToken(t) {
    return await n(this, a).getAccessToken(t);
  }
  async refreshAccessToken() {
    return await n(this, a).refreshAccessToken();
  }
  async revokeToken() {
    return await n(this, a).revokeToken();
  }
  async validateToken() {
    return await n(this, a).validateToken();
  }
  async oauth(t) {
    return await n(this, a).oauth(t);
  }
  async getAuthorizationUrl() {
    return await n(this, a).getAuthorizationUrl();
  }
  async getColumns() {
    return await n(this, a).getColumns();
  }
  async getDataSources() {
    return await n(this, a).getDataSources();
  }
  async getRelatedPhrases(t, s, l, g) {
    return await n(this, a).getRelatedPhrases(t, s, l, g);
  }
  async ping() {
    return await n(this, a).ping();
  }
  async getCurrentContentProperties(t) {
    return await n(this, a).getCurrentContentProperties(t);
  }
}
a = new WeakMap(), m = new WeakMap();
const A = new v(C.name);
export {
  A as SEMRUSH_CONTEXT_TOKEN,
  C as SemrushContext,
  C as default
};
//# sourceMappingURL=semrush.context-DpF4KTL0.js.map
