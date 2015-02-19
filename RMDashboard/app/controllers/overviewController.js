(function (angular) {
    'use strict';

    function OverviewController($interval, releaseManagementService, configService) {
        var vm = this;
        var refreshInterval = 20000;
        var autoRefresh = true;

        vm.title = "Release Management Dashboard";
        vm.theme = 'dark';
        vm.loadData = loadData;

        activate();

        ////////////

        /**
        * Activates the controller
        */
        function activate() {
            var config = configService.loadConfig();
            if (config) {
                $scope.title = config.title;
                $scope.theme = config.theme;
                refreshInterval = config.refreshInterval;
                autoRefresh = (config.autoRefresh == 'true');
            }

            // Always load data initially when activated.
            // After that, initiate the refresh using the given refresh time.
            loadData();

            if (autoRefresh) {
                $interval(function () { loadData(); }, refreshInterval);
            }
        }

        /**
        * Loads the data from the API
        */
        function loadData() {
            releaseManagementService.getReleases(function (err, data) {
                if (err) {
                    $scope.hasError = true;
                    $scope.error = err;
                }
                else {
                    $scope.hasError = false;
                    $scope.error = null;
                    $scope.data = data;
                }
            });
        };
    }

    OverviewController.$inject = ['$interval', 'releaseManagementService', 'configService'];

    angular.module('rmDashboardApp').controller('overviewController', OverviewController);
})(angular);