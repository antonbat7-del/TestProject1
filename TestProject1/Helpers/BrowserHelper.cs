using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using SeleniumExtras.WaitHelpers;

namespace RingAutoTests.Helpers
{
    public class BrowserHelper
    {
        public IWebDriver Driver;
        public WebDriverWait Wait;

        public BrowserHelper()
        {
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            var options = new ChromeOptions();

            // Гостевой режим — без плашек Google
            options.AddArgument("--guest");
            options.AddArgument("--disable-notifications");

            // НАСТРОЙКИ АВТОЗАГРУЗКИ ФАЙЛОВ
            options.AddUserProfilePreference("download.default_directory", downloadPath);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("download.directory_upgrade", true);
            options.AddUserProfilePreference("safebrowsing.enabled", true);
            options.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);
            options.AddUserProfilePreference("safebrowsing.disable_download_protection", true);

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

        /// <summary>
        /// Загружает файл на сайт через input type="file"
        /// </summary>
        /// <param name="fileInputLocator">Локатор для input type="file"</param>
        /// <param name="filePath">Полный путь к файлу</param>
        public void UploadFile(By fileInputLocator, string filePath)
        {
            // Ждём появления input и отправляем путь
            var fileInput = Wait.Until(ExpectedConditions.ElementExists(fileInputLocator));
            fileInput.SendKeys(filePath);
        }

        public void Close()
        {
            Driver.Quit();
        }
    }
}