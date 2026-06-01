using Xunit;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System;
using System.Threading;
using SeleniumExtras.WaitHelpers;
using System.Collections.Generic;
using System.IO;

namespace RingAutoTests.Tests.Настройки
{
    public class ServiceCostTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public ServiceCostTests()
        {
            _browser = new BrowserHelper();
            _wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(15));
        }

        [Fact]
        public void Test_UploadServiceCostTemplates()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("🧪 ТЕСТ: Загрузка шаблонов стоимости");
                Console.WriteLine("==================================================");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                string phone = "79160000071";
                string password = "Qwerty100";

                string workFilePath = @"D:\тесты\Шаблон стоимости работ (9)-09-11-2026-12-59-05.xlsx";
                string materialFilePath = @"D:\тесты\Шаблон стоимости материалов (9)-08-11-2026-12-59-13.xlsx";

                // 1. Логин
                Console.WriteLine("\n🔐 [1/5] Авторизация...");
                new LoginPage(_browser).Login(phone, password);
                Thread.Sleep(3000);
                Assert.True(_browser.Driver.Url.Contains("lk"), "❌ Логин не выполнен");
                Console.WriteLine("✅ Логин выполнен");

                // 2. Сменить юр. лицо
                Console.WriteLine("\n🏢 [2/5] Смена юридического лица...");
                try
                {
                    var orgDropdown = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'dropdown__value')]")));
                    orgDropdown.Click();
                    Thread.Sleep(1000);
                    var masterDent = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'dropdown-options__item') and contains(text(),'Мастер-Дент')]")));
                    masterDent.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Сменили юр.лицо");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ {ex.Message}");
                }

                // 3. Переход в Настройки
                Console.WriteLine("\n⚙️ [3/5] Переход в 'Настройки'...");
                var settingsLink = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[@href='/settings']")));
                settingsLink.Click();
                Thread.Sleep(3000);
                Assert.Contains("/settings", _browser.Driver.Url);
                Console.WriteLine("✅ Переход в Настройки");

                // 4. Переход на вкладку "Стоимость заявок"
                Console.WriteLine("\n📋 [4/5] Переход на вкладку 'Стоимость заявок'...");
                var serviceCostTab = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'tabs__tab') and contains(.,'Стоимость заявок')]")));
                serviceCostTab.Click();
                Thread.Sleep(3000);
                Console.WriteLine("✅ Переход на вкладку");

                // ========== 5. ЗАГРУЗКА ФАЙЛА "СПИСОК РАБОТ" ==========
                Console.WriteLine("\n📤 [5/6] ЗАГРУЗКА ФАЙЛА 'СПИСОК РАБОТ'...");

                // Кликаем по трём точкам
                var dotsWork = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("(//div[contains(@class,'menu-field')])[1]")));
                dotsWork.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Клик по трём точкам");

                // Выбираем пункт "Загрузить файл"
                var uploadWork = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'change-menu__item')]//p[text()='Загрузить файл']")));
                uploadWork.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Выбран пункт 'Загрузить файл'");

                // Загружаем файл (отправляем путь в input)
                var fileInput = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@type='file']")));
                fileInput.SendKeys(workFilePath);
                Thread.Sleep(3000);
                Console.WriteLine($"✅ Файл работ загружен: {Path.GetFileName(workFilePath)}");

                // ========== 6. ЗАГРУЗКА ФАЙЛА "СПИСОК МАТЕРИАЛОВ" ==========
                Console.WriteLine("\n📤 [6/6] ЗАГРУЗКА ФАЙЛА 'СПИСОК МАТЕРИАЛОВ'...");

                // Кликаем по вторым трём точкам
                var dotsMaterial = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("(//div[contains(@class,'menu-field')])[2]")));
                dotsMaterial.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Клик по трём точкам");

                // Выбираем пункт "Загрузить файл"
                var uploadMaterial = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'change-menu__item')]//p[text()='Загрузить файл']")));
                uploadMaterial.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Выбран пункт 'Загрузить файл'");

                // Загружаем файл
                var fileInputs = _browser.Driver.FindElements(By.XPath("//input[@type='file']"));
                if (fileInputs.Count > 1)
                {
                    fileInputs[1].SendKeys(materialFilePath);
                }
                else
                {
                    fileInputs[0].SendKeys(materialFilePath);
                }
                Thread.Sleep(3000);
                Console.WriteLine($"✅ Файл материалов загружен: {Path.GetFileName(materialFilePath)}");

                Console.WriteLine("\n🎉 ТЕСТ ПРОЙДЕН");

                stopwatch.Stop();
                Console.WriteLine($"⏱️ Время: {stopwatch.Elapsed.Seconds} сек");
                Console.WriteLine("==================================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ОШИБКА: {ex.Message}");
                throw;
            }
            finally
            {
                _browser.Close();
            }
        }
    }
}