namespace OracleCMS_DealerAPI.Models
{
    /// <summary>
    /// Model returned for error response consistency
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
