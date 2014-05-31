

angular.module("Hermes.Monitoring.Directives", [])
    .directive("tree", function() {
    	return {
    		template: '<ul><tree-node ng-repeat="item in items"></tree-node></ul>',
    		restrict: 'E',
    		replace: true,
    		scope: {
    			items: '=items',
    		}
    	};
    })
    .directive("tree-node", function ($compile) {
    	return {
    		restrict: 'E',
    		template: '<li >{{item.name}}</li>',
    		link: function (scope, elm, attrs) {
    			if (scope.item.items.length > 0) {
    				var children = $compile('<tree items="item.items"></tree>')(scope);
    				elm.append(children);
    			}
    		}
    	};
    });
