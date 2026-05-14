using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using RingAutoTests.Helpers;

namespace RingAutoTests.Pages
{
    public class OrderActionsPage
    {
        private BrowserHelper _b;

        public OrderActionsPage(BrowserHelper browser)
        {
            _b = browser;
        }

        public void GoToOrders()
        {
            var link = _b.WaitClickable(By.XPath("//a[@href='/orders']"));
            link.Click();
            Thread.Sleep(3000);
            _b.Wait.Until(d => d.FindElements(By.CssSelector("div.record-description")).Count > 0);
            Thread.Sleep(1000);
        }

        public void ProcessRequest(string description)
        {
            _b.Wait.Until(d => d.FindElements(
                By.XPath($"//div[contains(@class,'record-description') and contains(text(),'{description}')]")).Count > 0);
            Thread.Sleep(1000);

            // 1. Принять
            var acceptBtn = _b.Driver.FindElements(By.XPath(
                $"//div[contains(@class,'record-description') and contains(text(),'{description}')]/ancestor::tr//button[contains(.,'Принять')]"));
            if (acceptBtn.Count > 0 && acceptBtn[0].Displayed)
            {
                acceptBtn[0].Click();
                Thread.Sleep(1500);
            }

            // 2. Выбрать мастера (с ожиданием появления кнопки)
            _b.Wait.Until(d => d.FindElements(By.XPath(
                $"//div[contains(@class,'record-description') and contains(text(),'{description}')]/ancestor::tr//button[contains(.,'Выбрать мастера')]")).Count > 0);
            Thread.Sleep(500);

            var masterBtn = _b.Driver.FindElements(By.XPath(
                $"//div[contains(@class,'record-description') and contains(text(),'{description}')]/ancestor::tr//button[contains(.,'Выбрать мастера')]"));
            if (masterBtn.Count > 0 && masterBtn[0].Displayed)
            {
                masterBtn[0].Click();
                Thread.Sleep(2000);
                ClickSelectMaster("Смирнов Виктор Валерьевич");
            }

            // 3. Сменить мастера
            try { ClickThreeDotsAction(description, "Сменить мастера"); Thread.Sleep(1500); CheckAndHandleMasterModal(); } catch { }

            // 4. В карточку заявки
            Thread.Sleep(1000);
            ClickThreeDotsAction(description, "В карточку заявки");
            Thread.Sleep(2000);

            // 5. Вкладки
            ClickEquipmentTab("Обслуживание");
            ClickEquipmentTab("Технические данные");
            ClickEquipmentTab("Документация");
            ClickEquipmentTab("Заявки");

            // 6. В работу
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);"); Thread.Sleep(500);
            var workBtn = _b.Driver.FindElements(By.XPath("//button[contains(.,'работ') or contains(.,'paboty')]"));
            if (workBtn.Count > 0 && workBtn[0].Displayed) { ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", workBtn[0]); Thread.Sleep(1500); }

            // 7. Внести стоимость (или Редактировать)
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);"); Thread.Sleep(500);
            var vnestiBtn = _b.Driver.FindElements(By.XPath("//span[contains(text(),'Внести')]"));
            var editBtn = _b.Driver.FindElements(By.XPath("//*[contains(text(),'Итоговая стоимость')]/following::span[contains(text(),'Редактировать')][1]"));
            if (vnestiBtn.Count > 0 && vnestiBtn[0].Displayed) { ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", vnestiBtn[0]); }
            else if (editBtn.Count > 0) { ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", editBtn[0]); }

            if ((vnestiBtn.Count > 0 && vnestiBtn[0].Displayed) || editBtn.Count > 0)
            {
                Thread.Sleep(1500);
                bool hasRedBox = _b.Driver.FindElements(By.XPath("//*[contains(text(),'RedBox ремонт')]")).Count > 0;
                bool hasTopor = _b.Driver.FindElements(By.XPath("//*[contains(text(),'Топор')]")).Count > 0;
                bool hasMaterial = _b.Driver.FindElements(By.XPath("//*[contains(text(),'Расходники')]")).Count > 0;
                bool hasManualMaterial = false; // всегда заполняем

                // Работа из списка
                if (!hasRedBox)
                {
                    var categorySelect = _b.Driver.FindElement(By.XPath("//*[contains(text(),'категорию')]/ancestor::div[contains(@class,'ant-select')]//div[contains(@class,'ant-select-selector')]"));
                    categorySelect.Click(); Thread.Sleep(800);
                    _b.Driver.FindElement(By.XPath("//div[@title='Работа']")).Click(); Thread.Sleep(500);
                    var qtyInput = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Количество')]/following-sibling::input"));
                    qtyInput.Click(); Thread.Sleep(300); qtyInput.Clear();
                    ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].value = '1';", qtyInput); Thread.Sleep(300);
                    var workBlock = _b.Driver.FindElement(By.XPath("//*[contains(text(),'Наименование')]/following::div[contains(@class,'v-select')][1]"));
                    workBlock.FindElement(By.XPath(".//input[@type='search']")).Click(); Thread.Sleep(500);
                    _b.Driver.FindElement(By.XPath("//li[contains(@class,'vs__dropdown-option') and contains(.,'RedBox ремонт')]")).Click(); Thread.Sleep(500);
                    var addBtn = _b.Driver.FindElements(By.XPath("//button[contains(.,'Добавить')]"));
                    if (addBtn.Count > 0) { addBtn[addBtn.Count - 1].Click(); Thread.Sleep(1000); }
                }

                // Работа вручную
                if (!hasTopor)
                {
                    var addManualWork = _b.Driver.FindElements(By.XPath("//p[contains(@class,'option-title') and contains(text(),'Добавить работы вручную')]"));
                    if (addManualWork.Count > 0 && addManualWork[0].Displayed)
                    {
                        addManualWork[0].Click(); Thread.Sleep(1500);
                        var nameInput = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Наименование')]/following-sibling::input"));
                        nameInput.Click(); Thread.Sleep(300); nameInput.SendKeys("Топор"); Thread.Sleep(300);
                        var qtyInput2 = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Количество')]/following-sibling::input"));
                        qtyInput2.Click(); Thread.Sleep(300); qtyInput2.Clear(); qtyInput2.SendKeys(new Random().Next(1, 11).ToString()); Thread.Sleep(300);
                        var priceInput = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Стоимость за единицу')]/following-sibling::input"));
                        priceInput.Click(); Thread.Sleep(300);
                        for (int i = 0; i < 4; i++) { priceInput.SendKeys(Keys.Backspace); Thread.Sleep(100); }
                        priceInput.SendKeys(new Random().Next(500, 1001).ToString()); Thread.Sleep(300);
                        var addBtns2 = _b.Driver.FindElements(By.XPath("//button[contains(.,'Добавить')]"));
                        if (addBtns2.Count > 0) { addBtns2[addBtns2.Count - 1].Click(); Thread.Sleep(1000); }
                    }
                }

                // Материалы из списка
                if (!hasMaterial)
                {
                    var addMaterialList = _b.Driver.FindElements(By.XPath("//p[contains(@class,'option-title') and contains(text(),'Добавить материалы из списка')]"));
                    if (addMaterialList.Count > 0 && addMaterialList[0].Displayed)
                    {
                        addMaterialList[0].Click(); Thread.Sleep(1500);
                        var matCat = _b.Driver.FindElement(By.XPath("//*[contains(text(),'категорию')]/ancestor::div[contains(@class,'ant-select')]//div[contains(@class,'ant-select-selector')]"));
                        matCat.Click(); Thread.Sleep(800);
                        _b.Driver.FindElement(By.XPath("//div[@title='Расходники']")).Click(); Thread.Sleep(2000);
                        var coeff = _b.Driver.FindElement(By.XPath("//input[contains(@placeholder,'Нет')]"));
                        coeff.Click(); Thread.Sleep(500);
                        _b.Driver.FindElement(By.XPath("//li[contains(@class,'vs__dropdown-option') and contains(.,'Стандарт')]")).Click(); Thread.Sleep(500);
                        var matWork = _b.Driver.FindElement(By.XPath("//input[contains(@placeholder,'паботину') or contains(@placeholder,'работу')]"));
                        matWork.Click(); Thread.Sleep(500);
                        var opts = _b.Driver.FindElements(By.XPath("//*[contains(@id,'__option-')]"));
                        if (opts.Count > 1) opts[1].Click(); else if (opts.Count > 0) opts[0].Click();
                        Thread.Sleep(500);

                        // Количество
                        var allInputs = _b.Driver.FindElements(By.CssSelector("input.input__input[type='text']"));
                        if (allInputs.Count >= 2)
                        {
                            var qi = allInputs[allInputs.Count - 1];
                            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", qi);
                            Thread.Sleep(300);
                            qi.Clear();
                            qi.SendKeys(new Random().Next(1, 6).ToString());
                            Thread.Sleep(300);
                        }

                        // Добавить
                        Thread.Sleep(1000);
                        var addBtns3 = _b.Driver.FindElements(By.XPath("//button[contains(.,'Добавить')]"));
                        if (addBtns3.Count > 0) { addBtns3[addBtns3.Count - 1].Click(); Thread.Sleep(1000); }
                    }
                }

                // Материалы вручную
                if (!hasManualMaterial)
                {
                    var addMatManual = _b.Driver.FindElements(By.XPath("//p[contains(@class,'option-title') and contains(text(),'Добавить материалы вручную')]"));
                    if (addMatManual.Count > 0 && addMatManual[0].Displayed)
                    {
                        addMatManual[0].Click(); Thread.Sleep(1500);

                        // Наименование
                        var nl = _b.Driver.FindElements(By.XPath("//label[contains(text(),'Наименование')]"));
                        if (nl.Count > 0) { ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", nl[nl.Count - 1]); Thread.Sleep(300); var nInput = nl[nl.Count - 1].FindElement(By.XPath("./following-sibling::input")); nInput.Clear(); nInput.SendKeys("Отвертка"); Thread.Sleep(300); }

                        // Единица измерения
                        var ul = _b.Driver.FindElements(By.XPath("//label[contains(text(),'Единица измерения')]"));
                        if (ul.Count > 0) { ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", ul[ul.Count - 1]); Thread.Sleep(300); var uInput = ul[ul.Count - 1].FindElement(By.XPath("./following-sibling::input")); uInput.Clear(); uInput.SendKeys("шт"); Thread.Sleep(300); }

                        // Количество
                        var ql = _b.Driver.FindElements(By.XPath("//label[contains(text(),'Количество')]"));
                        if (ql.Count > 0) { ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", ql[ql.Count - 1]); Thread.Sleep(300); var qInput = ql[ql.Count - 1].FindElement(By.XPath("./following-sibling::input")); qInput.Clear(); qInput.SendKeys(new Random().Next(1, 6).ToString()); Thread.Sleep(300); }

                        // Стоимость за единицу
                        var pl = _b.Driver.FindElements(By.XPath("//label[contains(text(),'Стоимость за единицу')]"));
                        if (pl.Count > 0) { ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", pl[pl.Count - 1]); Thread.Sleep(300); var pInput = pl[pl.Count - 1].FindElement(By.XPath("./following-sibling::input")); pInput.Clear(); for (int i = 0; i < 3; i++) { pInput.SendKeys(Keys.Backspace); Thread.Sleep(100); } pInput.SendKeys(new Random().Next(100, 501).ToString()); Thread.Sleep(300); }

                        // Добавить
                        var addBtns4 = _b.Driver.FindElements(By.XPath("//button[contains(@class,'btn-blue')]"));
                        if (addBtns4.Count > 0) { ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", addBtns4[addBtns4.Count - 1]); Thread.Sleep(1000); }
                    }
                }

                // Подтвердить и сформировать акт
                var confirmActBtn = _b.Driver.FindElements(By.XPath("//button[contains(.,'Подтвердить')]"));
                if (confirmActBtn.Count > 0) { confirmActBtn[confirmActBtn.Count - 1].Click(); Thread.Sleep(1500); }

                // 8. Добавить фото выполненных работ
                Thread.Sleep(2000);
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Thread.Sleep(1000);
                var addPhotoBtn = _b.Driver.FindElements(By.XPath("//button[contains(.,'Добавить фото')]"));
                if (addPhotoBtn.Count > 0)
                {
                    ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", addPhotoBtn[addPhotoBtn.Count - 1]);
                    Thread.Sleep(2000);

                    try
                    {
                        var fileInput = _b.Driver.FindElement(By.XPath("//input[@type='file']"));
                        fileInput.SendKeys(Path.GetFullPath(@"C:\testfiles\photo2.jpg"));
                        Thread.Sleep(2000);
                    }
                    catch { }

                    var addBtn = _b.Driver.FindElements(By.XPath("//button[contains(.,'Добавить') and not(contains(.,'фото'))]"));
                    if (addBtn.Count > 0) { ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", addBtn[addBtn.Count - 1]); Thread.Sleep(2000); }

                    var saveBtn = _b.Driver.FindElements(By.XPath("//button[contains(.,'Охранить') or contains(.,'Сохранить')]"));
                    if (saveBtn.Count > 0) { ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", saveBtn[saveBtn.Count - 1]); Thread.Sleep(2000); }
                }

                // 9. Добавить акт выполненных работ
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Thread.Sleep(500);

                var actAddBtns = _b.Driver.FindElements(By.XPath("//*[contains(text(),'Акт выполненных работ')]/following::button[contains(.,'Добавить')][1]"));
                if (actAddBtns.Count > 0)
                {
                    ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", actAddBtns[0]);
                    Thread.Sleep(1500);

                    try
                    {
                        var uploadInput = _b.Driver.FindElement(By.Id("upload_3"));
                        uploadInput.SendKeys(Path.GetFullPath(@"C:\testfiles\photo.jpg"));
                        Thread.Sleep(2000);
                    }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Ошибка upload_3: " + ex.Message); }

                    var confirmAddBtns = _b.Driver.FindElements(By.XPath("//button[contains(.,'Добавить')]"));
                    if (confirmAddBtns.Count > 0)
                    {
                        ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", confirmAddBtns[confirmAddBtns.Count - 1]);
                        Thread.Sleep(1500);
                    }

                    var saveBtns = _b.Driver.FindElements(By.XPath("//button[contains(.,'Сохранить') or contains(.,'Охранить')]"));
                    if (saveBtns.Count > 0)
                    {
                        ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", saveBtns[saveBtns.Count - 1]);
                        Thread.Sleep(1500);
                    }
                }

                // 10. Добавить PDF-файл
                Thread.Sleep(2000);
                var pdfBtn = _b.Driver.FindElements(By.XPath("//span[contains(text(),'PDF-файл')]"));
                if (pdfBtn.Count > 0)
                {
                    ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", pdfBtn[0]);
                    Thread.Sleep(2000);

                    try
                    {
                        var upload4 = _b.Driver.FindElement(By.Id("upload_4"));
                        upload4.SendKeys(Path.GetFullPath(@"C:\testfiles\test.pdf"));
                        Thread.Sleep(4000); // увеличенная пауза

                        // Ждём появления кнопки «Сохранить»
                        _b.Wait.Until(d => d.FindElements(By.XPath("//button[contains(.,'Сохранить')]")).Count > 0);
                        var saveBtns = _b.Driver.FindElements(By.XPath("//button[contains(.,'Сохранить')]"));
                        if (saveBtns.Count > 0)
                        {
                            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", saveBtns[saveBtns.Count - 1]);
                            Thread.Sleep(2000);
                        }
                    }
                    catch { }
                }

                // 11. Отложить (если есть)
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Thread.Sleep(500);
                var postponeBtn = _b.Driver.FindElements(By.XPath("//button[contains(.,'Отложить')]"));
                if (postponeBtn.Count > 0 && postponeBtn[0].Displayed)
                {
                    ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", postponeBtn[postponeBtn.Count - 1]);
                    Thread.Sleep(1500);
                }

                // 12. Выбрать подстатус
                Thread.Sleep(1000);
                var subStatus = _b.Driver.FindElements(By.XPath("//span[contains(@class,'vs__selected')]"));
                if (subStatus.Count > 0)
                {
                    subStatus[subStatus.Count - 1].Click();
                    _b.Wait.Until(d => d.FindElements(By.XPath("//li[contains(@class,'vs__dropdown-option')]")).Count > 0);
                    Thread.Sleep(500);

                    var opt = _b.Driver.FindElement(By.XPath("//li[contains(@class,'vs__dropdown-option') and contains(.,'Заказать запчасть')]"));
                    ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", opt);
                    Thread.Sleep(500);
                }

                // 13. Комментарий + активировать + работа завершена + чат + сообщение
                Thread.Sleep(1000);

                var textarea = _b.Driver.FindElement(By.XPath("//textarea[contains(@placeholder,'комментарий')]"));
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", textarea);
                Thread.Sleep(500);

                // 1. Скрепка
                try
                {
                    var fileInput = _b.Driver.FindElement(By.Id("file-upload"));
                    fileInput.SendKeys(Path.GetFullPath(@"C:\testfiles\photo.jpg"));
                    Thread.Sleep(2000);
                }
                catch
                {
                    var allFileInputs = _b.Driver.FindElements(By.CssSelector("input[type='file']"));
                    if (allFileInputs.Count > 0)
                    {
                        allFileInputs[allFileInputs.Count - 1].SendKeys(Path.GetFullPath(@"C:\testfiles\photo.jpg"));
                        Thread.Sleep(2000);
                    }
                }

                // 2. @ — открыть и выбрать
                try
                {
                    var allIcons = _b.Driver.FindElements(By.XPath("//*[contains(@class,'iconify--ion')]"));
                    if (allIcons.Count >= 2)
                    {
                        new Actions(_b.Driver).MoveToElement(allIcons[allIcons.Count - 2]).Click().Perform();
                        Thread.Sleep(1500);

                        _b.Wait.Until(d => d.FindElements(By.XPath("//button[contains(@class,'dropdown__btn')]")).Count > 0);
                        Thread.Sleep(500);

                        var checkbox = _b.Driver.FindElement(By.XPath(
                            "//button[contains(@class,'dropdown__btn')][.//span[contains(text(),'Иванов Иван')]]//span[contains(@class,'chckbox__box')]"));
                        ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", checkbox);
                        Thread.Sleep(500);
                    }
                }
                catch { }

                // 3. Текст
                textarea.Click();
                Thread.Sleep(300);
                textarea.SendKeys(" тестовый комментарий");
                Thread.Sleep(500);

                // 4. Отправить
                IJavaScriptExecutor js = (IJavaScriptExecutor)_b.Driver;
                js.ExecuteScript(@"
    var ta = document.querySelector('textarea[placeholder*=""комментарий""]');
    var icons = ta.parentElement.querySelectorAll('svg.icon');
    if (icons.length >= 2) {
        icons[1].parentElement.click();
    }
");
                Thread.Sleep(1500);

                // 5. Активировать
                var activateBtn = _b.Driver.FindElements(By.XPath("//button[contains(.,'Активировать')]"));
                if (activateBtn.Count > 0 && activateBtn[0].Displayed)
                {
                    ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", activateBtn[0]);
                    Thread.Sleep(1500);
                }

                // 6. Работа завершена
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Thread.Sleep(500);
                var completeBtn = _b.Driver.FindElements(By.XPath("//button[contains(.,'Работа завершена')]"));
                if (completeBtn.Count > 0 && completeBtn[0].Displayed)
                {
                    ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", completeBtn[0]);
                    Thread.Sleep(1500);
                }

                // 7. Перейти в Чат
                Thread.Sleep(1000);
                var chatLink = _b.Driver.FindElement(By.XPath("//a[@href='/chat']"));
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", chatLink);
                Thread.Sleep(2000);

                // 8. Выбрать чат — клик по всей строке контакта
                Thread.Sleep(2000);
                _b.Wait.Until(d => d.FindElements(By.XPath("//div[contains(@class,'chat-block')]")).Count >= 3);
                Thread.Sleep(500);

                // Ищем контакт по тексту "Мастер-Дент"
                var contact = _b.Driver.FindElement(By.XPath(
                    "//span[contains(@class,'legal_entity_name') and contains(text(),'Мастер-Дент')]/ancestor::div[contains(@class,'chat-block')]"));
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", contact);
                Thread.Sleep(2000);

                // 9. Написать сообщение и отправить
                var messageInput = _b.Driver.FindElement(By.XPath("//*[contains(@placeholder,'Введите сообщение')]"));
                messageInput.Click();
                Thread.Sleep(300);
                messageInput.SendKeys("Тест готово");
                Thread.Sleep(500);
                messageInput.SendKeys(Keys.Enter);
                Thread.Sleep(1500);

                // 10. Открыть новую вкладку и войти под создателем
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("window.open();");
                var tabs = _b.Driver.WindowHandles;
                _b.Driver.SwitchTo().Window(tabs[1]);

                _b.Driver.Navigate().GoToUrl("https://lk.reeng.ru/");
                _b.WaitVisible(By.Id("phone")).SendKeys("79160000071");
                _b.WaitVisible(By.Id("password")).SendKeys("Qwerty100");
                _b.WaitClickable(By.Id("submit")).Click();
                Thread.Sleep(2000);

                var ordersLink = _b.WaitClickable(By.XPath("//a[@href='/orders']"));
                ordersLink.Click();
                Thread.Sleep(3000);

                var acceptBtn2 = _b.Driver.FindElements(By.XPath("//div[contains(@class,'record-description') and contains(text(),'Тестовая заявка ТО')]/ancestor::tr//button[contains(.,'Принять')]"));
                if (acceptBtn2.Count > 0 && acceptBtn2[0].Displayed)
                {
                    ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", acceptBtn2[0]);
                    Thread.Sleep(1500);
                }

                _b.Driver.Close();
                _b.Driver.SwitchTo().Window(tabs[0]);
            }
        }

        public void ClickEquipmentTab(string tabName)
        {
            try { _b.Driver.FindElement(By.XPath($"//h4[contains(text(),'{tabName}')]")).Click(); Thread.Sleep(500); } catch { }
        }

        public void AddWorkPhoto(string filePath)
        {
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);"); Thread.Sleep(1000);
            var workSection = _b.Driver.FindElement(By.XPath("//*[contains(text(),'Итоги работы')]"));
            var input = workSection.FindElement(By.XPath(".//input[@type='file']"));
            input.SendKeys(Path.GetFullPath(filePath)); Thread.Sleep(2000);
            var addBtns = workSection.FindElements(By.XPath(".//button[contains(.,'Добавить')]"));
            foreach (var btn in addBtns) { if (btn.Displayed && btn.Enabled) { btn.Click(); Thread.Sleep(1000); break; } }
        }

        private void ClickThreeDotsAction(string description, string actionText)
        {
            var descElement = _b.Driver.FindElement(By.XPath($"//div[contains(@class,'record-description') and contains(text(),'{description}')]"));
            var tr = descElement.FindElement(By.XPath("./ancestor::tr"));
            tr.FindElement(By.XPath(".//div[contains(@class,'grey-lamp')]")).Click(); Thread.Sleep(500);
            var action = _b.Driver.FindElements(By.XPath($"//span[contains(text(),'{actionText}')]"));
            if (action.Count > 0 && action[0].Displayed) { action[0].Click(); Thread.Sleep(1000); }
        }

        private void CheckAndHandleMasterModal()
        {
            var confirmBtns = _b.Driver.FindElements(By.XPath("//button[contains(.,'Подтвердить выбор')]"));
            var cancelBtns = _b.Driver.FindElements(By.XPath("//button[contains(.,'Отменить')]"));
            if (confirmBtns.Count > 0)
            {
                var activeRadio = _b.Driver.FindElements(By.XPath("//span[contains(@class,'radio__button') and contains(@class,'active')]/following-sibling::span[contains(@class,'radio__text')]"));
                if (activeRadio.Count > 0 && activeRadio[0].Text.Contains("Мастеров")) { if (cancelBtns.Count > 0) cancelBtns[0].Click(); Thread.Sleep(1000); }
                else { ClickSelectMaster("Мастеров Иван Константинович"); }
            }
        }

        private void ClickSelectMaster(string fullName)
        {
            var radioButton = _b.Driver.FindElement(By.XPath($"//span[contains(@class,'radio__text') and contains(.,'{fullName}')]/preceding-sibling::span[contains(@class,'radio__button')]"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)_b.Driver;
            js.ExecuteScript("arguments[0].click();", radioButton); Thread.Sleep(500);
            var confirmBtns = _b.Driver.FindElements(By.XPath("//button[contains(.,'Подтвердить выбор')]"));
            if (confirmBtns.Count > 0) { js.ExecuteScript("arguments[0].click();", confirmBtns[confirmBtns.Count - 1]); }
            Thread.Sleep(1000);
        }
    }
}