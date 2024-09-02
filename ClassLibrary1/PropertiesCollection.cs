using OpenQA.Selenium;

namespace ClassLibrary1
{
    internal class PropertiesCollection
    {
        public static IWebDriver Driver { get; set; }
        public static Customer Customer { get; set; }
    }
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
