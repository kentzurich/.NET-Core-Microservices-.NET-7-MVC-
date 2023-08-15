using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
				config.CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();
			});

			return mappingConfig;
		}
	}
}
