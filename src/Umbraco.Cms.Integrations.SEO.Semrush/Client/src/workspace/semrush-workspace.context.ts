import { UmbBlockTypeWithGroupKey } from "@umbraco-cms/backoffice/block-type";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { Observable, UmbObjectState } from "@umbraco-cms/backoffice/observable-api";
import { UmbPropertyDatasetContext } from "@umbraco-cms/backoffice/property";
import { UmbVariantId } from "@umbraco-cms/backoffice/variant";
import { UmbInvariantDatasetWorkspaceContext, UmbRoutableWorkspaceContext, UmbSubmittableWorkspaceContextBase } from "@umbraco-cms/backoffice/workspace";

export class SemrushWorkspaceContext<BlockTypeData extends UmbBlockTypeWithGroupKey = UmbBlockTypeWithGroupKey>
	extends UmbSubmittableWorkspaceContextBase<BlockTypeData>
	implements UmbInvariantDatasetWorkspaceContext, UmbRoutableWorkspaceContext{
    #data = new UmbObjectState<BlockTypeData | undefined>(undefined);
    readonly unique = this.#data.asObservablePart((data) => data?.contentElementTypeKey);
    getUnique(): string | undefined {
        throw new Error("Method not implemented.");
    }
    getEntityType(): string {
        throw new Error("Method not implemented.");
    }
    getData(): BlockTypeData | undefined {
        throw new Error("Method not implemented.");
    }
    protected submit(): Promise<void> {
        throw new Error("Method not implemented.");
    }

    readonly name = this.#data.asObservablePart(() => 'block');
    
    getName(): string | undefined {
        throw new Error("Method not implemented.");
    }
    setName(name: string): void {
        throw new Error("Method not implemented.");
    }
    propertyValueByAlias<ReturnType = unknown>(alias: string): Promise<Observable<ReturnType | undefined>> {
        throw new Error("Method not implemented.");
    }
    getPropertyValue<ReturnType = unknown>(alias: string): ReturnType {
        throw new Error("Method not implemented.");
    }
    setPropertyValue(alias: string, value: unknown): Promise<void> {
        throw new Error("Method not implemented.");
    }
    createPropertyDatasetContext(host: UmbControllerHost, variantId?: UmbVariantId): UmbPropertyDatasetContext {
        throw new Error("Method not implemented.");
    }
        
}

export { SemrushWorkspaceContext as api };