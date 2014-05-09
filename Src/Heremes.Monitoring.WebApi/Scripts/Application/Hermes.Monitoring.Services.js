angular.module('Hermes.Monitoring.Services', [])
    .factory('HermesPerformenceApiService', function() {


    })
    .factory('signalRHubProxy', ['$rootScope', 'signalRServer', function ($rootScope, signalRServer) {
        function signalRHubProxyFactory(serverUrl, hubName, startOptions) {
            var connection = $.hubConnection(signalRServer);
            var proxy = connection.createHubProxy(hubName);
            connection.start(startOptions).done(function () { });

            return {
                on: function (eventName, callback) {
                    proxy.on(eventName, function (result) {
                        $rootScope.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
                },
                off: function (eventName, callback) {
                    proxy.off(eventName, function (result) {
                        $rootScope.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
                },
                invoke: function (methodName, callback) {
                    proxy.invoke(methodName)
                        .done(function (result) {
                            $rootScope.$apply(function () {
                                if (callback) {
                                    callback(result);
                                }
                            });
                        });
                },
                connection: connection
            };
        };

        return signalRHubProxyFactory;
    }])
    .factory('highChartHelper', function() {

        var createChart = function(type, title, subtitle, yAxisDescription, xAxaisDescription, assignedSeries) {

            return {
                options:
                {
                    chart: {
                        type: type
                    },
                    title: {
                        text: title
                    },
                    subtitle: {
                        text: subtitle
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: yAxisDescription
                        }
                    },
                    xAxis: {
                        title: {
                            text: xAxaisDescription
                        }
                    },
                    tooltip: {
                        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                        pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                            '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
                        footerFormat: '</table>',
                        shared: true,
                        useHTML: true
                    },
                    plotOptions: {
                        column: {
                            pointPadding: 0.2,
                            borderWidth: 0
                        }
                    },
                },
                series: assignedSeries,
            };
        };

        return {
            CreateChart: function(type, title, subtitle, yAxisDescription, xAxaisDescription, assignedSeries) {
                return createChart(type, title, subtitle, yAxisDescription, xAxaisDescription, assignedSeries);
            },
            CreateLineChart: function (title, subtitle, yAxisDescription, xAxaisDescription, assignedSeries) {
                return createChart('line', title, subtitle, yAxisDescription, xAxaisDescription, assignedSeries);
            }
        };
    });