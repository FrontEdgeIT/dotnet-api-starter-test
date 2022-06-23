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
            // Repository vs UoW
            _context.SaveChanges();
        }

        public IEnumerable<Dish> GetAllDishes()
        {
            IEnumerable<Dish> dishes = _context.Dishes;

            return dishes;
        }

        public dynamic? GetAverageDishPrice()
        {
            double averageDishPrice = _context.Dishes.Average(dish => dish.Cost);

            return averageDishPrice;
        }

        public Dish GetDishById(int Id)
        {
            Dish? targetDish = _context.Dishes.FirstOrDefault(dish => dish.Id == Id);

            return targetDish;
        }

        public void DeleteDishById(int Id)
        {
            Dish dishToRemove = this.GetDishById(Id);
            _context.Dishes.Remove(dishToRemove);
            _context.SaveChanges();
        }

        public Dish CreateDish(Dish dish)
        {
            _context.Add(dish);
            _context.SaveChanges();

            return dish;
        }

        public Dish UpdateDish(Dish dish)
        {
            _context.Update(dish);
            _context.SaveChanges();

            return dish;
        }
    }
}