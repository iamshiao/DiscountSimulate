using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircleHsiao.idv.Extensions;

namespace DiscountSimulate
{
    public class MultiProductFixedAmountDiscount : IDiscountRule
    {
        public double FixedAmountDiscount { get; set; }

        public MultiProductFixedAmountDiscount(List<Product> products, double fixedAmount)
        {
            FixedAmountDiscount = fixedAmount;
            Scope.AddRange(products);
        }

        public List<Product> Scope { get; set; } = new List<Product>();

        public List<Discount> GetDiscounts(List<Product> basket)
        {
            List<Discount> validDiscountColl = new List<Discount>();
            if (basket.ContainAnotherWholeIEumable(Scope)) {
                Discount d = new Discount()
                {
                    Name = $"{this.GetType().Name}",
                    Amount = 0,
                    Combination = new List<Product>()
                };
                foreach (var product in Scope) {
                    var validProductColl = basket.Where(item => item.Name == product.Name);
                    d.Name = d.Name + $"-{ product.Name}";
                    //d.Amount += product.Price;
                    d.Combination.Add(product);
                }
                d.Amount = FixedAmountDiscount;

                validDiscountColl.Add(d);
            }
            return validDiscountColl;
        }
    }
}
