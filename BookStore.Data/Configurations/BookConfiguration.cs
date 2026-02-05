using BookStore.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Configurations
{
    public class BookConfiguration : EntityBaseConfiguration<Book>
    {
        public BookConfiguration()
        {
            Property(m => m.Title).IsRequired().HasMaxLength(100);
            Property(m => m.AuthorId).IsRequired();    
            Property(m => m.Rating).IsRequired();
            Property(m => m.Description).IsRequired().HasMaxLength(2000);
            Property(m => m.TrailURI).HasMaxLength(200);
            HasMany(m => m.Stocks).WithRequired().HasForeignKey(s => s.BookId);
        }
    }
}
