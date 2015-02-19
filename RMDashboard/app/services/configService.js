(function (angular,$) {
    'use strict';

    /**
    * Loads and stores the configuration of the application
    * from a cookie stored on the client computer
    */
    function configService() {
        var cookieName = 'RM_DASHBOARD_CONFIG';

        $.cookie.json = true;

        /**
        * Load the configuration from the cookie on the client
        */
        function loadConfig() {
            return $.cookie(cookieName);
        }

        /**
        * Save the configuration in a cookie on the client
        */
        function saveConfig(config) {
            $.cookie(cookieName, config, { expires: 9999 });
        }

        return {
            saveConfig: saveConfig,
            loadConfig: loadConfig
        };
    }

    angular.module('rmDashboardApp').factory('configService', configService);
})(angular,jQuery);