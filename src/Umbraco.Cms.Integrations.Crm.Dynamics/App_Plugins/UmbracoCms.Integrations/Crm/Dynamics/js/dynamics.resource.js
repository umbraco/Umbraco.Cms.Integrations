﻿angular.module('umbraco.resources').factory('umbracoCmsIntegrationsCrmDynamicsResource',
    function ($http, umbRequestHelper) {

        const apiEndpoint = "backoffice/UmbracoCmsIntegrationsCrmDynamics/Forms";

        return {
            getAuthorizationUrl: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetAuthorizationUrl`),
                    "Failed");
            },
            getAccessToken: function (authorizationCode) {
                return umbRequestHelper.resourcePromise(
                    $http.post(`${apiEndpoint}/GetAccessToken`, { code: authorizationCode }),
                    "Failed");
            },
            revokeAccessToken: function () {
                return umbRequestHelper.resourcePromise(
                    $http.delete(`${apiEndpoint}/RevokeAccessToken`),
                    "Failed");
            },
            getSystemUserFullName: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetSystemUserFullName`),
                    "Failed");
            },
            getForms: function (module) {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetForms?module=${module}`),
                    "Failed");
            },
            getEmbedCode: function (formId) {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/GetEmbedCode?formId=${formId}`),
                    "Failed");
            },
            checkOAuthConfiguration: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(`${apiEndpoint}/CheckOAuthConfiguration`),
                    "Failed");
            }
        };
    }
);