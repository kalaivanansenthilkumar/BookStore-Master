var ViewModel = function () {
    var self = this;
    self.books = ko.observableArray();
    self.error = ko.observable();
    self.detail = ko.observable();
    self.authors = ko.observableArray();
    self.Author= ko.observable();
    self.Title = ko.observable();
    self.Genre = ko.observable();
    self.Year = ko.observable();
    self.Price = ko.observable();
    self.selectedAuthorId = ko.observable();
    self.selectedBookId = ko.observable();
    self.Id = ko.observable();
    self.Name = ko.observable();
    self.authorDetail = ko.observable();

    var booksUri = '/api/books/';
    var authorsUri = '/api/authors/';

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
        });
    }

    function getAllBooks() {
        ajaxHelper(booksUri, 'GET').done(function (data) {
            self.books(data);
        });
    }

    self.getBookDetail = function (item) {
        ajaxHelper(booksUri + item.Id, 'GET').done(function (data) {
            self.detail(data);
        });
    }

    function getAuthors() {
        ajaxHelper(authorsUri, 'GET').done(function (data) {
            self.authors(data);
        });
    }


    self.addBook = function (formElement) {
        var newBook = {
            AuthorId: self.selectedAuthorId(),
            Genre: self.Genre(),
            Price: self.Price(),
            Title: self.Title(),
            Year: self.Year()
        };

        ajaxHelper(booksUri, 'POST', newBook).done(function (item) {
            self.clearFields();
            alert('Book Added Successfully');         
            self.books.push(item);
        });
    }
    
    self.getUpdateBook = function (book) {
        self.selectedBookId(book.Id);
        ajaxHelper(booksUri + book.Id, 'GET').done(function (data) {
            self.selectedAuthorId(data.AuthorId);
            self.Author(data.Author);
            self.Title(data.Title);
            self.Genre(data.Genre);
            self.Year(data.Year);
            self.Price(data.Price);

            $('#Save').hide();
            $('#Clear').hide();

            $('#Update').show();
            $('#Cancel').show();
        });
    }
    //Update Book  
    self.updateBook = function () {

        var updateBook = {
            Id: self.selectedBookId(),
            AuthorId: self.selectedAuthorId(),
            Genre: self.Genre(),
            Price: self.Price(),
            Title: self.Title(),
            Year: self.Year()
        };

        ajaxHelper(booksUri + self.selectedBookId(), 'PUT', updateBook).done(function () {
            alert('Book Updated Successfully !');
            getAllBooks();
            self.cancel();   
        });
    }
    //Delete Book  
    self.deleteBook = function (book) {
        ajaxHelper(booksUri + book.Id, 'DELETE').done(function () {
            alert('Book Deleted Successfully');
            getAllBooks();
        })

    }
    self.getAuthorDetail = function (item) {
        ajaxHelper(authorsUri + item.Id, 'GET').done(function (data) {
            self.authorDetail(data);
        });
    }

    self.addAuthor = function (formElement) {
        var newAuthor = {
            Name: self.Name(),

        };

        ajaxHelper(authorsUri, 'POST', newAuthor).done(function (item) {
            self.clearFields();
            alert('Author Added Successfully');
            self.authors.push(item);
        });
    }

    self.getUpdateAuthor = function (author) {
        self.selectedAuthorId(author.Id);
        ajaxHelper(authorsUri + author.Id, 'GET').done(function (data) {
            self.selectedAuthorId(data.Id);
            self.Name(data.Name);

            $('#Save').hide();
            $('#Clear').hide();

            $('#Update').show();
            $('#Cancel').show();
        });
    }
    //Update Book  
    self.updateAuthor = function () {

        var updateAuthor = {
            Id: self.selectedAuthorId(),
            Name: self.Name(),

        };

        ajaxHelper(authorsUri + self.selectedAuthorId(), 'PUT', updateAuthor).done(function () {
            alert('Author Updated Successfully !');
            getAuthors();
            self.cancel();
        });
    }
    //Delete Book  
    self.deleteAuthor = function (book) {
        ajaxHelper(booksUri + book.Id, 'DELETE').done(function () {
            alert('Author Deleted Successfully');
            getAuthors();
        })

    }
    // Clear Fields  
    self.clearFields = function clearFields() {
        self.selectedAuthorId('');
        self.Title('');
        self.Genre('');
        self.Year('');
        self.Price('');
        self.Name('');;
    }

    self.cancel = function () {

        self.clearFields();

        $('#Save').show();
        $('#Clear').show();

        $('#Update').hide();
        $('#Cancel').hide();
    }
    // Fetch the initial data.
    getAllBooks();
    getAuthors();
};

ko.applyBindings(new ViewModel());