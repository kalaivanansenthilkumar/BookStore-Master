using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.API.Models
{
    public class CategoryViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int NumberOfMovies { get; set; }
    }
}