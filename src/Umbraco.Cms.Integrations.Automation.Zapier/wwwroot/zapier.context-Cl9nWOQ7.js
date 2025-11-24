var y = (r) => {
  throw TypeError(r);
};
var m = (r, e, t) => e.has(r) || y("Cannot " + t);
var n = (r, e, t) => (m(r, e, "read from private field"), t ? t.call(r) : e.get(r)), h = (r, e, t) => e.has(r) ? y("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), l = (r, e, t, u) => (m(r, e, "write to private field"), u ? u.call(r, t) : e.set(r, t), t);
import { UmbControllerBase as p } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as d } from "@umbraco-cms/backoffice/context-api";
import { tryExecute as s } from "@umbraco-cms/backoffice/resources";
import { c } from "./index-Dr7fG1a3.js";
class i {
  static getCheckFormExtension(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/zapier/management/api/v1/check-form-extension",
      ...e
    });
  }
  static getContentByType(e) {
    return (e.client ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/zapier/management/api/v1/content-type/{alias}/content",
      ...e
    });
  }
  static getContentTypes(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/zapier/management/api/v1/content-types",
      ...e
    });
  }
  static postUpdateSubscription(e) {
    return ((e == null ? void 0 : e.client) ?? c).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/zapier/management/api/v1/subscription",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e == null ? void 0 : e.headers
      }
    });
  }
  static getSubscriptionHooks(e) {
    return ((e == null ? void 0 : e.client) ?? c).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/zapier/management/api/v1/subscription-hooks",
      ...e
    });
  }
  static postValidateUser(e) {
    return ((e == null ? void 0 : e.client) ?? c).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/zapier/management/api/v1/validate-user",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e == null ? void 0 : e.headers
      }
    });
  }
}
class g extends p {
  constructor(e) {
    super(e);
  }
  async getAll() {
    const { data: e, error: t } = await s(this, i.getSubscriptionHooks());
    return t || !e ? { error: t } : { data: e };
  }
  async getContentByType(e) {
    const { data: t, error: u } = await s(this, i.getContentByType({
      path: {
        alias: e
      }
    }));
    return u || !t ? { error: u } : { data: t };
  }
  async getContentTypes() {
    const { data: e, error: t } = await s(this, i.getContentTypes());
    return t || !e ? { error: t } : { data: e };
  }
  async checkFormsExtensionInstalled() {
    const { data: e, error: t } = await s(this, i.getCheckFormExtension());
    return t || !e ? { error: t } : { data: e };
  }
  async updatePreferences() {
    const { data: e, error: t } = await s(this, i.postUpdateSubscription());
    return t || !e ? { error: t } : { data: e };
  }
  async validateUser() {
    const { data: e, error: t } = await s(this, i.postValidateUser());
    return t || !e ? { error: t } : { data: e };
  }
}
var a;
class o extends p {
  constructor(t) {
    super(t);
    h(this, a);
    this.provideContext(C, this), l(this, a, new g(t));
  }
  async hostConnected() {
    super.hostConnected();
  }
  async getAll() {
    return await n(this, a).getAll();
  }
  async getContentByType(t) {
    return await n(this, a).getContentByType(t);
  }
  async getContentTypes() {
    return await n(this, a).getContentTypes();
  }
  async checkFormsExtensionInstalled() {
    return await n(this, a).checkFormsExtensionInstalled();
  }
  async updatePreferences() {
    return await n(this, a).updatePreferences();
  }
  async validateUser() {
    return await n(this, a).validateUser();
  }
}
a = new WeakMap();
const C = new d(o.name);
export {
  C as ZAPIER_CONTEXT_TOKEN,
  o as ZapierContext,
  o as default
};
//# sourceMappingURL=zapier.context-Cl9nWOQ7.js.map
