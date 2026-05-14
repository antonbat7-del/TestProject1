using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace RingAutoTests.Helpers
{
    public class BrowserHelper
    {
        public IWebDriver Driver;
        public WebDriverWait Wait;

        public BrowserHelper()
        {
            var options = new ChromeOptions();

            // Гостевой режим — без плашек Google
            options.AddArgument("--guest");
            options.AddArgument("--disable-notifications");

            Driver = new ChromeDriver(options);
            Driver.Manage().Window.Maximize();
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
        }

        public void GoToUrl(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public IWebElement WaitVisible(By locator)
        {
            return Wait.Until(driver =>
            {
                var el = driver.FindElement(locator);
                return el.Displayed ? el : null;
            });
        }

        public IWebElement WaitClickable(By locator)
        {
            return Wait.Until(driver =>
            {
                var el = driver.FindElement(locator);
                return (el.Displayed && el.Enabled) ? el : null;
            });
        }

        public void Close()
        {
            Driver.Quit();
        }
    }
}