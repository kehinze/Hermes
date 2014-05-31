

angular.module('Hermes.Monitoring', ['Hermes.Monitoring.Routes',
    'Hermes.Monitoring.Controllers',
    'Hermes.Monitoring.Services',
    'Hermes.Monitoring.Directives',
    'angularBootstrapNavTree'])
    .run(function() {

    })
    .value('signalRServer', '');