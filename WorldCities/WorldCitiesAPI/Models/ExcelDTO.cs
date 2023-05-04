namespace WorldCitiesAPI.Models
{
    public class CityDTO
    {
        public string city_ascii { get; set; } = null!;
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public string country { get; set; } = null!;
    }
    public class CountryDTO
    {
        public string country { get; set; } = null!;
        public string iso2 { get; set; } = null!;
        public string iso3 { get; set; } = null!;
    }
}
