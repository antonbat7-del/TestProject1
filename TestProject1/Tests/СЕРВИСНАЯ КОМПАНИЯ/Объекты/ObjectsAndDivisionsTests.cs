using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;

namespace RingAutoTests.Tests.СервиснаяКомпания
{
    public class ObjectsAndDivisionsTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public ObjectsAndDivisionsTests()
        {
            _browser = new BrowserHelper();
            _wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(15));
        }

        [Fact]
        public void Test_DivisionsAndObjects()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("🧪 ТЕСТ: Дивизионы и Объекты (создание, редактирование, удаление)");
                Console.WriteLine("==================================================");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                string phone = "79160000071";
                string password = "Qwerty100";

                string divisionName = "Тестовый дивизион";
                string divisionUpdatedName = "Обновлённый дивизион";
                string divisionDescription = "Краткое описание тестового дивизиона";
                string divisionUpdatedDescription = "Обновлённое описание тестового дивизиона";

                // Функция для очистки чисел - убирает пробелы, запятые, точки и лишние нули в конце
                string CleanNumber(string text)
                {
                    if (string.IsNullOrEmpty(text)) return "";
                    // Убираем пробелы, запятые, точки
                    string cleaned = text.Replace(" ", "").Replace("\u00A0", "").Replace(",", "").Replace(".", "");
                    // Убираем лишние нули в конце (например "41314600" -> "413146")
                    cleaned = Regex.Replace(cleaned, @"00$", "");
                    cleaned = Regex.Replace(cleaned, @",00$", "");
                    return cleaned;
                }

                // Генерация случайных данных для объекта (первое редактирование)
                var random = new Random();
                string randomPhoneDigits = $"{random.Next(100, 1000)}{random.Next(100, 1000)}{random.Next(10, 100)}{random.Next(10, 100)}";
                string randomPhone = $"+7 ({randomPhoneDigits.Substring(0, 3)}) {randomPhoneDigits.Substring(3, 3)}-{randomPhoneDigits.Substring(6, 2)}-{randomPhoneDigits.Substring(8, 2)}";
                string randomAddress = $"г. {GetRandomCity(random)}, ул. {GetRandomStreet(random)}, д. {random.Next(1, 50)}";
                string randomDescription = "Тестовое описание объекта";
                string randomContractNumber = $"ДОГ-{random.Next(100, 999)}";
                string randomContractDate = $"{random.Next(1, 28):D2}.{random.Next(1, 13):D2}.2025";
                string randomContractAmount = random.Next(10000, 500000).ToString();
                string randomEmail = $"test{random.Next(1000, 9999)}@example.com";
                string targetRegion = GetRandomRegion(random);

                // Генерация случайных данных для второго редактирования объекта
                string randomPhone2Digits = $"{random.Next(100, 1000)}{random.Next(100, 1000)}{random.Next(10, 100)}{random.Next(10, 100)}";
                string randomPhone2 = $"+7 ({randomPhone2Digits.Substring(0, 3)}) {randomPhone2Digits.Substring(3, 3)}-{randomPhone2Digits.Substring(6, 2)}-{randomPhone2Digits.Substring(8, 2)}";
                string randomAddress2 = $"г. {GetRandomCity(random)}, ул. {GetRandomStreet(random)}, д. {random.Next(1, 50)}";
                string randomContractNumber2 = $"ДОГ-{random.Next(100, 999)}";
                string randomContractDate2 = $"{random.Next(1, 28):D2}.{random.Next(1, 13):D2}.2025";
                string randomContractAmount2 = random.Next(10000, 500000).ToString();
                string randomEmail2 = $"test{random.Next(1000, 9999)}@example.com";
                string targetRegion2 = GetRandomRegion(random);

                // 1. Логин
                Console.WriteLine("\n🔐 [1/8] Авторизация...");
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
                    var masterDent = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'dropdown-options__item') and contains(text(),'Мастер-Дент')]")));
                    masterDent.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Сменили юр.лицо на 'ООО \"Мастер-Дент\"'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ {ex.Message}");
                }

                // 3. Переход в "Объекты"
                Console.WriteLine("\n🏭 Переход в 'Объекты'...");
                var objectsLink = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[@href='/objects']")));
                objectsLink.Click();
                Thread.Sleep(3000);
                Assert.Contains("/objects", _browser.Driver.Url);
                Console.WriteLine("✅ Переход выполнен");

                // ========== ЧАСТЬ А: ДИВИЗИОНЫ ==========
                Console.WriteLine("\n📂 РАЗДЕЛ: ДИВИЗИОНЫ");

                // 4. НАЖАТЬ КНОПКУ "ДОБАВИТЬ ДИВИЗИОН"
                Console.WriteLine("\n📝 [4/8] Открытие формы создания дивизиона...");
                var addDivisionBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(.,'Добавить дивизион')]")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", addDivisionBtn);
                Thread.Sleep(500);
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", addDivisionBtn);
                Thread.Sleep(1000);
                Console.WriteLine("✅ Форма создания дивизиона открыта");

                // 5. Заполнение формы дивизиона
                Console.WriteLine("\n✏️ [5/8] Заполнение формы дивизиона...");

                // Название
                var divisionNameInput = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//label[contains(@class,'inpt__label') and contains(text(),'Название')]/following-sibling::input")));
                divisionNameInput.Clear();
                divisionNameInput.SendKeys(divisionName);
                Thread.Sleep(500);
                Console.WriteLine($"✅ Название: '{divisionName}'");

                // Описание
                var descriptionTextarea = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//div[contains(@class,'txt__label') and contains(text(),'Краткое описание')]/following-sibling::div//textarea")));
                descriptionTextarea.Clear();
                descriptionTextarea.SendKeys(divisionDescription);
                Thread.Sleep(500);
                Console.WriteLine("✅ Описание введено");

                // Выбор объектов
                Console.WriteLine("\n📌 Добавление объектов...");
                var cafeCheckbox = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'buildings')]//span[text()='Кафе']/preceding-sibling::span[contains(@class,'chckbox__box')]")));
                cafeCheckbox.Click();
                Thread.Sleep(500);
                Console.WriteLine("✅ Выбран объект 'Кафе'");

                // 6. СОХРАНИТЬ ДИВИЗИОН
                Console.WriteLine("\n💾 [6/8] Сохранение дивизиона...");
                var saveDivisionBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(@class,'btn-red') and contains(.,'Добавить дивизион')]")));
                saveDivisionBtn.Click();
                Thread.Sleep(2000);
                Console.WriteLine("✅ Дивизион сохранён");

                // Проверяем, что дивизион появился в списке
                var createdDivision = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath($"//div[contains(@class,'accordion')]//h3[contains(text(),'{divisionName}')]")));
                Assert.True(createdDivision.Displayed, "❌ Дивизион не отображается в списке");
                Console.WriteLine("✅ Дивизион отображается в списке");

                // НАЖИМАЕМ НА НАЗВАНИЕ ДИВИЗИОНА
                Console.WriteLine("\n🖱️ Нажатие на название дивизиона...");
                var divisionTitle = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//div[contains(@class,'accordion')]//h3[contains(text(),'{divisionName}')]")));
                divisionTitle.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Карточка объекта открыта");

                // НАЖИМАЕМ НА ИКОНКУ РЕДАКТИРОВАНИЯ
                Console.WriteLine("\n✏️ Нажатие на иконку редактирования...");
                var editIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'object-card-actions')]//div[contains(@class,'object-card-actions__item')][1]//*[name()='svg']")));
                editIcon.Click();
                Thread.Sleep(500);
                Console.WriteLine("✅ Иконка редактирования нажата, форма редактирования открыта");

                // 7. РЕДАКТИРОВАНИЕ ДИВИЗИОНА
                Console.WriteLine("\n✏️ [7/8] Редактирование дивизиона...");

                // Меняем название
                var editNameInput = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//label[contains(@class,'inpt__label') and contains(text(),'Название')]/following-sibling::input")));
                editNameInput.Clear();
                editNameInput.SendKeys(divisionUpdatedName);
                Thread.Sleep(500);
                Console.WriteLine($"✅ Название изменено на: '{divisionUpdatedName}'");

                // Меняем описание
                var editDescriptionTextarea = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//div[contains(@class,'txt__label') and contains(text(),'Краткое описание')]/following-sibling::div//textarea")));
                editDescriptionTextarea.Clear();
                editDescriptionTextarea.SendKeys(divisionUpdatedDescription);
                Thread.Sleep(500);
                Console.WriteLine("✅ Описание изменено");

                // Выбираем чекбокс "Столовая Крячко"
                Console.WriteLine("\n📌 Выбор чекбокса 'Столовая Крячко'...");
                var stolovayaCheckbox = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'buildings')]//span[text()='Столовая Крячко']/preceding-sibling::span[contains(@class,'chckbox__box')]")));

                if (!stolovayaCheckbox.GetAttribute("class").Contains("checked") && !stolovayaCheckbox.Selected)
                {
                    stolovayaCheckbox.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Чекбокс 'Столовая Крячко' выбран");
                }
                else
                {
                    Console.WriteLine("ℹ️ Чекбокс 'Столовая Крячко' уже был выбран");
                }

                // Сохраняем изменения
                Console.WriteLine("\n💾 Сохранение изменений...");
                var saveBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(@class,'btn-red') and contains(.,'Сохранить дивизион')]")));
                saveBtn.Click();
                Thread.Sleep(2000);
                Console.WriteLine("✅ Изменения сохранены");

                // Проверяем обновление
                var updatedDivision = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath($"//div[contains(@class,'accordion')]//h3[contains(text(),'{divisionUpdatedName}')]")));
                Assert.True(updatedDivision.Displayed, "❌ Название дивизиона не обновилось");
                Console.WriteLine("✅ Название дивизиона обновлено");

                // РАСКРЫВАЕМ АККОРДЕОН
                Console.WriteLine("\n📂 Раскрытие аккордеона обновлённого дивизиона...");
                var accordionIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//div[contains(@class,'accordion')]//h3[contains(text(),'{divisionUpdatedName}')]/ancestor::div[contains(@class,'accordion')]//*[name()='svg']")));
                accordionIcon.Click();
                Thread.Sleep(500);
                Console.WriteLine("✅ Аккордеон раскрыт");

                // НАЖИМАЕМ НА ОБЪЕКТ "КАФЕ"
                Console.WriteLine("\n☕ Нажатие на объект 'Кафе'...");
                var cafeElement = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//div[contains(@class,'accordion')][.//h3[contains(text(),'{divisionUpdatedName}')]]//li[contains(.,'Кафе')]")));
                cafeElement.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Объект 'Кафе' выбран");

                // Ждём загрузки карточки объекта
                _wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[contains(@class,'object-card-main')]")));
                Thread.Sleep(1000);
                Console.WriteLine("✅ Карточка объекта загружена");

                // ПЕРВОЕ РЕДАКТИРОВАНИЕ ОБЪЕКТА - ПРОПУСКАЕМ, ТАК КАК В ТЕКУЩЕМ ТЕСТЕ ЕГО НЕТ

                // НАЖИМАЕМ НА КНОПКУ "МЕНЮ ОБЪЕКТА"
                Console.WriteLine("\n📋 Нажатие на кнопку 'Меню объекта'...");
                var menuButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(@class,'btn-blue') and contains(.,'Меню объекта')]")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", menuButton);
                Thread.Sleep(300);
                menuButton.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Кнопка 'Меню объекта' нажата");

                // ВЫБИРАЕМ ПУНКТ "РЕДАКТИРОВАТЬ ОБЪЕКТ"
                Console.WriteLine("\n✏️ Выбор пункта 'Редактировать объект'...");
                var editObjectMenuItem = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'menu__container')]//div[contains(text(),'Редактировать объект')] | //div[contains(@class,'menu__container')]//span[contains(text(),'Редактировать объект')]")));
                editObjectMenuItem.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Пункт 'Редактировать объект' выбран");

                // ========== ЗАПОЛНЕНИЕ ФОРМЫ РЕДАКТИРОВАНИЯ ОБЪЕКТА ==========
                Console.WriteLine("\n✏️ Заполнение формы редактирования объекта...");

                // 1. Название объекта (НЕ МЕНЯЕМ)
                Console.WriteLine("✅ Название объекта оставляем без изменений: 'Кафе'");

                // 2. Номер телефона
                Console.WriteLine($"📞 Генерируем случайный телефон: {randomPhone2}");
                var phoneInput = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//label[contains(text(),'Номер телефона')]/following-sibling::input")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].value = '';", phoneInput);
                phoneInput.SendKeys(randomPhone2);
                Thread.Sleep(500);
                Console.WriteLine("✅ Номер телефона введён");

                // 3. Выбор региона
                Console.WriteLine($"📍 Выбираем случайный регион: {targetRegion2}");
                var regionField = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'v-select__label') and contains(text(),'Регион')]/following-sibling::div//div[contains(@class,'vs__dropdown-toggle')]")));
                regionField.Click();
                Thread.Sleep(500);

                _wait.Until(ExpectedConditions.ElementExists(By.XPath("//li[contains(@class,'vs__dropdown-option')]")));
                var targetOption = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//li[contains(@class,'vs__dropdown-option') and normalize-space()='{targetRegion2}']")));
                targetOption.Click();
                Thread.Sleep(500);
                Console.WriteLine($"✅ Выбран регион: '{targetRegion2}'");

                // 4. Адрес
                Console.WriteLine($"🏠 Случайный адрес: {randomAddress2}");
                var addressInput = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//div[contains(@class,'autocomplete-label') and contains(text(),'Адрес')]/following-sibling::input")));
                addressInput.Clear();
                addressInput.SendKeys(randomAddress2);
                Thread.Sleep(500);
                Console.WriteLine("✅ Адрес введён");

                // 5. Краткое описание
                Console.WriteLine($"📝 Случайное описание: {randomDescription}");
                var descTextarea = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//div[contains(@class,'txt__label') and contains(text(),'Краткое описание')]/following-sibling::div//textarea")));
                descTextarea.Clear();
                descTextarea.SendKeys(randomDescription);
                Thread.Sleep(500);
                Console.WriteLine("✅ Краткое описание введено");

                // 6. Номер договора
                Console.WriteLine($"📄 Случайный номер договора: {randomContractNumber2}");
                var contractNumberInput = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//label[contains(text(),'Номер договора')]/following-sibling::input")));
                contractNumberInput.Clear();
                contractNumberInput.SendKeys(randomContractNumber2);
                Thread.Sleep(500);
                Console.WriteLine("✅ Номер договора введён");

                // 7. Дата договора
                Console.WriteLine($"📅 Случайная дата договора: {randomContractDate2}");
                var contractDateInput = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//label[contains(text(),'Дата договора')]/following-sibling::input")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].value = '';", contractDateInput);
                contractDateInput.SendKeys(randomContractDate2);
                Thread.Sleep(500);
                Console.WriteLine("✅ Дата договора введена");

                // 8. Сумма договора
                Console.WriteLine($"💰 Случайная сумма договора: {randomContractAmount2}");
                var contractAmountInput = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//label[contains(text(),'Сумма договора')]/following-sibling::input")));
                contractAmountInput.Clear();
                contractAmountInput.SendKeys(randomContractAmount2);
                Thread.Sleep(500);
                Console.WriteLine("✅ Сумма договора введена");

                // 9. Email
                Console.WriteLine($"📧 Случайный email: {randomEmail2}");
                var emailInput = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//label[contains(text(),'Адрес эл почты клиента')]/following-sibling::input")));
                emailInput.Clear();
                emailInput.SendKeys(randomEmail2);
                Thread.Sleep(500);
                Console.WriteLine("✅ Email клиента введён");

                // 10. Сохранить изменения объекта
                Console.WriteLine("\n💾 Сохранение изменений объекта...");
                var saveObjBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[normalize-space()='Сохранить']")));
                saveObjBtn.Click();
                Thread.Sleep(2000);
                Console.WriteLine("✅ Изменения объекта сохранены");

                // Закрываем карточку объекта
                try
                {
                    var closeCardBtn = _browser.Driver.FindElement(By.XPath("//a[contains(@class,'modal__close')]"));
                    if (closeCardBtn.Displayed)
                    {
                        closeCardBtn.Click();
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Карточка объекта закрыта");
                    }
                }
                catch { }

                // СНОВА НАЖИМАЕМ НА ОБНОВЛЁННЫЙ ДИВИЗИОН
                Console.WriteLine("\n📂 Повторное нажатие на обновлённый дивизион...");
                var divisionTitleAgain = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//div[contains(@class,'accordion')]//h3[contains(text(),'{divisionUpdatedName}')]")));
                divisionTitleAgain.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Дивизион открыт");

                // СНОВА НАЖИМАЕМ НА "КАФЕ"
                Console.WriteLine("\n☕ Повторное нажатие на объект 'Кафе'...");
                var cafeElementAgain = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//div[contains(@class,'accordion')][.//h3[contains(text(),'{divisionUpdatedName}')]]//li[contains(.,'Кафе')]")));
                cafeElementAgain.Click();
                Thread.Sleep(500);
                Console.WriteLine("✅ Объект 'Кафе' выбран");

                // Ждём загрузки карточки объекта
                _wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[contains(@class,'object-card-main')]")));
                Thread.Sleep(1000);
                Console.WriteLine("✅ Карточка объекта загружена");

                // ПРОВЕРКА ПОСЛЕ РЕДАКТИРОВАНИЯ
                Console.WriteLine("\n🔍 Проверка данных объекта ПОСЛЕ редактирования:");

                // Проверяем телефон
                var phoneValueElem = _wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//div[contains(@class,'object-card-main-item')]//div[contains(@class,'object-card-main-item__title') and contains(text(),'Телефон:')]/following-sibling::div[contains(@class,'object-card-main-item__value')]")));
                string actualPhone = phoneValueElem.Text;
                Console.WriteLine($"   Телефон: '{actualPhone}'");
                Assert.Equal(randomPhone2, actualPhone);

                // Проверяем адрес
                var addressValueElem = _browser.Driver.FindElement(By.XPath("//div[contains(@class,'object-card-main-item')]//div[contains(@class,'object-card-main-item__title') and contains(text(),'Адрес:')]/following-sibling::div[contains(@class,'object-card-main-item__value')]"));
                string actualAddress = addressValueElem.Text;
                Console.WriteLine($"   Адрес: '{actualAddress}'");
                Assert.Equal(randomAddress2, actualAddress);

                // Проверяем описание
                var descValueElem = _browser.Driver.FindElement(By.XPath("//div[contains(@class,'object-card-main-item')]//div[contains(@class,'object-card-main-item__title') and contains(text(),'Описание:')]/following-sibling::div[contains(@class,'object-card-main-item__value')]"));
                string actualDesc = descValueElem.Text;
                Console.WriteLine($"   Описание: '{actualDesc}'");
                Assert.Equal(randomDescription, actualDesc);

                // Проверяем номер договора
                var contractNumberElem = _browser.Driver.FindElement(By.XPath("//div[contains(@class,'object-card-section')]//div[contains(@class,'object-card-main-item')]//div[contains(@class,'object-card-main-item__title') and contains(text(),'Номер договора:')]/following-sibling::div[contains(@class,'object-card-main-item__value')]"));
                string actualContractNumber = contractNumberElem.Text;
                Console.WriteLine($"   Номер договора: '{actualContractNumber}'");
                Assert.Equal(randomContractNumber2, actualContractNumber);

                // Проверяем дату договора
                var contractDateElem = _browser.Driver.FindElement(By.XPath("//div[contains(@class,'object-card-section')]//div[contains(@class,'object-card-main-item')]//div[contains(@class,'object-card-main-item__title') and contains(text(),'Дата договора:')]/following-sibling::div[contains(@class,'object-card-main-item__value')]"));
                string actualContractDate = contractDateElem.Text;
                Console.WriteLine($"   Дата договора: '{actualContractDate}'");
                Assert.Equal(randomContractDate2, actualContractDate);

                // Проверяем сумму договора - ИСПРАВЛЕНО
                var contractAmountElem = _browser.Driver.FindElement(By.XPath("//div[contains(@class,'object-card-section')]//div[contains(@class,'object-card-main-item')]//div[contains(@class,'object-card-main-item__title') and contains(text(),'Сумма договора:')]/following-sibling::div[contains(@class,'object-card-main-item__value')]"));
                string actualContractAmount = CleanNumber(contractAmountElem.Text);
                string expectedContractAmount = CleanNumber(randomContractAmount2);
                Console.WriteLine($"   Сумма договора: '{actualContractAmount}'");
                Assert.Equal(expectedContractAmount, actualContractAmount);

                // Проверяем email
                var emailElem = _browser.Driver.FindElement(By.XPath("//div[contains(@class,'object-card-section')]//div[contains(@class,'object-card-main-item')]//div[contains(@class,'object-card-main-item__title') and contains(text(),'Адрес эл почты клиента:')]/following-sibling::div[contains(@class,'object-card-main-item__value')]"));
                string actualEmail = emailElem.Text;
                Console.WriteLine($"   Email: '{actualEmail}'");
                Assert.Equal(randomEmail2, actualEmail);

                Console.WriteLine("✅ Все данные объекта успешно обновлены и проверены");

                // Закрываем карточку объекта
                try
                {
                    var closeCardBtn = _browser.Driver.FindElement(By.XPath("//a[contains(@class,'modal__close')]"));
                    if (closeCardBtn.Displayed)
                    {
                        closeCardBtn.Click();
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Карточка объекта закрыта");
                    }
                }
                catch { }

                // 8. УДАЛЕНИЕ ДИВИЗИОНА
                Console.WriteLine("\n🗑️ [8/8] Удаление дивизиона...");

                // Нажимаем на название дивизиона
                var divisionTitleForDelete = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//div[contains(@class,'accordion')]//h3[contains(text(),'{divisionUpdatedName}')]")));
                divisionTitleForDelete.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Нажали на название дивизиона");

                // Ждём появления карточки
                _wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[contains(@class,'object-card')]")));
                Thread.Sleep(500);

                // Находим кнопку удаления (вторая иконка в object-card-actions)
                var deleteButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'object-card-actions')]//div[contains(@class,'object-card-actions__item')][2]//*[name()='svg']")));
                deleteButton.Click();
                Thread.Sleep(2000);
                Console.WriteLine("✅ Нажали на кнопку удаления (удаление произошло сразу, подтверждения нет)");

                // ОБНОВЛЯЕМ СТРАНИЦУ
                Console.WriteLine("\n🔄 Обновление страницы...");
                _browser.Driver.Navigate().Refresh();
                Thread.Sleep(3000);
                Console.WriteLine("✅ Страница обновлена");

                // ПРОВЕРЯЕМ, ЧТО ДИВИЗИОН ИСЧЕЗ
                Console.WriteLine("\n🔍 Проверка, что дивизион больше не отображается...");
                var deletedCheck = _browser.Driver.FindElements(By.XPath($"//div[contains(@class,'accordion')]//h3[contains(text(),'{divisionUpdatedName}')]"));
                Assert.True(deletedCheck.Count == 0, $"❌ Дивизион '{divisionUpdatedName}' всё ещё отображается в списке");
                Console.WriteLine("✅ Дивизион успешно удалён и больше не отображается");

                // ТЕСТ ЗАВЕРШЁН УСПЕШНО
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

        private string GetRandomCity(Random random)
        {
            var cities = new[] { "Москва", "Санкт-Петербург", "Новосибирск", "Екатеринбург", "Казань", "Нижний Новгород", "Красноярск", "Челябинск", "Самара", "Уфа" };
            return cities[random.Next(cities.Length)];
        }

        private string GetRandomStreet(Random random)
        {
            var streets = new[] { "Ленина", "Пушкина", "Советская", "Мира", "Центральная", "Молодёжная", "Садовая", "Лесная", "Школьная", "Заречная" };
            return streets[random.Next(streets.Length)];
        }

        private string GetRandomRegion(Random random)
        {
            var regions = new[] { "Москва", "Санкт-Петербург", "Московская", "Ленинградская", "Новосибирская", "Свердловская", "Татарстан", "Краснодарский", "Приморский", "Хабаровский" };
            return regions[random.Next(regions.Length)];
        }
    }
}