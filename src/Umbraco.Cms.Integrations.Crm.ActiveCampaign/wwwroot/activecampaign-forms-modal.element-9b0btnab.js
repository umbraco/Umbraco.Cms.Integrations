import { UmbModalBaseElement as $ } from "@umbraco-cms/backoffice/modal";
import { html as n, repeat as M, nothing as E, css as N, state as _, customElement as A } from "@umbraco-cms/backoffice/external/lit";
import { ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN as T } from "./activecampaign-forms.context-ey7eA06-.js";
import { UMB_NOTIFICATION_CONTEXT as O } from "@umbraco-cms/backoffice/notification";
var I = Object.defineProperty, x = Object.getOwnPropertyDescriptor, C = (t) => {
  throw TypeError(t);
}, u = (t, e, i, s) => {
  for (var o = s > 1 ? void 0 : s ? x(e, i) : e, m = t.length - 1, p; m >= 0; m--)
    (p = t[m]) && (o = (s ? p(e, i, o) : p(o)) || o);
  return s && o && I(e, i, o), o;
}, g = (t, e, i) => e.has(t) || C("Cannot " + i), d = (t, e, i) => (g(t, e, "read from private field"), e.get(t)), f = (t, e, i) => e.has(t) ? C("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, i), y = (t, e, i, s) => (g(t, e, "write to private field"), e.set(t, i), i), l = (t, e, i) => (g(t, e, "access private method"), i), c, h, a, P, v, b, w, F;
const S = "activecampaign-forms-modal";
let r = class extends $ {
  constructor() {
    super(), f(this, a), f(this, c), f(this, h), this._loading = !1, this._forms = [], this._filteredForms = [], this._currentPageNumber = 1, this._totalPages = 1, this.consumeContext(T, (t) => {
      t && (y(this, c, t), this.observe(t.configurationModel, (e) => {
        y(this, h, e);
      }));
    });
  }
  async connectedCallback() {
    super.connectedCallback(), l(this, a, P).call(this);
  }
  _renderFilter() {
    return n` <uui-input
			type="search"
			id="filter"
			@input="${l(this, a, b)}"
			placeholder="Type to filter..."
			label="Type to filter forms">
			<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
		</uui-input>`;
  }
  _onSelect(t) {
    this.value = { form: t }, this._submitModal();
  }
  async _showError(t) {
    const e = await this.getContext(O);
    e == null || e.peek("danger", {
      data: { message: t }
    });
  }
  _renderForm(t) {
    return n`
            <uui-ref-node-form
                selectable
                name=${t.name ?? ""}
                @selected=${() => this._onSelect(t)}>
            </uui-ref-node-form>
        `;
  }
  render() {
    return n`
            <umb-body-layout>
                <uui-box headline=${this.data.headline}>
                    ${this._renderFilter()}
                    ${this._loading ? n`<div class="center"><uui-loader></uui-loader></div>` : ""}
                    ${M(this._filteredForms, (t) => this._renderForm(t))}
                    ${l(this, a, F).call(this)}
                </uui-box>

                <uui-button slot="actions" label=${this.localize.term("general_close")} @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
  }
};
c = /* @__PURE__ */ new WeakMap();
h = /* @__PURE__ */ new WeakMap();
a = /* @__PURE__ */ new WeakSet();
P = async function() {
  if (!(!d(this, c) || !d(this, h))) {
    if (!d(this, h).isApiConfigurationValid) {
      this._showError("Invalid API configuration.");
      return;
    }
    await l(this, a, v).call(this);
  }
};
v = async function(t) {
  this._loading = !0;
  const { data: e } = await d(this, c).getForms(t);
  if (!e) {
    this._loading = !1;
    return;
  }
  this._totalPages = e.meta.totalPages, this._forms = e.forms ?? [], this._filteredForms = this._forms, this._loading = !1;
};
b = function(t) {
  let e = t.target.value || "";
  e = e.toLowerCase();
  const i = e ? this._forms.filter((s) => s.name.toLowerCase().includes(e)) : this._forms;
  this._filteredForms = i;
};
w = async function(t) {
  var e;
  this._currentPageNumber = (e = t.target) == null ? void 0 : e.current, await l(this, a, v).call(this, this._currentPageNumber);
};
F = function() {
  return n`
            ${this._totalPages > 1 ? n`
                    <div class="activecampaign-pagination">
                        <uui-pagination
					        class="pagination"
					        .current=${this._currentPageNumber}
					        .total=${this._totalPages}
					        @change=${l(this, a, w)}></uui-pagination>
                    </div>
                 ` : E}
        `;
};
r.styles = [
  N`
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

            .center {
                display: grid;
                place-items: center;
            }

            .activecampaign-pagination {
                width: 50%;
                margin-top: 10px;
                margin-left: auto;
                margin-right: auto;
            }
        `
];
u([
  _()
], r.prototype, "_loading", 2);
u([
  _()
], r.prototype, "_forms", 2);
u([
  _()
], r.prototype, "_filteredForms", 2);
u([
  _()
], r.prototype, "_currentPageNumber", 2);
u([
  _()
], r.prototype, "_totalPages", 2);
r = u([
  A(S)
], r);
export {
  r as default
};
//# sourceMappingURL=activecampaign-forms-modal.element-9b0btnab.js.map
