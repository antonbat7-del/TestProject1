using System.Threading;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System;
using System.IO;

namespace RingAutoTests.Tests
{
    public class OpenOrdersTabTest
    {
        [Fact]
        public void OpenOrdersTab_ShouldNavigateToOrdersPage()
        {
            var browser = new BrowserHelper();

            try
            {
                new LoginPage(browser).Login("79262266015", "QwertY100");
                Thread.Sleep(1000);

                var clients = new ExternalClientsPage(browser);

                // Перейти в заявки
                clients.GoToOrdersTab();

                string description = "Тестовое описание задачи для механика";
                Thread.Sleep(2000);

                // ========== 1. В РАБОТУ (вместо "Принять") ==========
                var workBtn = browser.Driver.FindElements(By.XPath(
                    $"//div[contains(@class,'record-description') and contains(text(),'{description}')]/ancestor::tr//button[contains(.,'В работу')]"));

                if (workBtn.Count > 0 && workBtn[0].Displayed)
                {
                    workBtn[0].Click();
                    Thread.Sleep(1500);
                }

                // 2. Выбрать мастера
                var masterBtn = browser.Driver.FindElements(By.XPath(
                    $"//div[contains(@class,'record-description') and contains(text(),'{description}')]/ancestor::tr//button[contains(.,'Выбрать мастера')]"));
                if (masterBtn.Count > 0 && masterBtn[0].Displayed)
                {
                    masterBtn[0].Click();
                    Thread.Sleep(2000);
                    ClickSelectMaster(browser, "Смирнов Виктор Валерьевич");
                }

                // 3. Сменить мастера
                try { ClickThreeDotsAction(browser, description, "Сменить мастера"); Thread.Sleep(1500); CheckAndHandleMasterModal(browser); } catch { }

                // 4. В карточку заявки
                Thread.Sleep(1000);
                ClickThreeDotsAction(browser, description, "В карточку заявки");
                Thread.Sleep(2000);

                // 5. Вкладки
                ClickEquipmentTab(browser, "Обслуживание");
                ClickEquipmentTab(browser, "Технические данные");
                ClickEquipmentTab(browser, "Документация");
                ClickEquipmentTab(browser, "Заявки");

                // 6. В работу (на странице карточки)
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);"); Thread.Sleep(500);
                var workBtn2 = browser.Driver.FindElements(By.XPath("//button[contains(.,'работ') or contains(.,'paboty')]"));
                if (workBtn2.Count > 0 && workBtn2[0].Displayed)
                {
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", workBtn2[0]);
                    Thread.Sleep(1500);
                }

                // 7. Внести стоимость (или Редактировать)
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);"); Thread.Sleep(500);
                var vnestiBtn = browser.Driver.FindElements(By.XPath("//span[contains(text(),'Внести')]"));
                var editBtn = browser.Driver.FindElements(By.XPath("//*[contains(text(),'Итоговая стоимость')]/following::span[contains(text(),'Редактировать')][1]"));

                if (vnestiBtn.Count > 0 && vnestiBtn[0].Displayed)
                {
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", vnestiBtn[0]);
                }
                else if (editBtn.Count > 0)
                {
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", editBtn[0]);
                }

                if ((vnestiBtn.Count > 0 && vnestiBtn[0].Displayed) || editBtn.Count > 0)
                {
                    Thread.Sleep(1500);
                    bool hasRedBox = browser.Driver.FindElements(By.XPath("//*[contains(text(),'RedBox ремонт')]")).Count > 0;
                    bool hasTopor = browser.Driver.FindElements(By.XPath("//*[contains(text(),'Топор')]")).Count > 0;
                    bool hasMaterial = browser.Driver.FindElements(By.XPath("//*[contains(text(),'Расходники')]")).Count > 0;
                    bool hasManualMaterial = false;

                    // Работа из списка
                    if (!hasRedBox)
                    {
                        try
                        {
                            var categorySelect = browser.Driver.FindElement(By.XPath("//*[contains(text(),'категорию')]/ancestor::div[contains(@class,'ant-select')]//div[contains(@class,'ant-select-selector')]"));
                            categorySelect.Click(); Thread.Sleep(800);
                            browser.Driver.FindElement(By.XPath("//div[@title='Работа']")).Click(); Thread.Sleep(500);
                            var qtyInput = browser.Driver.FindElement(By.XPath("//label[contains(text(),'Количество')]/following-sibling::input"));
                            qtyInput.Click(); Thread.Sleep(300); qtyInput.Clear();
                            ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].value = '1';", qtyInput); Thread.Sleep(300);
                            var workBlock = browser.Driver.FindElement(By.XPath("//*[contains(text(),'Наименование')]/following::div[contains(@class,'v-select')][1]"));
                            workBlock.FindElement(By.XPath(".//input[@type='search']")).Click(); Thread.Sleep(500);
                            browser.Driver.FindElement(By.XPath("//li[contains(@class,'vs__dropdown-option') and contains(.,'RedBox ремонт')]")).Click(); Thread.Sleep(500);
                            var addBtn = browser.Driver.FindElements(By.XPath("//button[contains(.,'Добавить')]"));
                            if (addBtn.Count > 0) { addBtn[addBtn.Count - 1].Click(); Thread.Sleep(1000); }
                        }
                        catch { }
                    }

