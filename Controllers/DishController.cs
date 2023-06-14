using AutoMapper;
using dotnet_api_test.Exceptions.ExceptionResponses;
using dotnet_api_test.Models.Dtos;
using dotnet_api_test.Persistence.Repositories.Interfaces;
using dotnet_api_test.Validation;
using Microsoft.AspNetCore.Mvc;

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
            _logger.LogInformation($"Controller action 'GetDishesAndAvarageDishPrice' executed on {DateTime.Now.TimeOfDay}");

            return Ok(new DishesAndAveragePriceDto()
            {
                AveragePrice = _dishRepository.GetAverageDishPrice(),
                Dishes = _dishRepository.GetAllDishes().Select(d => _mapper.Map<ReadDishDto>(d))
            });
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<ReadDishDto> GetDishById(int id)
        {
            _logger.LogInformation($"Controller action 'GetDishById' executed on {DateTime.Now.TimeOfDay}");

            var dish = _dishRepository.GetDishById(id);

            if (dish is null)
            {
                _logger.LogInformation($"Dish with ID {id} not found");
                throw new NotFoundRequestExceptionResponse($"Could not find dish with ID {id}");
            }

            _logger.LogInformation($"Retrieved dish with ID {id} successfully");
            return Ok(_mapper.Map<ReadDishDto>(dish));
        }

        [HttpPost]
        [Route("")]
        public ActionResult<ReadDishDto> CreateDish([FromBody] CreateDishDto createDishDto)
        {
            ModelValidation.ValidateCreateDishDto(createDishDto);

            _logger.LogInformation($"Controller action 'CreateDish' executed on {DateTime.Now.TimeOfDay}");

            // Throws exception if dish with the same name already exists.
            if (_dishRepository.GetAllDishes().Any(d => d.Name.Equals(createDishDto.Name)))
            {
                _logger.LogWarning($"Failed to create dish: dish with '{createDishDto.Name}' already exists");

                throw new BadRequestExceptionResponse(
                    $"Dish with name '{createDishDto.Name}' already exists");
            }

            var dish = _mapper.Map<Dish>(createDishDto);
            _dishRepository.CreateDish(dish);
            _dishRepository.SaveChanges();

            _logger.LogInformation("Dish created successfully.");

            var readDishDto = _mapper.Map<ReadDishDto>(dish);
            return CreatedAtAction(nameof(GetDishById), new { id = readDishDto.Id }, readDishDto);
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult<ReadDishDto> UpdateDishById(int id, UpdateDishDto updateDishDto)
        {
            ModelValidation.ValidateUpdateDishDto(updateDishDto);

            _logger.LogInformation($"Controller action 'UpdateDishById' executed on {DateTime.Now.TimeOfDay}");

            var dishToUpdate = _dishRepository.GetDishById(id);

            if (dishToUpdate is null)
            {
                _logger.LogInformation($"Dish with ID {id} not found");
                throw new NotFoundRequestExceptionResponse($"Could not find dish with ID {id}");
            }

            if (updateDishDto.Cost >= dishToUpdate.Cost * 1.2)
            {
                _logger.LogInformation("New price exceeds old price by 20%");
                throw new BadRequestExceptionResponse("New price exceeds 20% increase from the old price.");
            }

            dishToUpdate.Name = updateDishDto.Name;
            dishToUpdate.Cost = (double)updateDishDto.Cost;
            dishToUpdate.MadeBy = updateDishDto.MadeBy;

            _dishRepository.UpdateDish(dishToUpdate);
            _dishRepository.SaveChanges();

            _logger.LogInformation($"Dish with ID:{id} updated successfully.");

            return Ok(_mapper.Map<ReadDishDto>(dishToUpdate));
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteDishById(int id)
        {
            _logger.LogInformation($"Controller action 'DeleteDishById' executed on {DateTime.Now.TimeOfDay}");

            if (_dishRepository.GetDishById(id) is null)
            {
                _logger.LogInformation($"Dish with ID {id} not found");
                throw new NotFoundRequestExceptionResponse($"Could not find dish with ID {id}");
            }

            _dishRepository.DeleteDishById(id);
            _dishRepository.SaveChanges();

            _logger.LogInformation($"Dish with ID:{id} deleted successfully");

            return NoContent();
        }
    }
}