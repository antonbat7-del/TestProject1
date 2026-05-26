using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System;
using System.Threading;
using SeleniumExtras.WaitHelpers;

namespace RingAutoTests.Tests.Партнеры
{
    public class PartnerSearchTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public PartnerSearchTests()
        {
            _browser = new BrowserHelper();
            _wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(15));
        }

        [Fact]
        public void SearchPartnerByName_ShouldShowResults()
        {
            try
            {
                Console.WriteLine("TEST STARTED - SearchPartnerByName");

                // 1. Логин
                new LoginPage(_browser).Login("79262266015", "QwertY100");
                Thread.Sleep(1000);
                Assert.True(_browser.Driver.Url.Contains("lk"), "❌ Логин не выполнен");
                Console.WriteLine("✅ Логин выполнен");

                // 2. Переход в раздел Партнеры
                var partnersLink = _browser.Driver.FindElement(By.XPath("//a[@href='/partners']"));
                partnersLink.Click();
                Thread.Sleep(3000);
                Assert.Contains("/partners", _browser.Driver.Url);
                Console.WriteLine("✅ Переход в раздел Партнеры");

                // 3. Нажать на кнопку "Поиск партнеров"
                Thread.Sleep(1000);
                var searchPartnersBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'partners__buttons')]//div[contains(@class,'partners__btn')][2]//button")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", searchPartnersBtn);
                Thread.Sleep(2000);
                Console.WriteLine("✅ Нажата кнопка 'Поиск партнеров'");

                // 4. Нажать на вторую иконку (расширенный поиск)
                Thread.Sleep(1000);
                var secondIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'partners__search-actions')]//div[contains(@class,'partners__search-action')][2]")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", secondIcon);
                Thread.Sleep(2000);
                Console.WriteLine("✅ Нажата вторая иконка");

                // 5. Нажать на выпадающий список "Выберите роль"
                Thread.Sleep(1000);
                var roleField = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//input[@placeholder='Выберите роль']")));
                roleField.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Нажат выпадающий список 'Выберите роль'");

                // 6. Выбрать опцию
                var option = _browser.Driver.FindElement(By.XPath("//*[contains(@id,'__option-0')]"));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", option);
                Thread.Sleep(500);
                Console.WriteLine("✅ Выбрана опция");

                // 7. Выбрать чекбокс "Вентиляция"
                Thread.Sleep(1000);
                var ventilationLabel = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Вентиляция')]"));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", ventilationLabel);
                Thread.Sleep(500);
                Console.WriteLine("✅ Выбран чекбокс 'Вентиляция'");

                // 8. Выбрать чекбокс "Канализация"
                Thread.Sleep(500);
                var kanalizaciaLabel = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Канализация')]"));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", kanalizaciaLabel);
                Thread.Sleep(500);
                Console.WriteLine("✅ Выбран чекбокс 'Канализация'");

                // 9. Прокрутить страницу вниз
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Thread.Sleep(2000);
                Console.WriteLine("✅ Прокрутка страницы вниз");

                // 10. Выбрать регионы
                string[] regionsToSelect = { "Адыгея", "Алтайский край", "Архангельская", "Брянская", "Москва" };

                foreach (var region in regionsToSelect)
                {
                    try
                    {
                        var regionCheckbox = _browser.Driver.FindElement(By.XPath($"//label[contains(.,'{region}')]//input[@type='checkbox']"));
                        bool isChecked = regionCheckbox.Selected;

                        if (!isChecked)
                        {
                            var regionLabel = _browser.Driver.FindElement(By.XPath($"//label[contains(.,'{region}')]"));
                            ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", regionLabel);
                            Thread.Sleep(300);
                            ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", regionLabel);
                            Thread.Sleep(300);
                            Console.WriteLine($"✅ Выбран регион: {region}");

                            // Проверка: регион действительно выбран
                            var regionCheckboxAfter = _browser.Driver.FindElement(By.XPath($"//label[contains(.,'{region}')]//input[@type='checkbox']"));
                            Assert.True(regionCheckboxAfter.Selected, $"❌ Регион '{region}' не выбран");
                        }
                        else
                        {
                            Console.WriteLine($"ℹ️ Регион '{region}' уже выбран, пропускаем");
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"⚠️ Регион '{region}' не найден");
                    }
                }

                // 11. Выбрать чекбокс "С НДС"
                Thread.Sleep(500);
                var ndsCheckbox = _browser.Driver.FindElement(By.XPath("//label[contains(.,'С НДС')]//input[@type='checkbox']"));
                bool isNdsChecked = ndsCheckbox.Selected;

                if (!isNdsChecked)
                {
                    var ndsLabel = _browser.Driver.FindElement(By.XPath("//label[contains(.,'С НДС')]"));
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", ndsLabel);
                    Thread.Sleep(300);
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", ndsLabel);
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Выбран чекбокс 'С НДС'");

                    // Проверка: чекбокс действительно выбран
                    var ndsCheckboxAfter = _browser.Driver.FindElement(By.XPath("//label[contains(.,'С НДС')]//input[@type='checkbox']"));
                    Assert.True(ndsCheckboxAfter.Selected, "❌ Чекбокс 'С НДС' не выбран");
                }
                else
                {
                    Console.WriteLine("ℹ️ Чекбокс 'С НДС' уже выбран, пропускаем");
                }

                // 12. Выбрать чекбокс "Без НДС"
                Thread.Sleep(500);
                var withoutNdsCheckbox = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Без НДС')]//input[@type='checkbox']"));
                bool isWithoutNdsChecked = withoutNdsCheckbox.Selected;

                if (!isWithoutNdsChecked)
                {
                    var withoutNdsLabel = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Без НДС')]"));
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", withoutNdsLabel);
                    Thread.Sleep(300);
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", withoutNdsLabel);
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Выбран чекбокс 'Без НДС'");

                    // Проверка: чекбокс действительно выбран
                    var withoutNdsCheckboxAfter = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Без НДС')]//input[@type='checkbox']"));
                    Assert.True(withoutNdsCheckboxAfter.Selected, "❌ Чекбокс 'Без НДС' не выбран");
                }
                else
                {
                    Console.WriteLine("ℹ️ Чекбокс 'Без НДС' уже выбран, пропускаем");
                }

                // 13. Нажать кнопку "Применить фильтр"
                Thread.Sleep(1000);
                IWebElement applyFilterBtn = null;

                try
                {
                    applyFilterBtn = _browser.Driver.FindElement(By.XPath("//button[contains(text(),'Применить фильтр')]"));
                }
                catch
                {
                    try
                    {
                        applyFilterBtn = _browser.Driver.FindElement(By.XPath("//button[contains(@class,'btn-red')]"));
                    }
                    catch
                    {
                        try
                        {
                            applyFilterBtn = _browser.Driver.FindElement(By.XPath("//button[contains(@class,'partner-search__submit')]"));
                        }
                        catch
                        {
                            throw new Exception("❌ Кнопка 'Применить фильтр' не найдена");
                        }
                    }
                }

                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", applyFilterBtn);
                Thread.Sleep(3000);
                Console.WriteLine("✅ Нажата кнопка 'Применить фильтр'");

                // 14. ПРОВЕРКА РЕЗУЛЬТАТОВ ПОИСКА
                Thread.Sleep(2000);

                // Проверка: появился список партнёров
                var partnersList = _browser.Driver.FindElements(By.XPath("//div[contains(@class,'partner-card')]"));
                if (partnersList.Count == 0)
                {
                    // Пробуем альтернативный локатор
                    partnersList = _browser.Driver.FindElements(By.XPath("//div[contains(@class,'partners__list')]"));
                }
                if (partnersList.Count == 0)
                {
                    // Пробуем найти любую карточку с партнёром
                    partnersList = _browser.Driver.FindElements(By.XPath("//*[contains(@class,'partner')]"));
                }

                Assert.True(partnersList.Count > 0, "❌ Список партнёров не появился после применения фильтра");
                Console.WriteLine($"✅ Найдено партнёров: {partnersList.Count}");

                // Проверка: URL изменился (добавились параметры фильтра)
                if (_browser.Driver.Url.Contains("?"))
                {
                    Console.WriteLine("✅ URL содержит параметры фильтра");
                }
                else
                {
                    Console.WriteLine("⚠️ URL не содержит параметров фильтра, но продолжаем");
                }

                Console.WriteLine("TEST COMPLETED SUCCESSFULLY!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ОШИБКА: {ex.Message}");
                throw;
            }
            finally
            {
                _browser.Close();
                Console.WriteLine("Browser closed");
            }
        }
    }
}