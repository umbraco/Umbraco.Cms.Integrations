var h = (r) => {
  throw TypeError(r);
};
var p = (r, e, t) => e.has(r) || h("Cannot " + t);
var c = (r, e, t) => (p(r, e, "read from private field"), t ? t.call(r) : e.get(r)), m = (r, e, t) => e.has(r) ? h("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), n = (r, e, t, a) => (p(r, e, "write to private field"), a ? a.call(r, t) : e.set(r, t), t);
import { UmbControllerBase as y } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as A } from "@umbraco-cms/backoffice/context-api";
import { UmbObjectState as l } from "@umbraco-cms/backoffice/observable-api";
import { tryExecute as o } from "@umbraco-cms/backoffice/resources";
import { c as u } from "./index-DxsknWv0.js";
class g {
  static getApiAccess(e) {
    return ((e == null ? void 0 : e.client) ?? u).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/activecampaign-forms/management/api/v1/api-access",
      ...e
    });
  }
  static getForms(e) {
    return ((e == null ? void 0 : e.client) ?? u).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/activecampaign-forms/management/api/v1/forms",
      ...e
    });
  }
  static getFormsById(e) {
    return (e.client ?? u).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/activecampaign-forms/management/api/v1/forms/{id}",
      ...e
    });
  }
}
class d extends y {
  constructor(e) {
    super(e);
  }
  async checkApiAccess() {
    const { data: e, error: t } = await o(this, g.getApiAccess());
    return t || !e ? { error: t } : { data: e };
  }
  async getForm(e) {
    const { data: t, error: a } = await o(this, g.getFormsById({ path: { id: e } }));
    return a || !t ? { error: a } : { data: t };
  }
  async getForms(e) {
    const { data: t, error: a } = await o(this, g.getForms({ query: { page: e } }));
    return a || !t ? { error: a } : { data: t };
  }
}
var s, i;
class f extends y {
  constructor(t) {
    super(t);
    m(this, s);
    m(this, i);
    n(this, i, new l(void 0)), this.configurationModel = c(this, i).asObservable(), this.provideContext(F, this), n(this, s, new d(t));
  }
  async hostConnected() {
    super.hostConnected(), this.checkApiAccess();
  }
  async checkApiAccess() {
    const { data: t } = await c(this, s).checkApiAccess();
    c(this, i).setValue(t);
  }
  async getForms(t) {
    return await c(this, s).getForms(t);
  }
  async getForm(t) {
    return await c(this, s).getForm(t);
  }
}
s = new WeakMap(), i = new WeakMap();
const F = new A(f.name);
export {
  F as ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN,
  f as ActiveCampaignFormsContext,
  f as default
};
//# sourceMappingURL=activecampaign-forms.context-ey7eA06-.js.map
