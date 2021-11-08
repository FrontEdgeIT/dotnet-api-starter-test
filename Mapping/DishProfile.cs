using System.Collections.Generic;
using AutoMapper;
using dotnet_api_test.Models.Dtos;

namespace dotnet_api_test.Mapping
{
    public class DishProfile : Profile
    {
        public DishProfile()
        {
            CreateMap<Dish, ReadDishDto>();
            CreateMap<CreateDishDto, Dish>();
            CreateMap<ReadDishDto, Dish>();
            CreateMap<UpdateDishDto, Dish>();
            CreateMap<List<Dish>, List<ReadDishDto>>();
        }
    }
}