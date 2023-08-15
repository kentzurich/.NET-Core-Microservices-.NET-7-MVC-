using Mango.Web.Models;
using Mango.Web.Service.TokenProvider;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace Mango.Web.Service.BaseService
{
	public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
			_httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true)
		{
			try
			{
				HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
				HttpRequestMessage message = new();

				if(requestDTO.ContentType == StaticDetails.ContentType.MultipartFormData)
                    message.Headers.Add("Accept", "*/*");
				else
                    message.Headers.Add("Accept", "application/json");

                //token
                if (withBearer)
				{
					var token = _tokenProvider.GetToken();
					message.Headers.Add("Authorization", $"Bearer {token}");
				}

				message.RequestUri = new Uri(requestDTO.Url);

				if(requestDTO.ContentType == StaticDetails.ContentType.MultipartFormData)
				{
					var content = new  MultipartFormDataContent();
					
					foreach(var prop in requestDTO.Data.GetType().GetProperties())
					{
						var value = prop.GetValue(requestDTO.Data);
						if(value is FormFile)
						{
							var file = (FormFile)value;

							if(file != null)
								content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
						}
						else
						{
                            content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                        }
					}

					message.Content = content;
				}
				else
				{
                    if (requestDTO.Data != null)
                        message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
                }

				HttpResponseMessage? apiResponse = null;

				switch (requestDTO.APIType)
				{
					case StaticDetails.APIType.POST:
						message.Method = HttpMethod.Post;
						break;
					case StaticDetails.APIType.DELETE:
						message.Method = HttpMethod.Delete;
						break;
					case StaticDetails.APIType.PUT:
						message.Method = HttpMethod.Put;
						break;
					default:
						message.Method = HttpMethod.Get;
						break;
				}

				apiResponse = await client.SendAsync(message);

				switch (apiResponse.StatusCode)
				{
					case HttpStatusCode.NotFound:
						return new() { IsSuccess = false, Message = "Not Found" };
					case HttpStatusCode.Forbidden:
						return new() { IsSuccess = false, Message = "Access Denied" };
					case HttpStatusCode.Unauthorized:
						return new() { IsSuccess = false, Message = "Unauthorized" };
					case HttpStatusCode.InternalServerError:
						return new() { IsSuccess = false, Message = "Internal Server Error" };
                    case HttpStatusCode.BadRequest:
                        return new() { IsSuccess = false, Message = "Bad Request" };
                    default:
						var apiContent = await apiResponse.Content.ReadAsStringAsync();
						var apiResponseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
						return apiResponseDTO;
				}
			}
			catch(Exception ex)
			{
				var dto = new ResponseDTO()
				{
					IsSuccess = false,
					Message = ex.Message,
				};

				return dto;
			}
		}
	}
}
