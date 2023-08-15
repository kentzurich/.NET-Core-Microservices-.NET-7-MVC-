using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<OrderHeaderDTO, CartHeaderDTO>()
					.ForMember(from => from.CartTotal, to => to.MapFrom(src => src.OrderTotal))
					.ReverseMap();

				config.CreateMap<CartDetailsDTO, OrderDetailsDTO>()
					.ForMember(from => from.ProductName, to => to.MapFrom(src => src.Product.Name))
					.ForMember(from => from.Price, to => to.MapFrom(src => src.Product.Price));

                config.CreateMap<OrderDetailsDTO, CartDetailsDTO>();

                config.CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
				config.CreateMap<OrderDetails, OrderDetailsDTO>().ReverseMap();
            });

			return mappingConfig;
		}
	}
}
