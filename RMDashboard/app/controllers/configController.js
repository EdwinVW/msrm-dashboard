'use strict';

rmDashboardApp.controller('configController', ['$scope', 'releaseManagementService', 'configService',
    function ($scope, releaseManagementService, configService) {

    // initialize list of available releasepaths
    $scope.releasePaths = [];

    // initialize configuration
    $scope.config = {
        title: 'Release Management Dashboard',
        autoRefresh: true,
        refreshInterval: 300000,
        releaseCount: 5,
        theme: 'dark',
        includedReleasePaths: []
    };

    // load configuration
    function loadConfig() {
        var config = configService.loadConfig();
        if (config) {
            $scope.config = config;
        }
    };

    /// save the configuration
    $scope.saveConfig = function () {
        $scope.config.includedReleasePaths = [];
        $scope.releasePaths.forEach(function (releasePath) {
            if (releasePath.value) {
                $scope.config.includedReleasePaths.push(releasePath.id);
            }
        });
        configService.saveConfig($scope.config);
    };

    // load available releasepaths
    releaseManagementService.getReleasePaths(function (err, data) {
        if (err) {
            $scope.hasError = true;
            $scope.error = err;
        }
        else {
            $scope.hasError = false;
            $scope.error = null;

            // load configuration
            loadConfig();

            // fill checkboxes based on configuration
            data.forEach(function (releasePath) {
                var include = ($scope.config.includedReleasePaths.indexOf(releasePath.id) > -1);
                $scope.releasePaths.push({
                    id: releasePath.id,
                    name: releasePath.name,
                    value: include
                });
            });
        }
    });
}]);