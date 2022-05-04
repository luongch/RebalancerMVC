using System;
using System.ComponentModel.DataAnnotations;
namespace RebalancerMVC.Models
{
    public class Etf
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Percentage { get; set; }
    }
}
