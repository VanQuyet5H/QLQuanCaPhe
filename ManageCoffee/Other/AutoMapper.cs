using AutoMapper;
using ManageCoffee.Models;
using ManageCoffee.ViewModels;

namespace ManageCoffee.Other
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Order, CartItem>().ReverseMap();
            CreateMap<User, UserModel>().ReverseMap();
        }
    }
}
