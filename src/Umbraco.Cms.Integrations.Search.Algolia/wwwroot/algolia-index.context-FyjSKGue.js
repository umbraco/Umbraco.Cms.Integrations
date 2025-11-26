var g = (r) => {
  throw TypeError(r);
};
var m = (r, t, e) => t.has(r) || g("Cannot " + e);
var s = (r, t, e) => (m(r, t, "read from private field"), e ? e.call(r) : t.get(r)), x = (r, t, e) => t.has(r) ? g("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(r) : t.set(r, e), p = (r, t, e, a) => (m(r, t, "write to private field"), a ? a.call(r, e) : t.set(r, e), e), y = (r, t, e) => (m(r, t, "access private method"), e);
import { UmbControllerBase as b } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as w } from "@umbraco-cms/backoffice/context-api";
import { tryExecute as d } from "@umbraco-cms/backoffice/resources";
import { UMB_NOTIFICATION_CONTEXT as o } from "@umbraco-cms/backoffice/notification";
import { c as i } from "./index-DHm3D8am.js";
class h {
  static getContentTypes(t) {
    return ((t == null ? void 0 : t.client) ?? i).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/algolia-search/management/api/v1/search/content-type",
      ...t
    });
  }
  static getSearchContentTypeIndexById(t) {
    return (t.client ?? i).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/algolia-search/management/api/v1/search/content-type/index/{id}",
      ...t
    });
  }
  static getIndices(t) {
    return ((t == null ? void 0 : t.client) ?? i).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/algolia-search/management/api/v1/search/index",
      ...t
    });
  }
  static postSaveIndex(t) {
    return ((t == null ? void 0 : t.client) ?? i).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/algolia-search/management/api/v1/search/index",
      ...t,
      headers: {
        "Content-Type": "application/json",
        ...t == null ? void 0 : t.headers
      }
    });
  }
  static deleteSearchIndex(t) {
    return (t.client ?? i).delete({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/algolia-search/management/api/v1/search/index/{id}",
      ...t
    });
  }
  static getSearchIndexById(t) {
    return (t.client ?? i).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/algolia-search/management/api/v1/search/index/{id}",
      ...t
    });
  }
  static getSearchIndex(t) {
    return (t.client ?? i).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/algolia-search/management/api/v1/search/index/{indexId}/search",
      ...t
    });
  }
  static postBuildSearchIndex(t) {
    return ((t == null ? void 0 : t.client) ?? i).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/algolia-search/management/api/v1/search/index/build",
      ...t,
      headers: {
        "Content-Type": "application/json",
        ...t == null ? void 0 : t.headers
      }
    });
  }
}
var u, I;
class T extends b {
  constructor(e) {
    super(e);
    x(this, u);
  }
  async getIndices() {
    const { data: e, error: a } = await d(this, h.getIndices());
    return a || !e ? { error: a } : { data: e };
  }
  async getIndexById(e) {
    const { data: a, error: n } = await d(this, h.getSearchIndexById({
      path: {
        id: e
      }
    }));
    return n || !a ? { error: n } : { data: a };
  }
  async getContentTypes() {
    const { data: e, error: a } = await d(this, h.getContentTypes());
    return a || !e ? { error: a } : { data: e };
  }
  async getContentTypesWithIndex(e) {
    const { data: a, error: n } = await d(this, h.getSearchContentTypeIndexById({
      path: {
        id: e
      }
    }));
    return n || !a ? { error: n } : { data: a };
  }
  async saveIndex(e) {
    const { data: a, error: n } = await d(this, h.postSaveIndex({
      body: {
        id: e.id,
        name: e.name,
        contentData: e.contentData
      }
    }));
    if (n || !a)
      return { error: n };
    y(this, u, I).call(this, "Index saved.");
    const l = e.id != 0 ? window.location.href.replace(`/index/${e.id}`, "") : window.location.href.replace("/index", "");
    return window.history.pushState({}, "", l), { data: a };
  }
  async buildIndex(e) {
    const { data: a, error: n } = await d(this, h.postBuildSearchIndex({
      body: {
        id: e.id,
        name: e.name,
        contentData: e.contentData
      }
    }));
    return n || !a ? { error: n } : (y(this, u, I).call(this, "Index built."), { data: a });
  }
  async deleteIndex(e) {
    const { data: a, error: n } = await d(this, h.deleteSearchIndex({
      path: {
        id: e
      }
    }));
    return n || !a ? { error: n } : (y(this, u, I).call(this, "Index deleted"), { data: a });
  }
  async searchIndex(e, a) {
    const { data: n, error: l } = await d(this, h.getSearchIndex({
      path: {
        indexId: e
      },
      query: {
        query: a
      }
    }));
    return l || !n ? { error: l } : { data: n };
  }
}
u = new WeakSet(), I = async function(e) {
  const a = await this.getContext(o);
  a == null || a.peek("positive", {
    data: { message: e }
  });
};
var c;
class v extends b {
  constructor(e) {
    super(e);
    x(this, c);
    this.provideContext(S, this), p(this, c, new T(e));
  }
  async getIndices() {
    return await s(this, c).getIndices();
  }
  async getIndexById(e) {
    return s(this, c).getIndexById(e);
  }
  async getContentTypes() {
    return await s(this, c).getContentTypes();
  }
  async getContentTypesWithIndex(e) {
    return s(this, c).getContentTypesWithIndex(e);
  }
  async saveIndex(e) {
    return await s(this, c).saveIndex(e);
  }
  async buildIndex(e) {
    return s(this, c).buildIndex(e);
  }
  async deleteIndex(e) {
    return await s(this, c).deleteIndex(e);
  }
  async searchIndex(e, a) {
    return s(this, c).searchIndex(e, a);
  }
}
c = new WeakMap();
const S = new w(v.name);
export {
  S as ALGOLIA_CONTEXT_TOKEN,
  v as AlgoliaIndexContext,
  v as default
};
//# sourceMappingURL=algolia-index.context-FyjSKGue.js.map
