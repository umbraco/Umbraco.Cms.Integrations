import { ProductDtoModel } from "@umbraco-integrations/shopify/generated";
import type { ShopifyCollectionEntityType } from "../entities/entities";

export interface ShopifyCollectionModel {
	unique: string;
	entityType: ShopifyCollectionEntityType;
    products: Array<ProductDtoModel>;
}

export interface ShopifyProductPickerConfiguration {
    unique?: string;
    dataTypeId?: string;
    amount?: ShopifyAmountConfiguration;
}

export interface ShopifyAmountConfiguration {
    unique?: string;
    dataTypeId?: string;
    amountMin?: number;
    amountMax?: number;
}