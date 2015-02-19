(function (angular) {
    'use strict';

    /** 
    * Controller to work with the application configuration
    */
    function ConfigController(releaseManagementService, configService) {
        var vm = this;

        vm.releasePaths = [];
        vm.saveConfig = saveConfig;
        vm.config = {
            title: 'Release Management Dashboard',
            autoRefresh: true,
            showComponents: true,
            refreshInterval: 300000,
            releaseCount: 5,
            theme: 'dark',
            includedReleasePaths: []
        };

        activate();

        ////////////

        /**
        * Saves the configuration for the application
        */
        function saveConfig() {
            $scope.config.includedReleasePaths = [];
            $scope.releasePaths.forEach(function (releasePath) {
                if (releasePath.value) {
                    $scope.config.includedReleasePaths.push(releasePath.id);
                }
            });
            configService.saveConfig($scope.config);
        };

        /**
        * Loads the configuration for the application
        */
        function loadConfig() {
            var config = configService.loadConfig();
            if (config) {
                $scope.config = config;
            }
        };

        /**
        * Activates the controller
        */
        function activate() {
            releaseManagementService.getReleasePaths(function (err, data) {
                if (err) {
                    $scope.hasError = true;
                    $scope.error = err;
                }
                else {
                    $scope.hasError = false;
                    $scope.error = null;

                    loadConfig();

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
        }
    }

    ConfigController.$inject = ['releaseManagementService', 'configService'];

    angular.module('rmDashboardApp').controller('configController', ConfigController);
})(angular);