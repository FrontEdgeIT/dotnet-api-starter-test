using System.Linq;
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
            var allDishes = _dishRepository.GetAllDishes().ToList();
            if (allDishes.Count == 0) return NoContent();
            var dtoDishes = allDishes.Select(dish => _mapper.Map<ReadDishDto>(dish)).ToList();
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
                return Ok(_mapper.Map<ReadDishDto>(dish));
            }
            catch (NotFoundRequestExceptionResponse e)
            {
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
                return Ok(_mapper.Map<ReadDishDto>(dish));
            }
            catch (BadRequestExceptionResponse e)
            {
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
                ModelValidation
                    .ValidateUpdateDishDtoCostIsLessThanPercentageCap(currentDish.Cost, newCost, 1.2);
                currentDish.Cost = newCost;
                currentDish.Name = updateDishDto.Name!;
                currentDish.MadeBy = updateDishDto.MadeBy!;
                return Ok(_dishRepository.UpdateDish(currentDish));
            }
            catch (NotFoundRequestExceptionResponse e)
            {
                return NotFound(e.Message);
            }
            catch (BadRequestExceptionResponse e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteDishById(int id)
        {
            return Ok();
        }
    }
}