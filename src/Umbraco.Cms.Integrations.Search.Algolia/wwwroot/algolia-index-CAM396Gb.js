import { LitElement as D, nothing as m, html as a, css as N, property as S, state as f, customElement as A } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as W } from "@umbraco-cms/backoffice/element-api";
import { UMB_NOTIFICATION_CONTEXT as k } from "@umbraco-cms/backoffice/notification";
import { ALGOLIA_CONTEXT_TOKEN as M } from "./algolia-index.context-FyjSKGue.js";
var z = Object.defineProperty, F = Object.getOwnPropertyDescriptor, g = (e) => {
  throw TypeError(e);
}, u = (e, t, n, i) => {
  for (var o = i > 1 ? void 0 : i ? F(t, n) : t, p = e.length - 1, h; p >= 0; p--)
    (h = e[p]) && (o = (i ? h(t, n, o) : h(o)) || o);
  return i && o && z(t, n, o), o;
}, _ = (e, t, n) => t.has(e) || g("Cannot " + n), c = (e, t, n) => (_(e, t, "read from private field"), n ? n.call(e) : t.get(e)), y = (e, t, n) => t.has(e) ? g("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, n), G = (e, t, n, i) => (_(e, t, "write to private field"), t.set(e, n), n), r = (e, t, n) => (_(e, t, "access private method"), n), d, s, v, T, x, I, $, C, w, P, b, E;
const L = "algolia-index";
let l = class extends W(D) {
  constructor() {
    super(), y(this, s), y(this, d), this._model = {
      id: 0,
      name: "",
      contentData: []
    }, this._contentTypes = [], this._showContentTypeProperties = !1, this.consumeContext(M, (e) => {
      e && G(this, d, e);
    });
  }
  async connectedCallback() {
    super.connectedCallback(), this.indexId.length > 0 ? (await r(this, s, T).call(this), r(this, s, x).call(this)) : r(this, s, v).call(this);
  }
  // render
  renderContentTypes() {
    return this._contentTypes.length == 0 ? m : a`
            ${this._contentTypes.map((e) => a`
                    <uui-ref-node 
                        selectable
                        ?selected=${e.selected}
                        name=${e.name}
                        @selected=${() => r(this, s, $).call(this, e.id)}
                        @deselected=${() => r(this, s, C).call(this, e.id)}>
                        <umb-icon slot="icon" name=${e.icon}></umb-icon>
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
    if (this._showContentTypeProperties === !1) return m;
    const e = this._contentTypes.filter((t) => t.selected == !0);
    return e != null && e.length ? a`
            ${e.map((t) => a`
                <uui-form-layout-item>
                    <uui-label slot="label">${t.name} Properties</uui-label>
                        <div id="grid">
                            ${t.properties.map((n) => a`
                                    <uui-card-content-node 
                                        selectable
                                        ?selected=${n.selected}
                                        @selected=${() => r(this, s, w).call(this, t, n.id)}
                                        @deselected=${() => r(this, s, P).call(this, t, n.id)}
                                        name=${n.name}>
                                        ${n.selected ? a`<uui-tag size="s" slot="tag" color="positive">Selected</uui-tag>` : ""}
                                        <ul style="list-style: none; padding-inline-start: 0px; margin: 0;">
                                            <li><span style="font-weight: 700">Group: </span> ${n.group}</li>
                                        </ul>
                                    </uui-card-content-node>
                                `)}
                        </div>
                </uui-form-layout-item>
            `)}
        ` : m;
  }
  render() {
    return a`
            <uui-box headline=${this.indexId.length > 0 ? "Create Index Definition" : "Edit Index Definition"}>
                <uui-form>
                    <form id="manageIndexFrm" name="manageIndexFrm" @submit=${r(this, s, b)}>
                        <umb-property-layout 
                            label="Name" 
                            description="Please enter a name for the index. After save, you will not be able to change it."> 
                            <uui-input 
                                slot="editor" 
                                ?disabled=${this.indexId.length > 0} 
                                .value=${this._model.name}
                                @change=${r(this, s, I)}></uui-input>                                
                        </umb-property-layout>

                        <umb-property-layout 
                            label="Document Types" 
                            description="Please select the document types you would like to index, and choose the fields to include.">
                            <div slot="editor">
                                ${this.renderContentTypes()}
                                ${this.renderContentTypeProperties()}
                            </div>
                        </umb-property-layout>                        
          
                        <uui-button type="submit" label=${this.localize.term("buttons_save")} look="primary" color="positive"></uui-button>
                    </form>
                </uui-form>
            </uui-box>
        `;
  }
};
d = /* @__PURE__ */ new WeakMap();
s = /* @__PURE__ */ new WeakSet();
v = async function() {
  const { data: e } = await c(this, d).getContentTypes();
  e && (this._contentTypes = e);
};
T = async function() {
  const { data: e } = await c(this, d).getContentTypesWithIndex(Number(this.indexId));
  e && (this._contentTypes = e);
};
x = async function() {
  const { data: e } = await c(this, d).getIndexById(Number(this.indexId));
  e && (this._model = e, this._model.contentData.length && (this._showContentTypeProperties = !0));
};
I = function(e) {
  this._model.name = e.target.value.toString();
};
$ = async function(e) {
  this._contentTypes = this._contentTypes.map((t) => (t.id === e && (t.selected = !0), t)), this._showContentTypeProperties = !0;
};
C = async function(e) {
  this._contentTypes = this._contentTypes.map((t) => (t.id === e && (t.selected = !1), t)), this._showContentTypeProperties = this._contentTypes.filter((t) => t.selected).length !== 0;
};
w = async function(e, t) {
  e !== void 0 && (this._contentTypes = this._contentTypes.map((n) => (n.id != e.id || (n.properties = n.properties.map((i) => (i.id == t && (i.selected = !0), i))), n)));
};
P = async function(e, t) {
  e != null && (this._contentTypes = this._contentTypes.map((n) => (n.id != e.id || (n.properties = n.properties.map((i) => (i.id == t && (i.selected = !1), i))), n)));
};
b = async function(e) {
  var n, i;
  if (e.preventDefault(), this._model.name.length == 0 || this._contentTypes === void 0 || ((n = this._contentTypes) == null ? void 0 : n.filter((o) => o.selected).length) == 0) {
    r(this, s, E).call(this, "Index name and content schema are required.");
    return;
  }
  const t = {
    id: 0,
    name: this._model.name,
    contentData: []
  };
  this.indexId.length > 0 && (t.id = Number(this.indexId)), t.contentData = this._contentTypes, await ((i = c(this, d)) == null ? void 0 : i.saveIndex(t));
};
E = async function(e) {
  const t = await this.getContext(k);
  t == null || t.peek("danger", {
    data: { message: e }
  });
};
l.styles = [
  N`
          #grid {
            display: grid;
            grid-template-columns: 33% 33% 33%;
            gap: 10px;
          }
        `
];
u([
  S()
], l.prototype, "indexId", 2);
u([
  f()
], l.prototype, "_model", 2);
u([
  f()
], l.prototype, "_contentTypes", 2);
u([
  f()
], l.prototype, "_showContentTypeProperties", 2);
l = u([
  A(L)
], l);
const q = l;
export {
  l as AlgoliaIndexElement,
  q as default
};
//# sourceMappingURL=algolia-index-CAM396Gb.js.map
