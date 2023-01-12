function algoliaService(contentTypeResource) {
    return {
        getContentTypes: function (callback) {

            contentTypeResource.getAll().then(function (data) {

                var contentTypesArr = [];

                for (let i = 0; i < data.length; i++) {
                    
                    if (data[i].isElement) continue;

                    contentTypeResource.getWhereCompositionIsUsedInContentTypes(data[i].id).then(function (response) {
                        if (response.length === 0) {
                            contentTypesArr.push({
                                id: data[i].id,
                                icon: data[i].icon,
                                alias: data[i].alias,
                                name: data[i].name,
                                selected: false,
                                allowRemove: false
                            });
                        }
                    });

                    if (data.length - 1 === i) {
                        callback(contentTypesArr);
                    }
                }
            });
        },
        getPropertiesByContentTypeId: function (contentTypeId, callback) {
            contentTypeResource.getById(contentTypeId).then(function (data) {
                var properties = [];

                for (var i = 0; i < data.groups.length; i++) {
                    for (var j = 0; j < data.groups[i].properties.length; j++) {
                        properties.push({
                            id: data.groups[i].properties[j].id,
                            icon: "icon-indent",
                            alias: data.groups[i].properties[j].alias,
                            name: data.groups[i].properties[j].label,
                            group: data.groups[i].name,
                            selected: false
                        });
                    }
                }

                callback(properties);
            });
        }
    }
}

angular.module("umbraco.services")
    .factory("algoliaService", algoliaService);