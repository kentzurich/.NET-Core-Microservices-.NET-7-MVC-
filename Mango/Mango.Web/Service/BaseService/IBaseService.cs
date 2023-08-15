using Mango.Web.Models;

namespace Mango.Web.Service.BaseService
{
	public interface IBaseService
	{
		Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true);
	}
}
