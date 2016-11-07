using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountSimulate
{
    public class Discount : ICloneable
    {
        public string Name { get; set; }

        public List<Product> Combination { get; set; }

        public double Amount { get; set; }

        public double Distribution { get { return Amount / Combination.Count; } }

        public object Clone()
        {
            Discount discount = (Discount)this.MemberwiseClone();

            return discount;
        }
    }
}
