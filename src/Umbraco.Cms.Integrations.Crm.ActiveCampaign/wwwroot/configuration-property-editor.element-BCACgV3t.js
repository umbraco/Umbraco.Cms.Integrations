import { UmbElementMixin as f } from "@umbraco-cms/backoffice/element-api";
import { LitElement as _, html as c, customElement as u } from "@umbraco-cms/backoffice/external/lit";
import { ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN as m } from "./activecampaign-forms.context-ey7eA06-.js";
var h = Object.getOwnPropertyDescriptor, l = (t) => {
  throw TypeError(t);
}, C = (t, e, r, n) => {
  for (var i = n > 1 ? void 0 : n ? h(e, r) : e, o = t.length - 1, s; o >= 0; o--)
    (s = t[o]) && (i = s(i) || i);
  return i;
}, d = (t, e, r) => e.has(t) || l("Cannot " + r), p = (t, e, r) => (d(t, e, "read from private field"), r ? r.call(t) : e.get(t)), g = (t, e, r) => e.has(t) ? l("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), E = (t, e, r, n) => (d(t, e, "write to private field"), e.set(t, r), r), a;
const A = "activecampaign-forms-configuration";
let v = class extends f(_) {
  constructor() {
    super(), g(this, a), this.consumeContext(m, (t) => {
      t && this.observe(t.configurationModel, (e) => {
        E(this, a, e);
      });
    });
  }
  render() {
    var t;
    return c`
            <div>
                <p>
                    ${(t = p(this, a)) != null && t.isApiConfigurationValid ? c`Connected. Account name: <b>${p(this, a).account}</b>` : "Invalid API configuration."}
                </p>
            </div>
        `;
  }
};
a = /* @__PURE__ */ new WeakMap();
v = C([
  u(A)
], v);
export {
  v as default
};
//# sourceMappingURL=configuration-property-editor.element-BCACgV3t.js.map
