using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountSimulate
{
    public class SingleProductFixedAmountDiscount : IDiscountRule
    {
        public double FixedAmountDiscount { get; set; }

        public SingleProductFixedAmountDiscount(double fixedAmount, List<Product> products)
        {
            FixedAmountDiscount = fixedAmount;
            Scope.AddRange(products);
        }

        public List<Product> Scope { get; set; } = new List<Product>();

        public List<Discount> GetDiscounts(List<Product> basket)
        {
            List<Discount> validDiscountColl = new List<Discount>();
            foreach (var product in Scope) {
                var validProductColl = basket.Where(item => item.Name == product.Name);
                if (validProductColl.Any()) {
                    Discount d = new Discount()
                    {
                        Name = $"{this.GetType().Name}-{product.Name}",
                        Amount = FixedAmountDiscount,
                        Combination = new List<Product> { product },
                    };
                    validDiscountColl.Add(d);
                }
            }
            return validDiscountColl;
        }
    }
}
