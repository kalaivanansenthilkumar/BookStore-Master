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
    public class CustomersController :ApiController  
    {  
    //DbContext  
    private BookStoreContext db = new BookStoreContext();

    // GET api/Customers  
    public IQueryable<Customer> GetCustomers()
    {
        return db.Customers;
    }

        // GET api/Coutnries  
        public IQueryable<Country> GetCountries()
        {
            return db.Countries;
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

        db.Entry(customer).State = EntityState.Modified;

        try
        {
            db.SaveChanges();
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

        db.Customers.Add(customer);
        db.SaveChanges();

        return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
    }

    // DELETE api/Customers/5  
    [ResponseType(typeof(Customer))]
    public IHttpActionResult DeleteCustomer(int id)
    {
        Customer customer = db.Customers.Find(id);
        if (customer == null)
        {
            return NotFound();
        }

        db.Customers.Remove(customer);
        db.SaveChanges();

        return Ok(customer);
    }
        //GetCustomerByCountry returns list of nb customers by country   
        [Route("Customers/GetCustomerByCountry")]
        public IList<CustomerDTO> GetCustomerByCountry()
        {
            List<string> countryList = new List<string>() { "Morocco", "India", "USA", "Spain" };
            IEnumerable<Customer> customerList = db.Customers;
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
            db.Dispose();
        }
        base.Dispose(disposing);
    }

    private bool CustomerExists(int id)
    {
        return db.Customers.Count(e => e.CustomerId == id) > 0;
    }
 }  
}
