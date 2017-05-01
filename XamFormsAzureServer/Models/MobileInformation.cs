namespace XamFormsAzureServer.Models
{
    public class MobileInformation
    {
        public MobileInformation(string connectionId, string deviceId, string message, string token)
        {
            ConnectionId = connectionId;
            DeviceId = deviceId;
            Message = message;
            Token = token;
        }

		public string ConnectionId { get; set; }

		public string DeviceId { get; set; }

        public string Message { get; set; }

		public string Token { get; set; }
	}
}
