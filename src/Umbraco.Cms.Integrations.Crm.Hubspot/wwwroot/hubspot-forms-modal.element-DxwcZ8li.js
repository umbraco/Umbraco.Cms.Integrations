import { html as d, css as S, state as p, customElement as E } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as M } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT as T } from "@umbraco-cms/backoffice/notification";
import { H as $ } from "./index-Cq7aK7bY.js";
var A = Object.defineProperty, x = Object.getOwnPropertyDescriptor, w = (t) => {
  throw TypeError(t);
}, c = (t, e, i, n) => {
  for (var s = n > 1 ? void 0 : n ? x(e, i) : e, _ = t.length - 1, f; _ >= 0; _--)
    (f = t[_]) && (s = (n ? f(e, i, s) : f(s)) || s);
  return n && s && A(e, i, s), s;
}, v = (t, e, i) => e.has(t) || w("Cannot " + i), o = (t, e, i) => (v(t, e, "read from private field"), e.get(t)), m = (t, e, i) => e.has(t) ? w("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, i), g = (t, e, i, n) => (v(t, e, "write to private field"), e.set(t, i), i), u = (t, e, i) => (v(t, e, "access private method"), i), h, a, r, y, b, F, O, C;
const k = "hubspot-forms-modal";
let l = class extends M {
  constructor() {
    super(), m(this, r), m(this, h), m(this, a), this._serviceStatus = {
      isValid: !1,
      type: "",
      description: "",
      useOAuth: !1
    }, this._loading = !1, this._forms = [], this._filteredForms = this._forms, this.consumeContext($, (t) => {
      t && (g(this, h, t), this.observe(t.settingsModel, (e) => {
        g(this, a, e);
      }));
    });
  }
  async connectedCallback() {
    super.connectedCallback(), u(this, r, y).call(this);
  }
  _renderFilter() {
    return d` <uui-input
			type="search"
			id="filter"
			@input="${u(this, r, F)}"
			placeholder="Type to filter..."
			label="Type to filter forms">
			<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
		</uui-input>`;
  }
  _onSelect(t) {
    this.value = { form: t }, this._submitModal();
  }
  async _showError(t) {
    const e = await this.getContext(T);
    e == null || e.peek("danger", {
      data: { message: t }
    });
  }
  render() {
    return d`
            <umb-body-layout>
                <uui-box headline="HubSpot Forms">
                    ${this._loading ? d`<div class="center"><uui-loader></uui-loader></div>` : ""}
                    ${this._renderFilter()}
                    ${this._filteredForms.map((t) => d`
                            <uui-ref-node-form
                              name=${t.name ?? ""}
                              detail=${t.fields ?? ""}
                              @open=${() => this._onSelect(t)}>
                            </uui-ref-node-form>
                        `)}
                </uui-box>

                <uui-box headline="HubSpot API">
                    <hubspot-authorization @connect=${u(this, r, O)} @revoke=${u(this, r, C)}> </hubspot-authorization>
                </uui-box>

                <uui-button slot="actions" label=${this.localize.term("general_close")} @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
  }
};
h = /* @__PURE__ */ new WeakMap();
a = /* @__PURE__ */ new WeakMap();
r = /* @__PURE__ */ new WeakSet();
y = async function() {
  !o(this, h) || !o(this, a) || (this._serviceStatus = {
    isValid: o(this, a).isValid,
    type: o(this, a).type.value,
    description: "",
    useOAuth: o(this, a).isValid && o(this, a).type.value === "OAuth"
  }, await u(this, r, b).call(this));
};
b = async function() {
  this._loading = !0;
  const { data: t } = this._serviceStatus.useOAuth ? await o(this, h).getFormsOAuth() : await o(this, h).getFormsByApiKey();
  t && (this._forms = t.forms ?? [], this._filteredForms = t.forms ?? [], this._loading = !1, (!t.isValid || t.isExpired) && this._showError(t.error));
};
F = function(t) {
  let e = t.target.value || "";
  e = e.toLowerCase();
  const i = e ? this._forms.filter((n) => {
    var s;
    return (s = n.name) == null ? void 0 : s.toLowerCase().includes(e);
  }) : this._forms;
  this._filteredForms = i;
};
O = async function() {
  await u(this, r, b).call(this);
};
C = async function() {
  this._filteredForms = [], await u(this, r, y).call(this);
};
l.styles = [
  S`
            uui-box {
                margin-bottom: var(--uui-size-8);
            }

            #filter {
                width: 100%;
                margin-bottom: var(--uui-size-3);
            }

            uui-icon {
                margin: auto;
                margin-left: var(--uui-size-2);
            }
        `
];
c([
  p()
], l.prototype, "_serviceStatus", 2);
c([
  p()
], l.prototype, "_loading", 2);
c([
  p()
], l.prototype, "_forms", 2);
c([
  p()
], l.prototype, "_filteredForms", 2);
l = c([
  E(k)
], l);
export {
  l as default
};
//# sourceMappingURL=hubspot-forms-modal.element-DxwcZ8li.js.map
