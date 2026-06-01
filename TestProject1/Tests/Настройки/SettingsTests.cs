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

namespace RingAutoTests.Tests.Настройки
{
    public class SettingsTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public SettingsTests()
        {
            _browser = new BrowserHelper();
            _wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(15));
        }

        [Fact]
        public void Test_CreateNotificationGroup()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("TEST STARTED - CreateNotificationGroup");
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // ========== УЧЁТНЫЕ ДАННЫЕ ==========
                string phone = "79160000071";
                string password = "Qwerty100";
                // ==================================

                string groupName = "Тестовое уведомление 123";

                // Очистить куки
                _browser.Driver.Manage().Cookies.DeleteAllCookies();
                Thread.Sleep(500);

                // 1. Логин
                new LoginPage(_browser).Login(phone, password);
                Thread.Sleep(3000);
                Assert.True(_browser.Driver.Url.Contains("lk"), "❌ Логин не выполнен");
                Console.WriteLine($"✅ Логин выполнен");

                // 2. Сменить юридическое лицо на "Мастер-Дент"
                try
                {
                    var orgDropdown = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'dropdown__value')]")));
                    orgDropdown.Click();
                    Thread.Sleep(1000);
                    Console.WriteLine("✅ Открыт выпадающий список юр.лиц");

                    var masterDent = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'dropdown-options__item') and contains(text(),'Мастер-Дент')]")));
                    masterDent.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Сменили юр.лицо на 'ООО \"Мастер-Дент\"'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось сменить юр.лицо: {ex.Message}");
                }

                // 3. Переход в раздел Настройки
                var settingsLink = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[@href='/settings']")));
                settingsLink.Click();
                Thread.Sleep(3000);
                Assert.Contains("/settings", _browser.Driver.Url);
                Console.WriteLine("✅ Переход в раздел 'Настройки'");

                // 4. Нажать кнопку "Создать группу уведомлений"
                var createGroupBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(.,'Создать группу уведомлений')]")));
                createGroupBtn.Click();
                Thread.Sleep(2000);
                Console.WriteLine("✅ Нажата кнопка 'Создать группу уведомлений'");

                // 5. Ввести название группы
                var groupNameInput = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//input[@class='input name']")));
                groupNameInput.Clear();
                groupNameInput.SendKeys(groupName);
                Thread.Sleep(500);
                Console.WriteLine($"✅ Введено название группы: {groupName}");

                // 6. Сохранить (клик по иконке сохранения)
                var saveIcon = _browser.Driver.FindElement(By.XPath("//div[contains(@class,'controls')]/div[contains(@class,'icon')][1]"));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", saveIcon);
                Console.WriteLine("✅ Нажата иконка сохранения");

                // Ждём появления кнопки "Назад" (увеличенный таймаут)
                var waitForBack = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(30));
                var backButton = waitForBack.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(.,'Назад')]")));
                Console.WriteLine("✅ Появилась кнопка 'Назад'");

                // 7. Нажать кнопку "Назад"
                backButton.Click();
                Thread.Sleep(500);
                Console.WriteLine("✅ Нажата кнопка 'Назад'");

                // 8. Ждём загрузки страницы со списком групп
                Thread.Sleep(2000);
                Console.WriteLine("✅ Ожидание загрузки списка групп");

                // ========== НОВАЯ ПРОВЕРКА 1: группа отображается в списке ==========
                Console.WriteLine("\n=== ПРОВЕРКА: Группа отображается в списке ===");
                var createdGroup = _browser.Driver.FindElements(By.XPath($"//span[@class='name' and text()='{groupName}']"));
                Assert.True(createdGroup.Count > 0, $"❌ Группа '{groupName}' не отображается в списке после создания");
                Console.WriteLine($"✅ Группа '{groupName}' успешно создана и отображается в списке");

                // 9. Удалить созданную группу (с наведением мыши)
                Console.WriteLine("\n=== УДАЛЕНИЕ ГРУППЫ ===");
                try
                {
                    // Находим строку с нашей группой по span с классом name
                    var groups = _browser.Driver.FindElements(By.XPath($"//span[@class='name' and text()='{groupName}']"));

                    if (groups.Count > 0)
                    {
                        // Находим родительский элемент item
                        var groupRow = groups[0].FindElement(By.XPath("./ancestor::div[contains(@class,'item')]"));

                        // НАВОДИМ МЫШЬ на строку с группой (чтобы появились иконки)
                        var actions = new OpenQA.Selenium.Interactions.Actions(_browser.Driver);
                        actions.MoveToElement(groupRow).Perform();
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Наведены мышь на строку группы");

                        // Теперь ищем иконку удаления (корзину) - она должна стать видимой
                        var deleteIcon = groupRow.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'trash')]"));

                        // Клик по иконке удаления
                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", deleteIcon);
                        Thread.Sleep(500);
                        Console.WriteLine($"✅ Нажата иконка удаления для группы '{groupName}'");

                        // Подтверждение удаления (кнопка "Удалить")
                        var confirmDelete = _wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.XPath("//button[contains(@class,'btn-red') and contains(.,'Удалить')]")));
                        confirmDelete.Click();
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Подтверждено удаление");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Группа '{groupName}' не найдена");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Ошибка при удалении: {ex.Message}");
                }

                // ========== НОВАЯ ПРОВЕРКА 2: группа реально удалилась ==========
                Console.WriteLine("\n=== ПРОВЕРКА: Группа удалилась ===");
                Thread.Sleep(1000);
                var deletedGroup = _browser.Driver.FindElements(By.XPath($"//span[@class='name' and text()='{groupName}']"));
                Assert.True(deletedGroup.Count == 0, $"❌ Группа '{groupName}' не удалилась и всё ещё отображается в списке");
                Console.WriteLine($"✅ Группа '{groupName}' успешно удалена");

                stopwatch.Stop();
                Console.WriteLine($"\n✅ Время выполнения теста: {stopwatch.Elapsed.Seconds} секунд");
                Console.WriteLine("🎉 TEST COMPLETED SUCCESSFULLY!");
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