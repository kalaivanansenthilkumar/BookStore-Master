using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
