using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountSimulate
{
    public class SingleProductPercentDiscount : IDiscountRule
    {
        public double Percent { get; set; }

        public SingleProductPercentDiscount(double percent, List<Product> products)
        {
            Percent = percent;
            Scope.AddRange(products);
        }

        public List<Product> Scope { get; set; } = new List<Product>();

        public List<Discount> GetDiscountColl(List<Product> basket)
        {
            List<Discount> validDiscountColl = new List<Discount>();
            foreach (var product in Scope) {
                var validProductColl = basket.Where(item => item.Name == product.Name);
                if (validProductColl.Any()) {
                    Discount d = new Discount()
                    {
                        Name = $"{this.GetType().Name}-{product.Name}",
                        Amount = product.Price - (product.Price * Percent),
                        Combination = new List<Product> { product },
                    };
                    validDiscountColl.Add(d);
                }
            }
            return validDiscountColl;
        }
    }
}
