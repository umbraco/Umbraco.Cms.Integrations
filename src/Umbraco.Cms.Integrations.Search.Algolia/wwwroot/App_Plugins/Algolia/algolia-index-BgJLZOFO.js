import { LitElement as w, html as a, nothing as h, css as I, property as g, state as m, customElement as C } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as $ } from "@umbraco-cms/backoffice/element-api";
import { UMB_NOTIFICATION_CONTEXT as N } from "@umbraco-cms/backoffice/notification";
import { ALGOLIA_CONTEXT_TOKEN as P } from "./algolia-index.context-BcCnOnU6.js";
var E = Object.defineProperty, D = Object.getOwnPropertyDescriptor, x = (e) => {
  throw TypeError(e);
}, c = (e, t, i, n) => {
  for (var s = n > 1 ? void 0 : n ? D(t, i) : t, d = e.length - 1, r; d >= 0; d--)
    (r = e[d]) && (s = (n ? r(t, i, s) : r(s)) || s);
  return n && s && E(t, i, s), s;
}, v = (e, t, i) => t.has(e) || x("Cannot " + i), u = (e, t, i) => (v(e, t, "read from private field"), i ? i.call(e) : t.get(e)), _ = (e, t, i) => t.has(e) ? x("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), f = (e, t, i, n) => (v(e, t, "write to private field"), t.set(e, i), i), p, l;
let o = class extends $(w) {
  constructor() {
    super(), _(this, p), _(this, l), this._model = {
      id: 0,
      name: "",
      contentData: []
    }, this._contentTypes = [], this.consumeContext(N, (e) => {
      f(this, p, e);
    }), this.consumeContext(P, (e) => {
      f(this, l, e);
    }), this._showContentTypeProperties = !1;
  }
  connectedCallback() {
    super.connectedCallback(), this.indexId.length > 0 ? (this._getContentTypesWithIndex(), this._getIndex()) : this._getContentTypes();
  }
  render() {
    return a`
            <uui-box headline="${this.indexId.length > 0 ? "Create Index Definition" : "Edit Index Definition"}">
                <uui-form>
                    <form id="manageIndexFrm" name="manageIndexFrm" @submit=${this.handleSubmit}>
                        <uui-form-layout-item>
                            <uui-label slot="label" for="inName" required="">Name</uui-label>
                            <span class="alg-description" slot="description">Please enter a name for the index. After save,<br /> you will not be able to change it.</span>
                            <div>
                                ${this.indexId.length > 0 ? a`<uui-input type="text" name="indexName" label="indexName" disabled .value=${this._model.name} style="width: 17%"></uui-input>` : a`<uui-input type="text" name="indexName" label="indexName" .value=${this._model.name} style="width: 17%"></uui-input>`}
                                
                            </div>
                        </uui-form-layout-item>

                        <div class="alg-col-2">
                            <uui-form-layout-item>
                                <uui-label slot="label">Document Types</uui-label>
                                <span class="alg-description" slot="description">Please select the document types you would like to index, and choose the fields to include.</span>
                                <uui-icon-registry-essential>
                                    ${this.renderContentTypes()}
                                </uui-icon-registry-essential>
                            </uui-form-layout-item>
                            ${this.renderContentTypeProperties()}
                        </div>
                        <uui-button type="submit" label="Save" look="primary" color="positive">
                            Save
                        </uui-button>
                    </form>
                </uui-form>

            </uui-box>
        `;
  }
  async _getContentTypes() {
    var e;
    await ((e = u(this, l)) == null ? void 0 : e.getContentTypes().then((t) => {
      this._contentTypes = t;
    }).catch((t) => this._showError(t.message)));
  }
  async _getContentTypesWithIndex() {
    var e;
    await ((e = u(this, l)) == null ? void 0 : e.getContentTypesWithIndex(Number(this.indexId)).then((t) => {
      var i = t;
      this._contentTypes = i;
    }).catch((t) => this._showError(t.message)));
  }
  async _getIndex() {
    var e;
    await ((e = u(this, l)) == null ? void 0 : e.getIndexById(Number(this.indexId)).then((t) => {
      var i = t;
      this._model = i, this.indexName = i.name;
    }).catch((t) => this._showError(t.message)));
  }
  // render
  renderContentTypes() {
    return this._contentTypes.length == 0 ? h : a`
            ${this._contentTypes.map((e) => a`
                    <uui-ref-node id="dc_${e.alias}_${e.id}"
                                selectable
                                name=${e.name}
                                @selected=${() => this._contentTypeSelected(e.id)}
                                @deselected=${() => this._contentTypeDeselected(e.id)}>
                        <uui-icon slot="icon" name=${e.icon}></uui-icon>
                        ${e.selected ? a`<uui-tag size="s" slot="tag" color="positive">Selected</uui-tag>` : ""}
                        <uui-action-bar slot="actions">
                            <uui-button label="Remove" color="danger">
                                <uui-icon name="delete"></uui-icon>
                            </uui-button>
                        </uui-action-bar>
                    </uui-ref-node>
                    `)}
            `;
  }
  renderContentTypeProperties() {
    if (this._showContentTypeProperties === !1) return h;
    var e = this._contentTypes.find((t) => t.selected == !0);
    return e === void 0 ? h : a`
            <uui-form-layout-item>
                <uui-label slot="label">${e.name} Properties</uui-label>
                    <div class="alg-col-3">
                        ${e.properties.map((t) => a`
                                <uui-card-content-node selectable
                                            @selected=${() => this._contentTypePropertySelected(e, t.id)}
                                            @deselected=${() => this._contentTypePropertyDeselected(e, t.id)}
                                            name=${t.name}>
                                    ${t.selected ? a`<uui-tag size="s" slot="tag" color="positive">Selected</uui-tag>` : ""}
                                    <ul style="list-style: none; padding-inline-start: 0px; margin: 0;">
                                        <li><span style="font-weight: 700">Group: </span> ${t.group}</li>
                                    </ul>
                                </uui-card-content-node>
                            `)}
                    </div>
            </uui-form-layout-item>
        `;
  }
  async _contentTypeSelected(e) {
    this._contentTypes = this._contentTypes.map((t) => (t.id == e && (t.selected = !0), t)), this._showContentTypeProperties = !0;
  }
  async _contentTypeDeselected(e) {
    this._contentTypes = this._contentTypes.map((t) => (t.id == e && (t.selected = !1), t)), this._showContentTypeProperties = !1;
  }
  async _contentTypePropertySelected(e, t) {
    e !== void 0 && (this._contentTypes = this._contentTypes.map((i) => (i.id != e.id || (i.properties = i.properties.map((n) => (n.id == t && (n.selected = !0), n))), i)));
  }
  async _contentTypePropertyDeselected(e, t) {
    e != null && (this._contentTypes = this._contentTypes.map((i) => (i.id != e.id || (i.properties = i.properties.map((n) => (n.id == t && (n.selected = !1), n))), i)));
  }
  async handleSubmit(e) {
    var d;
    e.preventDefault();
    const t = e.target, i = new FormData(t);
    var n = this.indexId.length > 0 ? this.indexName : i.get("indexName");
    if (n.length == 0 || this._contentTypes === void 0 || this._contentTypes.filter((r) => r.selected).length == 0) {
      this._showError("Index name and content schema are required.");
      return;
    }
    var s = {
      id: 0,
      name: n,
      contentData: []
    };
    this.indexId.length > 0 && (s.id = Number(this.indexId)), s.contentData = this._contentTypes, await ((d = u(this, l)) == null ? void 0 : d.saveIndex(s).then((r) => {
      var y = r;
      if (y.success) {
        this._showSuccess("Index saved.");
        const T = this.indexId.length > 0 ? window.location.href.replace(`/index/${this.indexId}`, "") : window.location.href.replace("/index", "");
        window.history.pushState({}, "", T);
      } else
        this._showError(y.error);
    }).catch((r) => this._showError(r.message)));
  }
  // notifications
  _showSuccess(e) {
    var t;
    (t = u(this, p)) == null || t.peek("positive", {
      data: { message: e }
    });
  }
  _showError(e) {
    var t;
    (t = u(this, p)) == null || t.peek("danger", {
      data: { message: e }
    });
  }
};
p = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakMap();
o.styles = [
  I`
          .center {
            display: grid;
            place-items: center;
          }
          .alg-col-2 {
            display: grid;
            grid-template-columns: 25% 60%;
            gap: 20px;
          }
          .alg-col-3 {
            display: grid;
            grid-template-columns: 33% 33% 33%;
            gap: 10px;
          }
        `
];
c([
  g()
], o.prototype, "indexId", 2);
c([
  g()
], o.prototype, "indexName", 2);
c([
  m()
], o.prototype, "_model", 2);
c([
  m()
], o.prototype, "_contentTypes", 2);
c([
  m()
], o.prototype, "_showContentTypeProperties", 2);
o = c([
  C("algolia-index")
], o);
const k = o;
export {
  o as AlgoliaIndexElement,
  k as default
};
//# sourceMappingURL=algolia-index-BgJLZOFO.js.map
