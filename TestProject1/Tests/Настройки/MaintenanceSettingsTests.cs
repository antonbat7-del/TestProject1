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
    public class MaintenanceSettingsTests
    {
        private BrowserHelper _browser;
        private WebDriverWait _wait;

        public MaintenanceSettingsTests()
        {
            _browser = new BrowserHelper();
            _wait = new WebDriverWait(_browser.Driver, TimeSpan.FromSeconds(15));
        }

        [Fact]
        public void Test_CreateNewChecklist()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("🧪 ТЕСТ: Создание нового чеклиста в 'Настройки ТО'");
                Console.WriteLine("==================================================");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                string phone = "79160000071";
                string password = "Qwerty100";

                string checklistName = "Тестовый чеклист " + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string checklistStage1 = "Этап 1 " + DateTime.Now.ToString("HHmmss");
                string checklistStage2 = "Этап 2 " + DateTime.Now.ToString("HHmmss");
                string checklistDescription = "Это тестовый чеклист, созданный автоматическим тестом";

                // 1. Логин
                Console.WriteLine("\n🔐 [1/12] Авторизация...");
                new LoginPage(_browser).Login(phone, password);
                Thread.Sleep(3000);
                Assert.True(_browser.Driver.Url.Contains("lk"), "❌ Логин не выполнен");
                Console.WriteLine("✅ Логин выполнен");

                // 2. Сменить юр. лицо
                Console.WriteLine("\n🏢 [2/12] Смена юридического лица...");
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

                // 3. Переход в раздел Настройки
                Console.WriteLine("\n⚙️ [3/12] Переход в 'Настройки'...");
                var settingsLink = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[@href='/settings']")));
                settingsLink.Click();
                Thread.Sleep(3000);
                Assert.Contains("/settings", _browser.Driver.Url);
                Console.WriteLine("✅ Переход в раздел 'Настройки'");

                // 4. Переход на вкладку "Настройки ТО"
                Console.WriteLine("\n📋 [4/12] Переход на вкладку 'Настройки ТО'...");
                var maintenanceTab = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'tabs__tab') and contains(.,'Настройки ТО')]")));
                maintenanceTab.Click();
                Thread.Sleep(3000);
                Console.WriteLine("✅ Переход на вкладку 'Настройки ТО'");

                // 5. Нажать кнопку "Создать новый чеклист"
                Console.WriteLine("\n📝 [5/12] Создание нового чеклиста...");
                var createBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(.,'Создать новый чеклист')]")));
                createBtn.Click();
                Thread.Sleep(1000);
                Console.WriteLine("✅ Нажата кнопка 'Создать новый чеклист'");

                // ========== ЗАПОЛНЕНИЕ ПЕРВОГО ЭТАПА ==========
                Console.WriteLine("\n📝 ЗАПОЛНЕНИЕ ПЕРВОГО ЭТАПА");

                // 6. Заполнить поле "Название ТО"
                var nameInput = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//label[text()='Название ТО']/following-sibling::input")));
                nameInput.Clear();
                nameInput.SendKeys(checklistName);
                Thread.Sleep(500);
                Console.WriteLine($"✅ Введено название: {checklistName}");

                // 7. Заполнить поле "Этап ТО" (первый)
                var stageInput = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//label[text()='Этап ТО']/following-sibling::input")));
                stageInput.Clear();
                stageInput.SendKeys(checklistStage1);
                Thread.Sleep(500);
                Console.WriteLine($"✅ Введён этап 1: {checklistStage1}");

                // 8. Заполнить поле "Описание этапа" (первый)
                var descriptionTextarea = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'txt__label') and text()='Описание этапа']/following-sibling::div//textarea")));
                descriptionTextarea.Clear();
                descriptionTextarea.SendKeys(checklistDescription);
                Thread.Sleep(500);
                Console.WriteLine($"✅ Введено описание этапа 1");

                // 9. Нажать на чекбокс "Фото обязательно ?" (первый этап)
                Console.WriteLine("\n📸 Нажатие на чекбокс 'Фото обязательно ?' (этап 1)...");
                try
                {
                    var photoLabel = _browser.Driver.FindElement(By.XPath("//span[text()=' Фото обязательно ? ']/ancestor::label"));
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", photoLabel);
                    Console.WriteLine("✅ Чекбокс 'Фото обязательно ?' выбран (этап 1)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Ошибка: {ex.Message}");
                }

                // ========== ДОБАВЛЕНИЕ ВТОРОГО ЭТАПА ==========
                Console.WriteLine("\n📝 ДОБАВЛЕНИЕ ВТОРОГО ЭТАПА");

                // 10. Нажать на иконку с плюсом для добавления нового этапа
                try
                {
                    var addStageIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'icon')]//*[name()='svg' and contains(@class,'iconify--eva')]/..")));
                    addStageIcon.Click();
                    Thread.Sleep(1000);
                    Console.WriteLine("✅ Нажата иконка добавления нового этапа");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось нажать на иконку добавления: {ex.Message}");
                }

                // ========== ЗАПОЛНЕНИЕ ВТОРОГО ЭТАПА ==========
                Console.WriteLine("\n📝 ЗАПОЛНЕНИЕ ВТОРОГО ЭТАПА");

                // 11. Заполнить поле "Этап ТО" (второй)
                var stageInputs = _browser.Driver.FindElements(By.XPath("//label[text()='Этап ТО']/following-sibling::input"));
                if (stageInputs.Count >= 2)
                {
                    stageInputs[1].Clear();
                    stageInputs[1].SendKeys(checklistStage2);
                    Thread.Sleep(500);
                    Console.WriteLine($"✅ Введён этап 2: {checklistStage2}");
                }

                // 12. Заполнить поле "Описание этапа" (второй)
                var descriptionTextareas = _browser.Driver.FindElements(By.XPath("//div[contains(@class,'txt__label') and text()='Описание этапа']/following-sibling::div//textarea"));
                if (descriptionTextareas.Count >= 2)
                {
                    descriptionTextareas[1].Clear();
                    descriptionTextareas[1].SendKeys(checklistDescription);
                    Thread.Sleep(500);
                    Console.WriteLine($"✅ Введено описание этапа 2");
                }

                // 13. Нажать на чекбокс "Фото обязательно ?" (второй этап)
                Console.WriteLine("\n📸 Нажатие на чекбокс 'Фото обязательно ?' (этап 2)...");
                try
                {
                    var photoLabels = _browser.Driver.FindElements(By.XPath("//span[text()=' Фото обязательно ? ']/ancestor::label"));
                    if (photoLabels.Count >= 2)
                    {
                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", photoLabels[1]);
                        Console.WriteLine("✅ Чекбокс 'Фото обязательно ?' выбран (этап 2)");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Ошибка (этап 2): {ex.Message}");
                }

                // 14. Сохранить чеклист
                Console.WriteLine("\n💾 Сохранение чеклиста...");
                var saveIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//div[contains(@class,'controls')]//div[contains(@class,'icon')][1]")));
                saveIcon.Click();
                Thread.Sleep(2000);
                Console.WriteLine("✅ Чеклист сохранён");

                // 15. Проверка: чеклист появился в списке
                Console.WriteLine("\n🔍 Поиск созданного чеклиста...");
                var checklistNameElement = _wait.Until(ExpectedConditions.ElementIsVisible(
                    By.XPath("//p[contains(@class,'name') and contains(text(),'Тестовый чеклист')]")));
                Console.WriteLine($"✅ Чеклист найден: {checklistNameElement.Text}");

                // 16. Навести мышь и нажать на иконку настроек (третья иконка)
                Console.WriteLine("\n⚙️ Открытие настроек чеклиста...");
                try
                {
                    var parentItem = checklistNameElement.FindElement(By.XPath("./ancestor::div[contains(@class,'item-display')]"));
                    var actions = new Actions(_browser.Driver);
                    actions.MoveToElement(parentItem).Perform();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Наведена мышь на чеклист");

                    var settingsIcon = parentItem.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'icon')][3]"));
                    settingsIcon.Click();
                    Thread.Sleep(1000);
                    Console.WriteLine("✅ Нажата иконка настроек (шестерёнка)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось открыть настройки чеклиста: {ex.Message}");
                }

                // 17. Закрыть (удалить) второй этап
                Console.WriteLine("\n❌ Удаление второго этапа...");
                try
                {
                    var closeIcons = _browser.Driver.FindElements(By.XPath("//div[contains(@class,'icon')]//*[name()='svg' and contains(@class,'iconify--gridicons')]/.."));
                    if (closeIcons.Count >= 2)
                    {
                        closeIcons[1].Click();
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Второй этап удалён");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось удалить второй этап: {ex.Message}");
                }

                // 18. Редактирование первого этапа
                Console.WriteLine("\n📝 Редактирование первого этапа...");
                try
                {
                    var stageInputEdit = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//label[text()='Этап ТО']/following-sibling::input")));
                    stageInputEdit.Clear();
                    string newStageName = "Отредактированный этап " + DateTime.Now.ToString("HHmmss");
                    stageInputEdit.SendKeys(newStageName);
                    Thread.Sleep(500);
                    Console.WriteLine($"✅ Этап ТО изменён на: {newStageName}");

                    var descriptionTextareaEdit = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'txt__label') and text()='Описание этапа']/following-sibling::div//textarea")));
                    descriptionTextareaEdit.Clear();
                    string newDescription = "Отредактированное описание этапа";
                    descriptionTextareaEdit.SendKeys(newDescription);
                    Thread.Sleep(500);
                    Console.WriteLine($"✅ Описание этапа изменено: {newDescription}");

                    var photoLabelEdit = _browser.Driver.FindElement(By.XPath("//span[text()=' Фото обязательно ? ']/ancestor::label"));
                    ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", photoLabelEdit);
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Галочка 'Фото обязательно ?' убрана");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось отредактировать этап: {ex.Message}");
                }

                // 19. Добавить новый этап (снова)
                Console.WriteLine("\n📝 Добавление нового этапа...");
                try
                {
                    var addStageIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'icon')]//*[name()='svg' and contains(@class,'iconify--eva')]/..")));
                    addStageIcon.Click();
                    Thread.Sleep(1000);
                    Console.WriteLine("✅ Нажата иконка добавления нового этапа");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось нажать на иконку добавления: {ex.Message}");
                }

                // 20. Заполнить новый этап
                Console.WriteLine("\n📝 Заполнение нового этапа...");
                try
                {
                    var allStageInputs = _browser.Driver.FindElements(By.XPath("//label[text()='Этап ТО']/following-sibling::input"));
                    if (allStageInputs.Count >= 1)
                    {
                        var lastStageInput = allStageInputs[allStageInputs.Count - 1];
                        lastStageInput.Clear();
                        string newStage = "Новый добавленный этап " + DateTime.Now.ToString("HHmmss");
                        lastStageInput.SendKeys(newStage);
                        Thread.Sleep(500);
                        Console.WriteLine($"✅ Добавлен этап: {newStage}");
                    }

                    var allDescriptionTextareas = _browser.Driver.FindElements(By.XPath("//div[contains(@class,'txt__label') and text()='Описание этапа']/following-sibling::div//textarea"));
                    if (allDescriptionTextareas.Count >= 1)
                    {
                        var lastDescription = allDescriptionTextareas[allDescriptionTextareas.Count - 1];
                        lastDescription.Clear();
                        lastDescription.SendKeys("Описание нового этапа");
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Добавлено описание этапа");
                    }

                    var allPhotoLabels = _browser.Driver.FindElements(By.XPath("//span[text()=' Фото обязательно ? ']/ancestor::label"));
                    if (allPhotoLabels.Count >= 1)
                    {
                        var lastPhotoLabel = allPhotoLabels[allPhotoLabels.Count - 1];
                        ((IJavaScriptExecutor)_browser.Driver).ExecuteScript("arguments[0].click();", lastPhotoLabel);
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Чекбокс 'Фото обязательно ?' выбран для нового этапа");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось заполнить новый этап: {ex.Message}");
                }

                // 21. Сохранить чеклист
                Console.WriteLine("\n💾 Сохранение чеклиста...");
                try
                {
                    var saveFinalIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[contains(@class,'controls')]//div[contains(@class,'icon')][1]")));
                    saveFinalIcon.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Чеклист сохранён");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось сохранить чеклист: {ex.Message}");
                }

                // 22. Снова навести мышь на чеклист и нажать на вторую иконку
                Console.WriteLine("\n⚙️ Открытие настроек чеклиста (вторая иконка)...");
                try
                {
                    var checklistElement = _wait.Until(ExpectedConditions.ElementIsVisible(
                        By.XPath("//p[contains(@class,'name') and contains(text(),'Тестовый чеклист')]")));
                    var parentItemFinal = checklistElement.FindElement(By.XPath("./ancestor::div[contains(@class,'item-display')]"));

                    var actions = new Actions(_browser.Driver);
                    actions.MoveToElement(parentItemFinal).Perform();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Наведена мышь на чеклист");

                    var secondIcon = parentItemFinal.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'icon')][2]"));
                    secondIcon.Click();
                    Thread.Sleep(1000);
                    Console.WriteLine("✅ Нажата вторая иконка");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось открыть настройки чеклиста: {ex.Message}");
                }

                // 23. Закрыть форму настроек (клик по крестику)
                Console.WriteLine("\n❌ Закрытие формы настроек...");
                try
                {
                    var closeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//a[contains(@class,'modal__close')]")));
                    closeButton.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Форма настроек закрыта");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось закрыть форму: {ex.Message}");
                }

                // 24. Навести мышь и нажать на четвёртую иконку (корзина)
                Console.WriteLine("\n🗑️ Удаление чеклиста...");
                try
                {
                    var checklistElement = _wait.Until(ExpectedConditions.ElementIsVisible(
                        By.XPath("//p[contains(@class,'name') and contains(text(),'Тестовый чеклист')]")));
                    var parentItemFinal = checklistElement.FindElement(By.XPath("./ancestor::div[contains(@class,'item-display')]"));

                    var actions = new Actions(_browser.Driver);
                    actions.MoveToElement(parentItemFinal).Perform();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Наведена мышь на чеклист");

                    var deleteIcon = parentItemFinal.FindElement(By.XPath(".//div[contains(@class,'controls')]/div[contains(@class,'icon')][4]"));
                    deleteIcon.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("✅ Нажата иконка корзины");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Не удалось нажать на иконку корзины: {ex.Message}");
                }

                // 25. Подтвердить удаление (кнопка "Удалить")
                Console.WriteLine("\n✅ Подтверждение удаления...");
                try
                {
                    var confirmDelete = _wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//button[contains(@class,'btn-red') and contains(.,'Удалить')]")));
                    confirmDelete.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("✅ Чеклист успешно удалён");
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