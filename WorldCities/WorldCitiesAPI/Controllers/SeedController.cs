using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Security;
using WorldCitiesAPI.Data.Models;
using WorldCitiesAPI.Models;

namespace WorldCitiesAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SeedController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            if (!_env.IsDevelopment())
                throw new SecurityException("Not allow");

            FileInfo file = new FileInfo(@"C:\Users\ASUS\source\repos\WorldCities\WorldCitiesAPI\Data\Source\worldcities.xlsx");

            using var package = new ExcelPackage(file);
            using var countriesSheet = package.Workbook.Worksheets["Countries"];

            var countries = countriesSheet.ConvertSheetToObjects<CountryDTO>().Select(i => new Country
            {
                Name = i.country,
                ISO2 = i.iso2,
                ISO3 = i.iso3
            }).ToList();
            _context.Countries.AddRange(countries);
            await _context.SaveChangesAsync();

            using var citiesSheet = package.Workbook.Worksheets["Cities"];
            var cities = citiesSheet
                .ConvertSheetToObjects<CityDTO>()
                .Join(
                    countries,
                    _city => _city.country,
                    _country => _country.Name,
                    (_city, _country) => new City()
                    {
                        Name = _city.city_ascii,
                        Lat = _city.lat,
                        Lon = _city.lng,
                        CountryId = _country.Id
                    }
                )
                .ToList();
            _context.Cities.AddRange(cities);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                TotalCountry=countries.Count(),
                TotalCity=cities.Count()
            });
        }

    }
}
