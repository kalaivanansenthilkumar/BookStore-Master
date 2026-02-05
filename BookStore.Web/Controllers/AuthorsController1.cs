using BookStore.Web.Configuration;
using BookStore.Web.Models;
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
    public class AuthorsController1 : ApiController
    {
        private readonly DataBaseFactory _dataBaseFactory;
        private readonly IGcpSecretService _secretService;
        public AuthorsController1(IGcpSecretService secretService)
        {
            _secretService = secretService;
            _dataBaseFactory = new DataBaseFactory(_secretService);
        }
        public AuthorsController1()
        {

        }
        // GET: api/Authors
        public IQueryable<Author> GetAuthors()
        {
            return _dataBaseFactory.CreateContext().Authors;
        }

        // GET: api/Authors/5
        [ResponseType(typeof(Author))]
        public async Task<IHttpActionResult> GetAuthor(int id)
        {
            Author author = await _dataBaseFactory.CreateContext().Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        // PUT: api/Authors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAuthor(int id, Author author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != author.Id)
            {
                return BadRequest();
            }

            _dataBaseFactory.CreateContext().Entry(author).State = EntityState.Modified;

            try
            {
                await _dataBaseFactory.CreateContext().SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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

        // POST: api/Authors
        [ResponseType(typeof(Author))]
        public async Task<IHttpActionResult> PostAuthor(Author author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dataBaseFactory.CreateContext().Authors.Add(author);
            await _dataBaseFactory.CreateContext().SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = author.Id }, author);
        }

        // DELETE: api/Authors/5
        [ResponseType(typeof(Author))]
        public async Task<IHttpActionResult> DeleteAuthor(int id)
        {
            Author author = await _dataBaseFactory.CreateContext().Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _dataBaseFactory.CreateContext().Authors.Remove(author);
            await _dataBaseFactory.CreateContext().SaveChangesAsync();

            return Ok(author);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dataBaseFactory.CreateContext().Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AuthorExists(int id)
        {
            return _dataBaseFactory.CreateContext().Authors.Count(e => e.Id == id) > 0;
        }
    }
}