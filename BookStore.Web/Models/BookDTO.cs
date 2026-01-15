
namespace BookStore.Web.Models
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public string AuthorName { get; set; }
    }
}