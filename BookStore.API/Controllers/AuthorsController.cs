using AutoMapper;
using BookStore.API.Infrastructure.Core;
using BookStore.API.Infrastructure.Extensions;
using BookStore.API.Models;
using BookStore.Data;
using BookStore.Data.Infrastructure;
using BookStore.Data.Repositories;
using BookStore.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace BookStore.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/authors")]
    public class AuthorsController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Author> _authorsRepository;

        public AuthorsController(IEntityBaseRepository<Author> authorsRepository,
            IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _authorsRepository = authorsRepository;
        }

        [AllowAnonymous]
        [Route("latest")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var authors = _authorsRepository.GetAll().Take(6).ToList();

                IEnumerable<AuthorViewModel> authorVM = Mapper.Map<IEnumerable<Author>, IEnumerable<AuthorViewModel>>(authors);

                response = request.CreateResponse<IEnumerable<AuthorViewModel>>(HttpStatusCode.OK, authorVM);

                return response;
            });
        }

        [Route("details/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var author = _authorsRepository.GetSingle(id);

                AuthorViewModel movieVM = Mapper.Map<Author, AuthorViewModel>(author);

                response = request.CreateResponse<AuthorViewModel>(HttpStatusCode.OK, movieVM);

                return response;
            });
        }

        [AllowAnonymous]
        [Route("{page:int=0}/{pageSize=3}/{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Author> authors = null;
                int totalAuthors = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    authors = _authorsRepository
                        .FindBy(m => m.Email.ToLower()
                        .Contains(filter.ToLower().Trim()))
                        .OrderBy(m => m.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalAuthors = _authorsRepository
                        .FindBy(m => m.Email.ToLower()
                        .Contains(filter.ToLower().Trim()))
                        .Count();
                }
                else
                {
                    authors = _authorsRepository
                        .GetAll()
                        .OrderBy(m => m.ID)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalAuthors = _authorsRepository.GetAll().Count();
                }

                IEnumerable<AuthorViewModel> moviesVM = Mapper.Map<IEnumerable<Author>, IEnumerable<AuthorViewModel>>(authors);

                PaginationSet<AuthorViewModel> pagedSet = new PaginationSet<AuthorViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalAuthors,
                    TotalPages = (int)Math.Ceiling((decimal)totalAuthors / currentPageSize),
                    Items = moviesVM
                };

                response = request.CreateResponse<PaginationSet<AuthorViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Add(HttpRequestMessage request, AuthorViewModel author)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    Author newAuthor = new Author();
                    newAuthor.UpdateAuthor(author);
                    _authorsRepository.Add(newAuthor);
                    _unitOfWork.Commit();
                    // Update view model
                    author = Mapper.Map<Author, AuthorViewModel>(newAuthor);
                    response = request.CreateResponse<AuthorViewModel>(HttpStatusCode.Created, author);
                }

                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, AuthorViewModel author)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var authorDb = _authorsRepository.GetSingle(author.ID);
                    if (authorDb == null)
                        response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid movie.");
                    else
                    {
                        authorDb.UpdateAuthor(author);

                        _authorsRepository.Edit(authorDb);

                        _unitOfWork.Commit();
                        response = request.CreateResponse<AuthorViewModel>(HttpStatusCode.OK, author);
                    }
                }

                return response;
            });
        }
    }
}
