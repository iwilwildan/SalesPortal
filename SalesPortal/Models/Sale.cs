using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SalesPortal.Models
{
    public class Sale
    {
        
        public int SaleID { get; set; }
        public string CustomerName { get; set; }
        public int CountryID { get; set; }
        public int RegionID { get; set; }
        public int CityID { get; set; }
        public DateTime SaleDateTime { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        // Display properties (for presentation, not stored in the database)
        public string CountryName { get; set; }
        public string RegionName { get; set; }
        public string CityName { get; set; }
        public string ProductName { get; set; }
    }
}
