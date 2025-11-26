import { UmbElementMixin as M } from "@umbraco-cms/backoffice/element-api";
import { LitElement as E, html as o, nothing as A, css as b, property as w, state as y, customElement as O } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalToken as k, UMB_MODAL_MANAGER_CONTEXT as T } from "@umbraco-cms/backoffice/modal";
import { ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN as F } from "./activecampaign-forms.context-ey7eA06-.js";
const P = new k("ActiveCampaignForms.Modal", {
  modal: {
    type: "sidebar",
    size: "medium"
  }
});
var N = Object.defineProperty, S = Object.getOwnPropertyDescriptor, g = (e) => {
  throw TypeError(e);
}, f = (e, t, a, r) => {
  for (var i = r > 1 ? void 0 : r ? S(t, a) : t, d = e.length - 1, u; d >= 0; d--)
    (u = e[d]) && (i = (r ? u(t, a, i) : u(i)) || i);
  return r && i && N(t, a, i), i;
}, _ = (e, t, a) => t.has(e) || g("Cannot " + a), p = (e, t, a) => (_(e, t, "read from private field"), a ? a.call(e) : t.get(e)), s = (e, t, a) => t.has(e) ? g("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, a), h = (e, t, a, r) => (_(e, t, "write to private field"), t.set(e, a), a), W = (e, t, a) => (_(e, t, "access private method"), a), l, m, c, v, C;
const x = "activecampaign-form-picker";
let n = class extends M(E) {
  constructor() {
    super(), s(this, v), s(this, l), s(this, m), s(this, c), this.value = "", this._form = {
      id: "",
      name: ""
    }, this.consumeContext(T, (e) => {
      h(this, l, e);
    }), this.consumeContext(F, (e) => {
      e && (h(this, m, e), this.observe(e.configurationModel, (t) => {
        h(this, c, t);
      }));
    });
  }
  async connectedCallback() {
    if (super.connectedCallback(), this.value == null || this.value.length == 0) return;
    const { data: e } = await p(this, m).getForm(this.value);
    e && (this._form = {
      id: e.form.id,
      name: e.form.name
    });
  }
  async _openModal() {
    var a;
    const e = (a = p(this, l)) == null ? void 0 : a.open(this, P, {
      data: {
        headline: "ActiveCampaign Forms"
      }
    }), t = await (e == null ? void 0 : e.onSubmit());
    t && (this._form = {
      id: t.form.id,
      name: t.form.name
    }, this.value = t.form.id, this.dispatchEvent(new CustomEvent("property-value-change")));
  }
  _renderWarning() {
    return o`<div class="warning">Invalid API configuration.</div>`;
  }
  render() {
    var e, t;
    return o`
            ${this.value == null || this.value.length == 0 ? o`
                    <uui-button
				        class="add-button"
				        @click=${this._openModal}
				        label=${this.localize.term("general_add")}
				        look="placeholder"></uui-button>
                ` : o`
                    <uui-ref-node-form selectable name=${((e = this._form) == null ? void 0 : e.name) ?? ""}>
                        <uui-action-bar slot="actions">
                            <uui-button label="Remove" @click=${W(this, v, C)}>Remove</uui-button>
                        </uui-action-bar>
                    </uui-ref-node-form>  
                `}
            ${(t = p(this, c)) != null && t.isApiConfigurationValid ? A : this._renderWarning()}
		`;
  }
};
l = /* @__PURE__ */ new WeakMap();
m = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakMap();
v = /* @__PURE__ */ new WeakSet();
C = function() {
  this.value = "", this.dispatchEvent(new CustomEvent("property-value-change"));
};
n.styles = [
  b`
            .add-button {
                width: 100%;
            }

            .warning {
                background-color: #fff3cd;
                border-color: #ffeeba;
                position: relative;
                padding: .75rem 1.25rem;
                margin-top: 10px;
                border: 1px solid transparent;
                border-radius: .25rem;
            }
        `
];
f([
  w({ type: String })
], n.prototype, "value", 2);
f([
  y()
], n.prototype, "_form", 2);
n = f([
  O(x)
], n);
export {
  n as default
};
//# sourceMappingURL=form-picker-property-editor.element-B9o60pWw.js.map
