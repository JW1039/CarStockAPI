    namespace OracleCMS_DealerAPI.Models
    {
    /// <summary>
    /// Model Representing a Car object in the database
    /// </summary>
    public class Car
        {
            public int Id { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public int Year { get; set; }
            public string NumberPlate { get; set; }
            public int DealerId { get; set; }
        }


    public class CarDTO
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string NumberPlate { get; set; }
    }
}


