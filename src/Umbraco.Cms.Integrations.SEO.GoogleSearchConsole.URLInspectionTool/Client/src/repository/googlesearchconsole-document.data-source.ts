import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { DocumentService } from "@umbraco-cms/backoffice/external/backend-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";

export class GoogleSearchConsoleDocumentDataSource extends UmbControllerBase {

    async getUrls(documentId: string) {
        return await tryExecute(this,
            DocumentService.getDocumentUrls({
                query: {
                    id: [documentId]
                }
            })
        );
    }

}