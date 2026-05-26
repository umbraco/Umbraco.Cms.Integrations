var U = (t) => {
  throw TypeError(t);
};
var A = (t, e, i) => e.has(t) || U("Cannot " + i);
var E = (t, e, i) => (A(t, e, "read from private field"), i ? i.call(t) : e.get(t)), j = (t, e, i) => e.has(t) ? U("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, i), T = (t, e, i, s) => (A(t, e, "write to private field"), s ? s.call(t, i) : e.set(t, i), i);
import { UmbLitElement as M } from "@umbraco-cms/backoffice/lit-element";
import { css as D, property as R, customElement as G, when as g, html as r, state as b } from "@umbraco-cms/backoffice/external/lit";
import { UMB_NOTIFICATION_CONTEXT as Z } from "@umbraco-cms/backoffice/notification";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT as ee } from "@umbraco-cms/backoffice/document";
import { UmbControllerBase as L } from "@umbraco-cms/backoffice/class-api";
import { tryExecute as m } from "@umbraco-cms/backoffice/resources";
import { c as y } from "./index-0ywTe_NB.js";
import { DocumentService as te } from "@umbraco-cms/backoffice/external/backend-api";
class w {
  static getAuth(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/googlesearchconsole/management/api/v1/auth",
      ...e
    });
  }
  static postInspect(e) {
    return (e.client ?? y).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/googlesearchconsole/management/api/v1/inspect",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e.headers
      }
    });
  }
  static postOauthAccessToken(e) {
    return (e.client ?? y).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/googlesearchconsole/management/api/v1/oauth/access-token",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e.headers
      }
    });
  }
  static getOauthConfiguration(e) {
    return ((e == null ? void 0 : e.client) ?? y).get({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/googlesearchconsole/management/api/v1/oauth/configuration",
      ...e
    });
  }
  static postOauthRevoke(e) {
    return ((e == null ? void 0 : e.client) ?? y).post({
      security: [{ scheme: "bearer", type: "http" }],
      url: "/umbraco/googlesearchconsole/management/api/v1/oauth/revoke",
      ...e
    });
  }
}
class ie extends L {
  async getUrls(e) {
    return await m(
      this,
      te.getDocumentUrls({
        query: {
          id: [e]
        }
      })
    );
  }
}
var C;
class se extends L {
  constructor(i) {
    super(i);
    j(this, C);
    T(this, C, new ie(this));
  }
  async getOAuthConfiguration() {
    const { data: i, error: s } = await m(this, w.getOauthConfiguration());
    return s || !i ? { error: s } : { data: i };
  }
  async getAccessToken(i) {
    const { data: s, error: n } = await m(this, w.postOauthAccessToken({ body: { code: i } }));
    return n || !s ? { error: n } : { data: s };
  }
  async revokeAccessToken() {
    const { data: i, error: s } = await m(this, w.postOauthRevoke());
    return s || !i ? { error: s } : { data: i };
  }
  async getUrls(i) {
    return E(this, C).getUrls(i);
  }
  async inspect(i, s, n) {
    const { data: o, error: a } = await m(this, w.postInspect({
      body: {
        inspectionUrl: i,
        siteUrl: s,
        languageCode: n
      }
    }));
    return a || !o ? { error: a } : { data: o };
  }
}
C = new WeakMap();
var ne = Object.defineProperty, oe = Object.getOwnPropertyDescriptor, z = (t) => {
  throw TypeError(t);
}, _ = (t, e, i, s) => {
  for (var n = s > 1 ? void 0 : s ? oe(e, i) : e, o = t.length - 1, a; o >= 0; o--)
    (a = t[o]) && (n = (s ? a(e, i, n) : a(n)) || n);
  return s && n && ne(e, i, n), n;
}, ae = (t, e, i) => e.has(t) || z("Cannot " + i), re = (t, e, i) => e.has(t) ? z("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, i), P = (t, e, i) => (ae(t, e, "access private method"), i), $, W, q;
let d = class extends M {
  constructor() {
    super(...arguments), re(this, $), this.headline = "", this.headlineSlot = "", this.link = "", this.data = {}, this.content = "";
  }
  get keyValuePairs() {
    return Object.entries(this.data).map(([t, e]) => ({ key: t, value: e }));
  }
  render() {
    return r`
            <uui-box .headline=${this.headline}>
                <div slot="headline">
                    <h5>${this.headlineSlot}</h5>
                </div>
                <div>
                    ${g(this.link.length > 0, () => P(this, $, W).call(this))}
                    ${g(this.keyValuePairs.length > 0, () => P(this, $, q).call(this))}
                </div>
            </uui-box>
            <br />
        `;
  }
};
$ = /* @__PURE__ */ new WeakSet();
W = function() {
  return r`<a href=${this.link} target="_blank">${this.link}</a>`;
};
q = function() {
  return r`${this.keyValuePairs.map(({ key: t, value: e }) => r`<p><strong>${t}:</strong> ${e}</p>`)}`;
};
d.styles = [
  D`
          h5 {
            margin: 0;
            font-weight: normal;
            color: var(--uui-color-text-alt);
          }
        `
];
_([
  R({ type: String })
], d.prototype, "headline", 2);
_([
  R({ type: String })
], d.prototype, "headlineSlot", 2);
_([
  R({ type: String })
], d.prototype, "link", 2);
_([
  R({ attribute: !1 })
], d.prototype, "data", 2);
_([
  R({ type: String })
], d.prototype, "content", 2);
d = _([
  G("inspectresult-box")
], d);
var le = Object.defineProperty, ce = Object.getOwnPropertyDescriptor, N = (t) => {
  throw TypeError(t);
}, f = (t, e, i, s) => {
  for (var n = s > 1 ? void 0 : s ? ce(e, i) : e, o = t.length - 1, a; o >= 0; o--)
    (a = t[o]) && (n = (s ? a(e, i, n) : a(n)) || n);
  return s && n && le(e, i, n), n;
}, S = (t, e, i) => e.has(t) || N("Cannot " + i), u = (t, e, i) => (S(t, e, "read from private field"), i ? i.call(t) : e.get(t)), k = (t, e, i) => e.has(t) ? N("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, i), I = (t, e, i, s) => (S(t, e, "write to private field"), e.set(t, i), i), h = (t, e, i) => (S(t, e, "access private method"), i), p, v, O, l, B, V, X, F, H, K, Y, J;
const ue = "googlesearchconsole-workspace-view";
let c = class extends M {
  constructor() {
    super(), k(this, l), k(this, p, new se(this)), k(this, v), k(this, O), this._loading = !1, this._isConnected = !1, this._showResults = !1, this._authorizationUrl = "", this.inspectionObj = {
      urls: [],
      inspectionUrl: "",
      siteUrl: window.location.origin,
      languageCode: "",
      multipleUrls: !1,
      enabled: !1
    }, this.inspectionResult = null, this.consumeContext(Z, (t) => {
      I(this, v, t);
    }), this.consumeContext(ee, (t) => {
      t && (I(this, O, t), this.observe(t.unique, async (e) => {
        var s;
        if (e) {
          var i = await u(this, p).getUrls(e);
          ((s = i == null ? void 0 : i.data) == null ? void 0 : s.length) > 0 && (this.inspectionObj.multipleUrls = (i == null ? void 0 : i.data[0].urlInfos.length) > 1, i == null || i.data[0].urlInfos.forEach((n, o) => {
            if (o == 0 && n.culture && (this.inspectionObj.languageCode = n.culture), n.url) {
              const a = this._isRelativeUrl(n.url) ? `${window.location.origin}${n.url}` : n.url;
              this.inspectionObj.urls.push(a);
            }
          }), this.inspectionObj.inspectionUrl = this.inspectionObj.urls[0]);
        }
      }));
    });
  }
  async connectedCallback() {
    var e, i;
    super.connectedCallback(), this._loading = !0;
    const t = await u(this, p).getOAuthConfiguration();
    this._isConnected = ((e = t.data) == null ? void 0 : e.isConnected) ?? !1, this._authorizationUrl = ((i = t.data) == null ? void 0 : i.authorizationUrl) ?? "", this._loading = !1;
  }
  render() {
    return r`
            <umb-body-layout>
                <uui-box headline=${this.localize.term("urlInspectionTool_title")}>
                    ${h(this, l, K).call(this)}
                    <div>
                        <h5>About Google Search Console - URL Inspection API</h5>
                        <p>
                            The Search Console APIs are a way to access data outside of Search Console, through external applications and products.
                        </p>
                        <p>
                            You can request the data Search Console has about the indexed version of the current node, and the API will return the indexed information.
                        </p>
                        <p>
                            The request parameters include the URL you'd like to inspect and the URL of the property as defined in Search Console.
                        </p>
                        <p>
                            The response includes analysis results containing information from Search Console, including index status, AMP, rich results and mobile usability.
                        </p>
                        <p>
                            Usage limits - the quote is enforced per Search Console website property: 2000 queries per day / 600 queries per minute.
                        </p>
                    </div>
                    ${h(this, l, Y).call(this)}
                    ${this._loading ? r`<div class="loader"><uui-loader></uui-loader></div>` : ""}
                    <br/>
                    ${g(this._showResults, () => h(this, l, J).call(this))}
                </uui-box>
            </umb-body-layout>
        `;
  }
  _isRelativeUrl(t) {
    var e = new RegExp("^(?:[a-z]+:)?//", "i");
    return !e.test(t);
  }
};
p = /* @__PURE__ */ new WeakMap();
v = /* @__PURE__ */ new WeakMap();
O = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakSet();
B = function() {
  window.addEventListener("message", async (t) => {
    var n, o;
    if (t.data.type == "google:oauth:success") {
      var e = "?code=", i = "&scope=", s = t.data.url.slice(t.data.url.indexOf(e) + e.length, t.data.url.indexOf(i));
      const a = await u(this, p).getAccessToken(s), x = (a == null ? void 0 : a.data) && !(a != null && a.data.success), Q = {
        data: {
          title: "Google Search Console Authorization",
          message: x ? (a == null ? void 0 : a.data.errorMessage) ?? "Access Denied" : "Access Approved"
        }
      };
      (n = u(this, v)) == null || n.peek(x ? "danger" : "positive", Q), x || (this._isConnected = !0);
    } else if (t.data.type == "google:oauth:denied") {
      const a = { data: { title: "Google Search Console Authorization", message: "Access Denied" } };
      (o = u(this, v)) == null || o.peek("danger", a), this._isConnected = !1;
    }
  }, !1), window.open(
    this._authorizationUrl,
    "GoogleSearchConsole_Authorize",
    "width=900,height=700,modal=yes,alwaysRaised=yes"
  );
};
V = async function() {
  var t = await u(this, p).revokeAccessToken();
  t && (this._isConnected = !1);
};
X = async function() {
  this._loading = !0;
  const { data: t } = await u(this, p).inspect(this.inspectionObj.inspectionUrl, this.inspectionObj.siteUrl, this.inspectionObj.languageCode);
  t && (this._showResults = !0, this.inspectionResult = t), this._loading = !1;
};
F = function() {
  this.inspectionObj = { ...this.inspectionObj, multipleUrls: !1, enabled: !0 };
};
H = async function(t) {
  var n;
  const e = t.target.value, i = u(this, O).getUnique(), s = await u(this, p).getUrls(i);
  ((n = s == null ? void 0 : s.data) == null ? void 0 : n.length) > 0 && (s == null || s.data[0].urlInfos.forEach((o) => {
    e === o.url && (this.inspectionObj = {
      ...this.inspectionObj,
      languageCode: o.culture || ""
    });
  }));
};
K = function() {
  return r`
            <div slot="header-actions">
                 <button type="button" class="signin" look="default"
                    ?disabled=${this._isConnected}
                    @click=${h(this, l, B)}>
                    <img src="${this._isConnected ? "/App_Plugins/GoogleSearchConsole/images/btn_google_signin_dark_disabled_web.png" : "/App_Plugins/GoogleSearchConsole/images/btn_google_signin_dark_normal_web.png"}" />
                </button>
                <uui-button label="Revoke" look="primary" color="danger"
                    ?disabled=${!this._isConnected} 
                    @click=${h(this, l, V)}></uui-button> 
            </div>`;
};
Y = function() {
  return r`
            <div class="row">
                <div class="field">
                    <uui-label>Inspection URL</uui-label>
                        ${this.inspectionObj.multipleUrls ? r`<uui-select
                                    style="width: 100%"
                                    @change=${h(this, l, H)}
                                    .options=${this.inspectionObj.urls.map((t) => ({
    name: t,
    value: t,
    selected: t === this.inspectionObj.inspectionUrl
  }))}></uui-select>` : r`<uui-input 
                                    style="width: 100%"
                                    ?disabled=${!this.inspectionObj.enabled} 
                                    .value=${this.inspectionObj.inspectionUrl}></uui-input>`}
                        
                </div>
                <div class="field">
                    <uui-label>Site URL</uui-label>
                    <uui-input
                        style="width: 100%"
                        ?disabled=${!this.inspectionObj.enabled}
                        .value=${this.inspectionObj.siteUrl}></uui-input>
                </div>
            </div>
            <div style="row">
                <uui-button
                    look="primary"
                    label="Inspect"
                    ?disabled=${!this._isConnected}
                    @click=${h(this, l, X)}></uui-button>
                <uui-button
                    look="primary"
                    color="warning"
                    label="Edit"
                    ?disabled=${!this._isConnected}
                    @click=${h(this, l, F)}></uui-button>
            </div>
        `;
};
J = function() {
  var t, e, i, s, n;
  return r`
            <uui-box>
                <div>
                    <inspectresult-box 
                        headline="Inspection Result Link"
                        headlineSlot="Link to Search Console URL inspection."
                        .link=${(t = this.inspectionResult) == null ? void 0 : t.inspectionResultLink}></inspectresult-box>
                    ${g((e = this.inspectionResult) == null ? void 0 : e.indexStatusResult, () => {
    var o;
    return r`
                        <inspectresult-box
                            headline="Index Status Result"
                            headlineSlot="Result of the index status analysis."
                            .data=${(o = this.inspectionResult) == null ? void 0 : o.indexStatusResult}></inspectresult-box>
                    `;
  })}
                    ${g((i = this.inspectionResult) == null ? void 0 : i.ampResult, () => {
    var o;
    return r`
                        <inspectresult-box
                            headline="AMP Result"
                            headlineSlot="Result of the AMP analysis. Absent if the page is not an AMP page."
                            .data=${(o = this.inspectionResult) == null ? void 0 : o.ampResult}></inspectresult-box>
                    `;
  })}
                    ${g((s = this.inspectionResult) == null ? void 0 : s.mobileUsabilityResult, () => {
    var o;
    return r`
                        <inspectresult-box
                            headline="Mobile Usability Result"
                            headlineSlot="Result of the Mobile usability analysis."
                            .data=${(o = this.inspectionResult) == null ? void 0 : o.mobileUsabilityResult}></inspectresult-box>
                    `;
  })}
                    ${g((n = this.inspectionResult) == null ? void 0 : n.richResultsResult, () => {
    var o;
    return r`
                        <inspectresult-box
                            headline="Rich Results Result"
                            headlineSlot="Result of the Rich Results analysis. Absent if there are no rich results found."
                            .data=${(o = this.inspectionResult) == null ? void 0 : o.richResultsResult}></inspectresult-box>
                    `;
  })}
                </div>
            </uui-box>
        `;
};
c.styles = [
  D`
            .loader {
                display: flex;
                justify-content: center;
            }
            .signin {
                display: inline-flex;
                vertical-align: middle;
                height: 37px;
                background: none;
                border: none;
                cursor: pointer;
            }
            .row {
              display: flex;
              gap: var(--uui-size-space-5);
            }
            .field {
              flex: 0 0 auto;
              min-width: 30rem;
            }
        `
];
f([
  b()
], c.prototype, "_loading", 2);
f([
  b()
], c.prototype, "_isConnected", 2);
f([
  b()
], c.prototype, "_showResults", 2);
f([
  b()
], c.prototype, "_authorizationUrl", 2);
f([
  b()
], c.prototype, "inspectionObj", 2);
f([
  b()
], c.prototype, "inspectionResult", 2);
c = f([
  G(ue)
], c);
const ve = c;
export {
  c as GoogleSearchConsoleWorkspaceElement,
  ve as default
};
//# sourceMappingURL=googlesearchconsole-workspace.element-ULFLDrfD.js.map
