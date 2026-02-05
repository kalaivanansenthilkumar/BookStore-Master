using BookStore.API.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.API.Models
{
    public class AuthorViewModel 
    {
        public int ID { get; set; }
        public Guid AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public string Affiliation { get; set; }
    }
}