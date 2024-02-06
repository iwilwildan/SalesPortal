using Microsoft.AspNetCore.Mvc;
using SalesPortal.Models;
using SalesPortal.Repositories.Contract;
using SalesPortal.Utilities;
using SalesPortal.Utilities.Logger;
using System.Diagnostics;

namespace SalesPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomLogger _logger;
        private readonly ISaleRepository<Sale> _saleRepo;
        private readonly IGenericRepository<Country> _countryRepo;
        private readonly IGenericRepository<Region> _regionRepo;
        private readonly IGenericRepository<City> _cityRepo;
        private readonly IGenericRepository<Product> _productRepo;
        public HomeController(ICustomLogger logger,
            ISaleRepository<Sale> saleRepo,
            IGenericRepository<Country> countryRepo,
            IGenericRepository<Region> regionRepo,
            IGenericRepository<City> cityRepo, 
            IGenericRepository<Product> productRepo)
        {
            _logger = logger;
            _saleRepo = saleRepo;
            _countryRepo = countryRepo;
            _regionRepo = regionRepo;
            _cityRepo = cityRepo;
            _productRepo = productRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalPages(DateTime startDate,
            DateTime endDate,
            string selectedCountry,
            string selectedRegion,
            string selectedCity,
            int pageSize)
        {
            try
            {
                Dictionary<string, object> filters = new Dictionary<string, object>();
                filters.Add("StartDate", startDate <= DateTime.MinValue ? new DateTime(1999, 01, 01) : startDate.ToUniversalTime());
                filters.Add("EndDate", endDate <= DateTime.MinValue ? DateTime.MaxValue : endDate.ToUniversalTime());
                filters.Add("SelectedCountry", selectedCountry);
                filters.Add("SelectedRegion", selectedRegion);
                filters.Add("SelectedCity", selectedCity);
                filters.Add("PageSize", pageSize);
                int totalPages = await _saleRepo.GetTotalPages(filters);
                return Ok(new { TotalPages = totalPages });
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSales(DateTime startDate,
            DateTime endDate,
            string selectedCountry,
            string selectedRegion,
            string selectedCity,
            int pageSize,
            int pageNumber)
        {
            try
            {
                Dictionary<string, object> filters = new Dictionary<string, object>();
                filters.Add("StartDate", startDate <= DateTime.MinValue ? new DateTime(1999, 01, 01) : startDate.ToUniversalTime());
                filters.Add("EndDate", endDate <= DateTime.MinValue ? DateTime.MaxValue : endDate.ToUniversalTime());
                filters.Add("SelectedCountry", selectedCountry);
                filters.Add("SelectedRegion", selectedRegion);
                filters.Add("SelectedCity", selectedCity);
                filters.Add("PageSize", pageSize);
                filters.Add("PageNumber", pageNumber);

                List<Sale> sales = await _saleRepo.GetList(filters);
                if (sales == null)
                {
                    return NotFound();
                }
                else
                {
                    foreach (var item in sales)
                    {
                        item.SaleDateTime = item.SaleDateTime.ConvertToLocal();
                    }
                    return Ok(sales);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from Database");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                List<Product> products = await _productRepo.GetList(new Dictionary<string, object>());
                if (products == null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(products);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from Database");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                List<Country> countries = await _countryRepo.GetList(new Dictionary<string, object>());
                if (countries == null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(countries);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from Database");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetRegions(int countryId)
        {
            try
            {
                Dictionary<string, object> filters = new Dictionary<string, object>();
                filters.Add("CountryID", countryId);
                List<Region> regions = await _regionRepo.GetList(filters);
                if (regions == null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(regions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from Database");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetCities(int regionId)
        {
            try
            {
                Dictionary<string, object> filters = new Dictionary<string, object>();
                filters.Add("RegionID", regionId);
                List<City> cities = await _cityRepo.GetList(filters);
                if (cities == null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(cities);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from Database");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveSale([FromBody]Sale sale)
        {
            try
            {
                sale.SaleDateTime = sale.SaleDateTime.ToUniversalTime();
                bool result = await _saleRepo.Save(sale);
                if (!result)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}