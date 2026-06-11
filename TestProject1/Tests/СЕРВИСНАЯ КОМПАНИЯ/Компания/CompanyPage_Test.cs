using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace RingAutoTests.Tests.СервиснаяКомпания
{
    public class CompanyTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public CompanyTests()
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

        // Генерация случайного телефона
        private string GenerateRandomPhone()
        {
            var random = new Random();
            string part1 = random.Next(100, 1000).ToString();
            string part2 = random.Next(100, 1000).ToString();
            string part3 = random.Next(10, 100).ToString();
            string part4 = random.Next(10, 100).ToString();
            return $"+7 ({part1}) {part2}-{part3}-{part4}";
        }

        // Генерация случайного email
        private string GenerateRandomEmail()
        {
            var random = new Random();
            string[] domains = { "example.com", "test.ru", "mail.ru", "gmail.com", "yandex.ru" };
            return $"autotest_{random.Next(10000, 99999)}_{DateTime.Now:yyyyMMddHHmmss}@{domains[random.Next(domains.Length)]}";
        }

        // Генерация случайного адреса
        private string GenerateRandomAddress()
        {
            var random = new Random();
            string[] cities = { "Москва", "Санкт-Петербург", "Новосибирск", "Екатеринбург", "Казань", "Нижний Новгород", "Красноярск", "Челябинск", "Самара", "Уфа" };
            string[] streets = { "Ленина", "Пушкина", "Советская", "Мира", "Центральная", "Молодёжная", "Садовая", "Лесная", "Школьная", "Заречная" };
            return $"г. {cities[random.Next(cities.Length)]}, ул. {streets[random.Next(streets.Length)]}, д. {random.Next(1, 99)}";
        }

        [Fact]
        public void CompanyPage_Test()
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                Console.WriteLine("🧪 ТЕСТ Компания");

                string phone = "79160000071";
                string password = "Qwerty100";

                // Генерация случайных данных
                var random = new Random();
                string randomDescription = $"Автотест: описание компании {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                string randomEmailMain = GenerateRandomEmail();
                string randomPhoneMain = GenerateRandomPhone();
                string randomEmailAdditional = GenerateRandomEmail();
                string randomPhoneAdditional = GenerateRandomPhone();
                string randomContractNumber = $"ДОГ-{random.Next(100, 999)}";
                string randomContractDate = $"{random.Next(1, 28):D2}.{random.Next(1, 13):D2}.2025";
                string randomContractAmount = random.Next(10000, 500000).ToString();
                string randomBik = $"044525{random.Next(100, 999)}";
                string randomRs = $"407028107000000{random.Next(10000, 99999)}";
                string randomBankName = $"Банк {random.Next(1, 100)}";
                string randomKs = $"301018101000000{random.Next(10000, 99999)}";

                Console.WriteLine($"📋 Случайные данные:");
                Console.WriteLine($"   Описание: {randomDescription}");
                Console.WriteLine($"   Email основной: {randomEmailMain}");
                Console.WriteLine($"   Телефон основной: {randomPhoneMain}");
                Console.WriteLine($"   Email дополнительный: {randomEmailAdditional}");
                Console.WriteLine($"   Телефон дополнительный: {randomPhoneAdditional}");
                Console.WriteLine($"   Номер договора: {randomContractNumber}");
                Console.WriteLine($"   Дата договора: {randomContractDate}");
                Console.WriteLine($"   Сумма договора: {randomContractAmount}");
                Console.WriteLine($"   БИК: {randomBik}");
                Console.WriteLine($"   Расчетный счет: {randomRs}");
                Console.WriteLine($"   Наименование банка: {randomBankName}");
                Console.WriteLine($"   Корреспондентский счет: {randomKs}");

                // 1. Логин
                new LoginPage(_browser).Login(phone, password);
                Thread.Sleep(3000);

                Assert.Contains("lk", _browser.Driver.Url);

                // 2. Dropdown юрлица
                var orgDropdown = WaitClickable(By.XPath("//div[contains(@class,'dropdown__value')]"));
                orgDropdown.Click();

                Thread.Sleep(1000);

                var masterDent = WaitClickable(By.XPath(
                    "//div[contains(@class,'dropdown-options__item') and contains(text(),'Мастер-Дент')]"
                ));
                masterDent.Click();

                Thread.Sleep(2000);

                // 3. Вкладка Компания
                var companyTab = WaitClickable(By.XPath(
                    "//a[@href='/company'] | //span[contains(text(),'Компания')]/ancestor::a"
                ));
                companyTab.Click();

                Thread.Sleep(3000);

                Assert.Contains("/company", _browser.Driver.Url);

                // 4. Карточка
                var companyCard = WaitExists(By.XPath(
                    "//div[contains(@class,'card')]//div[contains(text(),'Мастер-Дент')]/ancestor::div[contains(@class,'card')]"
                ));

                // 5. Нажатие кнопки "Редактировать организацию"
                var editButton = companyCard.FindElement(By.XPath(
                    ".//button[contains(.,'Редактировать организацию')]"
                ));

                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", editButton);
                Thread.Sleep(500);
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", editButton);
                Console.WriteLine("✅ Нажата кнопка 'Редактировать организацию'");

                // 6. Выбор налогообложения (С НДС → обратно на Без НДС)
                Console.WriteLine("\n💰 Работа с налогообложением...");
                Thread.Sleep(2000);

                // Нажимаем "С НДС"
                var withVatLabel = _wait.Until(d => d.FindElement(By.XPath("//label[@for='with_vat']")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", withVatLabel);
                Console.WriteLine("✅ Выбрано 'С НДС'");
                Thread.Sleep(1000);

                // Возвращаем обратно на "Без НДС"
                var noVatLabel = _wait.Until(d => d.FindElement(By.XPath("//label[@for='no_vat']")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", noVatLabel);
                Console.WriteLine("✅ Возвращено на 'Без НДС'");
                Thread.Sleep(1000);

                // 7. Ввод краткого описания
                Console.WriteLine("\n📝 Ввод краткого описания...");

                try
                {
                    var descriptionInput = _wait.Until(d => d.FindElement(By.XPath("//textarea[contains(@class,'txt__textarea')]")));

                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", descriptionInput);
                    Thread.Sleep(500);

                    descriptionInput.Clear();
                    descriptionInput.SendKeys(randomDescription);
                    Thread.Sleep(500);

                    Console.WriteLine($"✅ Введено описание: '{randomDescription}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось ввести описание: {ex.Message}");
                }

                // 8. Заполнение банковских реквизитов
                Console.WriteLine("\n🏦 Заполнение банковских реквизитов...");

                // БИК банка
                var bikInput = _wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'БИК банка')]/following-sibling::input")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", bikInput);
                Thread.Sleep(500);
                bikInput.Clear();
                bikInput.SendKeys(randomBik);
                Console.WriteLine($"✅ Введен БИК: {randomBik}");
                Thread.Sleep(300);

                // Расчетный счет
                var rsInput = _wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'Расчетный счет')]/following-sibling::input")));
                rsInput.Clear();
                rsInput.SendKeys(randomRs);
                Console.WriteLine($"✅ Введен расчетный счет: {randomRs}");
                Thread.Sleep(300);

                // Наименование банка
                var bankNameInput = _wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'Наименование банка')]/following-sibling::input")));
                bankNameInput.Clear();
                bankNameInput.SendKeys(randomBankName);
                Console.WriteLine($"✅ Введено наименование банка: {randomBankName}");
                Thread.Sleep(300);

                // Корреспондентский счет
                var ksInput = _wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'Корреспондентский счет')]/following-sibling::input")));
                ksInput.Clear();
                ksInput.SendKeys(randomKs);
                Console.WriteLine($"✅ Введен корреспондентский счет: {randomKs}");
                Thread.Sleep(300);

                // 9. Заполнение контактов
                Console.WriteLine("\n📧 Заполнение контактов...");

                // Электронная почта основная
                var emailMainInput = _wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'Электронная почта(e-mail) основная')]/following-sibling::input")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", emailMainInput);
                Thread.Sleep(500);
                emailMainInput.Clear();
                emailMainInput.SendKeys(randomEmailMain);
                Console.WriteLine($"✅ Введен основной email: {randomEmailMain}");
                Thread.Sleep(300);

                // Телефон основной
                var phoneMainInput = _wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'Телефон основной')]/following-sibling::input")));
                phoneMainInput.Clear();
                phoneMainInput.SendKeys(randomPhoneMain);
                Console.WriteLine($"✅ Введен основной телефон: {randomPhoneMain}");
                Thread.Sleep(300);

                // Электронная почта дополнительная
                var emailAdditionalInput = _wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'Электронная почта(e-mail) дополнительная')]/following-sibling::input")));
                emailAdditionalInput.Clear();
                emailAdditionalInput.SendKeys(randomEmailAdditional);
                Console.WriteLine($"✅ Введен дополнительный email: {randomEmailAdditional}");
                Thread.Sleep(300);

                // Телефон дополнительный
                var phoneAdditionalInput = _wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'Телефон дополнительный')]/following-sibling::input")));
                phoneAdditionalInput.Clear();
                phoneAdditionalInput.SendKeys(randomPhoneAdditional);
                Console.WriteLine($"✅ Введен дополнительный телефон: {randomPhoneAdditional}");
                Thread.Sleep(500);

                // 10. Выбор сфер деятельности (очистка + случайные чекбоксы)
                Console.WriteLine("\n📋 Выбор сфер деятельности...");

                var scrollContainer = _wait.Until(d => d.FindElement(By.XPath("//div[contains(@class,'company-edit__grid')]")));

                var allActivityCheckboxes = _wait.Until(d => d.FindElements(By.XPath("//div[contains(@class,'company-edit__checkboxes')]//input[@type='checkbox']")));

                var activityCheckboxes = new List<IWebElement>();
                foreach (var cb in allActivityCheckboxes)
                {
                    try
                    {
                        var id = cb.GetAttribute("id");
                        if (id != null && !id.Contains("check-region_"))
                        {
                            activityCheckboxes.Add(cb);
                        }
                    }
                    catch { }
                }

                Console.WriteLine($"📋 Найдено чекбоксов сфер деятельности: {activityCheckboxes.Count}");

                // Снимаем все
                Console.WriteLine("🔄 Снятие всех выбранных чекбоксов...");
                int uncheckedCount = 0;
                foreach (var cb in activityCheckboxes)
                {
                    try
                    {
                        if (cb.Selected)
                        {
                            ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", cb);
                            uncheckedCount++;
                            Thread.Sleep(100);
                        }
                    }
                    catch { }
                }
                Console.WriteLine($"✅ Снято чекбоксов: {uncheckedCount}");
                Thread.Sleep(1000);

                // Выбираем случайные (8-12 штук)
                var selectedItems = new List<string>();

                int checkboxesToSelect = random.Next(8, 13);
                Console.WriteLine($"🎲 Будет выбрано чекбоксов: {checkboxesToSelect}");

                var shuffledCheckboxes = activityCheckboxes.OrderBy(x => random.Next()).ToList();

                int selectedCount = 0;
                for (int i = 0; i < shuffledCheckboxes.Count && selectedCount < checkboxesToSelect; i++)
                {
                    var checkbox = shuffledCheckboxes[i];

                    try
                    {
                        var label = checkbox.FindElement(By.XPath("./ancestor::label"));
                        var textElement = label.FindElement(By.XPath(".//span[contains(@class,'chckbox__text')]"));
                        string checkboxText = textElement.Text.Trim();

                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript(
                            "arguments[0].scrollIntoView({block: 'center', behavior: 'smooth'});", checkbox);
                        Thread.Sleep(200);

                        if (!checkbox.Selected)
                        {
                            ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", checkbox);
                            Console.WriteLine($"✅ Выбран чекбокс {selectedCount + 1}: '{checkboxText}'");
                            selectedItems.Add(checkboxText);
                            selectedCount++;
                        }
                        Thread.Sleep(150);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Не удалось выбрать чекбокс: {ex.Message}");
                    }
                }

                // 11. Выбор регионов (Москва всегда выбрана + случайные)
                Console.WriteLine("\n📍 Выбор регионов...");

                var allCheckboxes = _wait.Until(d => d.FindElements(By.XPath("//div[contains(@class,'company-edit__checkboxes')]//input[@type='checkbox']")));

                var regionsOnly = new List<IWebElement>();
                foreach (var cb in allCheckboxes)
                {
                    try
                    {
                        var id = cb.GetAttribute("id");
                        if (id != null && id.Contains("check-region_"))
                        {
                            regionsOnly.Add(cb);
                        }
                    }
                    catch { }
                }

                Console.WriteLine($"📋 Найдено регионов: {regionsOnly.Count}");

                // Снимаем все регионы
                Console.WriteLine("🔄 Снятие всех выбранных регионов...");
                int uncheckedRegionsCount = 0;
                foreach (var region in regionsOnly)
                {
                    try
                    {
                        if (region.Selected)
                        {
                            ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", region);
                            uncheckedRegionsCount++;
                            Thread.Sleep(100);
                        }
                    }
                    catch { }
                }
                Console.WriteLine($"✅ Снято регионов: {uncheckedRegionsCount}");
                Thread.Sleep(1000);

                // Выбираем Москву (обязательно)
                var moscowCheckbox = regionsOnly.FirstOrDefault(r =>
                {
                    try
                    {
                        var label = r.FindElement(By.XPath("./ancestor::label"));
                        var textElement = label.FindElement(By.XPath(".//span[contains(@class,'chckbox__text')]"));
                        return textElement.Text.Trim() == "Москва";
                    }
                    catch { return false; }
                });

                if (moscowCheckbox != null)
                {
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center', behavior: 'smooth'});", moscowCheckbox);
                    Thread.Sleep(500);
                    if (!moscowCheckbox.Selected)
                    {
                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", moscowCheckbox);
                        Console.WriteLine("✅ Обязательно выбран регион: 'Москва'");
                    }
                }

                // Выбираем случайные регионы (7-12 штук)
                var selectedRegions = new List<string>();

                int regionsToSelect = random.Next(7, 13);
                Console.WriteLine($"🎲 Будет выбрано дополнительно регионов: {regionsToSelect}");

                var shuffledRegions = regionsOnly.OrderBy(x => random.Next()).ToList();

                int selectedRegionCount = 0;
                for (int i = 0; i < shuffledRegions.Count && selectedRegionCount < regionsToSelect; i++)
                {
                    var regionCheckbox = shuffledRegions[i];

                    try
                    {
                        var label = regionCheckbox.FindElement(By.XPath("./ancestor::label"));
                        var textElement = label.FindElement(By.XPath(".//span[contains(@class,'chckbox__text')]"));
                        string regionText = textElement.Text.Trim();

                        // Пропускаем Москву (уже выбрана)
                        if (regionText == "Москва") continue;

                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript(
                            "arguments[0].scrollIntoView({block: 'center', behavior: 'smooth'});", regionCheckbox);
                        Thread.Sleep(200);

                        if (!regionCheckbox.Selected)
                        {
                            ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", regionCheckbox);
                            Console.WriteLine($"✅ Выбран регион {selectedRegionCount + 1}: '{regionText}'");
                            selectedRegions.Add(regionText);
                            selectedRegionCount++;
                        }
                        Thread.Sleep(100);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Не удалось выбрать регион: {ex.Message}");
                    }
                }

                Console.WriteLine($"✅ Всего выбрано регионов: {selectedRegions.Count + 1} (включая Москву)");

                // 12. Нажимаем на вкладку "Адреса"
                Console.WriteLine("\n🏠 Нажатие на вкладку 'Адреса'...");

                try
                {
                    var addressTab = _wait.Until(d => d.FindElement(By.XPath("//div[@class='tabs__tab' and contains(text(),'Адреса')]")));
                    addressTab.Click();
                    Console.WriteLine("✅ Нажата вкладка 'Адреса'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось нажать на вкладку 'Адреса': {ex.Message}");
                }

                Thread.Sleep(1000);

                // 13. Нажимаем на вкладку "Бренд"
                Console.WriteLine("\n🏷️ Нажатие на вкладку 'Бренд'...");

                try
                {
                    var allTabs = _wait.Until(d => d.FindElements(By.XPath("//div[contains(@class,'tabs')]//div[contains(@class,'tabs__tab')]")));
                    foreach (var tab in allTabs)
                    {
                        if (tab.Text.Trim() == "Бренд")
                        {
                            ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", tab);
                            Console.WriteLine("✅ Нажата вкладка 'Бренд'");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось нажать на вкладку 'Бренд': {ex.Message}");
                }

                Thread.Sleep(1000);

                // 13.1 Работа с выпадающим списком бренда
                Console.WriteLine("\n🔽 Работа с выпадающим списком бренда...");

                try
                {
                    // Ждём, пока элемент станет кликабельным, и кликаем
                    var brandSelect = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[contains(@class,'v-select')]//div[contains(@class,'vs__dropdown-toggle')]")));
                    brandSelect.Click();
                    Console.WriteLine("✅ Клик для открытия списка");
                    Thread.Sleep(500);

                    // Выбираем "Мальдивы" (второй элемент в списке - индекс 3)
                    var maldivesOption = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("(//li[contains(@class,'vs__dropdown-option')])[3]")));
                    maldivesOption.Click();
                    Console.WriteLine("✅ Выбран 'Мальдивы' (3-й элемент списка)");
                    Thread.Sleep(500);

                    // Снова кликаем, чтобы открыть список
                    brandSelect = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[contains(@class,'v-select')]//div[contains(@class,'vs__dropdown-toggle')]")));
                    brandSelect.Click();
                    Console.WriteLine("✅ Повторный клик для открытия списка");
                    Thread.Sleep(500);

                    // Выбираем "Царские припасы" (первый элемент в списке - индекс 2)
                    var tsarskieOption = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("(//li[contains(@class,'vs__dropdown-option')])[2]")));
                    tsarskieOption.Click();
                    Console.WriteLine("✅ Выбран 'Царские припасы' (2-й элемент списка)");
                    Thread.Sleep(500);

                    Console.WriteLine("✅ Бренд переключен");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Ошибка: {ex.Message}");
                }

                // 14. Сохранение изменений
                Console.WriteLine("\n💾 Сохранение изменений...");

                var saveButton = _wait.Until(d => d.FindElement(By.XPath(
                    "//button[contains(@class,'btn-red') and contains(.,'Сохранить')]")));
                saveButton.Click();
                Console.WriteLine("✅ Нажата кнопка 'Сохранить'");
                Thread.Sleep(2000);

                // 15. Проверка данных в карточке после сохранения
                Console.WriteLine("\n🔍 Проверка данных в карточке компании...");

                // Обновляем страницу, чтобы увидеть актуальные данные
                _browser.Driver.Navigate().Refresh();
                Thread.Sleep(3000);
                Console.WriteLine("✅ Страница обновлена");

                // Находим карточку заново
                var updatedCompanyCard = WaitExists(By.XPath(
                    "//div[contains(@class,'card')]//div[contains(text(),'Мастер-Дент')]/ancestor::div[contains(@class,'card')]"));

                // Проверяем регионы присутствия
                var regionsText = updatedCompanyCard.FindElement(By.XPath(".//div[contains(@class,'row')]//b[contains(text(),'Регионы присутствия')]/following-sibling::div//p")).Text;
                Console.WriteLine($"📋 Регионы: {regionsText}");

                // Проверяем, что Москва есть в списке регионов
                Assert.Contains("Москва", regionsText);
                Console.WriteLine("✅ Москва присутствует в регионах");

                // Проверяем сферы деятельности
                var spheresText = updatedCompanyCard.FindElement(By.XPath(".//div[contains(@class,'row')]//b[contains(text(),'Сферы деятельности')]/following-sibling::div//p")).Text;
                Console.WriteLine($"📋 Сферы деятельности: {spheresText}");

                // Проверяем налогообложение
                var taxText = updatedCompanyCard.FindElement(By.XPath(".//div[contains(@class,'row')]//b[contains(text(),'Налогообложение')]/following-sibling::div//p")).Text;
                Console.WriteLine($"📋 Налогообложение: {taxText}");
                Assert.Equal("Без НДС", taxText);
                Console.WriteLine("✅ Налогообложение: Без НДС");

                // Проверяем описание
                var descriptionText = updatedCompanyCard.FindElement(By.XPath(".//div[contains(@class,'row')]//b[contains(text(),'Описание')]/following-sibling::div//p")).Text;
                Console.WriteLine($"📋 Описание: {descriptionText}");
                Assert.Equal(randomDescription, descriptionText);
                Console.WriteLine("✅ Описание совпадает");

                // Проверяем контакты
                var contactsText = updatedCompanyCard.FindElement(By.XPath(".//div[contains(@class,'row')]//b[contains(text(),'Контакты')]/following-sibling::p")).Text;
                Console.WriteLine($"📋 Контакты: {contactsText}");
                Assert.Contains(randomPhoneMain, contactsText);
                Assert.Contains(randomEmailMain, contactsText);
                Console.WriteLine("✅ Основные контакты совпадают");

                // Проверяем банковские реквизиты
                var bankText = updatedCompanyCard.FindElement(By.XPath(".//div[contains(@class,'row')]//b[contains(text(),'Банковские реквизиты')]/following-sibling::div//p")).Text;
                Console.WriteLine($"📋 Банковские реквизиты: {bankText}");
                Assert.Contains(randomBik, bankText);
                Assert.Contains(randomRs, bankText);
                Console.WriteLine("✅ Банковские реквизиты совпадают");

                // Проверяем бренд
                var brandText = updatedCompanyCard.FindElement(By.XPath(".//div[contains(@class,'card__content')]//h3[contains(text(),'Бренд')]/following-sibling::h3")).Text;
                Console.WriteLine($"📋 Бренд: {brandText}");
                Assert.Equal("Царские припасы", brandText);
                Console.WriteLine("✅ Бренд: Царские припасы");

                Console.WriteLine("\n🎉 ВСЕ ДАННЫЕ УСПЕШНО ПРОВЕРЕНЫ!");

                Console.WriteLine("\n🎉 ТЕСТ ПРОЙДЕН");
            }
            finally
            {
                _browser.Close();
            }
        }
    }
}