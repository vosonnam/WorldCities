using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;
using WorldCitiesAPI.Models;
using System.Linq.Dynamic.Core;

namespace WorldCitiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountry()
        {
            if (_context.Countries == null)
            {
                return NotFound();
            }
            return await _context.Countries.ToListAsync();
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            if (_context.Countries == null)
            {
                return NotFound();
            }
            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            if (id != country.Id)
            {
                return BadRequest();
            }

            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country country)
        {
            if (_context.Countries == null)
            {
                return Problem("Entity set 'WorldCitiesAPIContext.Country'  is null.");
            }
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (_context.Countries == null)
            {
                return NotFound();
            }
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return (_context.Countries?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet("[action]/{pageIndex:int?}/{pageSize:int?}/{sortColumn?}/{sortOrder?}/{filterColumn?}/{filterQuery?}")]
        public async Task<ActionResult<ApiResult<Country>>>Page(
            int pageIndex=0, 
            int pageSize=10,
            string? sortColumn=null,
            string? sortOrder=null,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            return await ApiResult<Country>.CreateAsync(
                    _context.Countries.AsNoTracking(),
                    pageIndex,
                    pageSize,
                    sortColumn,
                    sortOrder,
                    filterColumn,
                    filterQuery
                );
        }

        

        [HttpPost("[action]")]
        public bool isDupeField(int countryId, string fieldName, string fieldValue)
        {
            //if (fieldName.Equals("name"))
            //    return (_context.Countries?.Any(e => e.Name.Equals(fieldValue) && e.Id == countryId)).GetValueOrDefault();
            //if (fieldName.Equals("ios2"))
            //    return (_context.Countries?.Any(e => e.ISO2.Equals(fieldValue) && e.Id == countryId)).GetValueOrDefault();
            //if (fieldName.Equals("iso3"))
            //    return (_context.Countries?.Any(e => e.ISO3.Equals(fieldValue) && e.Id == countryId)).GetValueOrDefault();
            //return false;
            return (ApiResult<Country>.isValidProperty(fieldName,false))?
                (_context.Countries?.Any(string.Format("{0}==@0 && Id!=@1"),fieldName, fieldValue)).GetValueOrDefault()
                :false;
        }
    }
}
