'use strict';

rmDashboardApp.factory('configService', function () {
    var cookieName = 'RM_DASHBOARD_CONFIG';

    $.cookie.json = true;

    var configService = {
        loadConfig: function () {
            return $.cookie(cookieName);
        },
        saveConfig: function(config){
            $.cookie(cookieName, config, { expires: 9999 });
        }
    };

    return configService;
});