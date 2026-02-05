using BookStore.Web.Configuration;
using BookStore.Web.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace BookStore.Web.Controllers
{
    public class BooksController1 : ApiController
    {
        private readonly DataBaseFactory _dataBaseFactory;
        private readonly IGcpSecretService _secretService;
        public BooksController1(IGcpSecretService secretService)
        {
            _secretService = secretService;
            _dataBaseFactory = new DataBaseFactory(_secretService);
        }
        public BooksController1()
        {
        }

        // GET: api/Books
        public IQueryable<BookDTO> GetBooks()
        {
            var books = from b in _dataBaseFactory.CreateContext().Books
                        select new BookDTO()
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Price = b.Price,
                            Year = b.Year,
                            Genre = b.Genre,
                            AuthorName = b.Author.Name
                        };

            return books;
        }

        // GET: api/Books/5
        [ResponseType(typeof(BookDetailDTO))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            var book = await _dataBaseFactory.CreateContext().Books.Include(b => b.Author).Select(b =>
                new BookDetailDTO()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Year = b.Year,
                    Price = b.Price,
                    AuthorId=b.Author.Id,
                    AuthorName = b.Author.Name,
                    Genre = b.Genre
                }).SingleOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // PUT: api/Books/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                return BadRequest();
            }

            _dataBaseFactory.CreateContext().Entry(book).State = EntityState.Modified;

            try
            {
                await _dataBaseFactory.CreateContext().SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // POST: api/Books
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dataBaseFactory.CreateContext().Books.Add(book);
            await _dataBaseFactory.CreateContext().SaveChangesAsync();

            // Load author name
            _dataBaseFactory.CreateContext().Entry(book).Reference(x => x.Author).Load();

            var dto = new BookDTO()
            {
                Id = book.Id,
                Title = book.Title,
                Price = book.Price,
                Year = book.Year,
                Genre = book.Genre,
                AuthorName = book.Author.Name
            };

            return CreatedAtRoute("DefaultApi", new { id = book.Id }, dto);
        }

        // DELETE: api/Books/5
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            Book book = await _dataBaseFactory.CreateContext().Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _dataBaseFactory.CreateContext().Books.Remove(book);
            await _dataBaseFactory.CreateContext().SaveChangesAsync();

            return Ok(book);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dataBaseFactory.CreateContext().Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return _dataBaseFactory.CreateContext().Books.Count(e => e.Id == id) > 0;
        }
    }
}