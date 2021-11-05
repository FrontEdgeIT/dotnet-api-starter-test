using System.Collections.Generic;
using System.Linq;
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
            throw new System.NotImplementedException();
        }

        public IEnumerable<Dish> GetAllDishes()
        {
            return _context.Dishes;
        }

        public dynamic? GetAverageDishPrice()
        {
            dynamic average = 0.0;
            if (_context.Dishes.Local.Count > 0) average = _context.Dishes.Average( (d) => d.Cost);
            return average;
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