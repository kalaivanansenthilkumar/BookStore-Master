using BookStore.Entities;
using BookStore.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.API.Infrastructure.Extensions
{
    public static class EntitiesExtensions
    {
        public static void UpdateBook(this Book movie, BookViewModel bookVm)
        {
            movie.Title = bookVm.Title;
            movie.Description = bookVm.Description;
            movie.AuthorId = bookVm.AuthorId;
            movie.Rating = bookVm.Rating;
            movie.TrailURI = bookVm.TrailURI;
            movie.IssueDate = bookVm.IssueDate;
        }

        public static void UpdateCustomer(this Customer customer, CustomerViewModel customerVm)
        {
            customer.FirstName = customerVm.FirstName;
            customer.LastName = customerVm.LastName;
            customer.IdentityCard = customerVm.IdentityCard;
            customer.Mobile = customerVm.Mobile;
            customer.DateOfBirth = customerVm.DateOfBirth;
            customer.Email = customerVm.Email;
            customer.UniqueKey = (customerVm.UniqueKey == null || customerVm.UniqueKey == Guid.Empty)
                ? Guid.NewGuid() : customerVm.UniqueKey;
            customer.RegistrationDate = (customer.RegistrationDate == DateTime.MinValue ? DateTime.Now : customerVm.RegistrationDate);
        }

        public static void UpdateAuthor(this Author author, AuthorViewModel authorVm)
        {
            author.FirstName = authorVm.FirstName;
            author.LastName = authorVm.LastName;
            author.Affiliation = authorVm.Affiliation;
            author.Bio = authorVm.Bio;
            author.BirthDate = authorVm.BirthDate;
            author.Email = authorVm.Email;
            author.AuthorId = (authorVm.AuthorId == null || authorVm.AuthorId == Guid.Empty)
                ? Guid.NewGuid() : authorVm.AuthorId;
            
        }
    }
}