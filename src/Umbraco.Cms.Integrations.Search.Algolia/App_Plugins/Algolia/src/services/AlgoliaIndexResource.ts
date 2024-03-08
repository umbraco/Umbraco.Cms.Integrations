import type { AlgoliaResultModel } from "../models/AlgoliaResultModel";
import type { AlgoliaSearchResultModel  } from "../models/AlgoliaSearchResultModel";
import type { AlgoliaContentTypeModel } from "../models/AlgoliaContentTypeModel";
import { AlgoliaIndexConfigurationModel } from "../models/AlgoliaIndexConfigurationModel";

export class AlgoliaIndexResource {

    private static apiPath:string = "/umbraco/algolia-search/management/api/v1/search/index";

    public static async getIndices(): Promise<any>{
        const response = await fetch(`${this.apiPath}`);
        const data = await response.json();
        return data;
    }

    public static async getIndexById(id: number): Promise<AlgoliaIndexConfigurationModel>{
        const response = await fetch(`${this.apiPath}/${id}`);
        const data = await response.json();
        return data;
    }

    public static async saveIndex(indexConfiguration: AlgoliaIndexConfigurationModel) : Promise<AlgoliaResultModel> {
        const response = await fetch(`${this.apiPath}`, {
            method: 'POST',
            body: JSON.stringify(indexConfiguration),
            headers: {
                'Content-Type': 'application/json',
                Accept: 'application/json',
            }
        });
        const data = await response.json();
        return data;
    }

    public static async buildIndex(indexConfiguration: AlgoliaIndexConfigurationModel) : Promise<AlgoliaResultModel> {
        const response = await fetch(`${this.apiPath}/build`, {
            method: 'POST',
            body: JSON.stringify(indexConfiguration),
            headers: {
                'Content-Type': 'application/json',
                Accept: 'application/json',
            }
        });
        const data = await response.json();
        return data;
    }

    public static async deleteIndex(id: Number): Promise<AlgoliaResultModel> {
        const response = await fetch(`${this.apiPath}/${id}`, { method: 'DELETE' });
        const data = await response.json();
        return data;
    }

    public static async searchIndex(id: Number, query: string): Promise<AlgoliaSearchResultModel> {
        const response = await fetch(`${this.apiPath}/${id}/search?query=${query}`);
        const data = await response.json();
        return data;
    }

    public static async getContentTypes(): Promise<Array<AlgoliaContentTypeModel>> {
        const response = await fetch('/umbraco/algolia-search/management/api/v1/search/content-type');
        const data = await response.json();
        return data;
    }

    public static async getContentTypesWithIndex(id: Number): Promise<Array<AlgoliaContentTypeModel>> {
        const response = await fetch(`/umbraco/algolia-search/management/api/v1/search/content-type/index/${id}`);
        const data = await response.json();
        return data;
    }
}