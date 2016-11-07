using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountSimulate
{
    public class Product : ICloneable
    {
        public string Name { get; set; }
        public double Price { get; set; }

        public object Clone()
        {
            Product product = (Product)this.MemberwiseClone();

            return product;
        }
    }
}
