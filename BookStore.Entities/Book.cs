using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Entities
{
    public class Book:IEntityBase
    {
        public Book()
        {
            Stocks = new List<Stock>();
        }
        public int ID { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public virtual Genre Genre { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int GenreId { get; set; }
        public virtual Author Author { get; set; }
        public DateTime IssueDate { get; set; }
        public byte Rating { get; set; }
        public string TrailURI { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
