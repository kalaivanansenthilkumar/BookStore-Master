using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookStore.Web.Controllers;
using BookStore.Web.Models;
using System.Web.Http.Results;

namespace BookStore.Web.Tests
{
    [TestClass]
    public class CustomersControllerTests
    {
        [TestMethod]
        public void PutCustomer_IdMismatch_ReturnsBadRequest()
        {
            var controller = new CustomersController();
            var customer = new Customer { CustomerId = 2 };

            var result = controller.PutCustomer(1, customer);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void PutCustomer_InvalidModel_ReturnsInvalidModelStateResult()
        {
            var controller = new CustomersController();
            controller.ModelState.AddModelError("Name", "Required");
            var customer = new Customer { CustomerId = 1 };

            var result = controller.PutCustomer(1, customer);

            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void PostCustomer_InvalidModel_ReturnsInvalidModelStateResult()
        {
            var controller = new CustomersController();
            controller.ModelState.AddModelError("Name", "Required");
            var customer = new Customer(); // Missing required properties

            var result = controller.PostCustomer(customer);

            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }
    }
}