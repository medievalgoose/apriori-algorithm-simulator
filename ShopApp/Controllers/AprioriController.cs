using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ShopApp.Data;
using ShopApp.Models;
using ShopApp.Utilities;
using System.Collections;

namespace ShopApp.Controllers
{
    public class AprioriController : Controller
    {
        private readonly ShopAppDbContext _context;

        public AprioriController(ShopAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Result(float minSupport, float minConfidence)
        {
            /*
             * Steps to do apriori analysis:
             * (!) Determine support and confidence threshold.
             * 1. List all the total occurence of one product.
             * 2. List all the total occurence of two products.
             * 3. List all the total occurencec of three products.
             * 4. Determine the probability of the first item to influence the second item.
             * 5. Dtermine the probability of the first and second item to influence the third item.
             */

            // Assumed support threshold = 40%;
            // Assumed confidence threshold = 70%;

            // Formula = min_support * total_itemset
            // float minimumSupport = (_context.Products.Count() * 40) / 100;
            float minimumSupport = (_context.Products.Count() * minSupport) / 100;
            float minimumConfidence = minConfidence;


            // ONE ITEM OCCURENCE

            // Count the item occurence (1 item).
            // oneTimeOccurence format = Key: productId, Value: occurence
            var oneItemOccurence = new Dictionary<int, int>();
            var qualifiedProductList = new List<Product>();
            var productList = _context.Products.ToList();

            var qualifiedProductDictionary = new Dictionary<int, Product>();

            foreach (var product in productList)
            {
                var itemCount = _context.SalesDetails.Where(sd => sd.ProductId == product.Id).Count();

                if (itemCount >= minimumSupport)
                {
                    oneItemOccurence[product.Id] = itemCount;
                    qualifiedProductList.Add(product);

                    qualifiedProductDictionary[product.Id] = product; 
                }
            }

            // One item occurence debugging:
            for (int i = 0; i < qualifiedProductList.Count; i++)
            {
                Console.WriteLine($"{qualifiedProductList[i].ProductName}, occurence:  {oneItemOccurence[qualifiedProductList[i].Id]}");
            }


            // TWO ITEMS COMBINATIONS

            // List the item combinaction occurence (2 items).
            var itemCombinations = new List<List<int>>();
            
            // Count the 2 items combination occurence.
            var twoItemOccurence = new Dictionary<List<int>, int>(new ListComparer());

            var qualifiedTwoItemCombinations = new List<List<int>>();

            /*
             * Konsepnya:
             * Menggunakan teori kombinasi tanpa pengulangan (combinations without repetition)
             * Caranya: ambil ID produk dan kombinasikan dengan item di index selanjutnya.
             */

            for (int a = 0; a < qualifiedProductList.Count; a++)
            {
                var tempList = new List<int>();

                for (int i = a+1; i < qualifiedProductList.Count; i++)
                {
                    Console.WriteLine(qualifiedProductList[a].Id);
                    Console.WriteLine(qualifiedProductList[i].Id);

                    tempList.Add(qualifiedProductList[a].Id);
                    tempList.Add(qualifiedProductList[i].Id);

                    itemCombinations.Add(tempList);

                    tempList = new List<int>();
                }
            }

            for (int i = 0; i < itemCombinations.Count; i++)
            {
                Console.WriteLine(i + "-----------------------------");

                for (int a = 0; a < itemCombinations[i].Count; a++)
                {
                    Console.WriteLine(itemCombinations[i][a]);
                }
                Console.WriteLine("-----------------------------");
            }

            for (int i = 0; i < itemCombinations.Count; i++)
            {
                var salesList = _context.Sales.ToList();
                int count = 0;

                for (int a = 0; a < salesList.Count; a++)
                {
                    // Dalam satu sales yang sama, cek kombinasi item.
                    var firstItem = _context.SalesDetails.Where(sd => sd.SalesId == salesList[a].Id && sd.ProductId == itemCombinations[i][0]).FirstOrDefault();
                    var secondItem = _context.SalesDetails.Where(sd => sd.SalesId == salesList[a].Id && sd.ProductId == itemCombinations[i][1]).FirstOrDefault();

                    if (firstItem != null && secondItem != null)
                    {
                        count += 1;
                    }
                }

                Console.WriteLine($"Count: {count}, Minimum support: {minimumSupport}");
                if (count >= minimumSupport)
                {
                    Console.WriteLine("Qualified count");
                    qualifiedTwoItemCombinations.Add(itemCombinations[i]);
                    twoItemOccurence[itemCombinations[i]] = count;
                }
            }

            // Debugging purpose.
            for (int i = 0; i < twoItemOccurence.Count; i++)
            {
                Console.WriteLine("-------------------");
                Console.WriteLine($"{qualifiedTwoItemCombinations[i][0]}, {qualifiedTwoItemCombinations[i][1]}");
                Console.WriteLine(twoItemOccurence[qualifiedTwoItemCombinations[i]]);
                Console.WriteLine("-------------------");
            }



            // THREE ITEMS COMBINATIONS
            var threeItemsCombinations = new List<List<int>>();
            var threeItemsCombinationOccurence = new Dictionary<List<int>, int>();
            var qualifiedThreeItemsCombinations = new List<List<int>>();

            for (int a = 0; a < qualifiedProductList.Count; a++)
            {
                var tempThreeItemList = new List<int>();

                for (int b = a + 1; b < qualifiedProductList.Count; b++)
                {
                    for (int c = b + 1; c < qualifiedProductList.Count; c++)
                    {
                        tempThreeItemList.Add(qualifiedProductList[a].Id);
                        tempThreeItemList.Add(qualifiedProductList[b].Id);
                        tempThreeItemList.Add(qualifiedProductList[c].Id);

                        threeItemsCombinations.Add(tempThreeItemList);

                        tempThreeItemList = new List<int>();
                    }
                }
            }

            for (int a = 0; a < threeItemsCombinations.Count; a++)
            {
                var salesList = _context.Sales.ToList();
                int count = 0;

                for (int b = 0; b < salesList.Count; b++)
                {
                    var firstItem = _context.SalesDetails.Where(sd => sd.SalesId == salesList[b].Id && sd.ProductId == threeItemsCombinations[a][0]).FirstOrDefault();
                    var secondItem = _context.SalesDetails.Where(sd => sd.SalesId == salesList[b].Id && sd.ProductId == threeItemsCombinations[a][1]).FirstOrDefault();
                    var thirdItem = _context.SalesDetails.Where(sd => sd.SalesId == salesList[b].Id && sd.ProductId == threeItemsCombinations[a][2]).FirstOrDefault();

                    if (firstItem != null && secondItem != null && thirdItem != null)
                    {
                        count += 1;
                    }
                }

                if (count >= minimumSupport)
                {
                    qualifiedThreeItemsCombinations.Add(threeItemsCombinations[a]);
                    threeItemsCombinationOccurence[threeItemsCombinations[a]] = count;
                }
            }


            for (int i = 0; i < qualifiedThreeItemsCombinations.Count; i++)
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine($"{qualifiedThreeItemsCombinations[i][0]}, {qualifiedThreeItemsCombinations[i][1]}, {qualifiedThreeItemsCombinations[i][2]}");
                Console.WriteLine($"{threeItemsCombinationOccurence[qualifiedThreeItemsCombinations[i]]}");
                Console.WriteLine("-------------------------");
            }


            // CALCULATE THE CONFIDENCE

            // two item combinations confidence:
            // formula : P(x -> y) = P(x & y) / P(x)

            // Format for confidence dictioary:
            // Key: the combinations
            // Value: list of confidence, according from the items. E.g. index 0 of result list means the P(x) is the first item in the combination. 

            var twoItemCombinationConfidence = new Dictionary<List<int>, List<float>>();

            for (int i = 0; i < qualifiedTwoItemCombinations.Count; i++)
            {
                var tempConfidenceList = new List<float>(); 

                for (int a = 0; a < qualifiedTwoItemCombinations[i].Count; a++)
                {
                    tempConfidenceList.Add(
                        ((float) twoItemOccurence[qualifiedTwoItemCombinations[i]]) / ((float) oneItemOccurence[qualifiedTwoItemCombinations[i][a]])
                        );
                }

                twoItemCombinationConfidence.Add(qualifiedTwoItemCombinations[i], tempConfidenceList);

                tempConfidenceList = new List<float>();
            }

            // two item confidence debugging:
            for (int i = 0; i < qualifiedTwoItemCombinations.Count; i++)
            {
                Console.WriteLine("//-------------------//");
                Console.WriteLine($"{qualifiedTwoItemCombinations[i][0]}, {qualifiedTwoItemCombinations[i][1]}");
                
                for (int a = 0; a < qualifiedTwoItemCombinations[i].Count; a++)
                {
                    var resultFloatList = twoItemCombinationConfidence[qualifiedTwoItemCombinations[i]];

                    Console.WriteLine($"P({qualifiedTwoItemCombinations[i][a]} -> {qualifiedTwoItemCombinations[i][0]}, {qualifiedTwoItemCombinations[i][1]}) " +
                        $", confidence: {resultFloatList[a]}");
                }
            }


            // Three items combinations confidence
            var threeItemCombinationConfidence = new Dictionary<List<int>, List<float>>();

            // Format for the result:
            // First result: combination of 1 and 2
            // Second result: combination of 1 and 3
            // Third result: combination of 2 and 3

            for (int i = 0; i < qualifiedThreeItemsCombinations.Count; i++)
            {
                var resultFloatList = new List<float>();

                var firstCombinationList = new List<int>() { qualifiedThreeItemsCombinations[i][0], qualifiedThreeItemsCombinations[i][1] };
                var firstCombinationConfidence = (float) threeItemsCombinationOccurence[qualifiedThreeItemsCombinations[i]] / (float) twoItemOccurence[firstCombinationList];

                var secondCombinationList = new List<int>() { qualifiedThreeItemsCombinations[i][0], qualifiedThreeItemsCombinations[i][2] };
                var secondCombinationConfidence = (float)threeItemsCombinationOccurence[qualifiedThreeItemsCombinations[i]] / (float)twoItemOccurence[secondCombinationList];

                var thirdCombinationList = new List<int>() { qualifiedThreeItemsCombinations[i][1], qualifiedThreeItemsCombinations[i][2] };
                var thirdCombinationConfidence = (float)threeItemsCombinationOccurence[qualifiedThreeItemsCombinations[i]] / (float)twoItemOccurence[thirdCombinationList];

                /* Debugging purpose
                for (int a = 0; a < firstCombinationList.Count; a++)
                {
                    Console.WriteLine(firstCombinationList[a]);
                }
                */

                resultFloatList.Add(firstCombinationConfidence);
                resultFloatList.Add(secondCombinationConfidence);
                resultFloatList.Add(thirdCombinationConfidence);

                threeItemCombinationConfidence[qualifiedThreeItemsCombinations[i]] = resultFloatList;

                resultFloatList = new List<float>();
                firstCombinationList = new List<int>();
                secondCombinationList = new List<int>();
                thirdCombinationList = new List<int>();
            }

            // Three items combinations debugging:
            for (int i = 0; i < qualifiedThreeItemsCombinations.Count; i++)
            {
                Console.WriteLine("*******************************");

                var resultFloat = threeItemCombinationConfidence[qualifiedThreeItemsCombinations[i]];

                Console.WriteLine($"P({qualifiedThreeItemsCombinations[i][0]}, {qualifiedThreeItemsCombinations[i][1]} -> {qualifiedThreeItemsCombinations[i][2]}), confidence: {resultFloat[0]}");

                Console.WriteLine($"P({qualifiedThreeItemsCombinations[i][0]}, {qualifiedThreeItemsCombinations[i][2]} -> {qualifiedThreeItemsCombinations[i][1]}), confidence: {resultFloat[1]}");

                Console.WriteLine($"P({qualifiedThreeItemsCombinations[i][1]}, {qualifiedThreeItemsCombinations[i][2]} -> {qualifiedThreeItemsCombinations[i][0]}), confidence: {resultFloat[2]}");

                Console.WriteLine("*******************************");
            }



            // Testing ViewBag capabilities.
            /*
            var testDict = new Dictionary<string, string>();

            testDict["kunaon?"] = "baik atuh mang";

            ViewBag.Products = _context.Products.ToList();
            ViewBag.Dictionary = testDict;
            */


            // Final step: Passing data to the view.
            ViewBag.MinimumSupportPercentage = minSupport;
            ViewBag.MinimumSupportResult = minimumSupport;
            ViewBag.MinimumConfidence = minimumConfidence;

            ViewBag.ProductList = _context.Products.ToList();
            ViewBag.QualifiedProductDictionary = qualifiedProductDictionary;

            ViewBag.QualifiedProductList = qualifiedProductList;
            ViewBag.OneItemOccurence = oneItemOccurence;

            ViewBag.QualifiedTwoItemCombinations = qualifiedTwoItemCombinations;
            ViewBag.TwoItemOccurence = twoItemOccurence;
            ViewBag.TwoItemCombinationConfidence = twoItemCombinationConfidence;

            Console.WriteLine(qualifiedTwoItemCombinations.Count);

            ViewBag.QualifiedThreeItemCombinations = qualifiedThreeItemsCombinations;
            ViewBag.ThreeItemOccurence = threeItemsCombinationOccurence;
            ViewBag.ThreeItemCombinationConfidence = threeItemCombinationConfidence;

            ViewBag.MinimumConfidence = minimumConfidence;

            return View();
        }
    }
}
