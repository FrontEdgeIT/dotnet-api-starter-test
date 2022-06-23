using AutoMapper;
using dotnet_api_test.Exceptions.ExceptionResponses;
using dotnet_api_test.Models.Dtos;
using dotnet_api_test.Persistence.Repositories.Interfaces;
using dotnet_api_test.Validation;
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

        private readonly string NotFoundMessage = "Dish does not exist!";

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
            var dishes = _dishRepository.GetAllDishes();
            if (dishes.Count() == 0) {
                _logger.LogInformation("No dishes where found in the database O_o");
                return NotFound(new NotFoundRequestExceptionResponse("There are no stored dishes available!", 404).Message);
            }

            var dishesDto = _mapper.Map<List<ReadDishDto>>(dishes);
            var averageDishPrice = _dishRepository.GetAverageDishPrice();
            var daapDto = new DishesAndAveragePriceDto { Dishes = dishesDto, AveragePrice = averageDishPrice };

            return Ok(daapDto);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<ReadDishDto> GetDishById(int id)
        {
            var dish = _dishRepository.GetDishById(id);
            if (dish is null) 
                return NotFound(new NotFoundRequestExceptionResponse(NotFoundMessage, 404).Message);

            var readDishDto = _mapper.Map<ReadDishDto>(dish);

            return Ok(readDishDto);
        }

        [HttpPost]
        [Route("")]
        public ActionResult<ReadDishDto> CreateDish([FromBody] CreateDishDto createDishDto)
        {
            ModelValidation.ValidateCreateDishDto(createDishDto);

            var dish = _mapper.Map<Dish>(createDishDto);
            _dishRepository.CreateDish(dish);
            var dishDto = _mapper.Map<ReadDishDto>(dish);

            return CreatedAtAction(nameof(GetDishById), new { id = dishDto.Id }, dishDto);
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult<ReadDishDto> UpdateDishById(int id, UpdateDishDto updateDishDto)
        {
            ModelValidation.ValidateUpdateDishDto(updateDishDto);

            var dishToUpdate = _dishRepository.GetDishById(id);
            if (dishToUpdate is null) 
                return NotFound(new NotFoundRequestExceptionResponse(NotFoundMessage, 404).Message);

            if (updateDishDto.Cost > 1.2 * dishToUpdate.Cost) 
                return BadRequest(new BadRequestExceptionResponse("You can't raise the price more than 20% at a time.", 400).Message);

            _mapper.Map(updateDishDto, dishToUpdate);
            var dish = _dishRepository.UpdateDish(dishToUpdate);
            var dishDto = _mapper.Map<ReadDishDto>(dish);

            return CreatedAtAction(nameof(GetDishById), new {id = dishDto.Id}, dishDto);
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteDishById(int id)
        {
            bool dishExists = _dishRepository.GetDishById(id) != null ? true : false;

            if (dishExists)
            {
                _dishRepository.DeleteDishById(id);
                return NoContent();
            }
            else
            {
                return BadRequest(new BadRequestExceptionResponse(NotFoundMessage, 400).Message);
            }
        }
    }
}