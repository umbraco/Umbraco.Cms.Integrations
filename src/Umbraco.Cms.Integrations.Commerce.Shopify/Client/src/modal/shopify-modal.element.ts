import { html, css, state, customElement } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import {
    UMB_NOTIFICATION_CONTEXT,
} from "@umbraco-cms/backoffice/notification";
import type { UUIInputEvent } from "@umbraco-cms/backoffice/external/uui";
import type { ShopifyServiceStatus } from "../models/shopify-service.model.js";
import type { ShopifyProductPickerModalData, ShopifyProductPickerModalValue } from "./shopify.modal-token.js";
import type { ProductsListDtoModel } from "../generated";
import { SHOPIFY_CONTEXT_TOKEN } from "../context/shopify.context.js";

const elementName = "shopify-modal";

export default class ShopifyModalElement extends UmbModalBaseElement<ShopifyProductPickerModalData, ShopifyProductPickerModalValue>{

}