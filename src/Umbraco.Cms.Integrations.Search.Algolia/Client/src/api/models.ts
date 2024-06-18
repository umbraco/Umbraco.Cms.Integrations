

export type ContentTypeDtoModel = {
    id: number
    icon: string
    alias: string
    name: string
    selected: boolean
    allowRemove: boolean
    properties: Array<ContentTypePropertyDtoModel>
};

export type ContentTypePropertyDtoModel = {
    id: number
    icon: string
    alias: string
    name: string
    group: string
    selected: boolean
};

export enum EventMessageTypeModel {
    DEFAULT = 'Default',
    INFO = 'Info',
    ERROR = 'Error',
    SUCCESS = 'Success',
    WARNING = 'Warning'
}

export type IndexConfigurationModel = {
    id: number
    name: string
    contentData: Array<ContentTypeDtoModel>
};

export type NotificationHeaderModel = {
    message: string
    category: string
    type: EventMessageTypeModel
};

export type ResponseModel = {
    itemsCount: number
    pagesCount: number
    itemsPerPage: number
    hits: Array<Record<string, string>>
};

export type ResultModel = {
    success: boolean
    error: string
    readonly failure: boolean
};

export type AlgoliaSearchData = {

    payloads: {
        GetContentTypesByIndexId: {
            id: number

        };
        SaveIndex: {
            requestBody?: IndexConfigurationModel

        };
        DeleteIndex: {
            id: number

        };
        GetIndexById: {
            id: number

        };
        Search: {
            indexId: number
            query?: string

        };
        BuildIndex: {
            requestBody?: IndexConfigurationModel

        };
    }


    responses: {
        GetContentTypes: Array<ContentTypeDtoModel>
        , GetContentTypesByIndexId: Array<ContentTypeDtoModel>
        , GetIndices: Array<IndexConfigurationModel>
        , SaveIndex: ResultModel
        , DeleteIndex: ResultModel
        , GetIndexById: IndexConfigurationModel
        , Search: ResponseModel
        , BuildIndex: ResultModel

    }

}