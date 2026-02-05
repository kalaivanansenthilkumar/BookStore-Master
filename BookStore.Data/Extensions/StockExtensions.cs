using BookStore.Data.Repositories;
using BookStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Extensions
{
    public static class StockExtensions
    {
        public static IEnumerable<Stock> GetAvailableItems(this IEntityBaseRepository<Stock> stocksRepository, int bookId)
        {
            IEnumerable<Stock> _availableItems;

            _availableItems = stocksRepository.GetAll().Where(s => s.BookId == bookId && s.IsAvailable);

            return _availableItems;
        }

        //public static IEnumerable<Stock> GetAllItems(this IEntityBaseRepository<Stock> stocksRepository, int movieId)
        //{
        //    IEnumerable<Stock> _allItems;

        //    _allItems = stocksRepository.GetAll().Where(s => s.MovieId == movieId);

        //    return _allItems;
        //}
    }
}
