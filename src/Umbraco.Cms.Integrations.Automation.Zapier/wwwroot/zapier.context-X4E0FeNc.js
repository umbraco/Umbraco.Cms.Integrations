var o = (r) => {
  throw TypeError(r);
};
var p = (r, e, t) => e.has(r) || o("Cannot " + t);
var s = (r, e, t) => (p(r, e, "read from private field"), t ? t.call(r) : e.get(r)), y = (r, e, t) => e.has(r) ? o("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), m = (r, e, t, a) => (p(r, e, "write to private field"), a ? a.call(r, t) : e.set(r, t), t);
import { UmbControllerBase as h } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as l } from "@umbraco-cms/backoffice/context-api";
import { tryExecute as c } from "@umbraco-cms/backoffice/resources";
import { c as i } from "./index-BJDjlq1l.js";
class u {
  static getCheckFormExtension(e) {
    return ((e == null ? void 0 : e.client) ?? i).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/zapier/management/api/v1/check-form-extension",
      ...e
    });
  }
  static getContentByType(e) {
    return (e.client ?? i).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/zapier/management/api/v1/content-type/{alias}/content",
      ...e
    });
  }
  static getContentTypes(e) {
    return ((e == null ? void 0 : e.client) ?? i).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/zapier/management/api/v1/content-types",
      ...e
    });
  }
  static postUpdateSubscription(e) {
    return (e.client ?? i).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/zapier/management/api/v1/subscription",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e.headers
      }
    });
  }
  static getSubscriptionHooks(e) {
    return ((e == null ? void 0 : e.client) ?? i).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/zapier/management/api/v1/subscription-hooks",
      ...e
    });
  }
  static postValidateUser(e) {
    return (e.client ?? i).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/zapier/management/api/v1/validate-user",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e.headers
      }
    });
  }
}
class d extends h {
  constructor(e) {
    super(e);
  }
  async getAll() {
    const { data: e, error: t } = await c(this, u.getSubscriptionHooks());
    return t || !e ? { error: t } : { data: e };
  }
  async getContentByType(e) {
    const { data: t, error: a } = await c(this, u.getContentByType({
      path: {
        alias: e
      }
    }));
    return a || !t ? { error: a } : { data: t };
  }
  async getContentTypes() {
    const { data: e, error: t } = await c(this, u.getContentTypes());
    return t || !e ? { error: t } : { data: e };
  }
  async checkFormsExtensionInstalled() {
    const { data: e, error: t } = await c(this, u.getCheckFormExtension());
    return t || !e ? { error: t } : { data: e };
  }
  async updatePreferences(e) {
    const { data: t, error: a } = await c(this, u.postUpdateSubscription({ body: e }));
    return a || !t ? { error: a } : { data: t };
  }
  async validateUser(e) {
    const { data: t, error: a } = await c(this, u.postValidateUser({ body: e }));
    return a || !t ? { error: a } : { data: t };
  }
}
var n;
class g extends h {
  constructor(t) {
    super(t);
    y(this, n);
    this.provideContext(b, this), m(this, n, new d(t));
  }
  async hostConnected() {
    super.hostConnected();
  }
  async getAll() {
    return await s(this, n).getAll();
  }
  async getContentByType(t) {
    return await s(this, n).getContentByType(t);
  }
  async getContentTypes() {
    return await s(this, n).getContentTypes();
  }
  async checkFormsExtensionInstalled() {
    return await s(this, n).checkFormsExtensionInstalled();
  }
  async updatePreferences(t) {
    return await s(this, n).updatePreferences(t);
  }
  async validateUser(t) {
    return await s(this, n).validateUser(t);
  }
}
n = new WeakMap();
const b = new l(g.name);
export {
  b as ZAPIER_CONTEXT_TOKEN,
  g as ZapierContext,
  g as default
};
//# sourceMappingURL=zapier.context-X4E0FeNc.js.map
