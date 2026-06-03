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
using OpenQA.Selenium.Interactions;

namespace RingAutoTests.Tests.СервиснаяКомпания
{
    public class NotificationsTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public NotificationsTests()
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
                Console.WriteLine("==================================================");
                Console.WriteLine("🧪 ТЕСТ: Создание группы уведомлений (Сервисная компания)");
                Console.WriteLine("==================================================");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                string phone = "79262266015";
                string password = "QwertY100";

                string groupName = "Тестовое уведомление";

                _browser.Driver.Manage().Cookies.DeleteAllCookies();
                Thread.Sleep(500);

                // 1. Логин
                new LoginPage(_browser).Login(phone, password);
                Thread.Sleep(3000);
                Assert.True(_browser.Driver.Url.Contains("lk"), "❌ Логин не выполнен");
                Console.WriteLine($"✅ Логин выполнен для аккаунта {phone}");

                // 2. Переход в раздел Настройки
                var settingsLink = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[@href='/settings']")));
                settingsLink.Click();
                Thread.Sleep(3000);
                Assert.Contains("/settings", _browser.Driver.Url);
                Console.WriteLine("✅ Переход в раздел 'Настройки'");

                // 3. Нажать кнопку "Создать группу уведомлений"
                var createGroupBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(.,'Создать группу уведомлений')]")));
                createGroupBtn.Click();
                Thread.Sleep(2000);
                Console.WriteLine("✅ Нажата кнопка 'Создать группу уведомлений'");

                // 4. Ввести название группы
                var groupNameInput = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//input[@class='input name']")));
                groupNameInput.Clear();
                groupNameInput.SendKeys(groupName);
                Thread.Sleep(500);
                Console.WriteLine($"✅ Введено название группы: {groupName}");

                // 5. Нажать иконку сохранения
                var saveIcon = _browser.Driver.FindElement(By.XPath("//div[contains(@class,'controls')]/div[contains(@class,'icon')][1]"));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", saveIcon);
                Console.WriteLine("✅ Нажата иконка сохранения");

                // Ждём появления кнопки "Назад"
                var waitForBack = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(30));
                var backButton = waitForBack.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(.,'Назад')]")));
                Console.WriteLine("✅ Появилась кнопка 'Назад'");

                // 6. Нажать кнопку "Назад"
                backButton.Click();
                Thread.Sleep(500);
                Console.WriteLine("✅ Нажата кнопка 'Назад'");

                // 7. Ждём загрузки страницы со списком групп
                Thread.Sleep(2000);
                Console.WriteLine("✅ Ожидание загрузки списка групп");

                // 8. Проверка: группа отображается в списке
                Console.WriteLine("\n=== ПРОВЕРКА: Группа отображается в списке ===");
                var createdGroup = _browser.Driver.FindElements(By.XPath($"//span[@class='name' and text()='{groupName}']"));
                Assert.True(createdGroup.Count > 0, $"❌ Группа '{groupName}' не отображается в списке");
                Console.WriteLine($"✅ Группа '{groupName}' успешно создана");

                // 9. Удалить созданную группу
                Console.WriteLine("\n=== УДАЛЕНИЕ ГРУППЫ ===");
                try
                {
                    var groups = _browser.Driver.FindElements(By.XPath($"//span[@class='name' and text()='{groupName}']"));

                    if (groups.Count > 0)
                    {
                        var groupRow = groups[0].FindElement(By.XPath("./ancestor::div[contains(@class,'item')]"));

                        var actions = new Actions(_browser.Driver);
                        actions.MoveToElement(groupRow).Perform();
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Наведены мышь на строку группы");

                        var deleteIcon = groupRow.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'trash')]"));
                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", deleteIcon);
                        Thread.Sleep(500);
                        Console.WriteLine($"✅ Нажата иконка удаления");

                        var confirmDelete = _wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.XPath("//button[contains(@class,'btn-red') and contains(.,'Удалить')]")));
                        confirmDelete.Click();
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Подтверждено удаление");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Ошибка при удалении: {ex.Message}");
                }

                // 10. Проверка: группа удалилась
                Console.WriteLine("\n=== ПРОВЕРКА: Группа удалилась ===");
                Thread.Sleep(1000);
                var deletedGroup = _browser.Driver.FindElements(By.XPath($"//span[@class='name' and text()='{groupName}']"));
                Assert.True(deletedGroup.Count == 0, $"❌ Группа '{groupName}' не удалилась");
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