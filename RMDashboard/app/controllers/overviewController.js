'use strict';

rmDashboardApp
    .controller('overviewController', ['$scope', '$interval', 'releaseManagementService', 'configService',
        function ($scope, $interval, releaseManagementService, configService) {

            // initialize fields
            $scope.title = "Release Management Dashboard";
            $scope.theme = 'dark';
            var refreshInterval = 20000;
            var autoRefresh = true;

            // load configuration
            var config = configService.loadConfig();
            if (config) {
                $scope.title = config.title;
                $scope.theme = config.theme;
                refreshInterval = config.refreshInterval;
                autoRefresh = (config.autoRefresh == 'true');
            }

            // load data
            $scope.loadData = function () {
                releaseManagementService.getReleases(function (err, data) {
                    if (err) {
                        $scope.hasError = true;
                        $scope.error = data;
                    }
                    else {
                        $scope.hasError = false;
                        $scope.error = null;
                        $scope.data = data;
                    }
                });
            };

            // initial load
            $scope.loadData();

            // refresh
            if (autoRefresh) {
                $interval(function () { $scope.loadData(); }, refreshInterval);
            }
        }]);