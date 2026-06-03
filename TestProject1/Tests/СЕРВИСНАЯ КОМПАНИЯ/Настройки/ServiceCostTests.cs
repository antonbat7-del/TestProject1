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

namespace RingAutoTests.Tests.СервиснаяКомпания
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
                Console.WriteLine("🧪 ТЕСТ: Загрузка шаблонов стоимости для ООО \"Мастер-Дент\"");
                Console.WriteLine("==================================================");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                string phone = "79262266015";
                string password = "QwertY100";

                // Пути к файлам
                string workFilePath = @"D:\тесты\Шаблон стоимости работ (9)-09-11-2026-12-59-05.xlsx";
                string materialFilePath = @"D:\тесты\Шаблон стоимости материалов (9)-08-11-2026-12-59-13.xlsx";

                // Проверяем, что файлы существуют
                if (!File.Exists(workFilePath))
                {
                    Console.WriteLine($"❌ Файл не найден: {workFilePath}");
                    throw new Exception($"Файл не найден: {workFilePath}");
                }
                if (!File.Exists(materialFilePath))
                {
                    Console.WriteLine($"❌ Файл не найден: {materialFilePath}");
                    throw new Exception($"Файл не найден: {materialFilePath}");
                }
                Console.WriteLine("✅ Файлы найдены");

                // 1. Логин (Мастер-Дент) - НЕ ВЫБИРАЕМ ЮРЛИЦО
                Console.WriteLine("\n🔐 [1/7] Авторизация (Мастер-Дент)...");
                new LoginPage(_browser).Login(phone, password);
                Thread.Sleep(3000);
                Assert.True(_browser.Driver.Url.Contains("lk"), "❌ Логин не выполнен");
                Console.WriteLine($"✅ Логин выполнен");

                // 2. Переход в Настройки
                Console.WriteLine("\n⚙️ [2/7] Переход в 'Настройки'...");
                var settingsLink = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[@href='/settings']")));
                settingsLink.Click();
                Thread.Sleep(3000);
                Assert.Contains("/settings", _browser.Driver.Url);
                Console.WriteLine("✅ Переход в Настройки");

                // 3. Переход на вкладку "Стоимость заявок"
                Console.WriteLine("\n📋 [3/7] Переход на вкладку 'Стоимость заявок'...");
                var serviceCostTab = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'tabs__tab') and contains(.,'Стоимость заявок')]")));
                serviceCostTab.Click();
                Thread.Sleep(3000);
                Console.WriteLine("✅ Переход на вкладку 'Стоимость заявок'");

                // Ждём загрузки таблицы
                _wait.Until(ExpectedConditions.ElementExists(By.XPath("//table | //div[contains(@class,'ant-table')]")));
                Thread.Sleep(1000);

                // 4. Найти строку партнёра "ООО "Мастер-Дент""
                Console.WriteLine("\n🔍 [4/7] Поиск партнёра 'ООО \"Мастер-Дент\"'...");
                var partnerRow = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//td[contains(.,'Мастер-Дент')]/ancestor::tr")));
                Console.WriteLine("✅ Найдена строка партнёра 'ООО \"Мастер-Дент\"'");

                // ========== 5. ЗАГРУЗКА ФАЙЛА "СПИСОК РАБОТ" ==========
                UploadFile(partnerRow, 3, workFilePath, "Список работ");

                // ========== 6. ЗАГРУЗКА ФАЙЛА "СПИСОК МАТЕРИАЛОВ" ==========
                UploadFile(partnerRow, 4, materialFilePath, "Список материалов");

                // 7. Закрываем браузер и открываем инкогнито
                Console.WriteLine("\n🌐 [5/7] Открытие инкогнито и логин под аккаунтом Мастерской...");
                _browser.Close();
                Thread.Sleep(2000);

                // Создаём новый браузер в инкогнито
                var incognitoBrowser = new BrowserHelper(true);
                var incognitoWait = new WebDriverWait(incognitoBrowser.Driver, TimeSpan.FromSeconds(15));

                // Логин под аккаунтом Мастерской
                Console.WriteLine("🔐 Логин (Мастерская)...");
                new LoginPage(incognitoBrowser).Login("79160000071", "Qwerty100");
                Thread.Sleep(3000);
                Assert.True(incognitoBrowser.Driver.Url.Contains("lk"), "❌ Логин Мастерской не выполнен");
                Console.WriteLine("✅ Логин Мастерской выполнен");

                // Сменить юридическое лицо на "Мастер-Дент"
                Console.WriteLine("\n🏢 Смена юридического лица на 'ООО \"Мастер-Дент\"'...");
                try
                {
                    var orgDropdown = incognitoWait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'dropdown__value')]")));
                    orgDropdown.Click();
                    Thread.Sleep(1000);
                    Console.WriteLine("✅ Открыт выпадающий список юр.лиц");

                    var masterDent = incognitoWait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'dropdown-options__item') and contains(text(),'Мастер-Дент')]")));
                    masterDent.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Сменили юр.лицо на 'ООО \"Мастер-Дент\"'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось сменить юр.лицо: {ex.Message}");
                }

                // Переход в Настройки
                Console.WriteLine("⚙️ Переход в 'Настройки'...");
                var settingsLink2 = incognitoWait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[@href='/settings']")));
                settingsLink2.Click();
                Thread.Sleep(3000);

                // Переход на вкладку "Стоимость заявок"
                Console.WriteLine("📋 Переход на вкладку 'Стоимость заявок'...");
                var serviceCostTab2 = incognitoWait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'tabs__tab') and contains(.,'Стоимость заявок')]")));
                serviceCostTab2.Click();
                Thread.Sleep(3000);

                // Ждём загрузки таблицы
                incognitoWait.Until(ExpectedConditions.ElementExists(By.XPath("//table | //div[contains(@class,'ant-table')]")));
                Thread.Sleep(1000);

                // ========== 8. ПОДТВЕРЖДЕНИЕ ШАБЛОНОВ ==========
                Console.WriteLine("\n✅ [6/7] Подтверждение шаблонов...");

                // Подтверждаем файлы у партнёра "ООО "Мастерская""
                bool workConfirmed = ConfirmFile(incognitoBrowser, incognitoWait, "Список работ");
                Assert.True(workConfirmed, "❌ Не удалось подтвердить файл 'Список работ'");

                bool materialConfirmed = ConfirmFile(incognitoBrowser, incognitoWait, "Список материалов");
                Assert.True(materialConfirmed, "❌ Не удалось подтвердить файл 'Список материалов'");

                Console.WriteLine("\n🎉 ТЕСТ ПРОЙДЕН");

                // Закрываем инкогнито браузер
                incognitoBrowser.Close();

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

        private void UploadFile(IWebElement partnerRow, int columnIndex, string filePath, string columnName)
        {
            Console.WriteLine($"\n📤 ЗАГРУЗКА ФАЙЛА В '{columnName}'...");

            // 1. Клик по трём точкам через JavaScript
            var dots = partnerRow.FindElement(By.XPath($".//td[{columnIndex}]//div[contains(@class,'menu-field')]"));
            ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", dots);
            Thread.Sleep(1000);
            Console.WriteLine($"✅ Клик по трём точкам ({columnName})");

            // 2. Ждём появления меню
            _wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[contains(@class,'change-menu')]")));
            Thread.Sleep(500);

            // 3. Ищем пункт "Загрузить файл"
            var uploadMenuItem = _wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[contains(@class,'change-menu')]//*[contains(text(),'Загрузить файл')] | " +
                         "//div[contains(@class,'change-menu')]//*[contains(text(),'Загрузить')]")));
            uploadMenuItem.Click();
            Thread.Sleep(500);
            Console.WriteLine($"✅ Выбран пункт 'Загрузить файл' ({columnName})");

            // 4. Загружаем файл
            var fileInput = _wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@type='file']")));
            fileInput.SendKeys(filePath);
            Thread.Sleep(3000);
            Console.WriteLine($"✅ Файл загружен: {Path.GetFileName(filePath)}");

            // ✅ ПРОВЕРКА: появилась кнопка "Отправить на согласование" (значит файл загрузился)
            Console.WriteLine($"🔍 Проверка: ожидание появления кнопки 'Отправить на согласование'...");
            var sendButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath($".//td[{columnIndex}]//button[contains(@class,'btn-red')]")));
            Assert.True(sendButton.Displayed, $"❌ Кнопка 'Отправить на согласование' не появилась для {columnName}");
            Console.WriteLine($"✅ Кнопка 'Отправить на согласование' появилась — файл загружен!");

            // 5. Нажимаем кнопку "Отправить на согласование"
            Console.WriteLine($"📨 Отправка на согласование ({columnName})...");
            ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", sendButton);
            Thread.Sleep(2000);
            Console.WriteLine($"✅ Отправлено на согласование ({columnName})");
        }

        private bool ConfirmFile(BrowserHelper browser, WebDriverWait wait, string columnName)
        {
            Console.WriteLine($"\n✅ ПОДТВЕРЖДЕНИЕ ФАЙЛА В '{columnName}'...");

            try
            {
                // Находим строку партнёра "ООО "Мастерская""
                var masterRow = wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//td[contains(.,'Мастерская')]/ancestor::tr")));
                Console.WriteLine("✅ Найдена строка партнёра 'ООО \"Мастерская\"'");

                // Определяем колонку: 3 - Список работ, 4 - Список материалов
                int columnIndex = columnName == "Список работ" ? 3 : 4;

                // ✅ ПРОВЕРКА: статус до подтверждения "На согласовании"
                var beforeStatus = masterRow.FindElement(By.XPath($".//td[{columnIndex}]//div[contains(@class,'badge')]"));
                string beforeStatusText = beforeStatus.Text;
                Console.WriteLine($"📋 Статус до подтверждения: '{beforeStatusText}'");
                Assert.Contains("На согласовании", beforeStatusText);
                Console.WriteLine($"✅ Статус 'На согласовании' подтверждён");

                // Внутри строки ищем кнопку "Подтвердить" в нужной колонке
                var confirmButton = masterRow.FindElement(By.XPath($".//td[{columnIndex}]//div[contains(@class,'file-item__controls')]//button[contains(@class,'btn-blue')]"));
                Console.WriteLine($"✅ Нашли кнопку 'Подтвердить' в колонке {columnIndex}");

                // Скроллим до кнопки и кликаем
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", confirmButton);
                Thread.Sleep(500);
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", confirmButton);
                Console.WriteLine($"✅ Кликнули по кнопке 'Подтвердить' ({columnName})");
                Thread.Sleep(2000);

                // ✅ ПРОВЕРКА: кнопка "Подтвердить" исчезла после клика
                var confirmButtons = masterRow.FindElements(By.XPath($".//td[{columnIndex}]//button[contains(@class,'btn-blue')]"));
                Assert.True(confirmButtons.Count == 0, $"❌ Кнопка 'Подтвердить' всё ещё видна для {columnName}");
                Console.WriteLine($"✅ Кнопка 'Подтвердить' исчезла");

                Console.WriteLine($"✅ Файл успешно подтверждён ({columnName})");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при подтверждении {columnName}: {ex.Message}");
                return false;
            }
        }
    }
}