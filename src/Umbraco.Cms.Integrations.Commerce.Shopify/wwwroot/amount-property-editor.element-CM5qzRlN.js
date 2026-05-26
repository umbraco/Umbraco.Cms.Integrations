import { html as y, property as l, customElement as d } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as E } from "@umbraco-cms/backoffice/lit-element";
import { UmbChangeEvent as c } from "@umbraco-cms/backoffice/event";
var x = Object.defineProperty, N = Object.getOwnPropertyDescriptor, v = (t) => {
  throw TypeError(t);
}, h = (t, e, n, p) => {
  for (var a = p > 1 ? void 0 : p ? N(e, n) : e, o = t.length - 1, m; o >= 0; o--)
    (m = t[o]) && (a = (p ? m(e, n, a) : m(a)) || a);
  return p && a && x(e, n, a), a;
}, b = (t, e, n) => e.has(t) || v("Cannot " + n), w = (t, e, n) => e.has(t) ? v("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, n), r = (t, e, n) => (b(t, e, "access private method"), n), i, s, f, _;
const A = "shopify-amount";
let u = class extends E {
  constructor() {
    super(...arguments), w(this, i);
  }
  set config(t) {
    t && (this.min = r(this, i, s).call(this, t.getValueByAlias("amountMin")), this.max = r(this, i, s).call(this, t.getValueByAlias("amountMax")));
  }
  render() {
    return y`
            <div>
                <uui-input
				    type="number"
				    .value=${this.min}
				    @input=${r(this, i, f)}></uui-input>
			    </uui-input>
                <span>-</span>
                <uui-input
				    type="number"
				    .value=${this.max}
				    @input=${r(this, i, _)}></uui-input>
			    </uui-input>
            </div>
        `;
  }
};
i = /* @__PURE__ */ new WeakSet();
s = function(t) {
  const e = Number(t);
  return Number.isNaN(e) ? void 0 : e;
};
f = function(t) {
  this.min = r(this, i, s).call(this, t.target.value), this.dispatchEvent(new c());
};
_ = function(t) {
  this.max = r(this, i, s).call(this, t.target.value), this.dispatchEvent(new c());
};
h([
  l({ type: Number })
], u.prototype, "min", 2);
h([
  l({ type: Number })
], u.prototype, "max", 2);
u = h([
  d(A)
], u);
const S = u;
export {
  u as ShopifyAmountElement,
  S as default
};
//# sourceMappingURL=amount-property-editor.element-CM5qzRlN.js.map
