using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Web.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}