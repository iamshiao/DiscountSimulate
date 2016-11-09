using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountSimulate
{
    public class BuyTwoGetOneFree : IDiscountRule
    {
        public BuyTwoGetOneFree(List<Product> products)
        {
            Scope.AddRange(products);
        }

        public List<Product> Scope { get; set; } = new List<Product>();

        public List<Discount> GetDiscounts(List<Product> basket)
        {
            List<Discount> validDiscountColl = new List<Discount>();
            foreach (var product in Scope) {
                var validProductColl = basket.Where(item => item.Name == product.Name);
                if (validProductColl.Count() >= 3) {
                    Discount d = new Discount()
                    {
                        Name = $"{this.GetType().Name}-{product.Name}",
                        Amount = validProductColl.First().Price,
                        Combination = new List<Product> { product, product, product },
                    };
                    validDiscountColl.Add(d);
                }
            }
            return validDiscountColl;
        }
    }
}
