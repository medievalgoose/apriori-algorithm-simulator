using Microsoft.AspNetCore.Mvc;
using ShopApp.Utilities;
using System.Text;

namespace ShopApp.Controllers
{
    public class FileBasedController : Controller
    {
        public class SuperstoreOrder
        {
            public string OrderID { get; set; }
            public List<SuperstoreProduct> ProductsList = new List<SuperstoreProduct>();
        }

        public class SuperstoreProduct
        {
            public string ProductID { get; set; }
            public string ProductName { get; set; }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Result(IFormFile inputFile, float minimumSupport, int minimumConfidence)
        {
            var uniqueOrderList = new List<SuperstoreOrder>();
            var uniqueProductList = new List<SuperstoreProduct>();

            var productDictionary = new Dictionary<string, SuperstoreProduct>();
            var orderDictionary = new Dictionary<string, SuperstoreOrder>();

            using (var reader = new StreamReader(inputFile.OpenReadStream()))
            {
                int index = 0;

                while (!reader.EndOfStream)
                {
                    /*
                    if (index == 1000)
                    {
                        break;
                    }
                    */
                    

                    var line = reader.ReadLine();
                    var values = line?.Split(',');

                    // Index 0 is the column names.
                    if (index != 0)
                    {
                        var currentProduct = new SuperstoreProduct();
                        // currentProduct.ProductID = values[13];
                        // currentProduct.ProductName = values[16];
                        currentProduct.ProductID = values[1];
                        currentProduct.ProductName = values[2];

                        SuperstoreProduct x;
                        if (productDictionary.TryGetValue(currentProduct.ProductID, out x) == false)
                        {
                            productDictionary[currentProduct.ProductID] = currentProduct;
                            uniqueProductList.Add(currentProduct);
                        }
                        else
                        {
                            // currentProduct = productDictionary[values[13]];
                            currentProduct = productDictionary[values[1]];
                        }

                        var currentOrder = new SuperstoreOrder();
                        // currentOrder.OrderID = values[1];
                        currentOrder.OrderID = values[0];

                        SuperstoreOrder xO;
                        if (orderDictionary.TryGetValue(currentOrder.OrderID, out xO) == false)
                        {
                            orderDictionary[currentOrder.OrderID] = currentOrder;
                            uniqueOrderList.Add(currentOrder);
                        }
                        else
                        {
                            // currentOrder = orderDictionary[values[1]];
                            currentOrder = orderDictionary[values[0]];
                        }

                        currentOrder.ProductsList.Add(currentProduct);
                    }

                    index++;
                }
            }

            foreach (var order in uniqueOrderList)
            {
                Console.WriteLine($"Order ID: {order.OrderID}");

                foreach (var product in order.ProductsList)
                {
                    Console.WriteLine($"Product in sales ID {order.OrderID}: {product.ProductID}");
                }
            }

            Console.WriteLine($"Unique products in {uniqueOrderList.Count} transactions: " + uniqueProductList.Count);
            Console.WriteLine($"Minimum Support: {minimumSupport} | Minimum Confidence: {minimumConfidence}");


            // APRIORI ALGORITHM PROCESSING - START
            float minimumSupportFinal = (uniqueOrderList.Count * minimumSupport) / 100;
            // float minimumSupportFinal = minimumSupport;
            float minimumConfidenceFinal = minimumConfidence;

            // CALCULATING SUPPORT - ONE ITEM
            var qualifiedProductList = new List<string>();
            var oneItemOccurence = new Dictionary<string, int>();

            foreach (var product in uniqueProductList)
            {
                int productCount = 0;

                for (int i = 0; i < uniqueOrderList.Count; i++)
                {
                    if (uniqueOrderList[i].ProductsList.Contains(product))
                    {
                        productCount++;
                    }
                }

                if (productCount >= minimumSupportFinal)
                {
                    qualifiedProductList.Add(product.ProductID);
                    oneItemOccurence[product.ProductID] = productCount;
                }
            }

            foreach (var qualProd in qualifiedProductList)
            {
                Console.WriteLine($"Product ID: {qualProd} | Occurence: {oneItemOccurence[qualProd]}");
            }

            // CALCULATING SUPPORT TWO ITEMS COMBINATIONS
            var rawTwoItemCombinations = new List<List<string>>();
            var qualifiedTwoItemCombinations = new List<List<string>>();
            var twoItemOccurence = new Dictionary<List<string>, int>(new StringListComparer());

            for (int i = 0; i < qualifiedProductList.Count; i++)
            {
                for (int a = i + 1; a < qualifiedProductList.Count; a++)
                {
                    var currentCombination = new List<string>() { qualifiedProductList[i], qualifiedProductList[a] };
                    rawTwoItemCombinations.Add(currentCombination);
                }
            }

            foreach(var combination in rawTwoItemCombinations)
            {
                Console.WriteLine($"First item: {combination[0]} | Second item: {combination[1]}");
            }

            for (int i = 0; i < rawTwoItemCombinations.Count; i++)
            {
                int currentCombinationCount = 0;

                foreach (var order in uniqueOrderList)
                {
                    if (order.ProductsList.Contains(productDictionary[rawTwoItemCombinations[i][0]]) 
                        && order.ProductsList.Contains(productDictionary[rawTwoItemCombinations[i][1]]))
                    {
                        currentCombinationCount++;
                    }
                }

                if (currentCombinationCount >= minimumSupportFinal)
                {
                    qualifiedTwoItemCombinations.Add(rawTwoItemCombinations[i]);

                    string firstProductID = rawTwoItemCombinations[i][0];
                    string secondProductID = rawTwoItemCombinations[i][1];

                    var tempList = new List<string>() { firstProductID, secondProductID };

                    twoItemOccurence[tempList] = currentCombinationCount;
                }
            }

            foreach (var combination in qualifiedTwoItemCombinations)
            {
                Console.WriteLine($"Item combination: {combination[0]}, {combination[1]} | Occurence: {twoItemOccurence[combination]}");
            }


            // CALCULATE SUPPORT FOR THREE ITEM COMBINATIONS

            var rawThreeItemCombinations = new List<List<string>>();
            var qualifiedThreeItemCombinations = new List<List<string>>();
            var threeItemOccurence = new Dictionary<List<string>, int>();

            for (int a = 0; a < qualifiedProductList.Count; a++)
            {
                for (int b = a + 1; b < qualifiedProductList.Count; b++)
                {
                    for (int c = b + 1; c < qualifiedProductList.Count; c++)
                    {
                        var tempList = new List<string>() { qualifiedProductList[a], qualifiedProductList[b], qualifiedProductList[c] };
                        rawThreeItemCombinations.Add(tempList);
                    }
                }
            }

            foreach (var combination in rawThreeItemCombinations)
            {
                int currentCombinationCount = 0;

                foreach (var order in uniqueOrderList)
                {
                    if (order.ProductsList.Contains(productDictionary[combination[0]]) 
                        && order.ProductsList.Contains(productDictionary[combination[1]]) 
                        && order.ProductsList.Contains(productDictionary[combination[2]]))
                    {
                        currentCombinationCount++;
                    }
                }

                if (currentCombinationCount >= minimumSupportFinal)
                {
                    qualifiedThreeItemCombinations.Add(combination);
                    threeItemOccurence[combination] = currentCombinationCount;
                }
            }

            foreach (var combination in qualifiedThreeItemCombinations)
            {
                Console.WriteLine($"Item combination: {combination[0]}, {combination[1]}, {combination[2]} | Occurence: {threeItemOccurence[combination]}");
            }


            // CALCULATING THE CONFIDENCE
            var twoItemCombinationConfidence = new Dictionary<List<string>, List<float>>();

            foreach (var combination in qualifiedTwoItemCombinations)
            {
                float firstResult = (float) twoItemOccurence[combination] / (float) oneItemOccurence[combination[0]];
                float secondResult = (float) twoItemOccurence[combination] / (float) oneItemOccurence[combination[1]];

                var tempResultList = new List<float>() { firstResult, secondResult };

                twoItemCombinationConfidence[combination] = tempResultList;
            }

            foreach (var combination in qualifiedTwoItemCombinations)
            {
                Console.WriteLine($"Item combination: {combination[0]}, {combination[1]} | Occurence: {twoItemOccurence[combination]} | Confidence : {twoItemCombinationConfidence[combination][0]} & {twoItemCombinationConfidence[combination][1]}");
            }


            var threeItemCombinationConfidence = new Dictionary<List<string>, List<float>>();

            foreach (var combination in qualifiedThreeItemCombinations)
            {
                var firstResultList = new List<string>() { combination[0], combination[1] };
                float firstResult = (float) threeItemOccurence[combination] / (float) twoItemOccurence[firstResultList];

                var secondResultList = new List<string>() { combination[0], combination[2] };
                float secondResult = (float)threeItemOccurence[combination] / (float)twoItemOccurence[secondResultList];

                var thirdResultList = new List<string>() { combination[1], combination[2] };
                float thirdResult = (float)threeItemOccurence[combination] / (float)twoItemOccurence[thirdResultList];

                var tempResultFloat = new List<float>() { firstResult, secondResult, thirdResult };
                threeItemCombinationConfidence[combination] = tempResultFloat;
            }

            foreach (var combination in qualifiedThreeItemCombinations)
            {
                Console.WriteLine($"Item combination: {combination[0]}, {combination[1]}, {combination[2]} | Occurence: {threeItemOccurence[combination]} | Confidence: {threeItemCombinationConfidence[combination][0]} & {threeItemCombinationConfidence[combination][1]} & {threeItemCombinationConfidence[combination][2]}");
            }


            // PASSING THE DATA TO VIEW
            ViewBag.FileName = inputFile.FileName;

            ViewBag.UniqueOrderList = uniqueOrderList;
            ViewBag.UniqueProductList = uniqueProductList;

            ViewBag.ProductDictionary = productDictionary;
            ViewBag.OrderDictionary = orderDictionary;

            ViewBag.MinimumSupport = minimumSupport;
            ViewBag.MinimumSupportFinal = minimumSupportFinal;
            ViewBag.MinimumConfidenceFinal = minimumConfidenceFinal;

            ViewBag.QualifiedProductList = qualifiedProductList;
            ViewBag.OneItemOccurence = oneItemOccurence;

            ViewBag.QualifiedTwoItemCombinations = qualifiedTwoItemCombinations;
            ViewBag.TwoItemOccurence = twoItemOccurence;
            ViewBag.TwoItemCombinationConfidence = twoItemCombinationConfidence;

            ViewBag.QualifiedThreeItemCombinations = qualifiedThreeItemCombinations;
            ViewBag.ThreeItemOccurence = threeItemOccurence;
            ViewBag.ThreeItemCombinationConfidence = threeItemCombinationConfidence;

            return View();
        }
    }
}
