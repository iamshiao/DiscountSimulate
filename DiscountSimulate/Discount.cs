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

        public double HowStarving { get { return GetStarvingVal(); } }

        private double GetStarvingVal()
        {
            double starvingVal = 1;
            foreach (var p in Combination) {
                starvingVal *= p.StarveRate;
            }
            return Amount * starvingVal;
        }

        public object Clone()
        {
            Discount discount = (Discount)this.MemberwiseClone();

            return discount;
        }
    }
}
