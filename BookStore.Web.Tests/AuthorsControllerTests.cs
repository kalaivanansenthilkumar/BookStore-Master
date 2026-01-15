using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using BookStore.Web.Controllers;
using BookStore.Web.Models;
using System.Web.Http.Results;

namespace BookStore.Web.Tests
{
    [TestClass]
    public class AuthorsControllerTests
    {
        [TestMethod]
        public async Task PutAuthor_IdMismatch_ReturnsBadRequest()
        {
            var controller = new AuthorsController();
            var author = new Author { Id = 2 };

            var result = await controller.PutAuthor(1, author);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task PutAuthor_InvalidModel_ReturnsInvalidModelStateResult()
        {
            var controller = new AuthorsController();
            controller.ModelState.AddModelError("Name", "Required");
            var author = new Author { Id = 1 };

            var result = await controller.PutAuthor(1, author);

            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public async Task PostAuthor_InvalidModel_ReturnsInvalidModelStateResult()
        {
            var controller = new AuthorsController();
            controller.ModelState.AddModelError("Name", "Required");
            var author = new Author { Id = 0 }; // Missing required Name

            var result = await controller.PostAuthor(author);

            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }
    }
}