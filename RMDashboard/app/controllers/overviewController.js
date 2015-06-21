(function (angular) {
    'use strict';

    function OverviewController($interval, $scope, $cacheFactory, releaseManagementService, configService) {
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
        * Caching mechanism to keep track of the state of the collapsed deploymentSteps
        * this prevents that the UI is changed to the initial state after a deployment refresh
        */
        var cache = $cacheFactory('deploymentStepState');
        $scope.put = function (key, stepStatus) {
            if (stepStatus != 'Pending') {
                var value = cache.get(key);
                cache.put(key, value === undefined ? true : !value);
            }
        };
        $scope.get = function (key, stepStatus) {
            if (stepStatus != 'Pending') {
                var value = cache.get(key);
                return value === undefined ? false : value;
            }
            else {
                return true;
            }
        };
        $scope.determineShowDeploymentStepText = function (key, stepStatus) {
            return stepStatus == 'Pending' ? '' : $scope.get(key, stepStatus) == true ? '-' : '+';
        };
        

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

    OverviewController.$inject = ['$interval', '$scope', '$cacheFactory', 'releaseManagementService', 'configService'];

    angular.module('rmDashboardApp').controller('overviewController', OverviewController);
})(angular);