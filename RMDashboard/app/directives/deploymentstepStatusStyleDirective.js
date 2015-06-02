(function (angular) {
    'use strict';

    /**
    * This directive displays the status of a deploymentstep
    */
    function deploymentstepStatusStyle() {
        function link($scope, element) {
            switch ($scope.deploymentStep.status) {
                case 'Pending':
                    element.addClass("pending");
                    break;
                case 'In Progress':
                    element.addClass("in-progress");
                    break;
                case 'Succeeded':
                    element.addClass("succeeded");
                    break;
                case 'Failed':
                    element.addClass("failed");
                    break;
                case 'Cancelled':
                    element.addClass("cancelled");
                    break;
            }
        }

        return {
            restrict: 'A',
            link: link
        }
    }

    angular.module('rmDashboardApp').directive('deploymentstepStatusStyle', deploymentstepStatusStyle);
})(angular);