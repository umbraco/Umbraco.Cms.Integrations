import { UmbModalBaseElement as T } from "@umbraco-cms/backoffice/modal";
import { html as c, repeat as M, nothing as $, css as E, state as h, customElement as A } from "@umbraco-cms/backoffice/external/lit";
import { ACTIVECAMPAIGN_FORMS_CONTEXT_TOKEN as O } from "./activecampaign-forms.context-CtsKAXiI.js";
import { UMB_NOTIFICATION_CONTEXT as k } from "@umbraco-cms/backoffice/notification";
var I = Object.defineProperty, x = Object.getOwnPropertyDescriptor, P = (t) => {
  throw TypeError(t);
}, l = (t, e, i, _) => {
  for (var n = _ > 1 ? void 0 : _ ? x(e, i) : e, g = t.length - 1, v; g >= 0; g--)
    (v = t[g]) && (n = (_ ? v(e, i, n) : v(n)) || n);
  return _ && n && I(e, i, n), n;
}, b = (t, e, i) => e.has(t) || P("Cannot " + i), s = (t, e, i) => (b(t, e, "read from private field"), e.get(t)), p = (t, e, i) => e.has(t) ? P("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, i), y = (t, e, i, _) => (b(t, e, "write to private field"), e.set(t, i), i), o = (t, e, i) => (b(t, e, "access private method"), i), m, d, u, a, C, f, w, F, N;
const S = "activecampaign-forms-modal";
let r = class extends T {
  constructor() {
    super(), p(this, a), p(this, m), p(this, d), this._loading = !1, this._forms = [], this._filteredForms = [], this._currentPageNumber = 1, this._totalPages = 1, this._searchQuery = "", p(this, u), this.consumeContext(O, (t) => {
      t && (y(this, m, t), this.observe(t.configurationModel, (e) => {
        y(this, d, e);
      }));
    });
  }
  async connectedCallback() {
    super.connectedCallback(), o(this, a, C).call(this);
  }
  disconnectedCallback() {
    super.disconnectedCallback(), s(this, u) && clearTimeout(s(this, u));
  }
  _renderFilter() {
    return c` <uui-input
			type="search"
			id="filter"
			@input="${o(this, a, w)}"
			placeholder="Type to filter..."
			label="Type to filter forms">
			<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
		</uui-input>`;
  }
  _onSelect(t) {
    this.value = { form: t }, this._submitModal();
  }
  async _showError(t) {
    const e = await this.getContext(k);
    e == null || e.peek("danger", {
      data: { message: t }
    });
  }
  _renderForm(t) {
    return c`
            <uui-ref-node-form
                name=${t.name ?? ""}
                @open=${() => this._onSelect(t)}>
            </uui-ref-node-form>
        `;
  }
  render() {
    return c`
            <umb-body-layout>
                <uui-box headline=${this.data.headline}>
                    ${this._renderFilter()}
                    ${this._loading ? c`<div class="center"><uui-loader></uui-loader></div>` : ""}
                    ${M(this._filteredForms, (t) => this._renderForm(t))}
                    ${o(this, a, N).call(this)}
                </uui-box>

                <uui-button slot="actions" label=${this.localize.term("general_close")} @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
  }
};
m = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakMap();
u = /* @__PURE__ */ new WeakMap();
a = /* @__PURE__ */ new WeakSet();
C = async function() {
  if (!(!s(this, m) || !s(this, d))) {
    if (!s(this, d).isApiConfigurationValid) {
      this._showError("Invalid API configuration.");
      return;
    }
    await o(this, a, f).call(this);
  }
};
f = async function(t, e) {
  this._loading = !0;
  const { data: i } = await s(this, m).getForms(t, e);
  if (!i) {
    this._loading = !1;
    return;
  }
  this._totalPages = Number(i.meta.totalPages), this._forms = i.forms ?? [], this._filteredForms = this._forms, this._loading = !1;
};
w = async function(t) {
  let e = t.target.value || "";
  e = e.toLowerCase(), this._searchQuery = e, s(this, u) && clearTimeout(s(this, u)), y(this, u, setTimeout(async () => {
    this._currentPageNumber = 1, await o(this, a, f).call(this, this._currentPageNumber, this._searchQuery);
  }, 500));
};
F = async function(t) {
  var e;
  this._currentPageNumber = (e = t.target) == null ? void 0 : e.current, await o(this, a, f).call(this, this._currentPageNumber, this._searchQuery);
};
N = function() {
  return c`
            ${this._totalPages > 1 ? c`
                    <div class="activecampaign-pagination">
                        <uui-pagination
					        class="pagination"
					        .current=${this._currentPageNumber}
					        .total=${this._totalPages}
					        @change=${o(this, a, F)}></uui-pagination>
                    </div>
                 ` : $}
        `;
};
r.styles = [
  E`
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
l([
  h()
], r.prototype, "_loading", 2);
l([
  h()
], r.prototype, "_forms", 2);
l([
  h()
], r.prototype, "_filteredForms", 2);
l([
  h()
], r.prototype, "_currentPageNumber", 2);
l([
  h()
], r.prototype, "_totalPages", 2);
l([
  h()
], r.prototype, "_searchQuery", 2);
r = l([
  A(S)
], r);
export {
  r as default
};
//# sourceMappingURL=activecampaign-forms-modal.element-nHGD1sQ7.js.map
