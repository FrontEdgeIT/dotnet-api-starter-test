using AutoMapper;
using dotnet_api_test.Models.Dtos;
using dotnet_api_test.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using dotnet_api_test.Validation;
using dotnet_api_test.Exceptions.ExceptionHandlers;
using dotnet_api_test.Exceptions.ExceptionResponses;

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
        [Route("getalldishesandaverageprice")]
        public ActionResult<DishesAndAveragePriceDto> GetDishesAndAverageDishPrice()
        {
            try
            {
                var allDishesModel = _dishRepository.GetAllDishes();
                var allDishesDto = _mapper.Map<IEnumerable<ReadDishDto>>(allDishesModel);

                var averagePrice = _dishRepository.GetAverageDishPrice();
                var dishesAndAveragePriceDto = _mapper.Map<DishesAndAveragePriceDto>(new DishesAndAveragePriceDto() { Dishes = allDishesDto, AveragePrice = averagePrice });

                _logger.LogInformation($"[Status Code: {StatusCodes.Status200OK}] All dishes and their average price were fetched succesfully.");
                return Ok(dishesAndAveragePriceDto);
            }
            catch (HttpExceptionResponse ex)
            {
                return CatchHttpExceptionResponse(ex);
            }
        }

        [HttpGet]
        [Route("getdish{id}")]
        public ActionResult<ReadDishDto> GetDishById(int id)
        {
            try
            {
                var dishModel = _dishRepository.GetDishById(id);
                var dishDto = _mapper.Map<ReadDishDto>(dishModel);
                _logger.LogInformation($"[Status Code: {StatusCodes.Status200OK}] Dish with Id: {id} was fetched succesfully.");
                return Ok(dishDto);
            }
            catch (HttpExceptionResponse ex)
            {
                return CatchHttpExceptionResponse(ex);
            }
        }

        [HttpPost]
        [Route("createdish")]
        public ActionResult<ReadDishDto> CreateDish([FromBody] CreateDishDto createDishDto)
        {
            try
            {
                ModelValidation.ValidateCreateDishDto(createDishDto);
                var dishModel = _mapper.Map<Dish>(createDishDto);
                dishModel = _dishRepository.CreateDish(dishModel);
                var dishDto = _mapper.Map<ReadDishDto>(dishModel);
                _logger.LogInformation($"[Status Code: {StatusCodes.Status200OK}] Dish was created succesfully.");
                return Ok(dishDto);
            }
            catch (HttpExceptionResponse ex)
            {
                return CatchHttpExceptionResponse(ex);
            }
        }

        [HttpPut]
        [Route("updatedish{id}")]
        public ActionResult<ReadDishDto> UpdateDishById(int id, UpdateDishDto updateDishDto)
        {
            try
            {
                ModelValidation.ValidateUpdateDishDto(updateDishDto);

                var dishModel = _dishRepository.GetDishById(id);

                dishModel.Name = updateDishDto.Name;
                dishModel.MadeBy = updateDishDto.MadeBy;
                dishModel.Cost = (double)updateDishDto.Cost;

                dishModel = _dishRepository.UpdateDish(dishModel);

                var dishDto = _mapper.Map<ReadDishDto>(dishModel);
                _logger.LogInformation($"[Status Code: {StatusCodes.Status200OK}] Dish was updated succesfully.");
                return Ok(dishDto);
            }
            catch (HttpExceptionResponse ex)
            {
                return CatchHttpExceptionResponse(ex);
            }
        }

        [HttpDelete]
        [Route("deletedish{id}")]
        public ActionResult DeleteDishById(int id)
        {
            try
            {
                _dishRepository.DeleteDishById(id);
                _logger.LogInformation($"[Status Code: {StatusCodes.Status200OK}] Dish with Id: {id} was deleted succesfully.");
                return Ok();
            }
            catch (HttpExceptionResponse ex)
            {
                return CatchHttpExceptionResponse(ex);
            }
        }


        // Normally I would not have made below method in the controller but instead part of a seperate service.
        // The method's purpose is to reduce repetetive copy paste code in each of the controllers methods.
        ObjectResult CatchHttpExceptionResponse(HttpExceptionResponse ex)
        {
            var objectResult = HttpResponseHandler.Respond(ex).CreateObjectResult();
            _logger.LogInformation($"[Status Code: {objectResult.StatusCode}] {objectResult.Value}");
            return objectResult;
        }
    }
}