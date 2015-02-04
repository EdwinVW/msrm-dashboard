'use strict';
rmDashboardApp
    .directive('releaseStatusStyle', function () {
        var directive = {};

        directive.restrict = 'A'; 

        directive.link = function ($scope, element, attributes) {

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

        return directive;
    })
