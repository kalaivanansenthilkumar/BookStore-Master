using BookStore.API.Infrastructure.Validators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.API.Models
{
    [Bind(Exclude = "Image")]
    public class BookViewModel : IValidatableObject
    {
        public int ID { get; set; }
        public Guid BookId { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Genre { get; set; }
        public int GenreId { get; set; }
        public DateTime IssueDate { get; set; }
        public byte Rating { get; set; }
        public string TrailURI { get; set; }
        public bool IsAvailable { get; set; }
        public int NumberOfStocks { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new BookViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}