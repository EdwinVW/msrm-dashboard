(function (angular) {
    'use strict';

    /**
    * Talks to the API endpoint to retrieve information about releases
    */
    function releaseManagementService($http, configService) {
        /**
        * Retrieves a list of releases available on the release manager server
        */
        function getReleases(callback) {
            var req = {
                method: 'GET',
                url: '/api/releases',
            }

            var config = configService.loadConfig();

            if (config) {
                req.headers = {
                    includedReleasePathIds: config.includedReleasePaths.toString(),
                    releaseCount: config.releaseCount,
                    showComponents: config.showComponents
                };
            }

            $http(req)
               .success(function (data) {
                   callback(null, data);
               })
               .error(function (data) {
                   callback(data, null);
               });
        }

        /**
        * Retrieves all release paths available on the release manager server
        */
        function getReleasePaths(callback ) {
            $http.get('/api/releasepaths')
               .success(function (data) {
                   callback(null, data);
               })
               .error(function (data) {
                   callback(data, null);
               });
        }

        return {
            getReleases: getReleases,
            getReleasePaths: getReleasePaths
        };
    }

    releaseManagementService.$inject = ['$http', 'configService'];

    angular.module('rmDashboardApp').factory('releaseManagmentService', releaseManagementService);
})(angular);