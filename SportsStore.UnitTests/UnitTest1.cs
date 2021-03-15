using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using Moq;
using SportsStore.Domain.Abstract; 
using SportsStore.Domain.Entities; 
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;
using System;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            // przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // działanie
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null,2).Model;

            // assercje
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });

            // utworzenie kontrolera i ustawienie 3-elementowej strony
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // działanie
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            // asercje
            PagingInfo pagingInfo = result.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);

        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            // przygotowanie - definiowanie metody pomocniczej HTML
            HtmlHelper myHelper = null;

            // przygotowanie - tworzenie danych PagingInfo
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // przygotwanie - konfigurowanie delegatu
            Func<int, string> pageUrlDelegate = i => "Strona" + i;

            // działanie
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);
            
            // asercje
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Strona1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Strona2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Strona3"">3</a>", result.ToString());
        }
        
        [TestMethod]
        public void Can_Filter_Products()
        {
            // przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // działanie
            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            // asercje
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
                new Product {ProductID = 2, Name = "P2", Category = "Jabłka"},
                new Product {ProductID = 3, Name = "P3", Category = "Śliwki"},
                new Product {ProductID = 4, Name = "P4", Category = "Pomarańcze"}
            });

            NavController target = new NavController(mock.Object);

            // działanie
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            // asercje
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Jabłka");
            Assert.AreEqual(results[1], "Pomarańcze");
            Assert.AreEqual(results[2], "Śliwki");
        }
    }
}
