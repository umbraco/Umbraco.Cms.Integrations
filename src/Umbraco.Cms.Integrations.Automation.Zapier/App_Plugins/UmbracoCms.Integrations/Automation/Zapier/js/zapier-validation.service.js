function zapierValidationService() {
    return {
        isValidConfig: (webHookUrl, contentTypeAlias) => {

            if (webHookUrl === undefined || webHookUrl.length === 0) return false;

            let url;

            try {
                url = new URL(webHookUrl);
            }
            catch (_) {
                return false;
            }

            if (!(url.protocol === "http:" || url.protocol === "https:")) return false;

            if (contentTypeAlias === undefined || contentTypeAlias.length === 0) return false;

            return true;
        }
    }
}

angular.module("umbraco.services")
    .service("umbracoCmsIntegrationsAutomationZapierValidationService", zapierValidationService);