using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EFGrid.Shared.Models
{
    public class Employee
    {
        [Key]
        public Int32 EmployeeID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string City { get; set; }
    }
    public class Order
    {
        [Key]
        public int? OrderID { get; set; }
       
        public string CustomerID { get; set; }
      
        public int EmployeeID { get; set; }
        public decimal Freight { get; set; }
      
        public DateTime OrderDate { get; set; }
        
        public string ShipCountry { get; set; }

    }
}
