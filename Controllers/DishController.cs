using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using dotnet_api_test.Exceptions.ExceptionResponses;
using dotnet_api_test.Models.Dtos;
using dotnet_api_test.Persistence.Repositories.Interfaces;
using dotnet_api_test.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static System.DateTime;

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
            _logger.LogInformation("{time} Returning all dishes.", Now);
            var dtoDishes = _mapper.Map<List<ReadDishDto>>(_dishRepository.GetAllDishes().ToList());
            return Ok(
                new DishesAndAveragePriceDto {Dishes = dtoDishes, AveragePrice = _dishRepository.GetAverageDishPrice()}
            );
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<ReadDishDto> GetDishById(int id)
        {
            try
            {
                Dish dish = _dishRepository.GetDishById(id);
                _logger.LogInformation("{time} Returning a dish with id:{id}.", Now, id);
                return Ok(_mapper.Map<ReadDishDto>(dish));
            }
            catch (NotFoundRequestExceptionResponse e)
            {
                _logger.LogInformation("{time} Could not find a dish with id:{id}.", Now, id);
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        [Route("")]
        public ActionResult<ReadDishDto> CreateDish([FromBody] CreateDishDto createDishDto)
        {
            try
            {
                ModelValidation.ValidateCreateDishDto(createDishDto);
                ModelValidation.ValidateDishNameIsUnique(createDishDto.Name!, _dishRepository.GetAllDishes());
                var dish = _dishRepository.CreateDish(_mapper.Map<Dish>(createDishDto));
                _logger.LogInformation("{time} Dish {Name} created with id:{Id}", Now, dish.Name, dish.Id);
                return Ok(_mapper.Map<ReadDishDto>(dish));
            }
            catch (BadRequestExceptionResponse e)
            {
                _logger.LogInformation("{time} Failed to create a new Dish: {msg}", Now, e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult<ReadDishDto> UpdateDishById(int id, UpdateDishDto updateDishDto)
        {
            try
            {
                var currentDish = _dishRepository.GetDishById(id);
                ModelValidation.ValidateUpdateDishDto(updateDishDto);
                var newCost = (double) updateDishDto.Cost!;
                ModelValidation.IsDishCostLessThanPercentageMax(currentDish.Cost, newCost, 1.2);
                _mapper.Map(updateDishDto, currentDish);
                _logger.LogInformation("{time} Updated Dish with id: {id}", Now, id);
                return Ok(_dishRepository.UpdateDish(currentDish));
            }
            catch (NotFoundRequestExceptionResponse e)
            {
                _logger.LogInformation("{time} Failed to update a dish: {msg}", Now, e.Message);
                return NotFound(e.Message);
            }
            catch (BadRequestExceptionResponse e)
            {
                _logger.LogInformation("{time} Failed to update a dish with id:{id}: {msg}", Now, id,
                    e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteDishById(int id)
        {
            try
            {
                _dishRepository.DeleteDishById(id);
                _logger.LogInformation("{time} Deleted Dish with id:{id}.", Now, id);
                return Ok();
            }
            catch (NotFoundRequestExceptionResponse e)
            {
                _logger.LogInformation("{time} Failed to Delete a dish: {msg}", Now, e.Message);
                return NotFound(e.Message);
            }
        }
    }
}