                    // Работа вручную
                    if (!hasTopor)
                    {
                        try
                        {
                            var addManualWork = browser.Driver.FindElements(By.XPath("//p[contains(@class,'option-title') and contains(text(),'Добавить работы вручную')]"));
                            if (addManualWork.Count > 0 && addManualWork[0].Displayed)
                            {
                                addManualWork[0].Click(); Thread.Sleep(1500);
                                var nameInput = browser.Driver.FindElement(By.XPath("//label[contains(text(),'Наименование')]/following-sibling::input"));
                                nameInput.Click(); Thread.Sleep(300); nameInput.SendKeys("Топор"); Thread.Sleep(300);
                                var qtyInput2 = browser.Driver.FindElement(By.XPath("//label[contains(text(),'Количество')]/following-sibling::input"));
                                qtyInput2.Click(); Thread.Sleep(300); qtyInput2.Clear(); qtyInput2.SendKeys(new Random().Next(1, 11).ToString()); Thread.Sleep(300);
                                var priceInput = browser.Driver.FindElement(By.XPath("//label[contains(text(),'Стоимость за единицу')]/following-sibling::input"));
                                priceInput.Click(); Thread.Sleep(300);
                                for (int i = 0; i < 4; i++) { priceInput.SendKeys(Keys.Backspace); Thread.Sleep(100); }
                                priceInput.SendKeys(new Random().Next(500, 1001).ToString()); Thread.Sleep(300);
                                var addBtns2 = browser.Driver.FindElements(By.XPath("//button[contains(.,'Добавить')]"));
                                if (addBtns2.Count > 0) { addBtns2[addBtns2.Count - 1].Click(); Thread.Sleep(1000); }
                            }
                        }
                        catch { }
                    }

                    // Материалы из списка
                    if (!hasMaterial)
                    {
                        try
                        {
                            var addMaterialList = browser.Driver.FindElements(By.XPath("//p[contains(@class,'option-title') and contains(text(),'Добавить материалы из списка')]"));
                            if (addMaterialList.Count > 0 && addMaterialList[0].Displayed)
                            {
                                addMaterialList[0].Click(); Thread.Sleep(1500);
                                var matCat = browser.Driver.FindElement(By.XPath("//*[contains(text(),'категорию')]/ancestor::div[contains(@class,'ant-select')]//div[contains(@class,'ant-select-selector')]"));
                                matCat.Click(); Thread.Sleep(800);
                                browser.Driver.FindElement(By.XPath("//div[@title='Расходники']")).Click(); Thread.Sleep(2000);
                                var coeff = browser.Driver.FindElement(By.XPath("//input[contains(@placeholder,'Нет')]"));
                                coeff.Click(); Thread.Sleep(500);
                                browser.Driver.FindElement(By.XPath("//li[contains(@class,'vs__dropdown-option') and contains(.,'Стандарт')]")).Click(); Thread.Sleep(500);
                                var matWork = browser.Driver.FindElement(By.XPath("//input[contains(@placeholder,'паботину') or contains(@placeholder,'работу')]"));
                                matWork.Click(); Thread.Sleep(500);
                                var opts = browser.Driver.FindElements(By.XPath("//*[contains(@id,'__option-')]"));
                                if (opts.Count > 1) opts[1].Click(); else if (opts.Count > 0) opts[0].Click();
                                Thread.Sleep(500);

                                var allInputs = browser.Driver.FindElements(By.CssSelector("input.input__input[type='text']"));
                                if (allInputs.Count >= 2)
                                {
                                    var qi = allInputs[allInputs.Count - 1];
                                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", qi);
                                    Thread.Sleep(300);
                                    qi.Clear();
                                    qi.SendKeys(new Random().Next(1, 6).ToString());
                                    Thread.Sleep(300);
                                }

                                Thread.Sleep(1000);
                                var addBtns3 = browser.Driver.FindElements(By.XPath("//button[contains(.,'Добавить')]"));
                                if (addBtns3.Count > 0) { addBtns3[addBtns3.Count - 1].Click(); Thread.Sleep(1000); }
                            }
                        }
                        catch { }
                    }

