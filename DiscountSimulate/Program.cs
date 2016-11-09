using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircleHsiao.idv.Extensions;

namespace DiscountSimulate
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 商品
            Product pA = new Product { Name = "A", Price = 100 };
            Product pB = new Product { Name = "B", Price = 100 };
            Product pC = new Product { Name = "C", Price = 100 };
            Product pD = new Product { Name = "D", Price = 100 };
            Product pE = new Product { Name = "E", Price = 100 };
            Product pF = new Product { Name = "F", Price = 100 };
            Product pG = new Product { Name = "G", Price = 60 };
            Product pH = new Product { Name = "H", Price = 100 };
            Product pI = new Product { Name = "I", Price = 100 };
            Product pJ = new Product { Name = "J", Price = 100 };
            Product pK = new Product { Name = "K", Price = 100 };
            Product pAA = new Product { Name = "AA", Price = 120 };
            Product pBB = new Product { Name = "BB", Price = 150 };
            Product pCC = new Product { Name = "CC", Price = 200 };

            List<Product> basket = new List<Product> { pA, pB, pC, pC, pC };
            //basket = AddNumsOfProductToBasket(basket, pA, 5);
            //basket = AddNumsOfProductToBasket(basket, pB, 6);
            //basket = AddNumsOfProductToBasket(basket, pC, 7);
            basket = AddNumsOfProductToBasket(basket, pD, 1);
            basket = AddNumsOfProductToBasket(basket, pE, 2);
            basket = AddNumsOfProductToBasket(basket, pF, 3);
            basket = AddNumsOfProductToBasket(basket, pG, 1);
            //basket = AddNumsOfProductToBasket(basket, pH, 2);
            //basket = AddNumsOfProductToBasket(basket, pI, 3);
            //basket = AddNumsOfProductToBasket(basket, pJ, 1);
            //basket = AddNumsOfProductToBasket(basket, pK, 2);
            //basket = AddNumsOfProductToBasket(basket, pAA, 55);
            //basket = AddNumsOfProductToBasket(basket, pBB, 78);
            //basket = AddNumsOfProductToBasket(basket, pCC, 87);
            #endregion

            List<IDiscountRule> discountRules = new List<IDiscountRule>();
            discountRules.Add(new SingleProductSpecialPrice(95, new List<Product> { pA, pC, pE, pI, pK }));
            discountRules.Add(new SingleProductFixedAmountDiscount(6, new List<Product> { pB, pD, pF, pG, pH, pJ }));
            discountRules.Add(new SingleProductPercentDiscount(0.96, new List<Product> { pI, pJ, pK }));
            //discountRules.Add(new BuyOneGetOneFree(new List<Product> { pG }));
            discountRules.Add(new BuyTwoGetOneFree(new List<Product> { pC }));
            //discountRules.Add(new FreeUpdate(pA, pAA));
            //discountRules.Add(new FreeUpdate(pB, pBB));
            //discountRules.Add(new FreeUpdate(pC, pCC));
            ////discountRules.Add(new BuyAGetBFree(pI, pJ));
            //discountRules.Add(new MultiProductPercentDiscount(new List<Product> { pA, pB }, 0.75));
            //discountRules.Add(new MultiProductFixedPrice(new List<Product> { pA, pA, pB }, 200));
            //discountRules.Add(new MultiProductFixedAmountDiscount(new List<Product> { pA, pB }, 5));
            //discountRules.Add(new MultiProductFixedAmountDiscount(new List<Product> { pA, pB, pC }, 120));
            //discountRules.Add(new MultiProductFixedPrice(new List<Product> { pA, pB, pD }, 250));
            //discountRules.Add(new MultiProductPercentDiscount(new List<Product> { pB, pC, pD }, 0.65));

            //discountRules.Add(new MultiProductPercentDiscount(new List<Product> { pC, pD }, 0.7));
            //discountRules.Add(new MultiProductFixedPrice(new List<Product> { pI, pJ, pH }, 200));
            //discountRules.Add(new MultiProductFixedAmountDiscount(new List<Product> { pG, pAA }, 105));
            //discountRules.Add(new MultiProductFixedPrice(new List<Product> { pE, pH }, 120));
            //discountRules.Add(new MultiProductPercentDiscount(new List<Product> { pF, pH, pK }, 0.95));

            discountRules.Add(new MultiProductPercentDiscount(new List<Product> { pC, pD, pE }, 0.75));
            discountRules.Add(new MultiProductFixedAmountDiscount(new List<Product> { pA, pB, pC }, 120));
            discountRules.Add(new MultiProductFixedAmountDiscount(new List<Product> { pF, pG }, 160));

            List<Discount> vaildDiscounts = new List<Discount>();
            foreach (var rule in discountRules) {
                vaildDiscounts.AddRange(rule.GetDiscounts(basket));
            }

            GetBestDiscountPath(basket, vaildDiscounts);
        }

        private static List<Product> AddNumsOfProductToBasket(List<Product> basket, Product p, int num)
        {
            for (int i = 0; i < num; i++) {
                basket.Add(p);
            }

            return basket;
        }

        /// <summary>移除折扣會耗用的產品</summary>
        /// <returns></returns>
        private static List<Product> RemoveUsedProducts(List<Product> basket, Discount usedDiscount)
        {
            basket = new List<Product>(basket);

            foreach (var usedProduct in usedDiscount.Combination) {
                var markedProduct = basket.FirstOrDefault(p => p.Name == usedProduct.Name);
                if (markedProduct != null) {
                    basket.Remove(markedProduct);
                }
            }
            return basket;
        }

        /// <summary>篩選商品組合可成立的折扣組合</summary>
        /// <param name="availableProducts"></param>
        /// <param name="availableDiscounts"></param>
        /// <returns></returns>
        private static List<Discount> ValidateDiscounts(List<Product> availableProducts, List<Discount> availableDiscounts)
        {
            // 商品剩餘數
            var productSupplies = availableProducts.GroupBy(p => p.Name)
                    .Select(group => new
                    {
                        Name = group.Key,
                        Count = group.Count()
                    });
            List<Discount> toRemove = new List<Discount>(); // 待移除清單
            foreach (var d in availableDiscounts) {
                // 折扣所需商品(組合)數
                var productDemands = d.Combination.GroupBy(p => p.Name)
                    .Select(group => new
                    {
                        Name = group.Key,
                        Count = group.Count()
                    });

                foreach (var qd in productDemands) {
                    var remainNumOfProduct = productSupplies.FirstOrDefault(qs => qs.Name == qd.Name);
                    // 若折扣所需商品不存在剩餘商品組合或數量不足以成立折扣則移除該折扣
                    if (remainNumOfProduct == null || remainNumOfProduct.Count < qd.Count) {
                        toRemove.Add(d);
                        break;
                    }
                }
            }
            // 移除
            availableDiscounts = availableDiscounts.RemoveWhere(d => d.In(toRemove.ToArray())).ToList();

            return availableDiscounts;
        }

        /// <summary>全展開遞迴</summary>
        /// <param name="availableProducts"></param>
        /// <param name="availableDiscounts"></param>
        /// <param name="currPath"></param>
        /// <returns>單一路徑</returns>
        private static void ExpanseAll(List<Product> availableProducts, List<Discount> availableDiscounts, List<Discount> currPath, Dictionary<string, List<Discount>> discountPaths)
        {
            if (availableProducts.Any() && availableDiscounts.Any()) {
                var vaildDiscounts = ValidateDiscounts(availableProducts, availableDiscounts);
                List<Discount> noneExpansedDiscounts = vaildDiscounts.Clone().ToList();
                if (discountPaths.Any()) {
                    var discNamesInOneDimension = discountPaths.Values.SelectMany(p => p).Select(p => p.Name).ToList();
                    noneExpansedDiscounts = vaildDiscounts.Where(discount => !discNamesInOneDimension.Contains(discount.Name)).ToList();
                }
                if (noneExpansedDiscounts.Any()) {
                    foreach (var d in noneExpansedDiscounts) {
                        var nxtPath = currPath.Clone().ToList();
                        nxtPath.Add(d);
                        var restProducts = RemoveUsedProducts(availableProducts, d);
                        ExpanseAll(restProducts, vaildDiscounts.Clone().ToList(), nxtPath, discountPaths);
                    }
                }
                else {
                    if (vaildDiscounts.Any()) {
                        foreach (var d in vaildDiscounts) {
                            var nxtPath = currPath.Clone().ToList();
                            nxtPath.Add(d);
                            var restProducts = RemoveUsedProducts(availableProducts, d);
                            ExpanseAll(restProducts, vaildDiscounts.Clone().ToList(), nxtPath, discountPaths);
                        }
                    }
                }

                if (!vaildDiscounts.Any()) {
                    currPath = currPath.OrderBy(p => p.Name).ToList();
                    discountPaths[string.Join("-", currPath.Select(p => p.Name))] = currPath;
                }
            }
            else {
                currPath = currPath.OrderBy(p => p.Name).ToList();
                discountPaths[string.Join("-", currPath.Select(p => p.Name))] = currPath;
            }
        }

        /// <summary>全展開最佳</summary>
        /// <param name="basket"></param>
        /// <param name="availableDiscounts"></param>
        /// <returns></returns>
        private static List<Discount> GetBestDiscountPath(List<Product> basket, List<Discount> availableDiscounts)
        {
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            Dictionary<string, List<Discount>> discountPaths = new Dictionary<string, List<Discount>>();
            List<Discount> bestDiscountPath = new List<Discount>();

            var vaildDiscounts = ValidateDiscounts(basket, availableDiscounts);
            List<Discount> noneUsedDiscounts = vaildDiscounts.Clone().ToList();
            while (noneUsedDiscounts.Any()) {
                var rootOfPath = noneUsedDiscounts.First();
                List<Discount> currPath = new List<Discount>();
                currPath.Add(rootOfPath);
                var restProducts = RemoveUsedProducts(basket, rootOfPath);
                ExpanseAll(restProducts, vaildDiscounts, currPath, discountPaths);
                Console.WriteLine("discount completed.");

                if (discountPaths.Any()) {
                    var discNamesInOneDimension = discountPaths.Values.SelectMany(p => p).Select(p => p.Name).ToList();
                    noneUsedDiscounts = vaildDiscounts.Where(discount => !discNamesInOneDimension.Contains(discount.Name)).ToList();
                }
            }

            var performanceSet = discountPaths.Select(dp => new
            {
                DiscountPath = dp.Value,
                TotalDiscountAmount = dp.Value.Sum(d => d.Amount)
            });

            bestDiscountPath = performanceSet.OrderByDescending(x => x.TotalDiscountAmount).First().DiscountPath.OrderByDescending(x => x.Amount).ToList();
            sw.Stop();

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            string answer = string.Format("{0:D2}h-{1:D2}m-{2:D2}s-{3:D3}ms",
                                    timeSpan.Hours,
                                    timeSpan.Minutes,
                                    timeSpan.Seconds,
                                    timeSpan.Milliseconds);
            return bestDiscountPath;
        }
    }
}
