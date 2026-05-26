import { UmbLitElement as x } from "@umbraco-cms/backoffice/lit-element";
import { html as u, nothing as C, css as N, state as r, customElement as Z } from "@umbraco-cms/backoffice/external/lit";
import { ZAPIER_CONTEXT_TOKEN as I } from "./zapier.context-X4E0FeNc.js";
import { UmbPaginationManager as T } from "@umbraco-cms/backoffice/utils";
var A = Object.defineProperty, M = Object.getOwnPropertyDescriptor, w = (t) => {
  throw TypeError(t);
}, i = (t, e, a, o) => {
  for (var s = o > 1 ? void 0 : o ? M(e, a) : e, g = t.length - 1, _; g >= 0; g--)
    (_ = t[g]) && (s = (o ? _(e, a, s) : _(s)) || s);
  return o && s && A(e, a, s), s;
}, y = (t, e, a) => e.has(t) || w("Cannot " + a), l = (t, e, a) => (y(t, e, "read from private field"), a ? a.call(t) : e.get(t)), p = (t, e, a) => e.has(t) ? w("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, a), b = (t, e, a, o) => (y(t, e, "write to private field"), e.set(t, a), a), f = (t, e, a) => (y(t, e, "access private method"), a), h, v, m, d, c, P, k, E;
const W = "zapier-management-dashboard";
let n = class extends x {
  constructor() {
    super(), p(this, c), p(this, h), p(this, v, new T()), p(this, m, []), p(this, d, !1), this._tableItems = [], this._currentPageNumber = 1, this._totalPages = 1, this._tableColumns = [
      {
        name: "Identifer",
        alias: "identifer"
      },
      {
        name: "Entity Type",
        alias: "entityType"
      },
      {
        name: "Hook URL",
        alias: "hookUrl"
      }
    ], this.consumeContext(I, (t) => {
      b(this, h, t);
    });
  }
  async connectedCallback() {
    super.connectedCallback(), await this.getAll(), await this.checkFormsExtension();
  }
  async getAll() {
    const { data: t } = await l(this, h).getAll();
    t && (b(this, m, t), f(this, c, P).call(this, l(this, m)));
  }
  async checkFormsExtension() {
    var e;
    const t = await ((e = l(this, h)) == null ? void 0 : e.checkFormsExtensionInstalled());
    t && b(this, d, t.data);
  }
  render() {
    return u`
            <umb-body-layout>
                <uui-box headline="Content Properties">
                    <p>
                        <a href="https://zapier.com/">Zapier</a> is an online platform that helps you automate workflows by connecting your apps and services you use.
                        This allows you to automate tasks without having to build this integration yourself.
                        When an event happens in one app, Zapier can tell another app to perform (or do) a particular action - no code necessary.
                    </p>
                    <p>
                        The heart of any automation boils down to this simple command: <b>WHEN</b> <span>this happens</span> <b>THEN</b> <span>do that</span>.
                    </p>
                    <p>
                        A Zap is an automated workflow that tells your apps to follow this simple command: "When this happens, do that."
                        Every Zap has a trigger and one or more actions. A trigger is an event that starts a Zap, and an action is what your Zap does for you.
                    </p>
                    <p>
                        Zap triggers use webhooks to execute the actions. Webhooks are automated messages sent from apps when something happens.
                    </p>
                    ${l(this, d) ? u`
                        <p>
                            You can initiate your automation when a content item of a particular document type is published or a form is submitted in Umbraco.
                        </p>
                        ` : u`
                        <p>
                            You can initiate your automation when a content item of a particular document type is published in Umbraco.
                        </p>
                        `}
                    <p>
                        The integration uses Zapier subscription hook triggers, allowing Zapier to set up and remove hook subscriptions when Zaps are created or removed on the platform.
                    </p>
                </uui-box>
                <uui-box id="subscriptionHooks" headline="Registed Subscription Hooks">
                        <umb-table
                            .columns=${this._tableColumns} 
                            .items=${this._tableItems}>
                        </umb-table>
                        ${f(this, c, k).call(this)}
                    </uui-box>
            </umb-body-layout>
		`;
  }
};
h = /* @__PURE__ */ new WeakMap();
v = /* @__PURE__ */ new WeakMap();
m = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakSet();
P = function(t) {
  this._tableItems = t.map((e) => ({
    id: e.id.toString(),
    data: [
      {
        columnAlias: "identifer",
        value: e.entityId
      },
      {
        columnAlias: "entityType",
        value: e.type
      },
      {
        columnAlias: "hookUrl",
        value: e.hookUrl
      }
    ]
  }));
};
k = function() {
  return u`
            ${this._totalPages > 1 ? u`
                <div class="shopify-pagination">
                    <uui-pagination
					    class="pagination"
					    .current=${this._currentPageNumber}
					    .total=${this._totalPages}
					    @change=${f(this, c, E)}></uui-pagination>
                </div>
             ` : C}
        `;
};
E = function(t) {
  var o;
  const a = ((o = t.target) == null ? void 0 : o.current) > this._currentPageNumber ? this._currentPageNumber + 1 : this._currentPageNumber - 1;
  l(this, v).setCurrentPageNumber(a), this._currentPageNumber = a;
};
n.styles = [N`
        #subscriptionHooks {
            margin-top: 20px;
        }
    `];
i([
  r()
], n.prototype, "_tableItems", 2);
i([
  r()
], n.prototype, "_currentPageNumber", 2);
i([
  r()
], n.prototype, "_totalPages", 2);
i([
  r()
], n.prototype, "_nextPageInfo", 2);
i([
  r()
], n.prototype, "_previousPageInfo", 2);
i([
  r()
], n.prototype, "_tableColumns", 2);
n = i([
  Z(W)
], n);
const S = n;
export {
  n as ZapierManagementDashboardElement,
  S as default
};
//# sourceMappingURL=zapier-management-dashboard.element-DZFmtCWl.js.map
