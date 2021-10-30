using dotnet_api_test.Exceptions;
using dotnet_api_test.Exceptions.ExceptionHandlers;
using dotnet_api_test.Exceptions.ExceptionResponses;
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

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public IEnumerable<Dish> GetAllDishes()
        {
            IEnumerable<Dish> dishes = _context.Dishes;

            if(dishes.ToArray().Length == 0)
                throw new BadRequestExceptionResponse($"No dishes were found in database.");

            return dishes;
        }

        public dynamic? GetAverageDishPrice()
        {
            var dishes = GetAllDishes();
            return dishes.Select(dish => dish.Cost).Average();
        }

        public Dish GetDishById(int Id)
        {
            var dish = _context.Find<Dish>(Id);

            if (dish == null)
                throw new BadRequestExceptionResponse($"No dish with id: {Id} was found in database.");

            return dish;
        }

        public void DeleteDishById(int Id)
        {
            var dish = GetDishById(Id);

            if (dish == null)
                throw new BadRequestExceptionResponse($"No dish with id: {Id} was found in database.");

            _context.Remove(dish);
            SaveChanges();
        }

        public Dish CreateDish(Dish dish)
        {
            _context.Add(dish);
            SaveChanges();

            return dish;
        }

        public Dish UpdateDish(Dish dish)
        {
            _context.Update(dish);
            SaveChanges();

            return dish;
        }
    }
}