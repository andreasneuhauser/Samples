var chatApp = angular.module('chatApp',['ngRoute', 'firebase']);

chatApp.config(function ($routeProvider){
    $routeProvider.when('/chat', {
            templateUrl: 'chat.html'
        })
        .when('/login', {
            templateUrl: 'login.html',
            controller: 'LoginController'
        })
        .when('/register', {
            templateUrl: 'register.html',
            controller: 'RegisterController'
        })
        .otherwise({
            redirectTo: '/login'
        });
});