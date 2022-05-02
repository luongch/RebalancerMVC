using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RebalancerMVC.Models
{
    public class EtfViewModel
    {
        public List<Etf> Etfs { get; set; }
        public int Cash { get; set; }
    }
}
