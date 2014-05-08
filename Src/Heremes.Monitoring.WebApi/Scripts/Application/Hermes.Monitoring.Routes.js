

angular.module('Hermes.Monitoring.Routes', ['ngRoute'])
    .config(function($routeProvider) {

        $routeProvider
            .when('/', { templateUrl: '/Web/Home.html', controller: 'HomeController', caseInsensitiveMatch: true })
            .when('/HermesPerformence', { templateUrl: '/Web/HermesPerformence.html', controller: 'HermesPerformenceController', caseInsensitiveMatch: true })
            .when('/TestSignalR', { templateUrl: '/Web/TestSignalR.html', controller: 'TestSignalRController', caseInsensitiveMatch: true })
            .when('/ServerTime', { templateUrl: '/Web/ServerTime.html', controller: 'ServerTimeController', caseInsensitiveMatch: true })
            .otherwise({
                redirectTo: '/'
            });
    });