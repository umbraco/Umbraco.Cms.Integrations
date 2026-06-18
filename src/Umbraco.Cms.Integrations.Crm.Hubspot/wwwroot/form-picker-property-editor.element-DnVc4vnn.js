import { UmbElementMixin as y } from "@umbraco-cms/backoffice/element-api";
import { LitElement as E, html as h, css as b, property as M, state as v, customElement as S } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalToken as C, UMB_MODAL_MANAGER_CONTEXT as w } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT as T } from "@umbraco-cms/backoffice/notification";
import { H as N, C as k } from "./index-CmGrvIxE.js";
const A = new C("HubspotForms.Modal", {
  modal: {
    type: "sidebar",
    size: "small"
  }
});
var F = Object.defineProperty, P = Object.getOwnPropertyDescriptor, O = (t) => {
  throw TypeError(t);
}, u = (t, e, s, n) => {
  for (var r = n > 1 ? void 0 : n ? P(e, s) : e, d = t.length - 1, p; d >= 0; d--)
    (p = t[d]) && (r = (n ? p(e, s, r) : p(r)) || r);
  return n && r && F(e, s, r), r;
}, f = (t, e, s) => e.has(t) || O("Cannot " + s), a = (t, e, s) => (f(t, e, "read from private field"), e.get(t)), c = (t, e, s) => e.has(t) ? O("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, s), _ = (t, e, s, n) => (f(t, e, "write to private field"), e.set(t, s), s), H = (t, e, s) => (f(t, e, "access private method"), s), l, i, m, g;
const I = "hubspot-form-picker";
let o = class extends y(E) {
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
    }, this.consumeContext(w, (t) => {
      _(this, l, t);
    }), this.consumeContext(N, (t) => {
      t && this.observe(t.settingsModel, (e) => {
        _(this, i, e);
      });
    });
  }
  async connectedCallback() {
    if (super.connectedCallback(), this.value == null || this.value.length == 0 || !a(this, i)) return;
    this._serviceStatus = {
      isValid: a(this, i).isValid,
      type: a(this, i).type.value,
      description: "",
      useOAuth: a(this, i).isValid && a(this, i).type.value === "OAuth"
    }, this._serviceStatus.isValid || this._showError(k.none);
    const t = JSON.parse(JSON.stringify(this.value));
    this._form = {
      id: t.id,
      name: t.name,
      fields: t.fields,
      portalId: t.portalId,
      region: t.region
    };
  }
  async _openModal() {
    var s;
    const t = (s = a(this, l)) == null ? void 0 : s.open(this, A, {
      data: {
        headline: "HubSpot Forms"
      }
    }), e = await (t == null ? void 0 : t.onSubmit());
    e && (this._form = {
      id: e.form.id,
      name: e.form.name,
      fields: e.form.fields,
      portalId: e.form.portalId,
      region: e.form.region
    }, this.value = JSON.stringify(e.form), this.dispatchEvent(new CustomEvent("property-value-change")));
  }
  async _showError(t) {
    const e = await this.getContext(T);
    e == null || e.peek("danger", {
      data: { message: t }
    });
  }
  render() {
    var t, e;
    return h`
            ${this.value == null || this.value.length == 0 ? h`
                    <uui-button
				        class="add-button"
				        @click=${this._openModal}
				        label=${this.localize.term("general_add")}
				        look="placeholder"></uui-button>
                ` : h`
                    <uui-ref-node-form name=${((t = this._form) == null ? void 0 : t.name) ?? ""} detail=${((e = this._form) == null ? void 0 : e.fields) ?? ""}>
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
o.styles = [
  b`
            :host {
                display: block;
            }
            .add-button {
                width: 100%;
            }
        `
];
u([
  M({ type: String })
], o.prototype, "value", 2);
u([
  v()
], o.prototype, "_form", 2);
u([
  v()
], o.prototype, "_serviceStatus", 2);
o = u([
  S(I)
], o);
const x = o;
export {
  o as HubspotFormPickerElement,
  x as default
};
//# sourceMappingURL=form-picker-property-editor.element-DnVc4vnn.js.map
