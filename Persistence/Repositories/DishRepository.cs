using System.Collections.Generic;
using System.Linq;
using dotnet_api_test.Exceptions.ExceptionResponses;
using dotnet_api_test.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api_test.Persistence.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly AppDbContext _context;
        private readonly IDishRepository _implementation;

        public DishRepository(AppDbContext context)
        {
            _context = context;
            _implementation = this;
        }

        void IDishRepository.SaveChanges()
        {
            _context.SaveChanges();
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
            var dish = _context.Find<Dish>(Id);
            if (dish is null) 
                throw new NotFoundRequestExceptionResponse($"Unable to find dish with id:{Id}.");
            return dish;
        }

        public void DeleteDishById(int Id)
        {
            Dish dish = GetDishById(Id);
            _context.Remove(dish);
            _implementation.SaveChanges();
        }

        public Dish CreateDish(Dish dish)
        {
            _context.Dishes.Add(dish);
            _implementation.SaveChanges();
            return dish;
        }

        public Dish UpdateDish(Dish dish)
        {
            var updatedDish = _context.Dishes.Update(dish);
            _implementation.SaveChanges();
            return updatedDish.Entity;
        }
    }
}