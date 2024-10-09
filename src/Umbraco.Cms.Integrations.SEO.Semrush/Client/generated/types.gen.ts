// This file is auto-generated by @hey-api/openapi-ts

export type AuthorizationRequestDtoModel = {
    code: string;
};

export type AuthorizationResponseDtoModel = {
    isAuthorized: boolean;
    isValid: boolean;
    isFreeAccount?: boolean | null;
};

export type ColumnDtoModel = {
    name: string;
    value: string;
    description: string;
};

export type ContentPropertyDtoModel = {
    propertyName: string;
    propertyValue: string;
};

export type ContentResult = {
    content?: string | null;
    contentType?: string | null;
    statusCode?: number | null;
};

export type DataSourceDtoModel = {
    items: Array<(DataSourceItemDtoModel)>;
};

export type DataSourceItemDtoModel = {
    code: string;
    region: string;
    researchTypes: string;
    googleSearchDomain: string;
};

export enum EventMessageTypeModel {
    DEFAULT = 'Default',
    INFO = 'Info',
    ERROR = 'Error',
    SUCCESS = 'Success',
    WARNING = 'Warning'
}

export type NotificationHeaderModel = {
    message: string;
    category: string;
    type: EventMessageTypeModel;
};

export type RelatedPhrasesDataDtoModel = {
    columnNames: Array<(string)>;
    rows: Array<Array<(string)>>;
};

export type RelatedPhrasesDtoModel = {
    readonly isSuccessful: boolean;
    error: string;
    status: number;
    data: RelatedPhrasesDataDtoModel;
    totalPages: number;
};

export type TokenDtoModel = {
    access_token: string;
    token_type: string;
    expires_in: number;
    refresh_token: string;
    readonly isAccessTokenAvailable: boolean;
};

export type GetTokenDetailsResponse = TokenDtoModel;

export type GetAccessTokenData = {
    requestBody?: AuthorizationRequestDtoModel;
};

export type GetAccessTokenResponse = string;

export type RefreshAccessTokenResponse = string;

export type RevokeTokenResponse = string;

export type ValidateTokenResponse = AuthorizationResponseDtoModel;

export type OauthData = {
    code?: string;
};

export type OauthResponse = ContentResult;

export type GetAuthorizationUrlResponse = string;

export type GetColumnsResponse = Array<(ColumnDtoModel)>;

export type GetCurrentContentPropertiesData = {
    contentId?: string;
};

export type GetCurrentContentPropertiesResponse = Array<(ContentPropertyDtoModel)>;

export type GetDataSourcesResponse = DataSourceDtoModel;

export type PingResponse = string;

export type GetRelatedPhrasesData = {
    dataSource?: string;
    method?: string;
    pageNumber?: number;
    phrase?: string;
};

export type GetRelatedPhrasesResponse = RelatedPhrasesDtoModel;

export type $OpenApiTs = {
    '/umbraco/semrush/management/api/v1/detail': {
        get: {
            res: {
                /**
                 * OK
                 */
                200: TokenDtoModel;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/get': {
        post: {
            req: GetAccessTokenData;
            res: {
                /**
                 * OK
                 */
                200: string;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/refresh': {
        post: {
            res: {
                /**
                 * OK
                 */
                200: string;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/revoke': {
        post: {
            res: {
                /**
                 * OK
                 */
                200: string;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/validate': {
        get: {
            res: {
                /**
                 * OK
                 */
                200: AuthorizationResponseDtoModel;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/auth': {
        get: {
            req: OauthData;
            res: {
                /**
                 * OK
                 */
                200: ContentResult;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/auth-url': {
        get: {
            res: {
                /**
                 * OK
                 */
                200: string;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/columns': {
        get: {
            res: {
                /**
                 * OK
                 */
                200: Array<(ColumnDtoModel)>;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/content-properties': {
        get: {
            req: GetCurrentContentPropertiesData;
            res: {
                /**
                 * OK
                 */
                200: Array<(ContentPropertyDtoModel)>;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/datasources': {
        get: {
            res: {
                /**
                 * OK
                 */
                200: DataSourceDtoModel;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/ping': {
        get: {
            res: {
                /**
                 * OK
                 */
                200: string;
            };
        };
    };
    '/umbraco/semrush/management/api/v1/related-phrases': {
        get: {
            req: GetRelatedPhrasesData;
            res: {
                /**
                 * OK
                 */
                200: RelatedPhrasesDtoModel;
            };
        };
    };
};