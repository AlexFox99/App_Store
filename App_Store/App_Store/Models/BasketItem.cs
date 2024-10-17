using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Store.Models
{
    public class BasketItem
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public string Image { get; set; }
    }
}
