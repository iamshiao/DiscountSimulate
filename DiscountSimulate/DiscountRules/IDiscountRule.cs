using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountSimulate
{
    public interface IDiscountRule
    {
        List<Product> Scope { get; set; }

        List<Discount> GetDiscounts(List<Product> basket);
    }
}
