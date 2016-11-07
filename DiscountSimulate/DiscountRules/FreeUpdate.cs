using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountSimulate
{
    public class FreeUpdate : IDiscountRule
    {
        public Product OriVerProduct { get; set; }

        public Product AdvanceVerProduct { get; set; }

        public FreeUpdate(Product oriVerProduct, Product advanceVerProduct)
        {
            OriVerProduct = oriVerProduct;
            AdvanceVerProduct = advanceVerProduct;
            Scope.Add(advanceVerProduct);
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
                        Amount = product.Price - OriVerProduct.Price,
                        Combination = new List<Product> { product },
                    };
                    validDiscountColl.Add(d);
                }
            }
            return validDiscountColl;
        }
    }
}
