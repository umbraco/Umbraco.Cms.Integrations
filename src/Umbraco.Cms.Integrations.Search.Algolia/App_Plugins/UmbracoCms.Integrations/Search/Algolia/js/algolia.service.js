function algoliaService(contentTypeResource) {
    return {
        getContentTypes: function (callback) {
            contentTypeResource.getAll().then(function (data) {
                callback(data.filter(item => item.parentId == -1 && !item.isElement).map((item) => {
                    return {
                        id: item.id,
                        icon: item.icon,
                        alias: item.alias,
                        name: item.name,
                        selected: false,
                        allowRemove: false
                    }
                }));
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