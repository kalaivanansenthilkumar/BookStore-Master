using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        BookStoreDBContext dbContext;
        public BookStoreDBContext Init()
        {
            return dbContext ?? (dbContext = new BookStoreDBContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
