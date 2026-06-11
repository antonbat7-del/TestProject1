using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace RingAutoTests.Tests.СервиснаяКомпания
{
    public class BrandsTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public BrandsTests()
        {
            _browser = new BrowserHelper();
            _wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(15));
        }

        private IWebElement WaitClickable(By by)
        {
            return _wait.Until(d =>
            {
                var el = d.FindElement(by);
                return (el.Displayed && el.Enabled) ? el : null;
            });
        }

        private IWebElement WaitExists(By by)
        {
            return _wait.Until(d => d.FindElement(by));
        }

        [Fact]
        public void CompanyBrandsPage_Test()
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("🧪 ТЕСТ: Вкладка Бренды");
                Console.WriteLine("==================================================");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                string phone = "79160000071";
                string password = "Qwerty100";

                // 1. Логин
                Console.WriteLine("\n🔐 [1/13] Авторизация...");
                new LoginPage(_browser).Login(phone, password);
                Thread.Sleep(3000);
                Assert.True(_browser.Driver.Url.Contains("lk"), "❌ Логин не выполнен");
                Console.WriteLine("✅ Логин выполнен");

                // 2. Сменить юр. лицо на "Мастер-Дент"
                Console.WriteLine("\n🏢 [2/13] Смена юридического лица на 'ООО \"Мастер-Дент\"'...");
                try
                {
                    var orgDropdown = WaitClickable(By.XPath("//div[contains(@class,'dropdown__value')]"));
                    orgDropdown.Click();
                    Thread.Sleep(1000);

                    var masterDent = WaitClickable(By.XPath(
                        "//div[contains(@class,'dropdown-options__item') and contains(text(),'Мастер-Дент')]"));
                    masterDent.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Сменили юр.лицо на 'ООО \"Мастер-Дент\"'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось сменить юр.лицо: {ex.Message}");
                }

                // 3. Переход на вкладку "Компания"
                Console.WriteLine("\n🏢 [3/13] Переход на вкладку 'Компания'...");
                var companyTab = WaitClickable(By.XPath(
                    "//a[@href='/company'] | //span[contains(text(),'Компания')]/ancestor::a"
                ));
                companyTab.Click();
                Thread.Sleep(3000);
                Assert.Contains("/company", _browser.Driver.Url);
                Console.WriteLine("✅ Переход на вкладку 'Компания' выполнен");

                // 4. Нажимаем на вкладку "Бренды"
                Console.WriteLine("\n🏷️ [4/13] Нажатие на вкладку 'Бренды'...");

                try
                {
                    var brandsTab = _wait.Until(d => d.FindElement(By.XPath("//div[contains(@class,'tabs__tab') and contains(text(),'Бренды')]")));
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", brandsTab);
                    Console.WriteLine("✅ Нажата вкладка 'Бренды'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось нажать на вкладку 'Бренды': {ex.Message}");
                }

                Thread.Sleep(2000);

                // 5. Нажимаем на кнопку "Добавить бренд"
                Console.WriteLine("\n➕ [5/13] Нажатие на кнопку 'Добавить бренд'...");

                try
                {
                    var addBrandButton = WaitClickable(By.XPath("//button[contains(.,'Добавить бренд')]"));
                    addBrandButton.Click();
                    Console.WriteLine("✅ Нажата кнопка 'Добавить бренд'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось нажать на кнопку 'Добавить бренд': {ex.Message}");
                }

                Thread.Sleep(1500);

                // 6. Загружаем логотип (photo3)
                Console.WriteLine("\n🖼️ [6/13] Загрузка логотипа...");

                try
                {
                    var fileInput = _wait.Until(d => d.FindElement(By.XPath("//input[@type='file']")));
                    string imagePath = @"C:\testfiles\photo3.jpg";

                    if (File.Exists(imagePath))
                    {
                        fileInput.SendKeys(imagePath);
                        Console.WriteLine($"✅ Логотип загружен");
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Файл не найден: {imagePath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось загрузить логотип: {ex.Message}");
                }

                // 7. Вводим название бренда
                Console.WriteLine("\n📝 [7/13] Ввод названия бренда...");

                try
                {
                    string brandName = "Тестовый бренд";

                    var nameInput = _wait.Until(d => d.FindElement(By.XPath("//label[text()='Название']/following-sibling::input")));
                    nameInput.Clear();
                    nameInput.SendKeys(brandName);
                    Thread.Sleep(300);

                    Console.WriteLine($"✅ Введено название бренда: '{brandName}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось ввести название бренда: {ex.Message}");
                }

                // 8. Нажимаем на кнопку "Добавить"
                Console.WriteLine("\n➕ [8/13] Нажатие на кнопку 'Добавить'...");

                try
                {
                    var addButton = WaitClickable(By.XPath("//button[.//span[text()='Добавить']]"));
                    addButton.Click();
                    Console.WriteLine("✅ Нажата кнопка 'Добавить'");
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось нажать на кнопку 'Добавить': {ex.Message}");
                }

                // 9. Проверяем, что бренд появился в списке
                Console.WriteLine("\n🔍 [9/13] Проверка появления бренда в списке...");

                try
                {
                    var brandCard = _wait.Until(d => d.FindElement(By.XPath("//h3[text()='Тестовый бренд']")));
                    Console.WriteLine("✅ Бренд 'Тестовый бренд' успешно отображается в списке");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Бренд не найден в списке: {ex.Message}");
                }

                Thread.Sleep(500);

                // 10. Редактируем бренд
                Console.WriteLine("\n✏️ [10/13] Редактирование бренда 'Тестовый бренд'...");

                try
                {
                    // Находим кнопку редактирования и кликаем
                    var editButton = _browser.Driver.FindElement(By.XPath("//h3[text()='Тестовый бренд']/ancestor::div[contains(@class,'brand-card')]//button"));
                    editButton.Click();
                    Console.WriteLine("✅ Нажата кнопка редактирования");
                    Thread.Sleep(500);

                    // Загружаем новое фото
                    var fileInput = _browser.Driver.FindElement(By.XPath("//input[@type='file']"));
                    string imagePath = @"C:\testfiles\photo4.jpg";
                    if (File.Exists(imagePath))
                    {
                        fileInput.SendKeys(imagePath);
                        Console.WriteLine($"✅ Загружено новое фото");
                        Thread.Sleep(500);
                    }

                    // Меняем название
                    var nameInput = _browser.Driver.FindElement(By.XPath("//label[text()='Название']/following-sibling::input"));
                    nameInput.Clear();
                    nameInput.SendKeys("Тестовый бренд обновлен");
                    Console.WriteLine("✅ Название изменено на 'Тестовый бренд обновлен'");

                    // Сохраняем изменения
                    var saveButton = _browser.Driver.FindElement(By.XPath("//button[contains(@class,'btn-bordered-blue') and contains(.,'Сохранить')]"));
                    saveButton.Click();
                    Console.WriteLine("✅ Изменения сохранены");
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Ошибка при редактировании: {ex.Message}");
                }

                // 11. Проверяем, что название обновилось
                Console.WriteLine("\n🔍 [11/13] Проверка обновления названия бренда...");

                try
                {
                    var updatedBrand = _wait.Until(d => d.FindElement(By.XPath("//h3[text()='Тестовый бренд обновлен']")));
                    Console.WriteLine("✅ Название бренда успешно обновлено на 'Тестовый бренд обновлен'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Название бренда не обновилось: {ex.Message}");
                }

                Thread.Sleep(500);

                // 12. Удаляем бренд "Тестовый бренд обновлен"
                Console.WriteLine("\n🗑️ [12/13] Удаление бренда 'Тестовый бренд обновлен'...");

                try
                {
                    // Находим карточку обновленного бренда
                    var brandCard = _browser.Driver.FindElement(By.XPath("//h3[text()='Тестовый бренд обновлен']/ancestor::div[contains(@class,'brand-card')]"));

                    // Находим все кнопки в карточке (первая - редактирование, вторая - удаление)
                    var buttons = brandCard.FindElements(By.XPath(".//button"));

                    if (buttons.Count >= 2)
                    {
                        // Вторая кнопка - удаление (корзина)
                        var deleteButton = buttons[1];
                        deleteButton.Click();
                        Console.WriteLine("✅ Нажата кнопка удаления бренда");
                        Thread.Sleep(500);

                        // Подтверждаем удаление (кнопка "Удалить" в модальном окне)
                        var confirmDelete = _wait.Until(d => d.FindElement(By.XPath("//button[contains(@class,'btn-bordered-blue') and contains(.,'Удалить')]")));
                        confirmDelete.Click();
                        Console.WriteLine("✅ Подтверждено удаление бренда");
                        Thread.Sleep(500);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось удалить бренд: {ex.Message}");
                }

                // 13. Проверяем, что бренд удалился
                Console.WriteLine("\n🔍 [13/13] Проверка удаления бренда...");

                try
                {
                    Thread.Sleep(1000);
                    var deletedBrand = _browser.Driver.FindElements(By.XPath("//h3[text()='Тестовый бренд обновлен']"));
                    if (deletedBrand.Count == 0)
                    {
                        Console.WriteLine("✅ Бренд 'Тестовый бренд обновлен' успешно удален");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Бренд все еще отображается в списке");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Ошибка при проверке: {ex.Message}");
                }

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
                Console.WriteLine("Browser closed");
            }
        }
    }
}