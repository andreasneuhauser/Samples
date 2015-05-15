chatApp.controller('RegisterController', function($scope, $firebaseArray){
    var users = new Firebase("https://codecampchatusers.firebaseio.com/");

    $scope.fbUsers = $firebaseArray(users);

    $scope.signUp = function(){
        console.log('Username: ' + $scope.username);
        console.log('Password: ' + $scope.password);

        $scope.fbUsers.$add({ username: $scope.username, password: $scope.password});

        alert('Button SignUp clicked');
    };
});