

angular.module('Hermes.Monitoring.Controllers', ['Hermes.Monitoring.Services', 'highcharts-ng'])
    .controller('HermesPerformenceController', ['$scope', 'HermesPerformenceApiService', 'signalRHubProxy', function ($scope, HermesPerformenceApiService, signalRHubProxy) {

        var hermesPerformenceHubProxy = signalRHubProxy(
             'http://localhost:61676', 'HermesPerformenceHub',
                 { logging: true });
        
        hermesPerformenceHubProxy.on('updateHermesPerformenceMonitor', function (data) {
            $scope.currentServerTime = data;

        });

        $scope.chartSeries = [
            {
                name: "Messages per second",
                data: [10,20,30,40,50,60]
            }];

        $scope.chartConfig = {
           options: {
               chart: {
                   type: 'column'
               }
           },
           series: $scope.chartSeries,
        };

        $scope.AddMessagePerSecondResult = function(messagesPerSecond) {
            $scope.chartSeries[0].data.push(messagesPerSecond);
        };

        $scope.$on('$viewContentLoaded', function () {
         
        });
        
    }])
    .controller('HomeController', function () {
        



    })
    .controller('TestSignalRController', function() {
    })
    .controller('ServerTimeController', ['$scope', 'signalRHubProxy', function ($scope, signalRHubProxy) {
        var clientPushHubProxy = signalRHubProxy(signalRHubProxy.defaultServer, 'clientPushHub', { logging: true });
        var serverTimeHubProxy = signalRHubProxy(signalRHubProxy.defaultServer, 'serverTimeHub');

        clientPushHubProxy.on('serverTime', function (data) {
            $scope.currentServerTime = data;
            var x = clientPushHubProxy.connection.id;
        });

        $scope.getServerTime = function () {
            serverTimeHubProxy.invoke('getServerTime', function (data) {
                $scope.currentServerTimeManually = data;
            });
        };
    }]);