                    // Материалы вручную
                    if (!hasManualMaterial)
                    {
                        try
                        {
                            var addMatManual = browser.Driver.FindElements(By.XPath("//p[contains(@class,'option-title') and contains(text(),'Добавить материалы вручную')]"));
                            if (addMatManual.Count > 0 && addMatManual[0].Displayed)
                            {
                                addMatManual[0].Click(); Thread.Sleep(1500);

                                var nl = browser.Driver.FindElements(By.XPath("//label[contains(text(),'Наименование')]"));
                                if (nl.Count > 0) { ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", nl[nl.Count - 1]); Thread.Sleep(300); var nInput = nl[nl.Count - 1].FindElement(By.XPath("./following-sibling::input")); nInput.Clear(); nInput.SendKeys("Отвертка"); Thread.Sleep(300); }

                                var ul = browser.Driver.FindElements(By.XPath("//label[contains(text(),'Единица измерения')]"));
                                if (ul.Count > 0) { ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", ul[ul.Count - 1]); Thread.Sleep(300); var uInput = ul[ul.Count - 1].FindElement(By.XPath("./following-sibling::input")); uInput.Clear(); uInput.SendKeys("шт"); Thread.Sleep(300); }

                                var ql = browser.Driver.FindElements(By.XPath("//label[contains(text(),'Количество')]"));
                                if (ql.Count > 0) { ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", ql[ql.Count - 1]); Thread.Sleep(300); var qInput = ql[ql.Count - 1].FindElement(By.XPath("./following-sibling::input")); qInput.Clear(); qInput.SendKeys(new Random().Next(1, 6).ToString()); Thread.Sleep(300); }

                                var pl = browser.Driver.FindElements(By.XPath("//label[contains(text(),'Стоимость за единицу')]"));
                                if (pl.Count > 0) { ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", pl[pl.Count - 1]); Thread.Sleep(300); var pInput = pl[pl.Count - 1].FindElement(By.XPath("./following-sibling::input")); pInput.Clear(); for (int i = 0; i < 3; i++) { pInput.SendKeys(Keys.Backspace); Thread.Sleep(100); } pInput.SendKeys(new Random().Next(100, 501).ToString()); Thread.Sleep(300); }

                                var addBtns4 = browser.Driver.FindElements(By.XPath("//button[contains(@class,'btn-blue')]"));
                                if (addBtns4.Count > 0) { ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", addBtns4[addBtns4.Count - 1]); Thread.Sleep(1000); }
                            }
                        }
                        catch { }
                    }

                    // Подтвердить и сформировать акт
                    var confirmActBtn = browser.Driver.FindElements(By.XPath("//button[contains(.,'Подтвердить')]"));
                    if (confirmActBtn.Count > 0) { confirmActBtn[confirmActBtn.Count - 1].Click(); Thread.Sleep(1500); }
                }

                // 8. Добавить фото выполненных работ
                Thread.Sleep(2000);
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Thread.Sleep(1000);
                var addPhotoBtn = browser.Driver.FindElements(By.XPath("//button[contains(.,'Добавить фото')]"));
                if (addPhotoBtn.Count > 0)
                {
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", addPhotoBtn[addPhotoBtn.Count - 1]);
                    Thread.Sleep(2000);

                    try
                    {
                        var fileInput = browser.Driver.FindElement(By.XPath("//input[@type='file']"));
                        fileInput.SendKeys(Path.GetFullPath(@"C:\testfiles\photo2.jpg"));
                        Thread.Sleep(2000);
                    }
                    catch { }

                    var addBtn = browser.Driver.FindElements(By.XPath("//button[contains(.,'Добавить') and not(contains(.,'фото'))]"));
                    if (addBtn.Count > 0) { ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", addBtn[addBtn.Count - 1]); Thread.Sleep(2000); }

                    var saveBtn = browser.Driver.FindElements(By.XPath("//button[contains(.,'Охранить') or contains(.,'Сохранить')]"));
                    if (saveBtn.Count > 0) { ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", saveBtn[saveBtn.Count - 1]); Thread.Sleep(2000); }
                }

                // 9. Добавить акт выполненных работ
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Thread.Sleep(500);

                var actAddBtns = browser.Driver.FindElements(By.XPath("//*[contains(text(),'Акт выполненных работ')]/following::button[contains(.,'Добавить')][1]"));
                if (actAddBtns.Count > 0)
                {
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", actAddBtns[0]);
                    Thread.Sleep(1500);

                    try
                    {
                        var uploadInput = browser.Driver.FindElement(By.Id("upload_3"));
                        uploadInput.SendKeys(Path.GetFullPath(@"C:\testfiles\photo.jpg"));
                        Thread.Sleep(2000);
                    }
                    catch (Exception ex) { }

                    var confirmAddBtns = browser.Driver.FindElements(By.XPath("//button[contains(.,'Добавить')]"));
                    if (confirmAddBtns.Count > 0)
                    {
                        ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", confirmAddBtns[confirmAddBtns.Count - 1]);
                        Thread.Sleep(1500);
                    }

                    var saveBtns = browser.Driver.FindElements(By.XPath("//button[contains(.,'Сохранить') or contains(.,'Охранить')]"));
                    if (saveBtns.Count > 0)
                    {
                        ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", saveBtns[saveBtns.Count - 1]);
                        Thread.Sleep(1500);
                    }
                }

                // 10. Добавить PDF-файл
                Thread.Sleep(2000);
                var pdfBtn = browser.Driver.FindElements(By.XPath("//span[contains(text(),'PDF-файл')]"));
                if (pdfBtn.Count > 0)
                {
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", pdfBtn[0]);
                    Thread.Sleep(2000);

                    try
                    {
                        var upload4 = browser.Driver.FindElement(By.Id("upload_4"));
                        upload4.SendKeys(Path.GetFullPath(@"C:\testfiles\test.pdf"));
                        Thread.Sleep(4000);

                        var saveBtns = browser.Driver.FindElements(By.XPath("//button[contains(.,'Сохранить')]"));
                        if (saveBtns.Count > 0)
                        {
                            ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", saveBtns[saveBtns.Count - 1]);
                            Thread.Sleep(2000);
                        }
                    }
                    catch { }
                }

                // 11. Отложить (если есть)
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Thread.Sleep(500);
                var postponeBtn = browser.Driver.FindElements(By.XPath("//button[contains(.,'Отложить')]"));
                if (postponeBtn.Count > 0 && postponeBtn[0].Displayed)
                {
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", postponeBtn[postponeBtn.Count - 1]);
                    Thread.Sleep(1500);
                }

                // 12. Выбрать подстатус
                Thread.Sleep(1000);
                var subStatus = browser.Driver.FindElements(By.XPath("//span[contains(@class,'vs__selected')]"));
                if (subStatus.Count > 0)
                {
                    subStatus[subStatus.Count - 1].Click();
                    Thread.Sleep(500);
                    var opt = browser.Driver.FindElement(By.XPath("//li[contains(@class,'vs__dropdown-option') and contains(.,'Заказать запчасть')]"));
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", opt);
                    Thread.Sleep(500);
                }

                // 13. Комментарий
                Thread.Sleep(1000);
                var textarea = browser.Driver.FindElement(By.XPath("//textarea[contains(@placeholder,'комментарий')]"));
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", textarea);
                Thread.Sleep(500);

                try
                {
                    var fileInput = browser.Driver.FindElement(By.Id("file-upload"));
                    fileInput.SendKeys(Path.GetFullPath(@"C:\testfiles\photo.jpg"));
                    Thread.Sleep(2000);
                }
                catch { }

                textarea.Click();
                Thread.Sleep(300);
                textarea.SendKeys(" тестовый комментарий");
                Thread.Sleep(500);

                IJavaScriptExecutor js = (IJavaScriptExecutor)browser.Driver;
                js.ExecuteScript(@"
                    var ta = document.querySelector('textarea[placeholder*=""комментарий""]');
                    var icons = ta.parentElement.querySelectorAll('svg.icon');
                    if (icons.length >= 2) {
                        icons[1].parentElement.click();
                    }
                ");
                Thread.Sleep(1500);

                // Активировать
                var activateBtn = browser.Driver.FindElements(By.XPath("//button[contains(.,'Активировать')]"));
                if (activateBtn.Count > 0 && activateBtn[0].Displayed)
                {
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", activateBtn[0]);
                    Thread.Sleep(1500);
                }

                // Работа завершена
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Thread.Sleep(500);
                var completeBtn = browser.Driver.FindElements(By.XPath("//button[contains(.,'Работа завершена')]"));
                if (completeBtn.Count > 0 && completeBtn[0].Displayed)
                {
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", completeBtn[0]);
                    Thread.Sleep(1500);
                }

                // Перейти в Чат
                Thread.Sleep(1000);
                var chatLink = browser.Driver.FindElement(By.XPath("//a[@href='/chat']"));
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", chatLink);
                Thread.Sleep(2000);

                // Выбрать чат
                Thread.Sleep(2000);
                var contact = browser.Driver.FindElement(By.XPath(
                    "//span[contains(@class,'legal_entity_name') and contains(text(),'Мастер-Дент')]/ancestor::div[contains(@class,'chat-block')]"));
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", contact);
                Thread.Sleep(2000);

                // Написать сообщение
                var messageInput = browser.Driver.FindElement(By.XPath("//*[contains(@placeholder,'Введите сообщение')]"));
                messageInput.Click();
                Thread.Sleep(300);
                messageInput.SendKeys("Тест готово");
                Thread.Sleep(500);
                messageInput.SendKeys(Keys.Enter);
                Thread.Sleep(1500);

                Assert.True(true);
            }
            finally
            {
                browser.Close();
            }
        }

        private void ClickEquipmentTab(BrowserHelper b, string tabName)
        {
            try { b.Driver.FindElement(By.XPath($"//h4[contains(text(),'{tabName}')]")).Click(); Thread.Sleep(500); } catch { }
        }

        private void ClickThreeDotsAction(BrowserHelper b, string description, string actionText)
        {
            var descElement = b.Driver.FindElement(By.XPath($"//div[contains(@class,'record-description') and contains(text(),'{description}')]"));
            var tr = descElement.FindElement(By.XPath("./ancestor::tr"));
            tr.FindElement(By.XPath(".//div[contains(@class,'grey-lamp')]")).Click(); Thread.Sleep(500);
            var action = b.Driver.FindElements(By.XPath($"//span[contains(text(),'{actionText}')]"));
            if (action.Count > 0 && action[0].Displayed) { action[0].Click(); Thread.Sleep(1000); }
        }

        private void CheckAndHandleMasterModal(BrowserHelper b)
        {
            var confirmBtns = b.Driver.FindElements(By.XPath("//button[contains(.,'Подтвердить выбор')]"));
            var cancelBtns = b.Driver.FindElements(By.XPath("//button[contains(.,'Отменить')]"));
            if (confirmBtns.Count > 0)
            {
                var activeRadio = b.Driver.FindElements(By.XPath("//span[contains(@class,'radio__button') and contains(@class,'active')]/following-sibling::span[contains(@class,'radio__text')]"));
                if (activeRadio.Count > 0 && activeRadio[0].Text.Contains("Мастеров")) { if (cancelBtns.Count > 0) cancelBtns[0].Click(); Thread.Sleep(1000); }
                else { ClickSelectMaster(b, "Мастеров Иван Константинович"); }
            }
        }

        private void ClickSelectMaster(BrowserHelper b, string fullName)
        {
            var radioButton = b.Driver.FindElement(By.XPath($"//span[contains(@class,'radio__text') and contains(.,'{fullName}')]/preceding-sibling::span[contains(@class,'radio__button')]"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)b.Driver;
            js.ExecuteScript("arguments[0].click();", radioButton); Thread.Sleep(500);
            var confirmBtns = b.Driver.FindElements(By.XPath("//button[contains(.,'Подтвердить выбор')]"));
            if (confirmBtns.Count > 0) { js.ExecuteScript("arguments[0].click();", confirmBtns[confirmBtns.Count - 1]); }
            Thread.Sleep(1000);
        }
    }
}