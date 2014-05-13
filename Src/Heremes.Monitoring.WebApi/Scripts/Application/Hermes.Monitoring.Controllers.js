

angular.module('Hermes.Monitoring.Controllers', ['Hermes.Monitoring.Services', 'highcharts-ng'])
    .controller('HermesPerformenceController', ['$scope', 'HermesPerformenceApiService', 'signalRHubProxy', 'highChartHelper',
        function ($scope, HermesPerformenceApiService, signalRHubProxy, highChartHelper) {
            // 'http://localhost:61676', 
       
        var hermesPerformenceHubProxy = signalRHubProxy.CreateProxy(
            'HermesPerformenceHub',
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
            type: 'column',
            data: [0],
        }];

        $scope.AverageTimeToDeliverMessagesSeries = {
            name: 'Average Time To Deliver Message',
            type: 'column',
            data: [0]
        };
        
        $scope.AverageTimeToProcessMessagesSeries = {
            name: 'Average Time To Process Message',
            type: 'column',
            data: [0]
        };
        
        $scope.AverageMessagesPerSecondChartConfig = highChartHelper.CreateColumnChart('Average Messages Per Second',
            'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
             'Average Messages',
             'Time Interval in ' + $scope.TotalTimeIntervalInSeconds + ' s',
           $scope.AverageMessagesPerSecondSeries
        );
        
        $scope.AverageTimeConfig = highChartHelper.CreateColumnChart('Average',
             'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
             'Time in Milliseconds',
             'dummy',
            [ $scope.AverageTimeToDeliverMessagesSeries, $scope.AverageTimeToProcessMessagesSeries]
        );
       
        $scope.$on('$viewContentLoaded', function () {
         
        });

        
        }])
    .controller('EndpointPerformence', ['$scope', 'signalRHubProxy', 'highChartHelper', function ($scope, signalRHubProxy, highChartHelper) {




    }])
    .controller('HomeController', function () {
        



    });
