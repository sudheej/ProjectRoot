using System;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Root.Controllers;
using Root;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace RootTestCases
{
    [TestClass]
    public class TestCase1
    {
     

            [TestMethod]
            public void Index()
            {
                // Arrange
                HomeController controller = new HomeController();
                // Act
                ViewResult result = controller.Index() as ViewResult;
                // Assert
                Assert.IsNotNull(result);
            }

            [TestMethod]
            public void About()
            {
                // Arrange
                HomeController controller = new HomeController();
                // Act
                ViewResult result = controller.About() as ViewResult;
                // Assert
                Assert.AreEqual("Your application description page.", result.ViewBag.Message);
            }

            [TestMethod]
            public void Contact()
            {
                // Arrange
                HomeController controller = new HomeController();
                // Act
                ViewResult result = controller.Contact() as ViewResult;
                // Assert
                Assert.IsNotNull(result);
            }



        }
}
