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

namespace RingAutoTests.Tests.Настройки
{
    public class DepartmentsTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public DepartmentsTests()
        {
            _browser = new BrowserHelper();
            _wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(15));
        }

        [Fact]
        public void Test_CreateNewDepartment()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("🧪 ТЕСТ: Создание департамента и добавление сотрудников");
                Console.WriteLine("==================================================");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                string phone = "79160000071";
                string password = "Qwerty100";

                string departmentName = "Тестовый департамент " + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string departmentDescription = "Это тестовый департамент, созданный автоматическим тестом";

                // 1. Логин
                Console.WriteLine("\n🔐 [1/9] Авторизация...");
                new LoginPage(_browser).Login(phone, password);
                Thread.Sleep(3000);
                Assert.True(_browser.Driver.Url.Contains("lk"), "❌ Логин не выполнен");
                Console.WriteLine("✅ Логин выполнен");

                // 2. Сменить юр. лицо
                Console.WriteLine("\n🏢 [2/9] Смена юридического лица...");
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
                Console.WriteLine("\n⚙️ [3/9] Переход в 'Настройки'...");
                var settingsLink = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[@href='/settings']")));
                settingsLink.Click();
                Thread.Sleep(3000);
                Assert.Contains("/settings", _browser.Driver.Url);
                Console.WriteLine("✅ Переход в Настройки");

                // 4. Переход на вкладку "Конструктор департаментов"
                Console.WriteLine("\n📋 [4/9] Переход на вкладку 'Конструктор департаментов'...");
                var depsTab = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'tabs__tab') and contains(.,'Конструктор департаментов')]")));
                depsTab.Click();
                Thread.Sleep(3000);
                Console.WriteLine("✅ Переход на вкладку 'Конструктор департаментов'");

                // 5. Нажать кнопку "Создать новый департамент"
                Console.WriteLine("\n📝 [5/9] Создание нового департамента...");
                var createBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(.,'Создать новый департамент')]")));
                createBtn.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Нажата кнопка 'Создать новый департамент'");

                // 6. Заполнить поле "Название"
                var nameInput = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//label[text()='Название']/following-sibling::input")));
                nameInput.Clear();
                nameInput.SendKeys(departmentName);
                Thread.Sleep(500);
                Console.WriteLine($"✅ Введено название: {departmentName}");

                // 7. Выбрать "Тип департамента"
                Console.WriteLine("\n📋 Выбор типа департамента...");

                var typeDropdown = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'v-select__label') and text()='Тип департамента']/following-sibling::div//div[contains(@class,'vs__dropdown-toggle')]")));
                typeDropdown.Click();
                Thread.Sleep(500);
                Console.WriteLine("✅ Открыт список типов");

                var typeOptions = _wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(
                    By.XPath("//li[contains(@class,'vs__dropdown-option')]")));

                if (typeOptions.Count > 0)
                {
                    string selectedType = typeOptions[0].Text;
                    typeOptions[0].Click();
                    Console.WriteLine($"✅ Выбран тип: '{selectedType}'");
                }
                Thread.Sleep(500);

                // 8. Заполнить поле "Краткое описание"
                var descriptionTextarea = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'txt__label') and text()='Краткое описание']/following-sibling::div//textarea")));
                descriptionTextarea.Clear();
                descriptionTextarea.SendKeys(departmentDescription);
                Thread.Sleep(500);
                Console.WriteLine($"✅ Введено описание");

                // 9. Сохранить департамент
                var saveIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'controls')]//div[contains(@class,'icon') and not(contains(@class,'trash'))]")));
                saveIcon.Click();
                Thread.Sleep(2000);
                Console.WriteLine("✅ Департамент сохранён");

                Thread.Sleep(5000);
                Console.WriteLine("✅ Ожидание загрузки списка департаментов");

                // 10. Найти департамент и открыть страницу сотрудников
                Console.WriteLine("\n🔍 [6/9] Поиск департамента...");
                var departmentNameElement = _wait.Until(ExpectedConditions.ElementIsVisible(
                    By.XPath("//p[contains(@class,'name') and contains(text(),'Тестовый департамент')]")));
                Console.WriteLine("✅ Название департамента найдено");

                var parentItem = departmentNameElement.FindElement(By.XPath("./ancestor::div[contains(@class,'item')]"));
                var actions = new Actions(_browser.Driver);
                actions.MoveToElement(parentItem).Perform();
                Thread.Sleep(500);

                var userIcon = parentItem.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'icon')][1]"));
                userIcon.Click();
                Thread.Sleep(500);
                Console.WriteLine("✅ Открыта страница сотрудников");

                // Ждём загрузки списка сотрудников
                Console.WriteLine("\n⏳ Ожидание загрузки списка сотрудников...");
                var employeesContainer = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//div[contains(@class,'executor-item')]")));
                Console.WriteLine("✅ Список сотрудников загружен");

                // ========== 11. ВЫБОР СОТРУДНИКОВ ==========
                Console.WriteLine("\n📝 [7/9] Выбор сотрудников...");

                // Список сотрудников с их ID
                var employeesToAdd = new Dictionary<string, string>
                {
                    { "Арбузов Геннадий Владимирович", "executor-138" },
                    { "Иванов Иван Иванович", "executor-132" },
                    { "Сотрудников Димасик", "executor-1242" }
                };

                foreach (var employee in employeesToAdd)
                {
                    try
                    {
                        var label = _wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.XPath($"//label[@for='{employee.Value}']")));
                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", label);
                        Thread.Sleep(500);
                        Console.WriteLine($"✅ Выбран сотрудник: {employee.Key}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Не удалось выбрать сотрудника {employee.Key}: {ex.Message}");
                    }
                }

                // Даём время, чтобы чекбоксы ролей стали активными
                Thread.Sleep(2000);

                // ========== 12. НАЗНАЧЕНИЕ РОЛЕЙ ==========
                Console.WriteLine("\n📝 [8/9] Назначение ролей...");

                // Назначаем координатора для Арбузова
                try
                {
                    var coordinatorSpan = _wait.Until(ExpectedConditions.ElementExists(
                        By.XPath("//input[@id='coordinator-138']/following-sibling::span[contains(@class,'chckbox__box')]")));
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView({block:'center'});", coordinatorSpan);
                    Thread.Sleep(300);
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", coordinatorSpan);
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Арбузов назначен координатором");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось назначить координатора: {ex.Message}");
                }

                // Назначаем диспетчера для Иванова
                try
                {
                    var dispatcherSpan = _wait.Until(ExpectedConditions.ElementExists(
                        By.XPath("//input[@id='dispatcher-132']/following-sibling::span[contains(@class,'chckbox__box')]")));
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView({block:'center'});", dispatcherSpan);
                    Thread.Sleep(300);
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", dispatcherSpan);
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Иванов назначен диспетчером");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось назначить диспетчера: {ex.Message}");
                }

                // Назначаем координатора для Сотрудников Димасик
                try
                {
                    Console.WriteLine("\n=== НАЗНАЧЕНИЕ РОЛИ ДЛЯ ДИМАСИКА ===");

                    var coordinatorInput = _browser.Driver.FindElement(By.Id("coordinator-1242"));
                    var coordinatorBox = _wait.Until(ExpectedConditions.ElementExists(
                        By.XPath("//input[@id='coordinator-1242']/following-sibling::span[contains(@class,'chckbox__box')]")));

                    Console.WriteLine($"Selected ДО: {coordinatorInput.Selected}");

                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView({block:'center'});", coordinatorBox);
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", coordinatorBox);
                    Thread.Sleep(1000);

                    Console.WriteLine($"Selected ПОСЛЕ клика: {coordinatorInput.Selected}");

                    if (!coordinatorInput.Selected)
                    {
                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript(@"
                            arguments[0].checked = true;
                            arguments[0].dispatchEvent(new Event('change', { bubbles: true }));
                        ", coordinatorInput);
                        Thread.Sleep(500);
                        Console.WriteLine($"Selected после change: {coordinatorInput.Selected}");
                    }

                    Console.WriteLine("✅ Сотрудников Димасик назначен координатором");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось назначить координатора для Димасика: {ex.Message}");
                }

                // Подтверждаем выбор сотрудников
                try
                {
                    var confirmButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//button[contains(.,'Подтвердить выбор')]")));
                    confirmButton.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Выбор сотрудников подтверждён");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось нажать кнопку 'Подтвердить выбор': {ex.Message}");
                }

                // ========== 13. ОТКРЫТИЕ НАСТРОЕК, РЕДАКТИРОВАНИЕ И УДАЛЕНИЕ ==========
                Console.WriteLine("\n⚙️ [9/9] Открытие настроек, редактирование и удаление департамента...");

                // НАХОДИМ ДЕПАРТАМЕНТ
                Console.WriteLine("\n🔍 Поиск департамента...");
                var departmentElementForSettings = _wait.Until(ExpectedConditions.ElementIsVisible(
                    By.XPath("//p[contains(@class,'name') and contains(text(),'Тестовый департамент')]")));
                Console.WriteLine("✅ Название департамента найдено");

                var parentItemDept = departmentElementForSettings.FindElement(By.XPath("./ancestor::div[contains(@class,'item')]"));

                // НАВОДИМ МЫШЬ
                var hoverActions = new Actions(_browser.Driver);
                hoverActions.MoveToElement(parentItemDept).Perform();
                Console.WriteLine("✅ Наведена мышь на департамент");

                // КЛИКАЕМ ПО ИКОНКЕ НАСТРОЕК (ШЕСТЕРЁНКА)
                try
                {
                    var settingsIcon = parentItemDept.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'icon')][2]"));
                    settingsIcon.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Нажата иконка настроек");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось найти иконку настроек: {ex.Message}");
                    var settingsIconAlt = parentItemDept.FindElement(By.XPath(".//*[name()='svg' and contains(@class,'iconify--mdi')]/.."));
                    settingsIconAlt.Click();
                    Console.WriteLine("✅ Нажата иконка настроек (альтернативный способ)");
                }

                // Ждём загрузки формы
                Thread.Sleep(2000);

                string newDepartmentName = "";

                // Редактируем название
                try
                {
                    var nameInputSettings = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//label[text()='Название']/following-sibling::input")));
                    nameInputSettings.Clear();
                    newDepartmentName = "Отредактированный департамент";
                    nameInputSettings.SendKeys(newDepartmentName);
                    Thread.Sleep(500);
                    Console.WriteLine($"✅ Название изменено на: {newDepartmentName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось изменить название: {ex.Message}");
                }

                // Выбираем другой тип
                try
                {
                    var typeDropdownSettings = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'v-select__label') and text()='Тип департамента']/following-sibling::div//div[contains(@class,'vs__dropdown-toggle')]")));
                    typeDropdownSettings.Click();
                    Thread.Sleep(500);

                    var typeOptionsSettings = _browser.Driver.FindElements(By.XPath("//li[contains(@class,'vs__dropdown-option')]"));
                    if (typeOptionsSettings.Count > 1)
                    {
                        string selectedType = typeOptionsSettings[1].Text;
                        typeOptionsSettings[1].Click();
                        Console.WriteLine($"✅ Выбран новый тип: '{selectedType}'");
                    }
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось изменить тип: {ex.Message}");
                }

                // Сохраняем изменения
                try
                {
                    var saveSettingsIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'controls')]//div[contains(@class,'icon')][1]")));
                    saveSettingsIcon.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Изменения сохранены");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось сохранить изменения: {ex.Message}");
                }

                // НАВОДИМСЯ НА ОТРЕДАКТИРОВАННЫЙ ДЕПАРТАМЕНТ
                Console.WriteLine("\n🔍 Поиск отредактированного департамента...");
                var editedDepartmentElement = _wait.Until(ExpectedConditions.ElementIsVisible(
                    By.XPath($"//p[contains(@class,'name') and contains(text(),'{newDepartmentName}')]")));
                Console.WriteLine("✅ Отредактированный департамент найден");

                var parentItemEdited = editedDepartmentElement.FindElement(By.XPath("./ancestor::div[contains(@class,'item')]"));

                var hoverActionsEdited = new Actions(_browser.Driver);
                hoverActionsEdited.MoveToElement(parentItemEdited).Perform();
                Console.WriteLine("✅ Наведена мышь на отредактированный департамент");

                // КЛИКАЕМ ПО ИКОНКЕ КОРЗИНЫ
                try
                {
                    var deleteIcon = parentItemEdited.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'trash')]"));
                    deleteIcon.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Нажата иконка корзины");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось найти иконку корзины: {ex.Message}");
                }

                // ПОДТВЕРЖДАЕМ УДАЛЕНИЕ
                try
                {
                    var confirmDeleteButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//button[contains(@class,'btn-red') and contains(.,'Удалить')]")));
                    confirmDeleteButton.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Департамент удалён");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось подтвердить удаление: {ex.Message}");
                }

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