var p = (r) => {
  throw TypeError(r);
};
var y = (r, e, t) => e.has(r) || p("Cannot " + t);
var c = (r, e, t) => (y(r, e, "read from private field"), t ? t.call(r) : e.get(r)), m = (r, e, t) => e.has(r) ? p("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), n = (r, e, t, a) => (y(r, e, "write to private field"), a ? a.call(r, t) : e.set(r, t), t);
import { UmbControllerBase as A } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as l } from "@umbraco-cms/backoffice/context-api";
import { UmbObjectState as d } from "@umbraco-cms/backoffice/observable-api";
import { tryExecute as o } from "@umbraco-cms/backoffice/resources";
import { c as u } from "./index-CVOpZ6ob.js";
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
class f extends A {
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
  async getForms(e, t) {
    const { data: a, error: h } = await o(this, g.getForms({ query: { page: e, searchQuery: t } }));
    return h || !a ? { error: h } : { data: a };
  }
}
var s, i;
class F extends A {
  constructor(t) {
    super(t);
    m(this, s);
    m(this, i);
    n(this, i, new d(void 0)), this.configurationModel = c(this, i).asObservable(), this.provideContext(b, this), n(this, s, new f(t));
  }
  async hostConnected() {
    super.hostConnected(), this.checkApiAccess();
  }
  async checkApiAccess() {
    const { data: t } = await c(this, s).checkApiAccess();
    c(this, i).setValue(t);
  }
  async getForms(t, a) {
    return await c(this, s).getForms(t, a);
  }
  async getForm(t) {
    return await c(this, s).getForm(t);
  }
}
s = new WeakMap(), i = new WeakMap();
const b = new l(F.name);
export {
  b as ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN,
  F as ActiveCampaignFormsContext,
  F as default
};
//# sourceMappingURL=activecampaign-forms.context-DO6P_YQW.js.map
