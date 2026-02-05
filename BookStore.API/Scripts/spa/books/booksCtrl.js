(function (app) {
    'use strict';

    app.controller('booksCtrl', booksCtrl);

    booksCtrl.$inject = ['$scope', 'apiService','notificationService'];

    function booksCtrl($scope, apiService, notificationService) {
        $scope.pageClass = 'page-movies';
        $scope.loadingBooks= true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        
        $scope.Books = [];

        $scope.search = search;
        $scope.clearSearch = clearSearch;

        function search(page) {
            page = page || 0;

            $scope.loadingBooks = true;

            var config = {
                params: {
                    page: page,
                    pageSize: 6,
                    filter: $scope.filterBooks
                }
            };

            apiService.get('/api/books/', config,
            booksLoadCompleted,
            booksLoadFailed);
        }

        function booksLoadCompleted(result) {
            $scope.Books = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingBooks = false;

            if ($scope.filterMovies && $scope.filterBooks.length)
            {
                notificationService.displayInfo(result.data.Items.length + ' books found');
            }
            
        }

        function booksLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function clearSearch() {
            $scope.filterBooks = '';
            search();
        }

        $scope.search();
    }

})(angular.module('bookStore'));