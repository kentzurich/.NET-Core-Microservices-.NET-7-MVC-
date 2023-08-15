using static Mango.Web.Utility.StaticDetails;

namespace Mango.Web.Models
{
	public class RequestDTO
	{
		public APIType APIType { get; set; } = APIType.GET;
		public string Url { get; set; }
		public object Data { get; set; }
		public string AccessToken { get; set; }
		public ContentType ContentType { get; set; } = ContentType.Json;
    }
}
