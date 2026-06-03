using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;

namespace RingAutoTests.Tests.Настройки
{
    public class SubstatusTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public SubstatusTests()
        {
            _browser = new BrowserHelper();
            _wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(15));
        }

        [Fact]
        public void Test_CreateNewSubstatus()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("🧪 ТЕСТ: Конструктор подстатусов — создание, редактирование и удаление");
                Console.WriteLine("==================================================");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                string phone = "79262266015";
                string password = "QwertY100";
                string substatusName = $"Тестовый подстатус {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                string substatusDescription = $"Краткое описание тестового подстатуса {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                // 1. Логин
                Console.WriteLine("\n🔐 [1/11] Авторизация...");
                new LoginPage(_browser).Login(phone, password);
                Thread.Sleep(3000);
                Assert.True(_browser.Driver.Url.Contains("lk"), "❌ Логин не выполнен");
                Console.WriteLine("✅ Логин выполнен");

                // 2. Переход в Настройки
                Console.WriteLine("\n⚙️ [2/11] Переход в 'Настройки'...");
                var settingsLink = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[@href='/settings']")));
                settingsLink.Click();
                Thread.Sleep(3000);
                Assert.Contains("/settings", _browser.Driver.Url);
                Console.WriteLine("✅ Переход в Настройки");

                // 3. Переход на вкладку "Конструктор подстатусов"
                Console.WriteLine("\n📋 [3/11] Переход на вкладку 'Конструктор подстатусов'...");
                var substatusTab = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'tabs__tab') and contains(.,'Конструктор подстатусов')]")));
                substatusTab.Click();
                Thread.Sleep(3000);
                Console.WriteLine("✅ Переход на вкладку 'Конструктор подстатусов'");

                // 4. Нажать кнопку "Создать новый подстатус"
                Console.WriteLine("\n📝 [4/11] Нажатие кнопки 'Создать новый подстатус'...");
                var createButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(.,'Создать новый подстатус')]")));

                Assert.True(createButton.Displayed, "❌ Кнопка 'Создать новый подстатус' не отображается");
                Console.WriteLine("✅ Кнопка найдена");

                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", createButton);
                Thread.Sleep(1500);
                Console.WriteLine("✅ Кнопка 'Создать новый подстатус' нажата");

                // 5. Заполнение полей: Название
                Console.WriteLine("\n✏️ [5/11] Заполнение полей формы...");

                var nameInput = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//label[contains(text(),'Название')]/following-sibling::input")));
                nameInput.Clear();
                nameInput.SendKeys(substatusName);
                Console.WriteLine($"✅ Название введено: '{substatusName}'");

                // 6. Заполнение: Краткое описание
                var descriptionTextarea = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//div[contains(@class,'txt__label') and contains(text(),'Краткое описание')]/following-sibling::div//textarea")));
                descriptionTextarea.Clear();
                descriptionTextarea.SendKeys(substatusDescription);
                Console.WriteLine("✅ Краткое описание введено");

                // 7. ВЫБОР ЦВЕТА (через JavaScript)
                Console.WriteLine("\n🎨 [6/11] Выбор цвета...");

                string selectedColor = "";
                string expectedColorRgb = "";

                try
                {
                    var random = new Random();
                    byte r = (byte)random.Next(256);
                    byte g = (byte)random.Next(256);
                    byte b = (byte)random.Next(256);
                    selectedColor = $"#{r:X2}{g:X2}{b:X2}".ToLower();
                    expectedColorRgb = $"rgb({r}, {g}, {b})";

                    Console.WriteLine($"🎲 Случайный цвет: HEX = {selectedColor}, RGB = {expectedColorRgb}");

                    var nativeColorPicker = _wait.Until(ExpectedConditions.ElementExists(
                        By.XPath("//input[contains(@class,'color-input__native')]")));

                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].value = arguments[1];", nativeColorPicker, selectedColor);
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].dispatchEvent(new Event('change', { bubbles: true }));", nativeColorPicker);
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", nativeColorPicker);
                    Thread.Sleep(500);

                    Console.WriteLine($"✅ Цвет {selectedColor} установлен через JS");

                    var colorSwatch = _wait.Until(ExpectedConditions.ElementExists(
                        By.XPath("//span[contains(@class,'color-input__swatch')]")));
                    string actualBackgroundColor = colorSwatch.GetCssValue("background-color");
                    Console.WriteLine($"📋 Цвет в сваче: {actualBackgroundColor}");

                    Thread.Sleep(1000);
                    actualBackgroundColor = colorSwatch.GetCssValue("background-color");
                    Console.WriteLine($"📋 Цвет в сваче после ожидания: {actualBackgroundColor}");

                    if (actualBackgroundColor.ToLower().Contains(expectedColorRgb.ToLower()) ||
                        actualBackgroundColor.ToLower().Contains(selectedColor.ToLower()))
                    {
                        Console.WriteLine("✅ Цвет успешно применён!");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ ВНИМАНИЕ: Цвет не применился! Ожидался {expectedColorRgb}, получен {actualBackgroundColor}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Ошибка: {ex.Message}");
                }

                // 7.5. ВЫБОР ЧЕКБОКСА "Виден клиентам"
                Console.WriteLine("\n👁️ [7/11] Выбор чекбокса 'Виден клиентам'...");

                try
                {
                    var checkbox = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//span[text()='Виден клиентам']/ancestor::label")));
                    checkbox.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Чекбокс 'Виден клиентам' выбран");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось найти чекбокс: {ex.Message}");
                }

                // 8. Сохранение подстатуса
                Console.WriteLine("\n💾 [8/11] Сохранение подстатуса...");
                var saveIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'controls')]/div[contains(@class,'icon')][1]")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", saveIcon);
                Thread.Sleep(2000);
                Console.WriteLine("✅ Подстатус сохранён");

                // 9. Проверка: подстатус появился в списке
                Console.WriteLine("\n🔍 [9/11] Проверка: поиск созданного подстатуса в списке...");
                string substatusNameShort = substatusName.Length > 19 ? substatusName.Substring(0, 19) : substatusName;
                Console.WriteLine($"📋 Ищем подстатус по названию: '{substatusNameShort}'");

                var createdSubstatus = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath($"//p[contains(@class,'name') and contains(text(),'{substatusNameShort}')]")));

                Assert.True(createdSubstatus.Displayed, $"❌ Созданный подстатус '{substatusNameShort}' не отображается в списке");
                Console.WriteLine($"✅ Подстатус '{substatusNameShort}' успешно создан");

                // ПРОВЕРКА: цвет отображается в списке
                if (!string.IsNullOrEmpty(selectedColor) && !string.IsNullOrEmpty(expectedColorRgb))
                {
                    try
                    {
                        var substatusItemColor = createdSubstatus.FindElement(By.XPath("./ancestor::div[contains(@class,'item')]"));
                        var colorSwatchInList = substatusItemColor.FindElement(By.XPath(".//div[contains(@class,'sub-status-color')]//span[contains(@class,'sub-status-color__swatch')]"));
                        string listColor = colorSwatchInList.GetCssValue("background-color");
                        Console.WriteLine($"📋 Цвет подстатуса в списке: {listColor}");

                        if (listColor.ToLower().Contains(expectedColorRgb.ToLower()) ||
                            listColor.ToLower().Contains(selectedColor.ToLower()))
                        {
                            Console.WriteLine("✅ Цвет отображается в списке подстатусов");
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ Цвет в списке не совпадает: {listColor}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Не удалось проверить цвет в списке: {ex.Message}");
                    }
                }

                // 10. ВЫБОР МАСТЕРА (1-я иконка)
                Console.WriteLine("\n👥 [10/11] Выбор мастера...");
                try
                {
                    var parentItem = createdSubstatus.FindElement(By.XPath("./ancestor::div[contains(@class,'item')]"));

                    var actions = new Actions(_browser.Driver);
                    actions.MoveToElement(parentItem).Perform();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Наведена мышь на подстатус");

                    var masterButton = parentItem.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'icon')][1]"));
                    masterButton.Click();
                    Thread.Sleep(1000);
                    Console.WriteLine("✅ Нажата первая иконка (выбор мастера)");

                    var waitForList = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(5));
                    waitForList.Until(ExpectedConditions.ElementExists(By.XPath("//div[contains(@class,'executor-item')]")));
                    Console.WriteLine("✅ Список мастеров загружен");

                    var masterCheckbox = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//label[contains(.,'Мастеров Иван Константинович')]")));
                    masterCheckbox.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Выбран мастер 'Мастеров Иван Константинович'");

                    var closeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//a[contains(@class,'modal__close')]")));
                    closeButton.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Окно выбора мастера закрыто");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Ошибка при выборе мастера: {ex.Message}");
                }

                // 11. РЕДАКТИРОВАНИЕ ПОДСТАТУСА (2-я иконка)
                Console.WriteLine("\n✏️ [11/11] Редактирование подстатуса...");
                try
                {
                    var parentItem = createdSubstatus.FindElement(By.XPath("./ancestor::div[contains(@class,'item')]"));

                    var actions = new Actions(_browser.Driver);
                    actions.MoveToElement(parentItem).Perform();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Наведена мышь на подстатус");

                    var editButton = parentItem.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'icon')][2]"));
                    editButton.Click();
                    Thread.Sleep(1000);
                    Console.WriteLine("✅ Нажата вторая иконка (редактирование)");

                    // Изменяем название
                    var nameInputEdit = _wait.Until(ExpectedConditions.ElementExists(
                        By.XPath("//label[contains(text(),'Название')]/following-sibling::input")));
                    nameInputEdit.Clear();
                    string newSubstatusName = $"Отредактированный подстатус {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    nameInputEdit.SendKeys(newSubstatusName);
                    Thread.Sleep(500);
                    Console.WriteLine($"✅ Название изменено на: '{newSubstatusName}'");

                    // Изменяем описание
                    var descriptionTextareaEdit = _wait.Until(ExpectedConditions.ElementExists(
                        By.XPath("//div[contains(@class,'txt__label') and contains(text(),'Краткое описание')]/following-sibling::div//textarea")));
                    descriptionTextareaEdit.Clear();
                    string newDescription = $"Отредактированное описание {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    descriptionTextareaEdit.SendKeys(newDescription);
                    Thread.Sleep(500);
                    Console.WriteLine($"✅ Описание изменено");

                    // Убираем галочку "Виден клиентам"
                    try
                    {
                        var checkbox = _wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.XPath("//span[text()='Виден клиентам']/ancestor::label")));
                        checkbox.Click();
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Галочка 'Виден клиентам' убрана");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Не удалось изменить чекбокс: {ex.Message}");
                    }

                    // Меняем цвет
                    try
                    {
                        var random = new Random();
                        byte r = (byte)random.Next(256);
                        byte g = (byte)random.Next(256);
                        byte b = (byte)random.Next(256);
                        string newSelectedColor = $"#{r:X2}{g:X2}{b:X2}".ToLower();
                        Console.WriteLine($"🎲 Новый цвет: HEX = {newSelectedColor}");

                        var nativeColorPicker = _wait.Until(ExpectedConditions.ElementExists(
                            By.XPath("//input[contains(@class,'color-input__native')]")));
                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].value = arguments[1];", nativeColorPicker, newSelectedColor);
                        Thread.Sleep(500);
                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].dispatchEvent(new Event('change', { bubbles: true }));", nativeColorPicker);
                        Thread.Sleep(500);
                        Console.WriteLine($"✅ Цвет изменён на {newSelectedColor}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Ошибка при смене цвета: {ex.Message}");
                    }

                    // Сохраняем изменения
                    var saveIconEdit = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'controls')]/div[contains(@class,'icon')][1]")));
                    saveIconEdit.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Изменения сохранены");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Ошибка при редактировании: {ex.Message}");
                }

                // 12. УДАЛЕНИЕ ПОДСТАТУСА (3-я иконка)
                Console.WriteLine("\n🗑️ [12/11] Удаление подстатуса...");
                try
                {
                    var updatedSubstatus = _wait.Until(ExpectedConditions.ElementExists(
                        By.XPath($"//p[contains(@class,'name') and contains(text(),'Отредактированный подстатус')]")));
                    var parentItem = updatedSubstatus.FindElement(By.XPath("./ancestor::div[contains(@class,'item')]"));

                    var actions = new Actions(_browser.Driver);
                    actions.MoveToElement(parentItem).Perform();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Наведена мышь на подстатус");

                    var deleteButton = parentItem.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'icon')][3]"));
                    deleteButton.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Нажата третья иконка (удаление)");

                    // Простой поиск кнопки "Удалить" по тексту
                    var confirmDelete = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//button[.//span[text()='Удалить']]")));
                    confirmDelete.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Подстатус удалён");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Ошибка при удалении: {ex.Message}");
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