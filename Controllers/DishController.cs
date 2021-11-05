using System.Linq;
using AutoMapper;
using dotnet_api_test.Exceptions.ExceptionHandlers;
using dotnet_api_test.Exceptions.ExceptionResponses;
using dotnet_api_test.Models.Dtos;
using dotnet_api_test.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnet_api_test.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly ILogger<DishController> _logger;
        private readonly IMapper _mapper;
        private readonly IDishRepository _dishRepository;

        public DishController(ILogger<DishController> logger, IMapper mapper, IDishRepository dishRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _dishRepository = dishRepository;
        }

        [HttpGet]
        [Route("")]
        public ActionResult<DishesAndAveragePriceDto> GetDishesAndAverageDishPrice()
        {
            var allDishes = _dishRepository.GetAllDishes();
            var dtoDishes = allDishes.Select(dish =>
                new ReadDishDto {Cost = dish.Cost, Id = dish.Id, MadeBy = dish.MadeBy, Name = dish.Name}).ToList();
            var result = new DishesAndAveragePriceDto
            {
                Dishes = dtoDishes,
                AveragePrice = _dishRepository.GetAverageDishPrice()
            };
            return result;
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<ReadDishDto> GetDishById(int id)
        {
            Dish dish = _dishRepository.GetDishById(id);
            var response =
                HttpResponseHandler.Respond(
                    new NotFoundRequestExceptionResponse($"The dish with id: {id} was not found.")
                    );
            if (dish == null) return response.CreateObjectResult();
            return new ReadDishDto {Id = dish.Id, Cost = dish.Cost, Name = dish.Name, MadeBy = dish.MadeBy};
        }

        [HttpPost]
        [Route("")]
        public ActionResult<ReadDishDto> CreateDish([FromBody] CreateDishDto createDishDto)
        {
            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult<ReadDishDto> UpdateDishById(int id, UpdateDishDto updateDishDto)
        {
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteDishById(int id)
        {
            return Ok();
        }
    }
}