(function (angular) {
    'use strict';

    function OverviewController($interval, releaseManagementService, configService) {
        var vm = this;
        var refreshInterval = 300000;
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
                vm.title = config.title;
                vm.theme = config.theme;
                refreshInterval = config.refreshInterval;
                autoRefresh = config.autoRefresh;
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
                    vm.hasError = true;
                    vm.error = err;
                }
                else {
                    vm.hasError = false;
                    vm.error = null;
                    vm.data = data;
                }
            });
        };
    }

    OverviewController.$inject = ['$interval', 'releaseManagementService', 'configService'];

    angular.module('rmDashboardApp').controller('overviewController', OverviewController);
})(angular);