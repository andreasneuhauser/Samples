chatApp.controller('LoginController', function($scope, $firebaseArray, $location){
    var users = new Firebase("https://codecampchatusers.firebaseio.com/");

    $scope.fbUsers = $firebaseArray(users);

    $scope.signIn = function() {
        console.log('signIn called');

        var indexOfUser = $scope.fbUsers.map(function(user) {
            return user.username;
        }).indexOf($scope.username);

        if(indexOfUser != -1) {
            console.log('user found');

            if ($scope.fbUsers[indexOfUser].username == $scope.username && $scope.fbUsers[indexOfUser].password == $scope.password) {
                $location.path('/chat');
            }
        }
    }
});