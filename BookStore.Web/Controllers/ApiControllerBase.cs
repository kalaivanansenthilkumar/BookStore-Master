using BookStore.Web.Infrastructure;
using BookStore.Web.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace BookStore.Web.Controllers
{
    public abstract class ApiControllerBase : ApiController
    {
        protected readonly ILogger _logger;

        // Constructor injection for logging
        protected ApiControllerBase(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        ///// <summary>
        ///// Creates a standardized success response.
        ///// </summary>
        //protected IHttpActionResult Success(object? data = null, string? message = null)
        //{
        //    return Ok(new
        //    {
        //        success = true,
        //        message = message ?? "Request successful",
        //        data
        //    });
        //}
        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response = null;

            try
            {
                response = function.Invoke();
            }
            catch (DbUpdateException ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }
        private void LogError(Exception ex)
        {
            try
            {
                Error _error = new Error()
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    DateCreated = DateTime.Now
                };

                //_errorsRepository.Add(_error);
                //_unitOfWork.Commit();
            }
            catch { }
        }
        /// <summary>
        /// Creates a standardized error response.
        /// </summary>
        //protected IHttpActionResult Error(string message, int statusCode = 400)
        //{
        //    return Sya(statusCode, new
        //    {
        //        success = false,
        //        message
        //    });
        //}
    }
}
