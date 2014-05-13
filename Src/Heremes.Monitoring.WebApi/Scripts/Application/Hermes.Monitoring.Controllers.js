

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
              'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
            [ $scope.AverageTimeToDeliverMessagesSeries, $scope.AverageTimeToProcessMessagesSeries]
        );
       
        $scope.$on('$viewContentLoaded', function () {
         
        });

        
        }])
    .controller('EndpointPerformenceController', ['$scope', 'signalRHubProxy', 'highChartHelper', function ($scope, signalRHubProxy, highChartHelper) {

        var hermesPerformenceHubProxy = signalRHubProxy.CreateProxy(
          'HermesPerformenceHub',
               { logging: true });
        
        hermesPerformenceHubProxy.on('updateEndpointPerformence', function (data) {

            for(var i=0; i<data.length; i++)
            {
                if ($scope.ExistInSeries(data[i].Endpoint.Queue).exists) {
                    $scope.AddDataToExistingEndpoint(data[i].Endpoint.Queue, data[i].AverageMessagesPerSecond);
                } else {
                    $scope.AddNewEndpoint(data[i].Endpoint.Queue, data[i].AverageMessagesPerSecond);
                }
            }
            $scope.UpdateEndpointsThatAreNotInResponseFromServer();
        });

        $scope.MaximumDataUnits = 50;
        
        $scope.AddData = function (dataArray, data) {
            if (dataArray.length > $scope.MaximumDataUnits) {
                dataArray.data.splice(0, 1);
                dataArray.data.push(data);
            } else {
                dataArray.data.push(data);
            }
        };

        $scope.HighestSeriesDataLength = function() {
            var highestCount = 0;
            for (var i = 0; i < $scope.EndpointSeriesCollection.length; i++) {
                var length = $scope.EndpointSeriesCollection[i].data.length;
                if (highestCount > length) {
                    highestCount = length;
                }
            }
            return highestCount;
        };

        $scope.UpdateEndpointsThatAreNotInResponseFromServer = function() {
            var highestSeriesDataLength = $scope.HighestSeriesDataLength();
            for (var i = 0; i < $scope.EndpointSeriesCollection.length; i++) {
                var length = $scope.EndpointSeriesCollection[i].data.length;
                if (length < highestSeriesDataLength) {
                    var howMuchLess = highestSeriesDataLength - length;
                    for (var j = 0; j < howMuchLess; j++) {
                        $scope.AddData($scope.EndpointSeriesCollection[i].data, 0);
                    }
                }
            }
        };

        $scope.GetEndpointSeries = function (queueName) {
            for (var i = 0; i < $scope.EndpointSeriesCollection.length; i++) {
                if ($scope.EndpointSeriesCollection[i].name == queueName) {
                    return $scope.EndpointSeriesCollection[i];
                }
            }
        };

        $scope.AddDataToExistingEndpoint = function (queueName, data) {
            $scope.AddData($scope.GetEndpointSeries(queueName), data);
        };

        $scope.AddNewEndpoint = function(queueName, data) {
            var series = {
                name: queueName,
                type: 'column',
                data: [data]
            };
            $scope.EndpointSeriesCollection.push(series);
        };

        $scope.ExistInSeries = function(queueName) {
            for (var i = 0; i < $scope.EndpointSeriesCollection.length; i++) {
                if ($scope.EndpointSeriesCollection[i].name == queueName) {
                    return {
                        exists: true,
                        index: i
                    };
                }
            }
            return {
                exists: false,
                index: i
            };
        };

        $scope.EndpointSeriesCollection = [];

        $scope.TotalTimeIntervalInSeconds = 10;

        $scope.EndpointPerformenceConfig = highChartHelper.CreateColumnChart('Average Messages Per Second',
            'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
            'Time in Milliseconds',
            'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
            $scope.EndpointSeriesCollection);
        

    }])
    .controller('HomeController', function () {
        



    });
