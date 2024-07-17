import { ProductDtoModel } from "@umbraco-integrations/shopify/generated";
import type { ShopifyCollectionEntityType, ShopifyVariantsCollectionEntityType } from "../entities/entities";

export interface ShopifyCollectionModel {
	unique: string;
	entityType: ShopifyCollectionEntityType;
    products: Array<ProductDtoModel>;
}

export interface ShopifyVariantsCollectionModel {
	unique: string;
	entityType: ShopifyVariantsCollectionEntityType;
    product_id: string;
    sku: string;
}