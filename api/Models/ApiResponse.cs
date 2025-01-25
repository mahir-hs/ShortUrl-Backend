namespace api.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public dynamic? Result { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }

        public ApiResponse(dynamic? result = default,
                           bool success = true,
                           string message = "Success",
                           string type = "General")
        {
            Result = result;
            Success = success;
            Message = message;
            Type = type;
        }
    }
}
