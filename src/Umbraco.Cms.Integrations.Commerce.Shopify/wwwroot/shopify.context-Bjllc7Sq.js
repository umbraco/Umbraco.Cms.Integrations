var l = (r) => {
  throw TypeError(r);
};
var m = (r, e, t) => e.has(r) || l("Cannot " + t);
var a = (r, e, t) => (m(r, e, "read from private field"), t ? t.call(r) : e.get(r)), y = (r, e, t) => e.has(r) ? l("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), g = (r, e, t, c) => (m(r, e, "write to private field"), c ? c.call(r, t) : e.set(r, t), t);
import { UmbControllerBase as d } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as k } from "@umbraco-cms/backoffice/context-api";
import { tryExecute as n } from "@umbraco-cms/backoffice/resources";
import { c as i } from "./index-BjHisZi7.js";
import { UmbObjectState as f } from "@umbraco-cms/backoffice/observable-api";
class u {
  static postAccessToken(e) {
    return (e.client ?? i).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/shopify/management/api/v1/access-token",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e.headers
      }
    });
  }
  static getAuthorizationUrl(e) {
    return ((e == null ? void 0 : e.client) ?? i).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/shopify/management/api/v1/authorization-url",
      ...e
    });
  }
  static getCheckConfiguration(e) {
    return ((e == null ? void 0 : e.client) ?? i).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/shopify/management/api/v1/check-configuration",
      ...e
    });
  }
  static getList(e) {
    return ((e == null ? void 0 : e.client) ?? i).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/shopify/management/api/v1/list",
      ...e
    });
  }
  static postListByIds(e) {
    return (e.client ?? i).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/shopify/management/api/v1/list-by-ids",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e.headers
      }
    });
  }
  static postRefreshAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? i).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/shopify/management/api/v1/refresh-access-token",
      ...e
    });
  }
  static postRevokeAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? i).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/shopify/management/api/v1/revoke-access-token",
      ...e
    });
  }
  static getTotalPages(e) {
    return ((e == null ? void 0 : e.client) ?? i).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/shopify/management/api/v1/total-pages",
      ...e
    });
  }
  static getValidateAccessToken(e) {
    return ((e == null ? void 0 : e.client) ?? i).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/shopify/management/api/v1/validate-access-token",
      ...e
    });
  }
}
class p extends d {
  constructor(e) {
    super(e);
  }
  async checkConfiguration() {
    const { data: e, error: t } = await n(this, u.getCheckConfiguration());
    return t || !e ? { error: t } : { data: e };
  }
  async getAccessToken(e) {
    const { data: t, error: c } = await n(this, u.postAccessToken({ body: e }));
    return c || !t ? { error: c } : { data: t };
  }
  async validateAccessToken() {
    const { data: e, error: t } = await n(this, u.getValidateAccessToken());
    return t || !e ? { error: t } : { data: e };
  }
  async revokeAccessToken() {
    const { data: e, error: t } = await n(this, u.postRevokeAccessToken());
    return t || !e ? { error: t } : { data: e };
  }
  async getList(e) {
    const { data: t, error: c } = await n(this, u.getList({ query: { pageInfo: e } }));
    return c || !t ? { error: c } : { data: t };
  }
  async getListByIds(e) {
    const { data: t, error: c } = await n(this, u.postListByIds({
      body: e
    }));
    return c || !t ? { error: c } : { data: t };
  }
  async getTotalPages() {
    const { data: e, error: t } = await n(this, u.getTotalPages());
    return t || !e ? { error: t } : { data: e };
  }
  async getAuthorizationUrl() {
    const { data: e, error: t } = await n(this, u.getAuthorizationUrl());
    return t || !e ? { error: t } : { data: e };
  }
  async refreshAccessToken() {
    const { data: e, error: t } = await n(this, u.postRefreshAccessToken());
    return t || !e ? { error: t } : { data: e };
  }
}
var s, o, h;
class T extends d {
  constructor(t) {
    super(t);
    y(this, s);
    y(this, o);
    y(this, h);
    g(this, o, new f(void 0)), g(this, h, new f(void 0)), this.settingsModel = a(this, h).asObservable(), this.provideContext(b, this), g(this, s, new p(t));
  }
  async hostConnected() {
    super.hostConnected(), this.checkConfiguration();
  }
  async checkConfiguration() {
    const { data: t } = await a(this, s).checkConfiguration();
    a(this, h).setValue(t);
  }
  async getAccessToken(t) {
    return await a(this, s).getAccessToken(t);
  }
  async validateAccessToken() {
    return await a(this, s).validateAccessToken();
  }
  async revokeAccessToken() {
    return await a(this, s).revokeAccessToken();
  }
  async getList(t) {
    return await a(this, s).getList(t);
  }
  async getListByIds(t) {
    return await a(this, s).getListByIds(t);
  }
  async getTotalPages() {
    return await a(this, s).getTotalPages();
  }
  async getAuthorizationUrl() {
    return await a(this, s).getAuthorizationUrl();
  }
  async refreshAccessToken() {
    return await a(this, s).refreshAccessToken();
  }
  getData() {
    return a(this, o).getValue();
  }
}
s = new WeakMap(), o = new WeakMap(), h = new WeakMap();
const b = new k(T.name);
export {
  b as SHOPIFY_CONTEXT_TOKEN,
  T as ShopifyContext,
  T as default
};
//# sourceMappingURL=shopify.context-Bjllc7Sq.js.map
