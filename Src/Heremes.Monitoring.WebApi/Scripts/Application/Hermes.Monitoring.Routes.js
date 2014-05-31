

angular.module('Hermes.Monitoring.Routes', ['ngRoute'])
    .config(function($routeProvider) {

        $routeProvider
            .when('/', { templateUrl: '/Web/Home.html', controller: 'HomeController', caseInsensitiveMatch: true })
            .when('/HermesPerformence', { templateUrl: '/Web/HermesPerformence.html', controller: 'HermesPerformenceController', caseInsensitiveMatch: true })
            .when('/EndpointPerformence', { templateUrl: '/Web/EndpointPerformence.html', controller: 'EndpointPerformenceController', caseInsensitiveMatch: true })
            .when('/QueueDisplay', {templateUrl: '/Web/QueueDisplay.html', controller: 'QueueDisplayController', caseInsensitiveMatch: true })
            .otherwise({
                redirectTo: '/'
            });
    });