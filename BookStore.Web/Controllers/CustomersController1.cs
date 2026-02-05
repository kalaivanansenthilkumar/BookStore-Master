using BookStore.Web.Configuration;
using BookStore.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace BookStore.Web.Controllers
{
    public class CustomersController1 :ApiController  
    {
        //DbContext  
        private readonly DataBaseFactory _dataBaseFactory;
        private readonly IGcpSecretService _secretService;
        public CustomersController1(IGcpSecretService secretService)
        {
            _secretService = secretService;
            _dataBaseFactory = new DataBaseFactory(_secretService);
        }
        public CustomersController1()
        {
        }
        // GET api/Customers  
        public IQueryable<Customer> GetCustomers()
        {
        return _dataBaseFactory.CreateContext().Customers;
        }

        // GET api/Coutnries  
        public IQueryable<Country> GetCountries()
        {
            return _dataBaseFactory.CreateContext().Countries;
        }


        // PUT api/Customers/5  
        [ResponseType(typeof(void))]
    public IHttpActionResult PutCustomer(int id, Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != customer.CustomerId)
        {
            return BadRequest();
        }

            _dataBaseFactory.CreateContext().Entry(customer).State = EntityState.Modified;

        try
        {
                _dataBaseFactory.CreateContext().SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CustomerExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return StatusCode(HttpStatusCode.NoContent);
    }

    // POST api/Customers  
    [ResponseType(typeof(Customer))]
    public IHttpActionResult PostCustomer(Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

            _dataBaseFactory.CreateContext().Customers.Add(customer);
            _dataBaseFactory.CreateContext().SaveChanges();

        return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
    }

    // DELETE api/Customers/5  
    [ResponseType(typeof(Customer))]
    public IHttpActionResult DeleteCustomer(int id)
    {
        Customer customer = _dataBaseFactory.CreateContext().Customers.Find(id);
        if (customer == null)
        {
            return NotFound();
        }

            _dataBaseFactory.CreateContext().Customers.Remove(customer);
            _dataBaseFactory.CreateContext().SaveChanges();

        return Ok(customer);
    }
        //GetCustomerByCountry returns list of nb customers by country   
        [Route("Customers/GetCustomerByCountry")]
        public IList<CustomerDTO> GetCustomerByCountry()
        {
            List<string> countryList = new List<string>() { "Morocco", "India", "USA", "Spain" };
            IEnumerable<Customer> customerList = _dataBaseFactory.CreateContext().Customers;
            List<CustomerDTO> result = new List<CustomerDTO>();

            foreach (var item in countryList)
            {
                int nbCustomer = customerList.Where(c => c.Country == item).Count();
                result.Add(new CustomerDTO()
                {
                    CountryName = item,
                    value = nbCustomer
                });
            }

            if (result != null)
            {
                return result;
            }

            return null;
        }

        protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
                _dataBaseFactory.CreateContext().Dispose();
        }
        base.Dispose(disposing);
    }

    private bool CustomerExists(int id)
    {
        return _dataBaseFactory.CreateContext().Customers.Count(e => e.CustomerId == id) > 0;
    }
 }  
}
