import { ProductDtoModel } from "@umbraco-integrations/shopify/generated";
import type { ShopifyCollectionEntityType } from "../entities/entities";

export interface ShopifyCollectionModel {
	unique: string;
	entityType: ShopifyCollectionEntityType;
    products: Array<ProductDtoModel>;
    skip: number;
	take: number;
}

export interface ShopifyProductPickerConfiguration {
    unique?: string;
    dataTypeId?: string;
    minItems?: number;
    maxItems?: number;
}