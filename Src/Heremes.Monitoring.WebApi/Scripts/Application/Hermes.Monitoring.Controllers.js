

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


            var process = $scope.EndpointAverageTimeToDeliverMessagesCollection;
            var process23 = $scope.EndpointAverageTimeToProcessMessagesCollection;

           
            $scope.UpdateEndpoints($scope.EndpointSeriesCollection, data, $scope.DataFields[0]);
            $scope.UpdateEndpoints($scope.EndpointAverageTimeToDeliverMessagesCollection, data, $scope.DataFields[1]);
            $scope.UpdateEndpoints($scope.EndpointAverageTimeToProcessMessagesCollection, data, $scope.DataFields[2]);
        });

        $scope.DataFields = ['AverageMessagesPerSecond', 'AverageTimeToDeliverMessages', 'AverageTimeToProcessMessages'];

        $scope.MaximumDataUnits = 50;
        
        $scope.AddData = function (dataArray, data) {
            if (dataArray.length > $scope.MaximumDataUnits) {
                dataArray.data.splice(0, 1);
                dataArray.data.push(data);
            } else {
                dataArray.data.push(data);
            }
        };
        
        $scope.ExistsInResponse = function (queueName, responseEndpoints) {
            for(var i=0; i<responseEndpoints.length; i++)
            {
                if (responseEndpoints[i].Endpoint.Queue == queueName) {
                    return true;
                }
            }
            return false;
        };

        $scope.UpdateEndpointsInResponse = function (seriesCollection, responseEndpoints, dataFieldName) {
            for (var i = 0; i < responseEndpoints.length; i++) {
                var dataValue = $scope.GetDataField(dataFieldName, responseEndpoints[i]);
                console.log("data " + dataFieldName + ":" + dataValue);
                if ($scope.ExistInSeries(seriesCollection, responseEndpoints[i].Endpoint.Queue).exists) {
                   
                    $scope.AddDataToExistingEndpoint(seriesCollection, responseEndpoints[i].Endpoint.Queue, dataValue);
                } else {
                    $scope.AddNewEndpoint(seriesCollection, responseEndpoints[i].Endpoint.Queue, dataValue);
                }
            }
        };

        $scope.UpdateEndpointsNotInResponse = function(seriesCollection, responseEndpoints) {
            for (var i = 0; i < seriesCollection.length; i++) {
                var exists = $scope.ExistsInResponse(seriesCollection[i].name, responseEndpoints);

                if (!exists) {
                    $scope.AddDataToExistingEndpoint(seriesCollection, seriesCollection[i].name, 0);
                } 
            }
        };

        $scope.GetSeries = function(dataFieldName) {
            if ($scope.DataFields[0] == dataFieldName) {
                return $scope.EndpointSeriesCollection;
            }
            if ($scope.DataFields[1] == dataFieldName) {
                return $scope.EndpointAverageTimeToProcessMessagesCollection;
            }
            if ($scope.DataFields[2] == dataFieldName) {
                return $scope.EndpointAverageTimeToProcessMessagesCollection;
            }
        };

        $scope.GetDataField = function (dataFieldName, endpointData) {
            if ($scope.DataFields[0] == dataFieldName) {
                return endpointData.AverageMessagesPerSecond;
            }
            if ($scope.DataFields[1] == dataFieldName) {
                return endpointData.AverageTimeToDeliver;
            }
            if ($scope.DataFields[2] == dataFieldName) {
                return endpointData.AverageTimeToProcess;
            }
        };
        
        $scope.UpdateEndpoints = function(seriesCollection, responseEnpoints, dataFieldName) {
            $scope.UpdateEndpointsInResponse(seriesCollection, responseEnpoints, dataFieldName);
            $scope.UpdateEndpointsNotInResponse(seriesCollection, responseEnpoints);
        };
        
        $scope.GetEndpointSeries = function (seriesCollection, queueName) {
            for (var i = 0; i < seriesCollection.length; i++) {
                if (seriesCollection[i].name == queueName) {
                    return seriesCollection[i];
                }
            }
        };

        $scope.AddDataToExistingEndpoint = function (seriesCollection, queueName, data) {
            $scope.AddData($scope.GetEndpointSeries(seriesCollection, queueName), data);
        };

        $scope.AddNewEndpoint = function (seriesCollection, queueName, data) {
            var series = {
                name: queueName,
                type: 'column',
                data: [data]
            };
            seriesCollection.push(series);
        };

        $scope.ExistInSeries = function (seriesCollection, queueName) {
            for (var i = 0; i < seriesCollection.length; i++) {
                if (seriesCollection[i].name == queueName) {
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
        $scope.EndpointAverageTimeToDeliverMessagesCollection = [];
        
        $scope.EndpointAverageTimeToProcessMessagesCollection = [];
        
        $scope.EndpointSeriesCollection = [];

        $scope.TotalTimeIntervalInSeconds = 10;

        $scope.EndpointPerformenceConfig = highChartHelper.CreateColumnChart('Average Messages Per Second',
            'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
            'Time in Seconds',
            'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
            $scope.EndpointSeriesCollection);
        
        $scope.EndpointAverageTimeToProcessMessagesConfig = highChartHelper.CreateColumnChart('Average time to deliver messages',
          'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
          'Time in Milliseconds',
          'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
           $scope.EndpointAverageTimeToDeliverMessagesCollection);


        $scope.EndpointAverageTimeToProcessMessagesConfig = highChartHelper.CreateColumnChart('Average time to process messages',
           'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
           'Time in Milliseconds',
           'Time Interval - ' + $scope.TotalTimeIntervalInSeconds + ' s',
           $scope.EndpointAverageTimeToProcessMessagesCollection);

    }])
    .controller('HomeController', function () {
        



    });
