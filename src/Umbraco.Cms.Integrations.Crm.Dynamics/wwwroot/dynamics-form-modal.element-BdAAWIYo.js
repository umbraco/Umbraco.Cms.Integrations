import { html as n, repeat as T, css as $, state as u, customElement as I } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as S } from "@umbraco-cms/backoffice/modal";
import { D as k } from "./index-BR4ZtEYT.js";
import { UMB_NOTIFICATION_CONTEXT as M } from "@umbraco-cms/backoffice/notification";
var c = /* @__PURE__ */ ((e) => (e.OUTBOUND = "Outbound", e.REAL_TIME = "RealTime", e.BOTH = "Both", e))(c || {});
function W(e) {
  switch (e) {
    case "1":
      return c.OUTBOUND;
    case "2":
      return c.REAL_TIME;
  }
}
var N = Object.defineProperty, U = Object.getOwnPropertyDescriptor, v = (e) => {
  throw TypeError(e);
}, l = (e, t, i, o) => {
  for (var r = o > 1 ? void 0 : o ? U(t, i) : t, f = e.length - 1, p; f >= 0; f--)
    (p = e[f]) && (r = (o ? p(t, i, r) : p(r)) || r);
  return o && r && N(t, i, r), r;
}, y = (e, t, i) => t.has(e) || v("Cannot " + i), m = (e, t, i) => (y(e, t, "read from private field"), i ? i.call(e) : t.get(e)), g = (e, t, i) => t.has(e) ? v("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), b = (e, t, i, o) => (y(e, t, "write to private field"), t.set(e, i), i), h = (e, t, i) => (y(e, t, "access private method"), i), d, _, a, w, F, O, C, E;
const A = "dynamics-forms-modal";
let s = class extends S {
  constructor() {
    super(), g(this, a), g(this, d), g(this, _), this._loading = !1, this._forms = [], this._filteredForms = [], this._selectedForm = {
      id: "",
      module: c.BOTH,
      name: "",
      rawHtml: "",
      standaloneHtml: "",
      iframeEmbedded: !1
    }, this.renderWithIFrame = !1, this.toggleLabel = "Render with Script", this.consumeContext(k, (e) => {
      e && (b(this, d, e), this.observe(e.settingsModel, (t) => {
        b(this, _, t);
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
    if (this.renderWithIFrame && W(this._selectedForm.module.toString()) == c.OUTBOUND) {
      var { data: e } = await m(this, d).getEmbedCode(this._selectedForm.id);
      if (!e || e.result.length == 0)
        return this._showError("Unable to embed selected form. Please check if it is live."), !1;
    }
    this._selectedForm.iframeEmbedded = this.renderWithIFrame, this.value = { selectedForm: this._selectedForm }, this._submitModal();
  }
  async _showError(e) {
    await this._showMessage(e, "danger");
  }
  async _showMessage(e, t) {
    const i = await this.getContext(M);
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
			@input="${h(this, a, E)}"
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
                                    ${T(this._filteredForms, (e) => n`
                                        <uui-ref-node-form
                                            selectable
                                            ?selected=${this._selectedForm.id == e.id}
                                            name=${e.name ?? ""}
                                            @click=${() => this._onSelect(e)}
                                            @keydown=${(t) => t.key === " " && this._onSelect(e)}>
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
_ = /* @__PURE__ */ new WeakMap();
a = /* @__PURE__ */ new WeakSet();
w = async function() {
  m(this, _) && m(this, _).isAuthorized && await h(this, a, F).call(this);
};
F = async function() {
  var t;
  this._loading = !0;
  const { data: e } = await m(this, d).getForms((t = this.data) == null ? void 0 : t.module);
  e && (this._forms = e, this._filteredForms = e, this._loading = !1);
};
O = async function() {
  await h(this, a, F).call(this);
};
C = async function() {
  this._filteredForms = [], await m(this, d).checkOauthConfiguration();
};
E = function(e) {
  let t = e.target.value || "";
  t = t.toLowerCase();
  const i = t ? this._forms.filter((o) => {
    var r;
    return (r = o.name) == null ? void 0 : r.toLowerCase().includes(t);
  }) : this._forms;
  this._filteredForms = i;
};
s.styles = [
  $`
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
], s.prototype, "_loading", 2);
l([
  u()
], s.prototype, "_forms", 2);
l([
  u()
], s.prototype, "_filteredForms", 2);
l([
  u()
], s.prototype, "_selectedForm", 2);
l([
  u()
], s.prototype, "renderWithIFrame", 2);
l([
  u()
], s.prototype, "toggleLabel", 2);
s = l([
  I(A)
], s);
export {
  s as default
};
//# sourceMappingURL=dynamics-form-modal.element-BdAAWIYo.js.map
