import { UmbElementMixin as u } from "@umbraco-cms/backoffice/element-api";
import { LitElement as m, html as l, customElement as f } from "@umbraco-cms/backoffice/external/lit";
var v = Object.defineProperty, h = Object.getOwnPropertyDescriptor, c = (t) => {
  throw TypeError(t);
}, _ = (t, e, n, o) => {
  for (var r = o > 1 ? void 0 : o ? h(e, n) : e, a = t.length - 1, p; a >= 0; a--)
    (p = t[a]) && (r = (o ? p(e, n, r) : p(r)) || r);
  return o && r && v(e, n, r), r;
}, d = (t, e, n) => e.has(t) ? c("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, n), s;
const E = "shopify-amount";
let i = class extends u(m) {
  constructor() {
    super(...arguments), d(this, s);
  }
  render() {
    return l`
            <div>
                <uui-input></uui-input>
                <span>-</span>
                <uui-input placeholder="∞"></uui-input>
            </div>
        `;
  }
};
s = /* @__PURE__ */ new WeakMap();
i = _([
  f(E)
], i);
const x = i;
export {
  i as ShopifyAmountElement,
  x as default
};
//# sourceMappingURL=amount-property-editor.element-eAoDmYox.js.map
