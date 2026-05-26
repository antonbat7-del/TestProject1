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

                // ========== ПРОВЕРКА РОЛИ ==========
                int wrongTypeCount = 0;
                foreach (var row in partnerRows)
                {
                    try
                    {
                        var roleElement = row.FindElement(By.XPath(".//td[3]//div[contains(@class,'partners__info')]"));
                        string role = roleElement.Text.Trim();
                        if (!role.Contains("Сервисная компания"))
                        {
                            wrongTypeCount++;
                        }
                    }
                    catch { }
                }
                Assert.True(wrongTypeCount == 0, $"❌ Найдено {wrongTypeCount} партнёров с неправильной ролью");
                Console.WriteLine("✅ 1. Все партнёры имеют тип 'Сервисная компания'");

                // ========== ПРОВЕРКА СТАТУСА НДС ==========
                int companiesWithNds = 0, companiesWithoutNds = 0, companiesWithoutNdsStatus = 0;

                foreach (var row in partnerRows)
                {
                    try
                    {
                        var infoDiv = row.FindElement(By.XPath(".//td[3]//div[contains(@class,'partners__info')]"));
                        string infoText = infoDiv.Text.Trim();

                        bool hasNds = infoText.Contains("С НДС") ||
                                      infoText.Contains("C НДС") ||
                                      (infoText.Contains("НДС") && !infoText.Contains("Без"));

                        bool hasWithoutNds = infoText.Contains("Без НДС") ||
                                             infoText.Contains("без НДС") ||
                                             infoText == "Без НДС.";

                        if (hasNds && !hasWithoutNds)
                            companiesWithNds++;
                        else if (hasWithoutNds)
                            companiesWithoutNds++;
                        else
                            companiesWithoutNdsStatus++;
                    }
                    catch
                    {
                        companiesWithoutNdsStatus++;
                    }
                }

                Console.WriteLine($"📊 Статистика НДС: С НДС={companiesWithNds}, Без НДС={companiesWithoutNds}, Не определено={companiesWithoutNdsStatus}");

                // Проверяем соответствие выбранным фильтрам НДС
                if (filterNdsSelected && !filterWithoutNdsSelected)
                {
                    if (companiesWithoutNds > 0)
                        Assert.True(false, $"❌ Найдено {companiesWithoutNds} компаний 'Без НДС', но фильтр 'Без НДС' не выбран");
                    if (companiesWithoutNdsStatus > 0)
                        Assert.True(false, $"❌ У {companiesWithoutNdsStatus} компаний не определён статус НДС");
                    Console.WriteLine("✅ 2. Проверка НДС: только 'С НДС'");
                }
                else if (!filterNdsSelected && filterWithoutNdsSelected)
                {
                    if (companiesWithNds > 0)
                        Assert.True(false, $"❌ Найдено {companiesWithNds} компаний 'С НДС', но фильтр 'С НДС' не выбран");
                    if (companiesWithoutNdsStatus > 0)
                        Assert.True(false, $"❌ У {companiesWithoutNdsStatus} компаний не определён статус НДС");
                    Console.WriteLine("✅ 2. Проверка НДС: только 'Без НДС'");
                }
                else if (filterNdsSelected && filterWithoutNdsSelected)
                {
                    if (companiesWithoutNdsStatus > 0)
                        Assert.True(false, $"❌ У {companiesWithoutNdsStatus} компаний не определён статус НДС");
                    Console.WriteLine("✅ 2. Проверка НДС: оба фильтра");
                }
                else
                {
                    Console.WriteLine("✅ 2. Проверка НДС: фильтры не выбраны, пропущена");
                }

                // ========== ПРОВЕРКА РЕГИОНОВ (6-й СТОЛБЕЦ) ==========
                int companiesWithWrongRegions = 0;
                int companiesWithNoRegions = 0;
                int companiesWithValidRegions = 0;

                Console.WriteLine("\n--- Проверка регионов ---");

                foreach (var row in partnerRows)
                {
                    try
                    {
                        string regionsText = "";

                        // Регионы находятся в 6-м столбце (td[6]) внутри tooltip
                        try
                        {
                            var tooltipDiv = row.FindElement(By.XPath(".//td[6]//div[contains(@class,'tooltip')]//div[contains(@class,'partners__info')]"));
                            regionsText = tooltipDiv.Text.Trim();
                        }
                        catch { }

                        // Если не нашли через tooltip, пробуем просто текст 6-го столбца
                        if (string.IsNullOrEmpty(regionsText))
                        {
                            try
                            {
                                var regionCell = row.FindElement(By.XPath(".//td[6]"));
                                regionsText = regionCell.Text.Trim();
                            }
                            catch { }
                        }

                        if (string.IsNullOrEmpty(regionsText) || regionsText == "-" || regionsText == "—")
                        {
                            companiesWithNoRegions++;
                            continue;
                        }

                        // Проверяем наличие хотя бы одного выбранного региона
                        bool hasRegion = false;
                        foreach (var region in selectedRegionsList)
                        {
                            if (regionsText.Contains(region))
                            {
                                hasRegion = true;
                                break;
                            }
                        }

                        if (hasRegion)
                        {
                            companiesWithValidRegions++;
                        }
                        else
                        {
                            companiesWithWrongRegions++;
                            Console.WriteLine($"   ❌ Нет выбранных регионов. Указано: '{regionsText}'");
                        }
                    }
                    catch
                    {
                        companiesWithWrongRegions++;
                    }
                }

                Console.WriteLine($"\n📊 Статистика регионов:");
                Console.WriteLine($"   - С выбранными регионами: {companiesWithValidRegions}");
                Console.WriteLine($"   - Без регионов (пропущено): {companiesWithNoRegions}");
                Console.WriteLine($"   - С неверными регионами: {companiesWithWrongRegions}");

                if (selectedRegionsList.Count > 0)
                {
                    if (companiesWithWrongRegions > 0)
                        Assert.True(false, $"❌ У {companiesWithWrongRegions} компаний указаны регионы, но нет ни одного из выбранных");

                    if (companiesWithValidRegions == 0 && companiesWithNoRegions > 0)
                        Console.WriteLine($"⚠️ ВНИМАНИЕ: У всех компаний отсутствуют регионы");

                    Console.WriteLine($"✅ 3. Проверка регионов: {companiesWithValidRegions} компаний имеют выбранный регион");
                }
                else
                {
                    Console.WriteLine("✅ 3. Проверка регионов: фильтры не выбраны, пропущена");
                }

                // ========== ПРОВЕРКА СФЕРЫ ДЕЯТЕЛЬНОСТИ (5-й СТОЛБЕЦ) ==========
                int companiesWithWrongActivities = 0;
                int companiesWithNoActivities = 0;
                int companiesWithValidActivities = 0;

                Console.WriteLine("\n--- Проверка сфер деятельности ---");

                foreach (var row in partnerRows)
                {
                    try
                    {
                        string activitiesText = "";

                        // Сферы деятельности в 5-м столбце (td[5])
                        try
                        {
                            var activityCell = row.FindElement(By.XPath(".//td[5]"));
                            activitiesText = activityCell.Text.Trim();
                        }
                        catch { }

                        if (string.IsNullOrEmpty(activitiesText) || activitiesText == "-" || activitiesText == "—")
                        {
                            companiesWithNoActivities++;
                            continue;
                        }

                        // Проверяем наличие хотя бы одной выбранной сферы
                        bool hasActivity = false;
                        foreach (var activity in selectedActivitiesList)
                        {
                            if (activitiesText.Contains(activity))
                            {
                                hasActivity = true;
                                break;
                            }
                        }

                        if (hasActivity)
                        {
                            companiesWithValidActivities++;
                        }
                        else
                        {
                            companiesWithWrongActivities++;
                            Console.WriteLine($"   ❌ Нет выбранных сфер. Указано: '{activitiesText}'");
                        }
                    }
                    catch
                    {
                        companiesWithWrongActivities++;
                    }
                }

                Console.WriteLine($"\n📊 Статистика сфер деятельности:");
                Console.WriteLine($"   - С выбранными сферами: {companiesWithValidActivities}");
                Console.WriteLine($"   - Без сфер (пропущено): {companiesWithNoActivities}");
                Console.WriteLine($"   - С неверными сферами: {companiesWithWrongActivities}");

                if (selectedActivitiesList.Count > 0)
                {
                    if (companiesWithWrongActivities > 0)
                        Assert.True(false, $"❌ У {companiesWithWrongActivities} компаний нет выбранных сфер деятельности");

                    Console.WriteLine($"✅ 4. Проверка сфер: {companiesWithValidActivities} компаний имеют выбранную сферу");
                }
                else
                {
                    Console.WriteLine("✅ 4. Проверка сфер: фильтры не выбраны, пропущена");
                }

                // ========== ПРОВЕРКА: НЕТ ПРОИЗВОДИТЕЛЕЙ ==========
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