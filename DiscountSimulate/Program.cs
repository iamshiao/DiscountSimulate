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
        private static List<Product> _basket = new List<Product>();
        private static List<Discount> _discounts = new List<Discount>();

        #region 商品
        private static Product pA = new Product { Name = "A", Price = 100 };
        private static Product pB = new Product { Name = "B", Price = 100 };
        private static Product pC = new Product { Name = "C", Price = 100 };
        private static Product pD = new Product { Name = "D", Price = 100 };
        private static Product pE = new Product { Name = "E", Price = 100 };
        private static Product pF = new Product { Name = "F", Price = 100 };
        private static Product pG = new Product { Name = "G", Price = 60 };
        private static Product pH = new Product { Name = "H", Price = 100 };
        private static Product pI = new Product { Name = "I", Price = 100 };
        private static Product pJ = new Product { Name = "J", Price = 100 };
        private static Product pK = new Product { Name = "K", Price = 100 };
        private static Product pAA = new Product { Name = "AA", Price = 120 };
        private static Product pBB = new Product { Name = "BB", Price = 150 };
        private static Product pCC = new Product { Name = "CC", Price = 200 };
        #endregion

        static void Main(string[] args)
        {
            UseLadData5();
            _discounts = _discounts.OrderBy(d => d.Amount).ToList();

            Stopwatch sw = new Stopwatch();
            sw.Reset();

            sw.Start();
            var strategyA = EvalBestDiscount(_basket.Clone().ToList(), _discounts.Clone().ToList());
            double totalA = strategyA.Sum(a => a.Amount);
            sw.Stop();
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            string cost = $"{timeSpan.Hours}h-{timeSpan.Minutes}m-{timeSpan.Seconds}s-{timeSpan.Milliseconds}ms";
            Console.WriteLine($"strategyA: {cost}");

            sw.Restart();
            var strategyB = EvalBestDistributionDiscount(_basket.Clone().ToList(), _discounts.Clone().ToList());
            double totalB = strategyB.Sum(b => b.Amount);
            sw.Stop();
            timeSpan = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            cost = $"{timeSpan.Hours}h-{timeSpan.Minutes}m-{timeSpan.Seconds}s-{timeSpan.Milliseconds}ms";
            Console.WriteLine($"strategyB: {cost}");

            sw.Restart();
            var strategyC = ExpanseAllDiscountPaths(_basket.Clone().ToList(), _discounts.Clone().ToList());
            double totalC = strategyC.Sum(c => c.Amount);
            sw.Stop();
            timeSpan = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            cost = $"{timeSpan.Hours}h-{timeSpan.Minutes}m-{timeSpan.Seconds}s-{timeSpan.Milliseconds}ms";
            Console.WriteLine($"strategyC: {cost}");

            sw.Restart();
            var strategyC2 = ExpansePrunedDiscountPaths(_basket.Clone().ToList(), _discounts.Clone().ToList());
            double totalC2 = strategyC2.Sum(c2 => c2.Amount);
            sw.Stop();
            timeSpan = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            cost = $"{timeSpan.Hours}h-{timeSpan.Minutes}m-{timeSpan.Seconds}s-{timeSpan.Milliseconds}ms";
            Console.WriteLine($"strategyC2: {cost}");

            sw.Restart();
            var strategyD = ExpanseBestDistributionDiscountTree(_basket.Clone().ToList(), _discounts.Clone().ToList());
            double totalD = strategyD.Sum(d => d.Amount);
            sw.Stop();
            timeSpan = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            cost = $"{timeSpan.Hours}h-{timeSpan.Minutes}m-{timeSpan.Seconds}s-{timeSpan.Milliseconds}ms";
            Console.WriteLine($"strategyD: {cost}");

            Console.ReadLine();
        }

        private static void UseLadData1()
        {
            //_basket = new List<Product> { pA, pB, pC, pD };
            _basket = new List<Product> { pA, pB, pC, pA, pB, pC, pD, pA, pA, pA };

            Discount dA = new Discount { Name = "dA", Amount = 50, Combination = new List<Product> { pA } };
            Discount dAp75 = new Discount { Name = "dAp75", Amount = pA.Price * 0.25, Combination = new List<Product> { pA } };
            Discount dB = new Discount { Name = "dB", Amount = 50, Combination = new List<Product> { pB } };
            Discount dC = new Discount { Name = "dC", Amount = 50, Combination = new List<Product> { pC } };
            Discount dD = new Discount { Name = "dD", Amount = 50, Combination = new List<Product> { pD } };

            //Discount dABC = new Discount { Name = "dABC", Amount = 130, Combination = new List<Product> { pA, pB, pC } };
            Discount dABC = new Discount { Name = "dABC", Amount = 174, Combination = new List<Product> { pA, pB, pC } };
            Discount dAB = new Discount { Name = "dAB", Amount = 95, Combination = new List<Product> { pA, pB } };
            Discount dBC = new Discount { Name = "dBC", Amount = 90, Combination = new List<Product> { pB, pC } };
            Discount dAC = new Discount { Name = "dAC", Amount = 90, Combination = new List<Product> { pA, pC } };
            Discount dBD = new Discount { Name = "dBD", Amount = 120, Combination = new List<Product> { pB, pD } };
            Discount dACD = new Discount { Name = "dACD", Amount = 160, Combination = new List<Product> { pA, pC, pD } };
            Discount dCD = new Discount { Name = "dCD", Amount = 110, Combination = new List<Product> { pC, pD } };

            //Discount dABp75 = new Discount { Amount = (pA.Price + pB.Price) * 0.75, Combination = new List<Product> { pA, pB } }; // 買 A 送 B
            //Discount dAB = new Discount { Amount = pB.Price, Combination = new List<Product> { pA, pB } }; // 買 A 送 B
            Discount d2A = new Discount { Name = "d2A", Amount = pA.Price, Combination = new List<Product> { pA, pA } }; // 買 A 送 A
            Discount d3B = new Discount { Name = "d3B", Amount = pB.Price, Combination = new List<Product> { pB, pB, pB } }; // 買 2B 送 B
            Discount d2CD = new Discount { Name = "d2CD", Amount = pD.Price, Combination = new List<Product> { pC, pC, pD } }; // 買 2C 送 D

            _discounts.AddRange(new List<Discount> { dA, dB, dC, dD, dAp75 });
            _discounts.AddRange(new List<Discount> { dABC, dAB, dBC, dAC, dBD, dACD, dCD });
            _discounts.AddRange(new List<Discount> { d2A, d3B, d2CD });
        }

        private static void UseLadData2()
        {
            _basket = new List<Product> { pA, pB, pC, pC, pC };
            _basket = AddNumsOfProductToBasket(_basket, pA, 95);
            _basket = AddNumsOfProductToBasket(_basket, pB, 6);
            _basket = AddNumsOfProductToBasket(_basket, pC, 95);
            _basket = AddNumsOfProductToBasket(_basket, pD, 1);
            _basket = AddNumsOfProductToBasket(_basket, pE, 2);
            _basket = AddNumsOfProductToBasket(_basket, pF, 3);
            _basket = AddNumsOfProductToBasket(_basket, pG, 1);
            _basket = AddNumsOfProductToBasket(_basket, pH, 2);
            _basket = AddNumsOfProductToBasket(_basket, pI, 3);
            _basket = AddNumsOfProductToBasket(_basket, pJ, 1);
            _basket = AddNumsOfProductToBasket(_basket, pK, 2);
            _basket = AddNumsOfProductToBasket(_basket, pAA, 15);
            _basket = AddNumsOfProductToBasket(_basket, pBB, 18);
            _basket = AddNumsOfProductToBasket(_basket, pCC, 17);

            List<IDiscountRule> discountRules = new List<IDiscountRule>();
            discountRules.Add(new SingleProductSpecialPrice(95, new List<Product> { pA, pC, pE, pI, pK }));
            discountRules.Add(new SingleProductFixedAmountDiscount(6, new List<Product> { pB, pD, pF, pG, pH, pJ }));
            discountRules.Add(new SingleProductPercentDiscount(0.96, new List<Product> { pI, pJ, pK }));
            discountRules.Add(new BuyOneGetOneFree(new List<Product> { pG }));
            discountRules.Add(new BuyTwoGetOneFree(new List<Product> { pC }));
            discountRules.Add(new FreeUpdate(pA, pAA));
            discountRules.Add(new FreeUpdate(pB, pBB));
            discountRules.Add(new FreeUpdate(pC, pCC));
            discountRules.Add(new BuyAGetBFree(pI, pJ));
            discountRules.Add(new MultiProductPercentDiscount(new List<Product> { pA, pB }, 0.75));
            discountRules.Add(new MultiProductFixedPrice(new List<Product> { pA, pA, pB }, 200));
            discountRules.Add(new MultiProductFixedAmountDiscount(new List<Product> { pA, pB }, 5));
            discountRules.Add(new MultiProductFixedAmountDiscount(new List<Product> { pA, pB, pC }, 120));
            discountRules.Add(new MultiProductFixedPrice(new List<Product> { pA, pB, pD }, 250));
            discountRules.Add(new MultiProductPercentDiscount(new List<Product> { pB, pC, pD }, 0.65));

            discountRules.Add(new MultiProductPercentDiscount(new List<Product> { pC, pD }, 0.7));
            discountRules.Add(new MultiProductFixedPrice(new List<Product> { pI, pJ, pH }, 200));
            discountRules.Add(new MultiProductFixedAmountDiscount(new List<Product> { pG, pAA }, 105));
            discountRules.Add(new MultiProductFixedPrice(new List<Product> { pE, pH }, 120));
            discountRules.Add(new MultiProductPercentDiscount(new List<Product> { pF, pH, pK }, 0.95));

            discountRules.Add(new MultiProductPercentDiscount(new List<Product> { pC, pD, pE }, 0.75));
            discountRules.Add(new MultiProductFixedAmountDiscount(new List<Product> { pA, pB, pC }, 120));
            discountRules.Add(new MultiProductFixedAmountDiscount(new List<Product> { pF, pG }, 160));

            _discounts = new List<Discount>();
            foreach (var rule in discountRules) {
                _discounts.AddRange(rule.GetDiscounts(_basket));
            }
        }

        private static void UseLadData3()
        {
            // 可以讓 strategy A, B 都錯的 case

            _basket = new List<Product> { pA, pB, pC, pD, pE, pF, pG, pH, pI, pJ };

            Discount dAB = new Discount { Name = "dAB", Amount = 60, Combination = new List<Product> { pA, pB } };
            Discount dCD = new Discount { Name = "dCD", Amount = 60, Combination = new List<Product> { pC, pD } };
            Discount dABCDE = new Discount { Name = "dABCDE", Amount = 100, Combination = new List<Product> { pA, pB, pC, pD, pE } };
            Discount dFG = new Discount { Name = "dFG", Amount = 45, Combination = new List<Product> { pF, pG } };
            Discount dHI = new Discount { Name = "dHI", Amount = 45, Combination = new List<Product> { pH, pI } };
            Discount dFGHIJ = new Discount { Name = "dFGHIJ", Amount = 100, Combination = new List<Product> { pF, pG, pH, pI, pJ } };

            _discounts.AddRange(new List<Discount> { dAB, dCD, dABCDE, dFG, dHI, dFGHIJ });
        }

        private static void UseLadData4()
        {
            _basket = new List<Product> { pA, pB, pC, pD, pA, pA };

            Discount dAB = new Discount { Name = "dAB", Amount = 14, Combination = new List<Product> { pA, pB } };
            Discount dAA = new Discount { Name = "dAA", Amount = 10, Combination = new List<Product> { pA, pA } };
            Discount dAC = new Discount { Name = "dAC", Amount = 20, Combination = new List<Product> { pA, pC } };
            Discount dBD = new Discount { Name = "dBD", Amount = 15, Combination = new List<Product> { pB, pD } };
            Discount dD = new Discount { Name = "dD", Amount = 12, Combination = new List<Product> { pD } };

            _discounts.AddRange(new List<Discount> { dAB, dAA, dAC, dBD, dD });
        }

        private static void UseLadData5()
        {
            _basket = new List<Product> { pA, pB, pC, pB, pC, pB, pC };

            Discount dAB = new Discount { Name = "dAB", Amount = 10, Combination = new List<Product> { pA, pB } };
            Discount dBC = new Discount { Name = "dBC", Amount = 9, Combination = new List<Product> { pB, pD } };
            Discount dBCBCBC = new Discount { Name = "dBCBCBC", Amount = 29, Combination = new List<Product> { pB, pC, pB, pC, pB, pC } };

            _discounts.AddRange(new List<Discount> { dAB, dBC, dBCBCBC });
        }

        private static List<Product> AddNumsOfProductToBasket(List<Product> basket, Product p, int num)
        {
            for (int i = 0; i < num; i++) {
                basket.Add(p);
            }

            return basket;
        }

        /// <summary>移除耗用的商品</summary>
        /// <param name="availableProducts"></param>
        /// <param name="usedDiscount"></param>
        /// <returns></returns>
        private static List<Product> RemoveUsedProducts(List<Product> availableProducts, Discount usedDiscount)
        {
            availableProducts = new List<Product>(availableProducts);

            foreach (var usedProduct in usedDiscount.Combination) {
                var markedProduct = availableProducts.FirstOrDefault(p => p.Name == usedProduct.Name);
                if (markedProduct != null) {
                    availableProducts.Remove(markedProduct);
                }
            }
            return availableProducts;
        }

        /// <summary>找出依商品最大折扣排列選出的折扣組合</summary>
        /// <param name="products"></param>
        /// <param name="discounts"></param>
        /// <returns></returns>
        private static List<Discount> EvalBestDiscount(List<Product> products, List<Discount> discounts)
        {
            List<Discount> bestDiscounts = new List<Discount>();

            while (products.Any() && discounts.Any()) { // 當折扣或商品組合未用盡則持續尋找折扣
                List<Discount> bestDiscountsOfEachProduct = new List<Discount>();
                foreach (var p in products) {
                    var currProductDiscounts = discounts.Where(d => d.Combination.Any(ele => ele.Name == p.Name)); // 包含該商品的折扣組合
                    if (currProductDiscounts.Any()) {
                        Discount currBest = currProductDiscounts.First();
                        foreach (var d in currProductDiscounts) {
                            if (d.Amount > currBest.Amount) {
                                currBest = d;
                            }
                        }
                        bestDiscountsOfEachProduct.Add(currBest);
                    }
                }
                var bestDiscountThisLoop = bestDiscountsOfEachProduct.OrderByDescending(ele => ele.Amount).FirstOrDefault();
                bestDiscounts.Add(bestDiscountThisLoop);

                // 移除本輪已耗用的商品組合
                foreach (var element in bestDiscountThisLoop.Combination) {
                    products.Remove(products.FirstOrDefault(p => p.Name == element.Name));
                }

                discounts = ValidateDiscounts(products, discounts);
            }

            return bestDiscounts;
        }

        /// <summary>找出依商品最大分配折扣額排列選出的折扣組合</summary>
        /// <param name="products"></param>
        /// <param name="discounts"></param>
        /// <returns></returns>
        private static List<Discount> EvalBestDistributionDiscount(List<Product> products, List<Discount> discounts)
        {
            List<Discount> bestDiscounts = new List<Discount>();

            while (products.Any() && discounts.Any()) { // 當折扣或商品組合未用盡則持續尋找折扣
                List<Discount> bestDiscountsOfEachProduct = new List<Discount>();
                foreach (var p in products) {
                    var currProductDiscounts = discounts.Where(d => d.Combination.Any(ele => ele.Name == p.Name));
                    if (currProductDiscounts.Any()) {
                        Discount currBest = currProductDiscounts.First();
                        foreach (var d in currProductDiscounts) {
                            if (d.Distribution >= currBest.Distribution) {
                                currBest = d;
                            }
                        }
                        bestDiscountsOfEachProduct.Add(currBest);
                    }
                }
                var bestDiscountThisLoop = bestDiscountsOfEachProduct.OrderByDescending(ele => ele.Distribution).FirstOrDefault();
                bestDiscounts.Add(bestDiscountThisLoop);

                // 移除本輪已耗用的商品組合
                foreach (var element in bestDiscountThisLoop.Combination) {
                    products.Remove(products.FirstOrDefault(p => p.Name == element.Name));
                }

                discounts = ValidateDiscounts(products, discounts);
            }

            return bestDiscounts;
        }

        /// <summary>以各商品最大分配折扣額為基準的擴展樹</summary>
        /// <param name="products"></param>
        /// <param name="discounts"></param>
        /// <returns></returns>
        private static List<Discount> ExpanseBestDistributionDiscountTree(List<Product> products, List<Discount> discounts)
        {
            List<Discount> bestDiscountPath = new List<Discount>();
            List<List<Discount>> discountPaths = new List<List<Discount>>();

            foreach (var currProduct in products) {
                var currProductDiscounts = discounts.Where(d => d.Combination.Any(ele => ele.Name == currProduct.Name));
                if (currProductDiscounts.Any()) {
                    Discount currBest = currProductDiscounts.First();
                    foreach (var d in currProductDiscounts) {
                        if (d.Distribution >= currBest.Distribution) {
                            currBest = d;
                        }
                    }
                    var bestDiscountPathThisLoop = GetDerivativePath(currBest, products, discounts);
                    discountPaths.Add(bestDiscountPathThisLoop);
                }
            }

            var performanceSet = discountPaths.Select(
                path => new
                {
                    DiscountPath = path,
                    TotalDiscountAmount = path.Sum(d => d.Amount)
                });

            bestDiscountPath = performanceSet.OrderByDescending(x => x.TotalDiscountAmount).First().DiscountPath.OrderByDescending(x => x.Amount).ToList();

            return bestDiscountPath;
        }

        /// <summary>取得衍生自根折扣的商品最大可能分配折扣額路徑</summary>
        /// <param name="root">根折扣</param>
        /// <param name="products"></param>
        /// <param name="discounts"></param>
        /// <returns></returns>
        private static List<Discount> GetDerivativePath(Discount root, List<Product> products, List<Discount> discounts)
        {
            List<Discount> derivativePath = new List<Discount>();

            var copyOfProducts = products.Clone().ToList();
            var copyOfDiscounts = discounts.Clone().ToList();

            derivativePath.Add(root);
            // 因為同商品可能有複數個所以不能用 Where，要迴圈砍
            foreach (var element in root.Combination) {
                copyOfProducts.Remove(copyOfProducts.FirstOrDefault(p => p.Name == element.Name));
            }
            copyOfDiscounts = ValidateDiscounts(copyOfProducts, copyOfDiscounts);

            while (copyOfProducts.Any() && copyOfDiscounts.Any()) {
                List<Discount> bestDiscountsOfEachProduct = new List<Discount>();
                foreach (var currProduct in copyOfProducts) {
                    var currProductDiscounts = copyOfDiscounts.Where(d => d.Combination.Any(ele => ele.Name == currProduct.Name));
                    if (currProductDiscounts.Any()) {
                        Discount currBest = currProductDiscounts.First();
                        foreach (var d in currProductDiscounts) {
                            if (d.Distribution >= currBest.Distribution) {
                                currBest = d;
                            }
                        }
                        bestDiscountsOfEachProduct.Add(currBest);
                    }
                }
                var bestDiscountThisLoop = bestDiscountsOfEachProduct.OrderByDescending(ele => ele.Amount).FirstOrDefault();
                derivativePath.Add(bestDiscountThisLoop);

                foreach (var element in bestDiscountThisLoop.Combination) {
                    copyOfProducts.Remove(copyOfProducts.FirstOrDefault(p => p.Name == element.Name));
                }

                copyOfDiscounts = ValidateDiscounts(copyOfProducts, copyOfDiscounts);
            }

            return derivativePath;
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

        /// <summary>剪枝展開遞迴</summary>
        /// <param name="availableProducts"></param>
        /// <param name="availableDiscounts"></param>
        /// <param name="currPath"></param>
        /// <param name="discountPaths">所有可能的折扣路徑</param>
        private static void ExpanseAndPruningRecursively(List<Product> availableProducts, List<Discount> availableDiscounts, List<Discount> currPath, Dictionary<string, List<Discount>> discountPaths)
        {
            if (availableProducts.Any() && availableDiscounts.Any()) {
                var vaildDiscounts = ValidateDiscounts(availableProducts, availableDiscounts);
                List<Discount> noneExpansedDiscounts = null;
                if (discountPaths.Any()) {
                    var discNamesInOneDimension = discountPaths.Values.SelectMany(p => p).Select(p => p.Name).ToList();
                    noneExpansedDiscounts = vaildDiscounts.Where(discount => !discNamesInOneDimension.Contains(discount.Name)).ToList();
                }
                if (noneExpansedDiscounts != null && noneExpansedDiscounts.Any()) {
                    foreach (var d in noneExpansedDiscounts) {
                        var nxtPath = currPath.Clone().ToList();
                        nxtPath.Add(d);
                        var restProducts = RemoveUsedProducts(availableProducts, d);
                        ExpanseAndPruningRecursively(restProducts, vaildDiscounts.Clone().ToList(), nxtPath, discountPaths);
                    }
                }
                else {
                    if (vaildDiscounts.Any()) {
                        foreach (var d in vaildDiscounts) {
                            var nxtPath = currPath.Clone().ToList();
                            nxtPath.Add(d);
                            var restProducts = RemoveUsedProducts(availableProducts, d);
                            ExpanseAndPruningRecursively(restProducts, vaildDiscounts.Clone().ToList(), nxtPath, discountPaths);
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

        /// <summary>剪枝展開最佳</summary>
        /// <param name="availableProducts"></param>
        /// <param name="availableDiscounts"></param>
        /// <returns>最佳折扣路徑</returns>
        private static List<Discount> ExpansePrunedDiscountPaths(List<Product> availableProducts, List<Discount> availableDiscounts)
        {
            List<Discount> bestDiscountPath = new List<Discount>();
            Dictionary<string, List<Discount>> discountPaths = new Dictionary<string, List<Discount>>();

            var validDiscounts = ValidateDiscounts(availableProducts, availableDiscounts);
            List<Discount> noneUsedDiscounts = validDiscounts.Clone().ToList();
            while (noneUsedDiscounts.Any()) {
                var rootOfPath = noneUsedDiscounts.First();
                List<Discount> currPath = new List<Discount>();
                currPath.Add(rootOfPath);
                var restProducts = RemoveUsedProducts(availableProducts, rootOfPath);
                ExpanseAndPruningRecursively(restProducts, validDiscounts, currPath, discountPaths);

                if (discountPaths.Any()) {
                    var discNamesInOneDimension = discountPaths.Values.SelectMany(p => p).Select(p => p.Name).ToList();
                    noneUsedDiscounts = validDiscounts.Where(discount => !discNamesInOneDimension.Contains(discount.Name)).ToList();
                }
            }

            var performanceSet = discountPaths.Select(dp => new
            {
                DiscountPath = dp.Value,
                TotalDiscountAmount = dp.Value.Sum(d => d.Amount)
            });

            bestDiscountPath = performanceSet.OrderByDescending(x => x.TotalDiscountAmount).First().DiscountPath.OrderByDescending(x => x.Amount).ToList();

            return bestDiscountPath;
        }

        /// <summary>全展開遞迴</summary>
        /// <param name="availableProducts"></param>
        /// <param name="availableDiscounts"></param>
        /// <param name="currPath"></param>
        /// <param name="discountPaths"></param>
        private static void ExpanseRecursively(List<Product> availableProducts, List<Discount> availableDiscounts, List<Discount> currPath, Dictionary<string, List<Discount>> discountPaths)
        {
            if (availableProducts.Any() && availableDiscounts.Any()) {
                var vaildDiscountColl = ValidateDiscounts(availableProducts, availableDiscounts);
                foreach (var d in vaildDiscountColl) {
                    var nxtPath = currPath.Clone().ToList();
                    nxtPath.Add(d);
                    var restProductColl = RemoveUsedProducts(availableProducts, d);
                    ExpanseRecursively(restProductColl, vaildDiscountColl.Clone().ToList(), nxtPath, discountPaths);
                }

                if (!vaildDiscountColl.Any()) {
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
        /// <param name="avaliableProducts"></param>
        /// <param name="availableDiscounts"></param>
        /// <returns></returns>
        private static List<Discount> ExpanseAllDiscountPaths(List<Product> avaliableProducts, List<Discount> availableDiscounts)
        {
            Dictionary<string, List<Discount>> discountPaths = new Dictionary<string, List<Discount>>();
            List<Discount> bestDiscountPath = new List<Discount>();

            var vaildDiscounts = ValidateDiscounts(avaliableProducts, availableDiscounts);
            foreach (var d in vaildDiscounts) {
                List<Discount> currPath = new List<Discount>();
                currPath.Add(d);
                var restProducts = RemoveUsedProducts(avaliableProducts, d);
                ExpanseRecursively(restProducts, vaildDiscounts, currPath, discountPaths);
            }

            var performanceSet = discountPaths.Select(dp => new
            {
                DiscountPath = dp.Value,
                TotalDiscountAmount = dp.Value.Sum(d => d.Amount)
            });

            bestDiscountPath = performanceSet.OrderByDescending(x => x.TotalDiscountAmount).First().DiscountPath.OrderByDescending(x => x.Amount).ToList();

            return bestDiscountPath;
        }
    }
}
