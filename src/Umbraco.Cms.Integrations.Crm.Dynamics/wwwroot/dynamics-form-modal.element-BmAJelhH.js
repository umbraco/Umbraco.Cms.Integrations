import { html as n, repeat as E, css as I, state as u, customElement as T } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as $ } from "@umbraco-cms/backoffice/modal";
import { D as S } from "./index-4U7V6RVH.js";
import { UMB_NOTIFICATION_CONTEXT as k } from "@umbraco-cms/backoffice/notification";
const _ = {
  OUTBOUND: 1,
  REAL_TIME: 2,
  BOTH: 3
};
function W(e) {
  switch (e) {
    case "1":
      return _.OUTBOUND;
    case "2":
      return _.REAL_TIME;
  }
}
var D = Object.defineProperty, N = Object.getOwnPropertyDescriptor, v = (e) => {
  throw TypeError(e);
}, l = (e, t, i, o) => {
  for (var s = o > 1 ? void 0 : o ? N(t, i) : t, f = e.length - 1, p; f >= 0; f--)
    (p = e[f]) && (s = (o ? p(t, i, s) : p(s)) || s);
  return o && s && D(t, i, s), s;
}, y = (e, t, i) => t.has(e) || v("Cannot " + i), c = (e, t, i) => (y(e, t, "read from private field"), i ? i.call(e) : t.get(e)), g = (e, t, i) => t.has(e) ? v("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), b = (e, t, i, o) => (y(e, t, "write to private field"), t.set(e, i), i), h = (e, t, i) => (y(e, t, "access private method"), i), d, m, a, w, F, O, C, M;
const U = "dynamics-forms-modal";
let r = class extends $ {
  constructor() {
    super(), g(this, a), g(this, d), g(this, m), this._loading = !1, this._forms = [], this._filteredForms = [], this._selectedForm = {
      id: "",
      module: _.BOTH,
      name: "",
      rawHtml: "",
      standaloneHtml: "",
      iframeEmbedded: !1
    }, this.renderWithIFrame = !1, this.toggleLabel = "Render with Script", this.consumeContext(S, (e) => {
      e && (b(this, d, e), this.observe(e.settingsModel, (t) => {
        b(this, m, t);
      }));
    });
  }
  async connectedCallback() {
    super.connectedCallback(), await h(this, a, w).call(this);
  }
  async _onSelect(e) {
    this._selectedForm = e;
  }
  async _onSubmit() {
    if (this.renderWithIFrame && W(this._selectedForm.module.toString()) == _.OUTBOUND) {
      var { data: e } = await c(this, d).getEmbedCode(this._selectedForm.id);
      if (!e || e.result.length == 0)
        return this._showError("Unable to embed selected form. Please check if it is live."), !1;
    }
    this._selectedForm.iframeEmbedded = this.renderWithIFrame, this.value = { selectedForm: this._selectedForm }, this._submitModal();
  }
  async _showError(e) {
    await this._showMessage(e, "danger");
  }
  async _showMessage(e, t) {
    const i = await this.getContext(k);
    i == null || i.peek(t, {
      data: { message: e }
    });
  }
  onMessageOnSubmitIsHtmlChange() {
    this.renderWithIFrame = !this.renderWithIFrame, this.toggleLabel = this.renderWithIFrame ? "Render with iFrame" : "Render with Script";
  }
  _renderFilter() {
    return n` <uui-input
			type="search"
			id="filter"
			@input="${h(this, a, M)}"
			placeholder="Type to filter..."
			label="Type to filter forms">
			<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
		</uui-input>`;
  }
  render() {
    return n`
            <umb-body-layout>
            ${this._loading ? n`<div class="center loader"><uui-loader></uui-loader></div>` : n`
                    <uui-box headline=${this.data.headline}>
                            ${this._renderFilter()}
                            ${this._filteredForms.length > 0 ? n`
                                    ${E(this._filteredForms, (e) => n`
                                        <uui-ref-node-form
                                            selectable
                                            ?selected=${this._selectedForm.id == e.id}
                                            name=${e.name ?? ""}
                                            @open=${() => this._onSelect(e)}>
                                        </uui-ref-node-form>
                                    `)}
                                    <uui-toggle
                                        ?checked=${this.renderWithIFrame}
                                        .label=${this.toggleLabel}
                                        @change=${this.onMessageOnSubmitIsHtmlChange}></uui-toggle>
                                ` : n``}
                        </uui-box>

                    <br />

                    <uui-box headline="Dynamics - OAuth Status">
                        <dynamics-authorization @connect=${h(this, a, O)} @revoke=${h(this, a, C)}></dynamics-authorization>
                    </uui-box>
                `}

                <uui-button look="primary" slot="actions" label="Submit" @click=${this._onSubmit}></uui-button>
                <uui-button slot="actions" label=${this.localize.term("general_close")} @click=${this._rejectModal}></uui-button>
            </umb-body-layout>
        `;
  }
};
d = /* @__PURE__ */ new WeakMap();
m = /* @__PURE__ */ new WeakMap();
a = /* @__PURE__ */ new WeakSet();
w = async function() {
  c(this, m) && c(this, m).isAuthorized && await h(this, a, F).call(this);
};
F = async function() {
  var t;
  this._loading = !0;
  const { data: e } = await c(this, d).getForms((t = this.data) == null ? void 0 : t.module);
  e && (this._forms = e, this._filteredForms = e, this._loading = !1);
};
O = async function() {
  await h(this, a, F).call(this);
};
C = async function() {
  this._filteredForms = [], await c(this, d).checkOauthConfiguration();
};
M = function(e) {
  let t = e.target.value || "";
  t = t.toLowerCase();
  const i = t ? this._forms.filter((o) => {
    var s;
    return (s = o.name) == null ? void 0 : s.toLowerCase().includes(t);
  }) : this._forms;
  this._filteredForms = i;
};
r.styles = [
  I`
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
l([
  u()
], r.prototype, "_loading", 2);
l([
  u()
], r.prototype, "_forms", 2);
l([
  u()
], r.prototype, "_filteredForms", 2);
l([
  u()
], r.prototype, "_selectedForm", 2);
l([
  u()
], r.prototype, "renderWithIFrame", 2);
l([
  u()
], r.prototype, "toggleLabel", 2);
r = l([
  T(U)
], r);
export {
  r as default
};
//# sourceMappingURL=dynamics-form-modal.element-BmAJelhH.js.map
