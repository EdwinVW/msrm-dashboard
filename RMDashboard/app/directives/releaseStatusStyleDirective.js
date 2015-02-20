(function (angular) {
    'use strict';

    /**
    * This directive displays the status of a release
    */
    function releaseStatusStyle() {
        function link($scope, element) {
            switch ($scope.release.status) {
                case 'In Progress':
                    element.addClass("in-progress");
                    break;
                case 'Released':
                    element.addClass("released");
                    break;
                case 'Rejected':
                    element.addClass("error");
                    break;
                case 'Stopped':
                    element.addClass("error");
                    break;
                case 'Abandoned':
                    element.addClass("error");
                    break;
                case 'Draft':
                    element.addClass("draft");
                    break;
            }
        }

        return {
            restrict: 'A',
            link: link
        }
    }

    angular.module('rmDashboardApp').directive('releaseStatusStyle', releaseStatusStyle);
})(angular);
