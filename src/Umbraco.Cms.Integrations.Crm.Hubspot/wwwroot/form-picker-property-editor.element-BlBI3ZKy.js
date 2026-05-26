import { UmbElementMixin as y } from "@umbraco-cms/backoffice/element-api";
import { LitElement as E, html as h, css as b, property as M, state as v, customElement as S } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalToken as C, UMB_MODAL_MANAGER_CONTEXT as w } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT as T } from "@umbraco-cms/backoffice/notification";
import { H as N, C as k } from "./index-Cm1-jVt_.js";
const A = new C("HubspotForms.Modal", {
  modal: {
    type: "sidebar",
    size: "small"
  }
});
var F = Object.defineProperty, P = Object.getOwnPropertyDescriptor, O = (e) => {
  throw TypeError(e);
}, u = (e, t, s, n) => {
  for (var r = n > 1 ? void 0 : n ? P(t, s) : t, d = e.length - 1, p; d >= 0; d--)
    (p = e[d]) && (r = (n ? p(t, s, r) : p(r)) || r);
  return n && r && F(t, s, r), r;
}, f = (e, t, s) => t.has(e) || O("Cannot " + s), o = (e, t, s) => (f(e, t, "read from private field"), t.get(e)), c = (e, t, s) => t.has(e) ? O("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, s), _ = (e, t, s, n) => (f(e, t, "write to private field"), t.set(e, s), s), H = (e, t, s) => (f(e, t, "access private method"), s), l, i, m, g;
const I = "hubspot-form-picker";
let a = class extends y(E) {
  constructor() {
    super(), c(this, m), c(this, l), c(this, i), this.value = "", this._form = {
      name: "",
      id: "",
      fields: "",
      portalId: "",
      region: ""
    }, this._serviceStatus = {
      isValid: !1,
      type: "",
      description: "",
      useOAuth: !1
    }, this.consumeContext(w, (e) => {
      _(this, l, e);
    }), this.consumeContext(N, (e) => {
      e && this.observe(e.settingsModel, (t) => {
        _(this, i, t);
      });
    });
  }
  async connectedCallback() {
    if (super.connectedCallback(), this.value == null || this.value.length == 0 || !o(this, i)) return;
    this._serviceStatus = {
      isValid: o(this, i).isValid,
      type: o(this, i).type.value,
      description: "",
      useOAuth: o(this, i).isValid && o(this, i).type.value === "OAuth"
    }, this._serviceStatus.isValid || this._showError(k.none);
    const e = JSON.parse(JSON.stringify(this.value));
    this._form = {
      id: e.id,
      name: e.name,
      fields: e.fields,
      portalId: e.portalId,
      region: e.region
    };
  }
  async _openModal() {
    var s;
    const e = (s = o(this, l)) == null ? void 0 : s.open(this, A, {
      data: {
        headline: "HubSpot Forms"
      }
    }), t = await (e == null ? void 0 : e.onSubmit());
    t && (this._form = {
      id: t.form.id,
      name: t.form.name,
      fields: t.form.fields,
      portalId: t.form.portalId,
      region: t.form.region
    }, this.value = JSON.stringify(t.form), this.dispatchEvent(new CustomEvent("property-value-change")));
  }
  async _showError(e) {
    const t = await this.getContext(T);
    t == null || t.peek("danger", {
      data: { message: e }
    });
  }
  render() {
    var e, t;
    return h`
            ${this.value == null || this.value.length == 0 ? h`
                    <uui-button
				        class="add-button"
				        @click=${this._openModal}
				        label=${this.localize.term("general_add")}
				        look="placeholder"></uui-button>
                ` : h`
                    <uui-ref-node-form selectable name=${((e = this._form) == null ? void 0 : e.name) ?? ""} detail=${((t = this._form) == null ? void 0 : t.fields) ?? ""}>
                        <uui-action-bar slot="actions">
                            <uui-button label="Remove" @click=${H(this, m, g)}>Remove</uui-button>
                        </uui-action-bar>
                    </uui-ref-node-form>  
                `}
		`;
  }
};
l = /* @__PURE__ */ new WeakMap();
i = /* @__PURE__ */ new WeakMap();
m = /* @__PURE__ */ new WeakSet();
g = function() {
  this.value = "", this.dispatchEvent(new CustomEvent("property-value-change"));
};
a.styles = [
  b`
            .add-button {
                width: 100%;
            }
        `
];
u([
  M({ type: String })
], a.prototype, "value", 2);
u([
  v()
], a.prototype, "_form", 2);
u([
  v()
], a.prototype, "_serviceStatus", 2);
a = u([
  S(I)
], a);
const x = a;
export {
  a as HubspotFormPickerElement,
  x as default
};
//# sourceMappingURL=form-picker-property-editor.element-BlBI3ZKy.js.map
