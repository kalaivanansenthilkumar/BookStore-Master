(function (app) {
    'use strict';

    app.controller('indexCtrl', indexCtrl);

    indexCtrl.$inject = ['$scope','apiService', 'notificationService'];

    function indexCtrl($scope, apiService, notificationService) {
        $scope.pageClass = 'page-home';
        $scope.loadingBooks = true;
        $scope.loadingGenres = true;
        $scope.isReadOnly = true;

        $scope.latestBooks = [];
        $scope.loadData = loadData;

        function loadData() {
            apiService.get('/api/books/latest', null,
                booksLoadCompleted,
                booksLoadFailed);

            apiService.get("/api/genres/", null,
                genresLoadCompleted,
                genresLoadFailed);
        }

        function booksLoadCompleted(result) {
            $scope.latestBooks = result.data;
            $scope.loadingBooks = false;
        }

        function genresLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function booksLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function genresLoadCompleted(result) {
            var genres = result.data;
            Morris.Bar({
                element: "genres-bar",
                data: genres,
                xkey: "Name",
                ykeys: ["NumberOfMovies"],
                labels: ["Number Of Movies"],
                barRatio: 0.4,
                xLabelAngle: 55,
                hideHover: "auto",
                resize: 'true'
            });
            //.on('click', function (i, row) {
            //    $location.path('/genres/' + row.ID);
            //    $scope.$apply();
            //});

            $scope.loadingGenres = false;
        }

        loadData();
    }

})(angular.module('bookStore'));