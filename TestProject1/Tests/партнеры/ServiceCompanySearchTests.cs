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

namespace RingAutoTests.Tests.Партнеры
{
    public class ServiceCompanySearchTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public ServiceCompanySearchTests()
        {
            _browser = new BrowserHelper();
            _wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(15));
        }

        [Fact]
        public void SearchServiceCompany_ShouldShowResults()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("TEST STARTED - SearchServiceCompany");
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

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
                var searchPartnersBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'partners__buttons')]//div[contains(@class,'partners__btn')][2]//button")));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", searchPartnersBtn);
                Thread.Sleep(2000);
                Console.WriteLine("✅ Нажата кнопка 'Поиск партнеров'");

                // 4. Нажать на вторую иконку (расширенный поиск)
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
                Thread.Sleep(800);
                Console.WriteLine("✅ Нажат выпадающий список 'Выберите роль'");

                // 6. Выбрать "Сервисные компании" (вторая опция)
                var serviceCompanyOption = _browser.Driver.FindElement(By.XPath("//*[contains(@id,'__option-1')]"));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", serviceCompanyOption);
                Thread.Sleep(500);
                Console.WriteLine("✅ Выбрана опция 'Сервисные компании'");

                // ✅ ПРОВЕРКА: в поле отображается выбранная роль
                try
                {
                    var selectedRoleText = _browser.Driver.FindElement(By.XPath("//div[contains(@class,'vs__selected-options')]//span[contains(@class,'vs__selected')]")).Text;
                    Assert.Contains("Сервисные", selectedRoleText);
                    Console.WriteLine($"✅ Проверка: выбрана роль '{selectedRoleText}'");
                }
                catch
                {
                    var selectedRoleValue = roleField.GetAttribute("value");
                    if (!string.IsNullOrEmpty(selectedRoleValue))
                    {
                        Assert.Contains("Сервисные", selectedRoleValue);
                        Console.WriteLine($"✅ Проверка: выбрана роль '{selectedRoleValue}'");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Не удалось проверить выбранную роль, но продолжаем");
                    }
                }

                // 7. Выбрать чекбокс "IT-система"
                Thread.Sleep(500);
                var itSystemLabel = _browser.Driver.FindElement(By.XPath("//label[contains(.,'IT-система')]"));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", itSystemLabel);
                Thread.Sleep(300);
                Console.WriteLine("✅ Выбран чекбокс 'IT-система'");
                var itSystemCheckbox = _browser.Driver.FindElement(By.XPath("//label[contains(.,'IT-система')]//input[@type='checkbox']"));
                Assert.True(itSystemCheckbox.Selected, "❌ Чекбокс 'IT-система' не выбран");

                // 8. Выбрать чекбокс "Вендинговое оборудование"
                var vendingLabel = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Вендинговое оборудование')]"));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", vendingLabel);
                Thread.Sleep(300);
                Console.WriteLine("✅ Выбран чекбокс 'Вендинговое оборудование'");
                var vendingCheckbox = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Вендинговое оборудование')]//input[@type='checkbox']"));
                Assert.True(vendingCheckbox.Selected, "❌ Чекбокс 'Вендинговое оборудование' не выбран");

                // 9. Выбрать чекбокс "Вентиляция"
                var ventilationLabel = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Вентиляция')]"));
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", ventilationLabel);
                Thread.Sleep(300);
                Console.WriteLine("✅ Выбран чекбокс 'Вентиляция'");
                var ventilationCheckbox = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Вентиляция')]//input[@type='checkbox']"));
                Assert.True(ventilationCheckbox.Selected, "❌ Чекбокс 'Вентиляция' не выбран");

                // 10. Прокрутить страницу вниз
                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Thread.Sleep(2000);
                Console.WriteLine("✅ Прокрутка страницы вниз");

                // 11. Выбрать регионы (с прокруткой к каждому)
                string[] regionsToSelect = { "Адыгея", "Алтайский край", "Архангельская", "Брянская", "Москва" };

                foreach (var region in regionsToSelect)
                {
                    try
                    {
                        var regionLabel = _browser.Driver.FindElement(By.XPath($"//label[contains(.,'{region}')]"));
                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", regionLabel);
                        Thread.Sleep(300);

                        var regionCheckbox = _browser.Driver.FindElement(By.XPath($"//label[contains(.,'{region}')]//input[@type='checkbox']"));
                        bool isChecked = regionCheckbox.Selected;

                        if (!isChecked)
                        {
                            ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", regionLabel);
                            Thread.Sleep(300);
                            Console.WriteLine($"✅ Выбран регион: {region}");

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

                // 12. Выбрать чекбокс "С НДС"
                Thread.Sleep(500);
                var ndsCheckbox = _browser.Driver.FindElement(By.XPath("//label[contains(.,'С НДС')]//input[@type='checkbox']"));
                bool isNdsChecked = ndsCheckbox.Selected;

                if (!isNdsChecked)
                {
                    var ndsLabel = _browser.Driver.FindElement(By.XPath("//label[contains(.,'С НДС')]"));
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", ndsLabel);
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Выбран чекбокс 'С НДС'");

                    var ndsCheckboxAfter = _browser.Driver.FindElement(By.XPath("//label[contains(.,'С НДС')]//input[@type='checkbox']"));
                    Assert.True(ndsCheckboxAfter.Selected, "❌ Чекбокс 'С НДС' не выбран");
                }
                else
                {
                    Console.WriteLine("ℹ️ Чекбокс 'С НДС' уже выбран, пропускаем");
                }

                // 13. Выбрать чекбокс "Без НДС"
                Thread.Sleep(500);
                var withoutNdsCheckbox = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Без НДС')]//input[@type='checkbox']"));
                bool isWithoutNdsChecked = withoutNdsCheckbox.Selected;

                if (!isWithoutNdsChecked)
                {
                    var withoutNdsLabel = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Без НДС')]"));
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", withoutNdsLabel);
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Выбран чекбокс 'Без НДС'");

                    var withoutNdsCheckboxAfter = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Без НДС')]//input[@type='checkbox']"));
                    Assert.True(withoutNdsCheckboxAfter.Selected, "❌ Чекбокс 'Без НДС' не выбран");
                }
                else
                {
                    Console.WriteLine("ℹ️ Чекбокс 'Без НДС' уже выбран, пропускаем");
                }

                // ========== АВТОМАТИЧЕСКОЕ ОПРЕДЕЛЕНИЕ ВЫБРАННЫХ ФИЛЬТРОВ (ДО НАЖАТИЯ КНОПКИ) ==========
                Console.WriteLine("\n🔍 Автоопределение выбранных фильтров...");

                // Определяем выбранные чекбоксы НДС
                bool filterNdsSelected = false;
                bool filterWithoutNdsSelected = false;
                try
                {
                    var ndsCheckboxFinal = _browser.Driver.FindElement(By.XPath("//label[contains(.,'С НДС')]//input[@type='checkbox']"));
                    filterNdsSelected = ndsCheckboxFinal.Selected;

                    var withoutNdsCheckboxFinal = _browser.Driver.FindElement(By.XPath("//label[contains(.,'Без НДС')]//input[@type='checkbox']"));
                    filterWithoutNdsSelected = withoutNdsCheckboxFinal.Selected;

                    Console.WriteLine($"   НДС: С НДС={filterNdsSelected}, Без НДС={filterWithoutNdsSelected}");
                }
                catch
                {
                    Console.WriteLine("   ⚠️ Не удалось определить фильтры НДС");
                }

                // Определяем выбранные регионы
                List<string> selectedRegionsList = new List<string>();
                try
                {
                    foreach (var region in regionsToSelect)
                    {
                        try
                        {
                            var regionCheckbox = _browser.Driver.FindElement(By.XPath($"//label[contains(.,'{region}')]//input[@type='checkbox']"));
                            if (regionCheckbox.Selected)
                            {
                                selectedRegionsList.Add(region);
                            }
                        }
                        catch { }
                    }
                    Console.WriteLine($"   Регионы: выбрано {selectedRegionsList.Count} из {regionsToSelect.Length}");
                    if (selectedRegionsList.Count > 0)
                        Console.WriteLine($"      Список: {string.Join(", ", selectedRegionsList)}");
                }
                catch
                {
                    selectedRegionsList = new List<string>(regionsToSelect);
                    Console.WriteLine("   ⚠️ Используем список регионов по умолчанию");
                }

                // Определяем выбранные сферы деятельности
                string[] allActivities = { "Вентиляция", "IT-система", "Вендинговое оборудование" };
                List<string> selectedActivitiesList = new List<string>();
                try
                {
                    foreach (var activity in allActivities)
                    {
                        try
                        {
                            var activityCheckbox = _browser.Driver.FindElement(By.XPath($"//label[contains(.,'{activity}')]//input[@type='checkbox']"));
                            if (activityCheckbox.Selected)
                            {
                                selectedActivitiesList.Add(activity);
                            }
                        }
                        catch { }
                    }
                    Console.WriteLine($"   Сферы: выбрано {selectedActivitiesList.Count} из {allActivities.Length}");
                    if (selectedActivitiesList.Count > 0)
                        Console.WriteLine($"      Список: {string.Join(", ", selectedActivitiesList)}");
                }
                catch
                {
                    selectedActivitiesList = new List<string>(allActivities);
                    Console.WriteLine("   ⚠️ Используем список сфер по умолчанию");
                }

                // 14. Нажать кнопку "Применить фильтр"
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

                Assert.True(applyFilterBtn.Enabled, "❌ Кнопка 'Применить фильтр' неактивна");
                Console.WriteLine("✅ Кнопка 'Применить фильтр' активна");

                ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", applyFilterBtn);
                Thread.Sleep(3000);
                Console.WriteLine("✅ Нажата кнопка 'Применить фильтр'");

                // ========== 15. ПОЛНАЯ ПРОВЕРКА РЕЗУЛЬТАТОВ ПОИСКА ==========
                Thread.Sleep(3000);

                // Находим все строки с партнёрами в таблице
                var partnerRows = _browser.Driver.FindElements(By.XPath("//tbody//tr"));

                if (partnerRows.Count == 0)
                {
                    partnerRows = _browser.Driver.FindElements(By.XPath("//div[contains(@class,'partners__company')]/ancestor::tr"));
                }

                // ✅ ПРОВЕРКА 1: список не пуст
                Assert.True(partnerRows.Count > 0, "❌ Список партнёров пуст");
                Console.WriteLine($"\n✅ Найдено партнёров: {partnerRows.Count}");

                // ========== ПОСТРОЧНАЯ ПРОВЕРКА КАЖДОЙ КОМПАНИИ ==========
                int companyIndex = 0;
                int totalErrors = 0;
                List<string> errorMessages = new List<string>();

                Console.WriteLine("\n=== ПОСТРОЧНАЯ ПРОВЕРКА КОМПАНИЙ ===\n");

                foreach (var row in partnerRows)
                {
                    companyIndex++;
                    List<string> companyErrors = new List<string>();

                    Console.WriteLine($"--- Компания {companyIndex} ---");

                    // 1. ПРОВЕРКА РОЛИ
                    try
                    {
                        var roleElement = row.FindElement(By.XPath(".//td[3]//div[contains(@class,'partners__info')]"));
                        string roleText = roleElement.Text.Trim();

                        if (!roleText.Contains("Сервисная компания"))
                        {
                            companyErrors.Add($"Роль: ожидается 'Сервисная компания', фактически '{roleText}'");
                        }
                        else
                        {
                            Console.WriteLine($"   ✅ Роль: '{roleText}'");
                        }
                    }
                    catch (Exception ex)
                    {
                        companyErrors.Add($"Роль: не удалось получить текст - {ex.Message}");
                    }

                    // 2. ПРОВЕРКА НДС
                    try
                    {
                        var infoDiv = row.FindElement(By.XPath(".//td[3]//div[contains(@class,'partners__info')]"));
                        string infoText = infoDiv.Text.Trim();

                        bool hasNds = infoText.Contains("С НДС") || infoText.Contains("C НДС");
                        bool hasWithoutNds = infoText.Contains("Без НДС");

                        string actualNds = "";
                        if (hasNds && !hasWithoutNds) actualNds = "С НДС";
                        else if (hasWithoutNds) actualNds = "Без НДС";
                        else actualNds = "НЕ ОПРЕДЕЛЕН";

                        // Проверяем соответствие фильтру
                        bool ndsValid = true;
                        if (filterNdsSelected && !filterWithoutNdsSelected)
                        {
                            ndsValid = (actualNds == "С НДС");
                            if (!ndsValid) companyErrors.Add($"НДС: ожидается 'С НДС', фактически '{actualNds}'");
                        }
                        else if (!filterNdsSelected && filterWithoutNdsSelected)
                        {
                            ndsValid = (actualNds == "Без НДС");
                            if (!ndsValid) companyErrors.Add($"НДС: ожидается 'Без НДС', фактически '{actualNds}'");
                        }
                        else if (filterNdsSelected && filterWithoutNdsSelected)
                        {
                            ndsValid = (actualNds == "С НДС" || actualNds == "Без НДС");
                            if (!ndsValid) companyErrors.Add($"НДС: ожидается 'С НДС' или 'Без НДС', фактически '{actualNds}'");
                        }

                        if (ndsValid) Console.WriteLine($"   ✅ НДС: '{actualNds}'");
                    }
                    catch (Exception ex)
                    {
                        companyErrors.Add($"НДС: ошибка - {ex.Message}");
                    }

                    // 3. ПРОВЕРКА РЕГИОНОВ (6-й столбец)
                    try
                    {
                        string regionsText = "";
                        try
                        {
                            var tooltipDiv = row.FindElement(By.XPath(".//td[6]//div[contains(@class,'tooltip')]//div[contains(@class,'partners__info')]"));
                            regionsText = tooltipDiv.Text.Trim();
                        }
                        catch
                        {
                            var regionCell = row.FindElement(By.XPath(".//td[6]"));
                            regionsText = regionCell.Text.Trim();
                        }

                        if (string.IsNullOrEmpty(regionsText) || regionsText == "-" || regionsText == "—")
                        {
                            if (selectedRegionsList.Count > 0)
                                companyErrors.Add($"Регионы: не указаны (ожидается один из: {string.Join(", ", selectedRegionsList)})");
                            else
                                Console.WriteLine($"   ⚠️ Регионы: не указаны (пропускаем)");
                        }
                        else
                        {
                            bool hasRegion = false;
                            string foundRegion = "";
                            foreach (var region in selectedRegionsList)
                            {
                                if (regionsText.Contains(region))
                                {
                                    hasRegion = true;
                                    foundRegion = region;
                                    break;
                                }
                            }

                            if (hasRegion)
                            {
                                Console.WriteLine($"   ✅ Регионы: '{regionsText}' (найден '{foundRegion}')");
                            }
                            else
                            {
                                companyErrors.Add($"Регионы: '{regionsText}' не содержит ни одного из выбранных регионов [{string.Join(", ", selectedRegionsList)}]");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        companyErrors.Add($"Регионы: ошибка - {ex.Message}");
                    }

                    // 4. ПРОВЕРКА СФЕР ДЕЯТЕЛЬНОСТИ (5-й столбец)
                    try
                    {
                        string activitiesText = "";
                        try
                        {
                            var activityCell = row.FindElement(By.XPath(".//td[5]"));
                            activitiesText = activityCell.Text.Trim();
                        }
                        catch { }

                        if (string.IsNullOrEmpty(activitiesText) || activitiesText == "-" || activitiesText == "—")
                        {
                            if (selectedActivitiesList.Count > 0)
                                companyErrors.Add($"Сферы: не указаны (ожидается одна из: {string.Join(", ", selectedActivitiesList)})");
                            else
                                Console.WriteLine($"   ⚠️ Сферы: не указаны (пропускаем)");
                        }
                        else
                        {
                            bool hasActivity = false;
                            string foundActivity = "";
                            foreach (var activity in selectedActivitiesList)
                            {
                                if (activitiesText.Contains(activity))
                                {
                                    hasActivity = true;
                                    foundActivity = activity;
                                    break;
                                }
                            }

                            if (hasActivity)
                            {
                                Console.WriteLine($"   ✅ Сферы: '{activitiesText}' (найдена '{foundActivity}')");
                            }
                            else
                            {
                                companyErrors.Add($"Сферы: '{activitiesText}' не содержит ни одной из выбранных сфер [{string.Join(", ", selectedActivitiesList)}]");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        companyErrors.Add($"Сферы: ошибка - {ex.Message}");
                    }

                    // ВЫВОД ОШИБОК ДЛЯ КОМПАНИИ
                    if (companyErrors.Count > 0)
                    {
                        totalErrors++;
                        Console.WriteLine($"   ❌ ОШИБКИ ({companyErrors.Count}):");
                        foreach (var err in companyErrors)
                        {
                            Console.WriteLine($"      - {err}");
                            errorMessages.Add($"Компания {companyIndex}: {err}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"   ✅ Все проверки пройдены");
                    }
                    Console.WriteLine();
                }

                // ========== ИТОГОВАЯ ПРОВЕРКА ==========
                Console.WriteLine("=== ИТОГИ ПОСТРОЧНОЙ ПРОВЕРКИ ===");
                Console.WriteLine($"✅ Всего компаний: {companyIndex}");
                Console.WriteLine($"✅ Компаний без ошибок: {companyIndex - totalErrors}");

                if (totalErrors > 0)
                {
                    Console.WriteLine($"❌ Компаний с ошибками: {totalErrors}");
                    Console.WriteLine("\n=== СПИСОК ОШИБОК ===");
                    foreach (var err in errorMessages)
                    {
                        Console.WriteLine($"   {err}");
                    }
                    Assert.True(false, $"❌ Найдено {totalErrors} компаний с ошибками. Подробности в логе.");
                }
                else
                {
                    Console.WriteLine($"✅ Все {companyIndex} компаний прошли все проверки!");
                }

                // ========== ДОПОЛНИТЕЛЬНАЯ ПРОВЕРКА: НЕТ ПРОИЗВОДИТЕЛЕЙ ==========
                var wrongRoles = _browser.Driver.FindElements(By.XPath("//*[contains(text(),'Производители оборудования')]"));
                Assert.True(wrongRoles.Count == 0, "❌ Найдены производители оборудования");
                Console.WriteLine("✅ 5. В результатах нет производителей оборудования");

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