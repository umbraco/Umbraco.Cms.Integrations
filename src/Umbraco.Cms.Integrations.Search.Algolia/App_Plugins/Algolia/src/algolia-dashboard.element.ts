import { 
    LitElement,
    html, 
    customElement,
    state,
    nothing
  } from "@umbraco-cms/backoffice/external/lit";
  import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
  import type {
    UmbRoute,
    UmbRouterSlotChangeEvent, 
    UmbRouterSlotInitEvent
  } from "@umbraco-cms/backoffice/router";
  
  import type { AlgoliaIndexElement } from "./views/algolia-index.js";
  import type { AlgoliaSearchElement } from "./views/algolia-search.js";

  @customElement("algolia-dashboard-element")
  export class AlgoliaDashboardElement extends UmbElementMixin(LitElement) {
    
    @state()
    private _routes: UmbRoute[] = [
      {
        path: `/index/:indexId`,
        component: () => import('./views/algolia-index.js'),
        setup: (component, info) => {
            const element = component as AlgoliaIndexElement;
            element.indexId = info.match.params.indexId;
        },
      },
      {
        path: `/index`,
        component: () => import('./views/algolia-index.js'),
        setup: (component) => {
            const element = component as AlgoliaIndexElement;
            element.indexId = '';
        },
      },
      {
        path: `/search/:indexId`,
        component: () => import('./views/algolia-search.js'),
        setup: (component, info) => {
            const element = component as AlgoliaSearchElement;
            element.indexId = info.match.params.indexId;
        },
      },
      {
        path: ``,
        component: () => import('./views/algolia-dashboard-overview.js'),
      },
    ];
  
    @state()
    private _routerPath?: string;
  
    @state()
    private _activePath = '';
  
    render() {
      return html`
        <umb-body-layout header-transparent>
            ${this.#renderHeader()}
            <div id="main">
                <umb-router-slot
                .routes=${this._routes}
                @init=${(event: UmbRouterSlotInitEvent) => {
                    this._routerPath = event.target.absoluteRouterPath;
                }}
                @change=${(event: UmbRouterSlotChangeEvent) => {
                    this._activePath = event.target.localActiveViewPath || '';
                }}></umb-router-slot>
            </div>
        </umb-body-layout>
      `;
    }

    #renderHeader() {
		return this._routerPath && this._activePath !== ''
			? html`
					<div id="header" slot="header">
						<a href=${this._routerPath}> &larr; Back to overview </a>
					</div>
			  `
			: nothing;
	}
  }
  
  export default AlgoliaDashboardElement;
  
  declare global {
    interface HTMLElementTagNameMap {
      'algolia-dashboard-element': AlgoliaDashboardElement
    }
  }