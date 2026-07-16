import { html as l, css as T, state as g, property as C, customElement as b } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as k } from "@umbraco-cms/backoffice/lit-element";
import { D } from "./index-DAz-CdMR.js";
import { UmbModalToken as F, UMB_MODAL_MANAGER_CONTEXT as N } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT as P } from "@umbraco-cms/backoffice/notification";
const A = new F("Dynamics.Modal", {
  modal: {
    type: "sidebar",
    size: "large"
  }
});
var S = Object.defineProperty, $ = Object.getOwnPropertyDescriptor, E = (e) => {
  throw TypeError(e);
}, d = (e, t, o, r) => {
  for (var i = r > 1 ? void 0 : r ? $(t, o) : t, a = e.length - 1, n; a >= 0; a--)
    (n = e[a]) && (i = (r ? n(t, o, i) : n(i)) || i);
  return r && i && S(t, o, i), i;
}, y = (e, t, o) => t.has(e) || E("Cannot " + o), _ = (e, t, o) => (y(e, t, "read from private field"), t.get(e)), h = (e, t, o) => t.has(e) ? E("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, o), p = (e, t, o, r) => (y(e, t, "write to private field"), t.set(e, o), o), v = (e, t, o) => (y(e, t, "access private method"), o), f, u, m, c, M, w, O;
const I = "dynamics-form-picker";
let s = class extends k {
  constructor() {
    super(), h(this, c), h(this, f), h(this, u), h(this, m), this.value = "", this.consumeContext(N, (e) => {
      p(this, m, e);
    }), this.consumeContext(D, (e) => {
      e && (p(this, f, e), this.observe(e.settingsModel, (t) => {
        p(this, u, t);
      }));
    });
  }
  set config(e) {
    this._config = v(this, c, M).call(this, e);
  }
  async connectedCallback() {
    super.connectedCallback(), setTimeout(() => {
      v(this, c, w).call(this);
    }, 3e3);
  }
  async _openModal() {
    var r, i, a, n;
    const e = ((r = this._config) == null ? void 0 : r.module) == "Both" ? "Outbound | Real-Time" : (i = this._config) == null ? void 0 : i.module, t = (n = _(this, m)) == null ? void 0 : n.open(this, A, {
      data: {
        headline: `Dynamics Forms - ${e} Marketing Forms`,
        module: (a = this._config) == null ? void 0 : a.module
      }
    }), o = await (t == null ? void 0 : t.onSubmit());
    o && (this.value = JSON.stringify(o.selectedForm), this.selectedForm = o.selectedForm, this.dispatchEvent(new CustomEvent("property-value-change")));
  }
  async _showError(e) {
    const t = await this.getContext(P);
    t == null || t.peek("danger", {
      data: { message: e }
    });
  }
  render() {
    return l`
        ${this.value == null || this.value.length == 0 ? l`
                <div>
                    <uui-button
                        class="add-button"
                        @click=${this._openModal}
                        label=${this.localize.term("general_add")}
                        look="placeholder"></uui-button>
                </div>
            ` : l`
            ${this.selectedForm ? l`
                    <div>
                        <uui-ref-node-form name=${this.selectedForm.name}>
                            <uui-action-bar slot="actions">
                                <uui-button label="Remove" @click=${v(this, c, O)}>Remove</uui-button>
                            </uui-action-bar>
                        </uui-ref-node-form>
                    </div>
                ` : l`
                    <div class="center loader"><uui-loader></uui-loader></div>
                `}
            `}
        `;
  }
};
f = /* @__PURE__ */ new WeakMap();
u = /* @__PURE__ */ new WeakMap();
m = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakSet();
M = function(e) {
  return {
    module: e == null ? void 0 : e.getValueByAlias("modules")
  };
};
w = function() {
  this.value == null || this.value.length == 0 || _(this, u) && (_(this, u).isAuthorized || this._showError("Unable to connect to Dynamics. Please review the settings of the form picker property's data type."), this.selectedForm = JSON.parse(JSON.stringify(this.value)));
};
O = function() {
  this.value = "", this.dispatchEvent(new CustomEvent("property-value-change"));
};
s.styles = [
  T`
            .add-button {
                width: 100%;
            }
        `
];
d([
  g()
], s.prototype, "_config", 2);
d([
  C({ attribute: !1 })
], s.prototype, "config", 1);
d([
  C({ type: String })
], s.prototype, "value", 2);
d([
  g()
], s.prototype, "selectedForm", 2);
s = d([
  b(I)
], s);
const z = s;
export {
  s as DynamicsFormPickerPropertyEditor,
  z as default
};
//# sourceMappingURL=dynamics-form-picker-property-editor.element-Bl1JkvgJ.js.map
