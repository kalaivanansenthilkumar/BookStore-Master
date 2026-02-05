using FluentValidation;
using BookStore.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.API.Infrastructure.Validators
{
    public class BookViewModelValidator : AbstractValidator<BookViewModel>
    {
        public BookViewModelValidator()
        {
            RuleFor(movie => movie.GenreId).GreaterThan(0)
                .WithMessage("Select a Genre");

            RuleFor(movie => movie.Description).NotEmpty()
                .WithMessage("Select a description");

            RuleFor(movie => movie.Rating).InclusiveBetween((byte)0, (byte)5)
                .WithMessage("Rating must be less than or equal to 5");

            RuleFor(movie => movie.TrailURI).NotEmpty().Must(ValidTrailerURI)
                .WithMessage("Only Youtube Trailers are supported");
        }

        private bool ValidTrailerURI(string trailerURI)
        {
            return (!string.IsNullOrEmpty(trailerURI) && trailerURI.ToLower().StartsWith("https://www.youtube.com/watch?"));
        }
    }
}