namespace OracleCMS_DealerAPI.Models
{
    /// <summary>
    /// Model Representing a Dealer object in the database
    /// </summary>
    public class Dealer
    {
        public int DealerId { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Model for Dealer Login action parameters
    /// </summary>
    public class DealerDTO
    {
        public string Name { get; set; }
        public string Password { get; set; }

    }
}
