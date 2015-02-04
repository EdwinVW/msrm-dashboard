'use strict';
rmDashboardApp
    .directive('stepStatusStyle', function () {
        var directive = {};

        directive.restrict = 'A'; 

        directive.link = function ($scope, element, attributes) {

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

        return directive;
    })
