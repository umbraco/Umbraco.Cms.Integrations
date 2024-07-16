import type { ShopifyCollectionEntityType, ShopifyVariantsCollectionEntityType } from "../entities/entities";

export interface ShopifyCollectionModel {
	unique: string;
	entityType: ShopifyCollectionEntityType;
    title: string;
    vendor: string;
    status: string;
    tags: string;
    variants: Array<ShopifyVariantsCollectionModel>;
    barcode: string;
    price: string;
}

export interface ShopifyVariantsCollectionModel {
	unique: string;
	entityType: ShopifyVariantsCollectionEntityType;
    product_id: string;
    sku: string;
}