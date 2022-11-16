function algoliaService(contentTypeResource) {
    return {
        getContentTypes: function (callback) {
            contentTypeResource.getAll().then(function (data) {
                callback(data.map((item) => { return { id: item.id, alias: item.alias, name: item.name, checked: false } }));
            });
        },
        getPropertiesByContentTypeId: function (contentTypeId, callback) {
            contentTypeResource.getById(contentTypeId).then(function (data) {
                var properties = [];

                for (var i = 0; i < data.groups.length; i++) {
                    for (var j = 0; j < data.groups[i].properties.length; j++) {
                        properties.push({
                            id: data.groups[i].properties[j].id,
                            alias: data.groups[i].properties[j].alias,
                            name: data.groups[i].properties[j].label,
                            checked: false
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