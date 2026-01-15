using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using BookStore.Web.Controllers;
using BookStore.Web.Models;
using System.Web.Http.Results;

namespace BookStore.Web.Tests
{
    [TestClass]
    public class BooksControllerTests
    {
        [TestMethod]
        public async Task PutBook_IdMismatch_ReturnsBadRequest()
        {
            var controller = new BooksController();
            var book = new Book { Id = 2 };

            var result = await controller.PutBook(1, book);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task PutBook_InvalidModel_ReturnsInvalidModelStateResult()
        {
            var controller = new BooksController();
            controller.ModelState.AddModelError("Title", "Required");
            var book = new Book { Id = 1 };

            var result = await controller.PutBook(1, book);

            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public async Task PostBook_InvalidModel_ReturnsInvalidModelStateResult()
        {
            var controller = new BooksController();
            controller.ModelState.AddModelError("Title", "Required");
            var book = new Book { Id = 0 }; // Missing required Title

            var result = await controller.PostBook(book);

            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }
    }
}