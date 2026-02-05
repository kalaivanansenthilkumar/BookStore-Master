using BookStore.Web.Configuration;
using BookStore.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BookStore.Web.Models
{
    public class BookStoreDBContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        //public BookStoreContext() : base("name=BookStoreContext")
        //{
        //    this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        //}
        
        public BookStoreDBContext(string connectionString): base(connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            Database.SetInitializer<BookStoreDBContext>(null);
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }
        public System.Data.Entity.DbSet<BookStore.Web.Models.Author> Authors { get; set; }

        public System.Data.Entity.DbSet<BookStore.Web.Models.Book> Books { get; set; }

        public System.Data.Entity.DbSet<BookStore.Web.Models.Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<BookStore.Web.Models.Country> Countries { get; set; }
       
    }
}
