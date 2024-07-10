

export type ConfigurationTypeModel = {
        readonly value: string
    };

export type EditorSettingsModel = {
        isValid: boolean
type: ConfigurationTypeModel
    };

export enum EventMessageTypeModel {
    DEFAULT = 'Default',
    INFO = 'Info',
    ERROR = 'Error',
    SUCCESS = 'Success',
    WARNING = 'Warning'
}

export type NotificationHeaderModel = {
        message: string
category: string
type: EventMessageTypeModel
    };

export type OAuthRequestDtoModel = {
        code: string
    };

export type ProductDtoModel = {
        id: number
title: string
body: string
vendor: string
status: string
tags: string
variants: Array<ProductVariantDtoModel>
image: ProductImageDtoModel
    };

export type ProductImageDtoModel = {
        src: string
    };

export type ProductVariantDtoModel = {
        price: string
sku: string
position: number
barcode: string
inventoryQuantity: number
    };

export type ProductsListDtoModel = {
        products: Array<ProductDtoModel>
totalPages: number
    };

export type RequestDtoModel = {
        ids: Array<string>
    };

export type ResponseDtoProductsListDtoModel = {
        isValid: boolean
nextPageInfo: string
previousPageInfo: string
isExpired: boolean
result: ProductsListDtoModel
message: string
    };

export type ShopifyData = {
        
        payloads: {
            GetAccessToken: {
                        requestBody?: OAuthRequestDtoModel
                        
                    };
GetList: {
                        pageInfo?: string
                        
                    };
GetListByIds: {
                        requestBody?: RequestDtoModel
                        
                    };
        }
        
        
        responses: {
            CheckConfiguration: EditorSettingsModel
                ,GetAccessToken: string
                ,GetAuthorizationUrl: string
                ,GetList: ResponseDtoProductsListDtoModel
                ,GetListByIds: ResponseDtoProductsListDtoModel
                ,RevokeAccessToken: string
                ,GetTotalPages: number
                ,ValidateAccessToken: ResponseDtoProductsListDtoModel
                
        }
        
    }