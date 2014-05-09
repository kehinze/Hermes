

angular.module('Hermes.Monitoring.Controllers', ['Hermes.Monitoring.Services', 'highcharts-ng'])
    .controller('HermesPerformenceController', ['$scope', 'HermesPerformenceApiService', 'signalRHubProxy', 'highChartHelper',
        function ($scope, HermesPerformenceApiService, signalRHubProxy, highChartHelper) {

       
        var hermesPerformenceHubProxy = signalRHubProxy(
             'http://localhost:61676', 'HermesPerformenceHub',
                 { logging: true });
        
        hermesPerformenceHubProxy.on('updateHermesPerformenceMonitor', function (data) {
        
            $scope.AddItem($scope.AverageMessagesPerSecondSeries[0].data, data.MessagesPerSecond);
            $scope.AddItem($scope.AverageTimeToDeliverMessagesSeries.data,  data.AverageTimeToDeliverMessages);
            $scope.AddItem($scope.AverageTimeToProcessMessagesSeries.data, data.AverageTimeToProcessMessages);
            $scope.TotalTimeIntervalInSeconds = $scope.TotalTimeIntervalInSeconds;

        });

        $scope.MaximumDataUnits = 50;

        $scope.TotalTimeIntervalInSeconds = 10;

        $scope.AddItem = function (dataArray, data) {
            if (dataArray.length > $scope.MaximumDataUnits) {
                dataArray.splice(0, 1);
                dataArray.push(data);
            } else {
                dataArray.push(data);
            }
        };

        $scope.AverageMessagesPerSecondSeries = [{
            name: 'Average Messages per second',
            type: 'line',
            data: [0],
        }];

        $scope.AverageTimeToDeliverMessagesSeries = {
            name: 'Average Time To Deliver Message',
            type: 'line',
            data: [0]
        };
        
        $scope.AverageTimeToProcessMessagesSeries = {
            name: 'Average Time To Process Message',
            type: 'line',
            data: [0]
        };
        
        $scope.AverageMessagesPerSecondChartConfig = highChartHelper.CreateLineChart('Average Messages Per Second',
            'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
             'Average Messages',
             'Time Interval in ' + $scope.TotalTimeIntervalInSeconds + ' s',
           $scope.AverageMessagesPerSecondSeries
        );
        
        $scope.AverageTimeConfig = highChartHelper.CreateLineChart('Average',
             'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
             'Time in Milliseconds',
             'dummy',
            [ $scope.AverageTimeToDeliverMessagesSeries, $scope.AverageTimeToProcessMessagesSeries]
        );
       
        $scope.$on('$viewContentLoaded', function () {
         
        });

        
    }])
    .controller('HomeController', function () {
        



    })
    .controller('TestSignalRController', function() {
    });
