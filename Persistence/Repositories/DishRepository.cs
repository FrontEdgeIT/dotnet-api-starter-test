using System.Collections.Generic;
using dotnet_api_test.Persistence.Repositories.Interfaces;

namespace dotnet_api_test.Persistence.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly AppDbContext _context;

        public DishRepository(AppDbContext context)
        {
            _context = context;
        }

        void IDishRepository.SaveChanges()
        {
            _context.SaveChanges();
        }

        public IEnumerable<Dish> GetAllDishes()
        {
            return _context.Dishes.ToList();
        }

        public dynamic? GetAverageDishPrice()
        {
            return _context.Dishes.Average(d => d.Cost);
        }

        public Dish GetDishById(int Id)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteDishById(int Id)
        {
            throw new System.NotImplementedException();
        }

        public Dish CreateDish(Dish dish)
        {
            throw new System.NotImplementedException();
        }

        public Dish UpdateDish(Dish dish)
        {
            throw new System.NotImplementedException();
        }
    }
}