(function (angular) {
    'use strict';

    /**
    * This directive displays the status of a single step in the release process
    */
    function stepStatusStyle() {
        function link($scope, element) {
            switch ($scope.step.status) {
                case 'Pending':
                    element.addClass("pending");
                    break;
                case 'Done':
                    element.addClass("done");
                    break;
                case 'Stopped':
                    element.addClass("stopped");
                    break;
                case 'Rejected':
                    element.addClass("rejected");
                    break;
                case 'Cancelled':
                    element.addClass("cancelled");
                    break;
                case 'Reassigned':
                    element.addClass("reassigned");
                    break;
            }
        }

        return {
            restrict: 'A',
            link: link
        };
    }

    angular.module('rmDashboardApp').directive('stepStatusStyle', stepStatusStyle);
})(angular);
