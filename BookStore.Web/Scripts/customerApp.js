var CustomerViewModel = function () {

    var self = this;
    self.CustomerId = ko.observable();
    self.FirstName = ko.observable();
    self.LastName = ko.observable();
    self.Email = ko.observable();
    self.CountryList = ko.observableArray(['Morocco', 'India', 'USA', 'Spain']);
    self.countries = ko.observableArray();
    self.Country = ko.observable();

    self.customerList = ko.observableArray([]);

    var CustomerUri = '/api/customers/';



    function ajaxFunction(uri, method, data) {

        //self.errorMessage('');  

        return $.ajax({

            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data:data? JSON.stringify(data) : null  
  
        }).fail(function (jqXHR, textStatus, errorThrown) {
               // alert('Error  ' + errorThrown);
            });
    }


    // Clear Fields  
    self.clearFields = function clearFields() {
        self.FirstName('');
        self.LastName('');
        self.Email('');
        self.Country('');
    }

    //Add new Customer  
    self.addNewCustomer = function addNewCustomer(newCustomer) {

        var CustObject = {
            CustID: self.CustID(),
            FirstName: self.FirstName(),
            LastName: self.LastName(),
            Email:self.Email(),
            Country: self.Country()
        };
        ajaxFunction(CustomerUri, 'POST', CustObject).done(function () {

            self.clearFields();
            alert('Customer Added Successfully !');
            getCustomerList()
        });
    }

    //Get Customer List  
    function getCustomerList() {
        $("div.loadingZone").show();
        ajaxFunction(CustomerUri, 'GET').done(function (data) {
            $("div.loadingZone").hide();
            self.customerList(data);
        });

    }
    function getCountries() {
        ajaxFunction(CustomerUri, 'GET').done(function (data) {
            self.countries(data);
        });
    }
    //Get Detail Customer  
    self.detailCustomer = function (selectedCustomer) {

        self.CustID(selectedCustomer.CustID);
        self.FirstName(selectedCustomer.FirstName);
        self.LastName(selectedCustomer.LastName);
        self.Email(selectedCustomer.Email);
        self.Country(selectedCustomer.Country);

        $('#Save').hide();
        $('#Clear').hide();

        $('#Update').show();
        $('#Cancel').show();

    };

    self.cancel = function () {

        self.clearFields();

        $('#Save').show();
        $('#Clear').show();

        $('#Update').hide();
        $('#Cancel').hide();
    }

    //Update Customer  
    self.updateCustomer = function () {

        var CustObject = {
            CustID: self.CustID(),
            FirstName: self.FirstName(),
            LastName: self.LastName(),
            Email :self.Email(),
            Country :self.Country()
        };

        ajaxFunction(CustomerUri + self.CustID(), 'PUT', CustObject).done(function () {
            alert('Customer Updated Successfully !');
            getCustomerList();
            self.cancel();
        });
    }

    //Delete Customer  
    self.deleteCustomer = function (customer) {

        ajaxFunction(CustomerUri + customer.CustID, 'DELETE').done(function () {

            alert('Customer Deleted Successfully');
            getCustomerList();
        })

    }

    //Chart Line function used to display a chart which represents nb of customers by country  
    function chartLine() {

        ajaxFunction('http://localhost:50524/Customers/GetCustomerByCountry', 'GET').done(function (result) {
            console.log(result);
            Morris.Line({
                element: 'line-chart',
                data:result,
                xkey: 'CountryName',
                // A list of names of data record attributes that contain y-values.  
                ykeys: ['value'],
                // Labels for the ykeys -- will be displayed when you hover over the  
                // chart.  
                labels :['Value'],

                parseTime: false  
            });


        });

    };
    getCountries();
    chartLine();
    getCustomerList();
    
};

ko.applyBindings(new CustomerViewModel());  