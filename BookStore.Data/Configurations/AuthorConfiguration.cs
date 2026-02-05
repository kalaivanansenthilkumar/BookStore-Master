using BookStore.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Configurations
{
    public class AuthorConfiguration : EntityBaseConfiguration<Author>
    {
        public AuthorConfiguration()
        {
            Property(m => m.FirstName).IsRequired().HasMaxLength(100);
            Property(m => m.LastName).IsRequired();
            Property(m => m.Email).IsRequired().HasMaxLength(100);         
            Property(m => m.BirthDate).IsRequired();
            Property(m => m.Bio).IsRequired().HasMaxLength(2000);
            Property(m => m.Affiliation).IsRequired().HasMaxLength(100);
            Property(m => m.Nationality).IsRequired();
        }
    }
}
