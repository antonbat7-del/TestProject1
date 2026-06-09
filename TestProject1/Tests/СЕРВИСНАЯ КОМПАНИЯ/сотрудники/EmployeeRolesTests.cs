using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace RingAutoTests.Tests.СервиснаяКомпания
{
    public class EmployeeRolesTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public EmployeeRolesTests()
        {
            _browser = new BrowserHelper();
            _wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(15));
        }

        [Fact]
        public void Test_EditEmployeeRole()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("🧪 ТЕСТ: Проверка всех ролей сотрудника");
                Console.WriteLine("==================================================");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                string phone = "79160000071";
                string password = "Qwerty100";
                string employeeName = "Арбузов Геннадий Владимирович";

                // Список всех ролей для проверки
                List<string> rolesToTest = new List<string>
                {
                    "Администратор компании",
                    "Администратор юрлица",
                    "Сотрудник",
                    "Специалист собственного сервиса",
                    "Территориальный управляющий",
                    "Руководитель собственного сервиса"
                };

                // 1. Логин
                Console.WriteLine("\n🔐 [1/3] Авторизация...");
                new LoginPage(_browser).Login(phone, password);
                Thread.Sleep(3000);
                Assert.True(_browser.Driver.Url.Contains("lk"), "❌ Логин не выполнен");
                Console.WriteLine("✅ Логин выполнен");

                // 2. Сменить юр. лицо на "Мастер-Дент"
                Console.WriteLine("\n🏢 Смена юридического лица на 'ООО \"Мастер-Дент\"'...");
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

                // 3. Переход в "Сотрудники"
                Console.WriteLine("\n👥 Переход в 'Сотрудники'...");
                var staffLink = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[@href='/staff']")));
                staffLink.Click();
                Thread.Sleep(3000);
                Assert.Contains("/staff", _browser.Driver.Url);
                Console.WriteLine("✅ Переход выполнен");

                // 4. Таблица загружена
                Console.WriteLine("\n📊 Проверка таблицы...");
                var table = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//div[contains(@class,'staff__table')]//table")));
                Assert.True(table.Displayed, "❌ Таблица не загружена");
                Console.WriteLine("✅ Таблица загружена");

                // 5. Найти сотрудника
                Console.WriteLine($"\n🧑 Поиск сотрудника '{employeeName}'...");

                var allRows = _browser.Driver.FindElements(By.XPath("//div[contains(@class,'staff__table')]//tbody//tr"));
                IWebElement employeeRow = null;
                foreach (var row in allRows)
                {
                    if (row.Text.Contains(employeeName))
                    {
                        employeeRow = row;
                        Console.WriteLine("✅ Сотрудник найден");
                        break;
                    }
                }

                if (employeeRow == null)
                {
                    throw new Exception($"❌ Сотрудник '{employeeName}' не найден в таблице");
                }

                // Запоминаем старую роль для проверки
                var roleCells = employeeRow.FindElements(By.XPath(".//td"));
                string currentRole = roleCells.Count >= 5 ? roleCells[4].Text : "";
                Console.WriteLine($"📋 Текущая роль перед началом: '{currentRole}'");

                // 6. ЦИКЛ ПО ВСЕМ РОЛЯМ
                int roleIndex = 1;

                foreach (string newRole in rolesToTest)
                {
                    Console.WriteLine($"\n--------------------------------------------------");
                    Console.WriteLine($"🔄 [Шаг {roleIndex}/{rolesToTest.Count}] Проверка роли: '{newRole}'");
                    Console.WriteLine($"--------------------------------------------------");

                    // ЗАПОМИНАЕМ ТЕКУЩИЕ ДЕЙСТВИЯ (текст из staff__hover через textContent)
                    var currentActions = new List<string>();
                    try
                    {
                        var actionHovers = employeeRow.FindElements(By.XPath(".//div[contains(@class,'staff__hover')]"));
                        foreach (var hover in actionHovers)
                        {
                            string actionText = hover.GetAttribute("textContent");
                            if (string.IsNullOrWhiteSpace(actionText))
                            {
                                actionText = hover.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(actionText))
                            {
                                currentActions.Add(actionText.Trim());
                            }
                        }
                    }
                    catch { }

                    Console.WriteLine($"📋 ДЕЙСТВИЯ ДО изменения ({currentActions.Count}):");
                    foreach (var action in currentActions)
                    {
                        Console.WriteLine($"   - {action}");
                    }

                    // Открываем меню "Ещё"
                    var moreButton = employeeRow.FindElement(By.XPath(".//div[contains(@class,'staff__action') and contains(@class,'last')]"));
                    moreButton.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Меню 'Ещё' открыто");

                    // Выбираем пункт "Профиль"
                    var profileOption = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'staff__dropdown-item') and contains(text(),'Профиль')]")));
                    profileOption.Click();
                    Thread.Sleep(1000);
                    Console.WriteLine("✅ Форма профиля открыта");

                    // СНАЧАЛА МЕНЯЕМ РОЛЬ
                    Console.WriteLine($"\n🎭 Смена роли на '{newRole}'...");

                    var roleSelect = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'v-select__label') and contains(text(),'Роль')]/following-sibling::div//div[contains(@class,'vs__dropdown-toggle')]")));
                    roleSelect.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Выпадающий список ролей открыт");

                    var newRoleOption = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath($"//li[contains(@class,'vs__dropdown-option') and normalize-space()='{newRole}']")));
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", newRoleOption);
                    Thread.Sleep(300);
                    newRoleOption.Click();
                    Thread.Sleep(500);
                    Console.WriteLine($"✅ Выбрана роль '{newRole}'");

                    // ПРОВЕРЯЕМ, АКТИВНА ЛИ КНОПКА "СОХРАНИТЬ ИЗМЕНЕНИЯ"
                    bool isSaveButtonEnabled = false;
                    try
                    {
                        var saveButtonCheck = _browser.Driver.FindElement(By.XPath("//button[contains(@class,'btn-red') and contains(.,'Сохранить изменения')]"));
                        isSaveButtonEnabled = saveButtonCheck.Enabled && !saveButtonCheck.GetAttribute("class").Contains("disabled");
                    }
                    catch
                    {
                        isSaveButtonEnabled = false;
                    }

                    // Если кнопка НЕ активна → нужно внести изменения (вкладка "Права")
                    if (!isSaveButtonEnabled)
                    {
                        Console.WriteLine("\n🟢 Кнопка 'Сохранить изменения' неактивна → нужно внести изменения, идём на вкладку 'Права'...");

                        bool hasRightsTab = false;
                        try
                        {
                            var rightsTabCheck = _browser.Driver.FindElement(By.XPath("//div[contains(@class,'tabs__tab') and contains(text(),'Права')]"));
                            hasRightsTab = rightsTabCheck.Displayed;
                        }
                        catch
                        {
                            hasRightsTab = false;
                        }

                        if (hasRightsTab)
                        {
                            var rightsTab = _wait.Until(ExpectedConditions.ElementToBeClickable(
                                By.XPath("//div[contains(@class,'tabs__tab') and contains(text(),'Права')]")));
                            rightsTab.Click();
                            Thread.Sleep(500);
                            Console.WriteLine("✅ Вкладка 'Права' открыта");

                            var accordionIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                                By.XPath("//div[contains(@class,'accordion__title')]//*[name()='svg']")));
                            accordionIcon.Click();
                            Thread.Sleep(800);
                            Console.WriteLine("✅ Аккордеон раскрыт");

                            Console.WriteLine("\n☑️ Выбор чекбокса...");
                            try
                            {
                                var checkboxBox = _wait.Until(ExpectedConditions.ElementToBeClickable(
                                    By.XPath("//div[contains(@class,'list__item')]//span[contains(@class,'chckbox__box')]")));
                                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", checkboxBox);
                                Thread.Sleep(500);
                                Console.WriteLine("✅ Чекбокс выбран");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"⚠️ Не удалось выбрать чекбокс: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n⚠️ Вкладка 'Права' не найдена, пропускаем дополнительные действия");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\n🔴 Кнопка 'Сохранить изменения' активна → изменения уже есть, пропускаем вкладку 'Права'");
                    }

                    // Сохраняем изменения
                    Console.WriteLine("\n💾 Сохранение изменений...");
                    var saveButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//button[contains(@class,'btn-red') and contains(.,'Сохранить изменения')]")));
                    saveButton.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Изменения сохранены");

                    // Закрываем форму профиля
                    try
                    {
                        var closeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.XPath("//a[contains(@class,'modal__close')]")));
                        closeButton.Click();
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Форма профиля закрыта");
                    }
                    catch { }

                    // Проверяем, что роль изменилась в таблице
                    var updatedRows = _browser.Driver.FindElements(By.XPath("//div[contains(@class,'staff__table')]//tbody//tr"));
                    IWebElement updatedEmployeeRow = null;
                    foreach (var row in updatedRows)
                    {
                        if (row.Text.Contains(employeeName))
                        {
                            updatedEmployeeRow = row;
                            break;
                        }
                    }

                    var updatedRoleCells = updatedEmployeeRow.FindElements(By.XPath(".//td"));
                    string newActualRole = updatedRoleCells.Count >= 5 ? updatedRoleCells[4].Text : "";
                    Console.WriteLine($"📋 НОВАЯ роль в таблице: '{newActualRole}'");

                    Assert.Contains(newRole, newActualRole);
                    Console.WriteLine($"✅ Роль '{newRole}' успешно установлена");

                    // ЗАПОМИНАЕМ НОВЫЕ ДЕЙСТВИЯ ПОСЛЕ ИЗМЕНЕНИЯ
                    var newActions = new List<string>();
                    try
                    {
                        var newActionHovers = updatedEmployeeRow.FindElements(By.XPath(".//div[contains(@class,'staff__hover')]"));
                        foreach (var hover in newActionHovers)
                        {
                            string actionText = hover.GetAttribute("textContent");
                            if (string.IsNullOrWhiteSpace(actionText))
                            {
                                actionText = hover.Text;
                            }
                            if (!string.IsNullOrWhiteSpace(actionText))
                            {
                                newActions.Add(actionText.Trim());
                            }
                        }
                    }
                    catch { }

                    Console.WriteLine($"\n📋 ДЕЙСТВИЯ ПОСЛЕ изменения ({newActions.Count}):");
                    foreach (var action in newActions)
                    {
                        Console.WriteLine($"   - {action}");
                    }

                    // СРАВНИВАЕМ ДЕЙСТВИЯ ДО И ПОСЛЕ
                    bool actionsChanged = currentActions.Count != newActions.Count;
                    if (!actionsChanged)
                    {
                        for (int i = 0; i < currentActions.Count; i++)
                        {
                            if (currentActions[i] != newActions[i])
                            {
                                actionsChanged = true;
                                break;
                            }
                        }
                    }

                    if (actionsChanged)
                    {
                        Console.WriteLine("✅ Действия (Возможные действия) изменились после смены роли");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ ВНИМАНИЕ: Действия (Возможные действия) не изменились!");
                    }

                    // Обновляем employeeRow для следующей итерации
                    employeeRow = updatedEmployeeRow;
                    roleIndex++;
                }

                Console.WriteLine($"\n--------------------------------------------------");
                Console.WriteLine($"🎉 ТЕСТ ПРОЙДЕН — ВСЕ РОЛИ ПРОВЕРЕНЫ");
                Console.WriteLine($"--------------------------------------------------");

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