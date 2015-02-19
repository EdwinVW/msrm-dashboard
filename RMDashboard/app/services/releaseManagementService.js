'use strict';

rmDashboardApp.factory('releaseManagementService', ['$http', 'configService', function ($http, configService) {
    var releaseManagementService = {

        // get all release data
        getReleases: function (callback) {

            // initialize request
            var req = {
                method: 'GET',
                url: '/api/releases',
            }

            // load configuration
            var config = configService.loadConfig();

            // add header with releasepathids to include in the query
            if (config) {
                req.headers = {
                    includedReleasePathIds: config.includedReleasePaths.toString(),
                    releaseCount: config.releaseCount,
                    showComponents: config.showComponents
                };
            }

            // execute request
            $http(req)
               .success(function (data, status, header, config) {
                   callback(null, data);
               })
               .error(function (data, status, header, config) {
                   callback(data, null);
               });
        },

        // get all available releasepaths
        getReleasePaths: function (callback) {
            $http.get('/api/releasepaths')
               .success(function (data, status, header, config) {
                   callback(null, data);
               })
               .error(function (data, status, header, config) {
                   callback(data, null);
               });
        }
    };

    return releaseManagementService;
}]);