// This file is auto-generated by @hey-api/openapi-ts

export type ContentTypeDtoModel = {
    id: number;
    icon: string;
    alias: string;
    name: string;
    selected: boolean;
    allowRemove: boolean;
    properties: Array<(ContentTypePropertyDtoModel)>;
};

export type ContentTypePropertyDtoModel = {
    id: number;
    icon: string;
    alias: string;
    name: string;
    group: string;
    selected: boolean;
};

export enum EventMessageTypeModel {
    DEFAULT = 'Default',
    INFO = 'Info',
    ERROR = 'Error',
    SUCCESS = 'Success',
    WARNING = 'Warning'
}

export type IndexConfigurationModel = {
    id: number;
    name: string;
    contentData: Array<(ContentTypeDtoModel)>;
};

export type NotificationHeaderModel = {
    message: string;
    category: string;
    type: EventMessageTypeModel;
};

export type ResponseModel = {
    itemsCount: number;
    pagesCount: number;
    itemsPerPage: number;
    hits: Array<{
        [key: string]: (string);
    }>;
};

export type ResultModel = {
    success: boolean;
    error: string;
    readonly failure: boolean;
};

export type GetContentTypesResponse = Array<(ContentTypeDtoModel)>;

export type GetContentTypesByIndexIdData = {
    id: number;
};

export type GetContentTypesByIndexIdResponse = Array<(ContentTypeDtoModel)>;

export type GetIndicesResponse = Array<(IndexConfigurationModel)>;

export type SaveIndexData = {
    requestBody?: IndexConfigurationModel;
};

export type SaveIndexResponse = ResultModel;

export type DeleteIndexData = {
    id: number;
};

export type DeleteIndexResponse = ResultModel;

export type GetIndexByIdData = {
    id: number;
};

export type GetIndexByIdResponse = IndexConfigurationModel;

export type SearchData = {
    indexId: number;
    query?: string;
};

export type SearchResponse = ResponseModel;

export type BuildIndexData = {
    requestBody?: IndexConfigurationModel;
};

export type BuildIndexResponse = ResultModel;

export type $OpenApiTs = {
    '/umbraco/algolia-search/management/api/v1/search/content-type': {
        get: {
            res: {
                /**
                 * OK
                 */
                200: Array<(ContentTypeDtoModel)>;
            };
        };
    };
    '/umbraco/algolia-search/management/api/v1/search/content-type/index/{id}': {
        get: {
            req: GetContentTypesByIndexIdData;
            res: {
                /**
                 * OK
                 */
                200: Array<(ContentTypeDtoModel)>;
            };
        };
    };
    '/umbraco/algolia-search/management/api/v1/search/index': {
        get: {
            res: {
                /**
                 * OK
                 */
                200: Array<(IndexConfigurationModel)>;
            };
        };
        post: {
            req: SaveIndexData;
            res: {
                /**
                 * OK
                 */
                200: ResultModel;
            };
        };
    };
    '/umbraco/algolia-search/management/api/v1/search/index/{id}': {
        delete: {
            req: DeleteIndexData;
            res: {
                /**
                 * OK
                 */
                200: ResultModel;
            };
        };
        get: {
            req: GetIndexByIdData;
            res: {
                /**
                 * OK
                 */
                200: IndexConfigurationModel;
            };
        };
    };
    '/umbraco/algolia-search/management/api/v1/search/index/{indexId}/search': {
        get: {
            req: SearchData;
            res: {
                /**
                 * OK
                 */
                200: ResponseModel;
            };
        };
    };
    '/umbraco/algolia-search/management/api/v1/search/index/build': {
        post: {
            req: BuildIndexData;
            res: {
                /**
                 * OK
                 */
                200: ResultModel;
            };
        };
    };
};