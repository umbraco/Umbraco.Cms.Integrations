function zapierValidationService() {

    const resources = {
        WebHookEmpty: "WebHook Url is required.",
        WebHookUrlInvalid: "WebHook Url format is invalid.",
        ContentTypeEmpty: "Content type is required."
    };

    return {
        validateConfiguration: (webHookUrl, contentTypeAlias) => {

            if (webHookUrl === undefined || webHookUrl.length === 0) return resources.WebHookEmpty;

            let url;

            try {
                url = new URL(webHookUrl);
            }
            catch (_) {
                return resources.WebHookUrlInvalid;
            }

            if (!(url.protocol === "http:" || url.protocol === "https:")) return resources.WebHookUrlInvalid;

            if (contentTypeAlias === undefined || contentTypeAlias.length === 0) return resources.ContentTypeEmpty;

            return "";
        }
    }
}

angular.module("umbraco.services")
    .service("umbracoCmsIntegrationsAutomationZapierValidationService", zapierValidationService);