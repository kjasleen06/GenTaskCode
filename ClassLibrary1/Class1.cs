using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using System;

namespace ClassLibrary1
{
    public class Class1
    {
        PageObjects pageObjects;

        [SetUp]
        public void SetUp()
        {
            Log.SetupLogFile();

            PropertiesCollection.Driver = new ChromeDriver();
            pageObjects = new PageObjects();
            pageObjects.ReadJsonData();

            PropertiesCollection.Driver.Navigate().GoToUrl("https://automationintesting.online/");
            PropertiesCollection.Driver.Manage().Window.Maximize();
            PropertiesCollection.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMinutes(2);
        }

        [Test]
        public void TestMethod()
        {
            try
            {
                pageObjects.ClickOnBookThisRoom();
                pageObjects.NaviagateToCalender();
                pageObjects.PerformBookingOperation();
                pageObjects.EnterCustomerDetails();
                pageObjects.VerifySuccessMessage();
            }
            catch (Exception ex)
            {
                Log.Debug("Test case failed with issue: " + ex.ToString());
                throw;
            }
        }

        [TearDown]
        public void TearDown()
        {
            PropertiesCollection.Driver.Close();
        }
    }
}
