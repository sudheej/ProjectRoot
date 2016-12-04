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
        public void HomeTabTest()
        {
            HomeController controller = new HomeController();
            ViewResult result = controller.Index() as ViewResult;
            Assert.AreEqual("About", result.ViewData);
           
        }
    }
}
