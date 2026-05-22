using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using RingAutoTests.Helpers;
using System.Collections.Generic;

namespace RingAutoTests.Pages
{
    public class ExternalClientsPage
    {
        private BrowserHelper _b;

        public ExternalClientsPage(BrowserHelper browser)
        {
            _b = browser;
        }

        public void GoToExternalClients()
        {
            var partnersLink = _b.WaitClickable(By.XPath("//a[@href='/partners']"));
            partnersLink.Click();
            Thread.Sleep(2000);

            var externalClientsTab = _b.WaitClickable(By.XPath("//div[contains(@class,'partners__link') and contains(text(),'Внешние клиенты')]"));
            externalClientsTab.Click();
            Thread.Sleep(2000);
        }

        public void ClickCreateOrganization()
        {
            var createBtn = _b.WaitClickable(By.XPath("//button[contains(@class,'btn-bordered-blue') and contains(.,'Создать')]"));
            createBtn.Click();
            Thread.Sleep(1500);

            var requisitesCheckbox = _b.WaitClickable(By.XPath("//span[contains(@class,'chckbox__box')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", requisitesCheckbox);
            Thread.Sleep(1000);
        }

        public void FillRequisites(string inn, string kpp, string ogrn, string name, string address)
        {
            Thread.Sleep(1000);

            var innInput = _b.Driver.FindElement(By.Id("inn"));
            innInput.Clear();
            innInput.SendKeys(inn);
            Thread.Sleep(300);

            var kppInput = _b.Driver.FindElement(By.Id("kpp"));
            kppInput.Clear();
            kppInput.SendKeys(kpp);
            Thread.Sleep(300);

            var allInputs = _b.Driver.FindElements(By.CssSelector("input.input__input[type='text']"));
            if (allInputs.Count >= 3)
            {
                allInputs[2].Clear();
                allInputs[2].SendKeys(ogrn);
                Thread.Sleep(300);
            }

            var textareas = _b.Driver.FindElements(By.CssSelector("textarea.txt__textarea"));
            if (textareas.Count >= 2)
            {
                textareas[0].Clear();
                textareas[0].SendKeys(name);
                Thread.Sleep(300);

                textareas[1].Clear();
                textareas[1].SendKeys(address);
                Thread.Sleep(300);
            }
        }

        public void FillContacts(string fullName, string email, string phone)
        {
            Thread.Sleep(1000);

            var nameLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Имя')]"));
            var nameInput = nameLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            nameInput.Click();
            nameInput.Clear();
            nameInput.SendKeys(fullName);
            Thread.Sleep(300);

            var emailLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'почт')]"));
            var emailInput = emailLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            emailInput.Click();
            emailInput.Clear();
            emailInput.SendKeys(email);
            Thread.Sleep(300);

            var phoneLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Телефон')]"));
            var phoneInput = phoneLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            phoneInput.Click();
            phoneInput.Clear();
            phoneInput.SendKeys(phone);
            Thread.Sleep(300);

            // Прокручиваем к полю "Тип обслуживаемого оборудования"
            var equipLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Тип обслуживаемого оборудования')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", equipLabel);
            Thread.Sleep(500);

            var equipInput = equipLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            equipInput.Click();
            equipInput.Clear();
            equipInput.SendKeys("Торговое оборудование");
            Thread.Sleep(300);

            var allTextareas = _b.Driver.FindElements(By.TagName("textarea"));
            if (allTextareas.Count > 0)
            {
                var last = allTextareas[allTextareas.Count - 1];
                last.Click();
                last.Clear();
                last.SendKeys("Тестовое описание организации");
                Thread.Sleep(300);
            }

            Console.WriteLine("✅ Контакты заполнены");
        }

        public void ClickBankRequisites()
        {
            var bankTab = _b.WaitClickable(By.XPath("//span[contains(@class,'ant-collapse-header-text') and contains(text(),'Банковские')]"));
            bankTab.Click();
            Thread.Sleep(1000);
        }

        public void FillBankRequisites(string bik, string bankName, string account, string corrAccount)
        {
            Thread.Sleep(1000);

            var bankHeader = _b.Driver.FindElement(By.XPath("//span[contains(text(),'Банковские')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", bankHeader);
            Thread.Sleep(500);

            var allInputs = _b.Driver.FindElements(By.CssSelector("input[type='text']"));
            var visibleInputs = new List<IWebElement>();
            foreach (var inp in allInputs)
            {
                if (inp.Displayed && inp.Enabled)
                    visibleInputs.Add(inp);
            }

            int count = visibleInputs.Count;
            if (count >= 4)
            {
                visibleInputs[count - 4].Clear();
                visibleInputs[count - 4].SendKeys(bik);
                Thread.Sleep(200);

                visibleInputs[count - 3].Clear();
                visibleInputs[count - 3].SendKeys(bankName);
                Thread.Sleep(200);

                visibleInputs[count - 2].Clear();
                visibleInputs[count - 2].SendKeys(account);
                Thread.Sleep(200);

                visibleInputs[count - 1].Clear();
                visibleInputs[count - 1].SendKeys(corrAccount);
                Thread.Sleep(200);
            }
        }

        public void SelectNDS()
        {
            Thread.Sleep(500);

            var sNds = _b.Driver.FindElement(By.Id("with_vat"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", sNds);
            Thread.Sleep(500);

            var bezNds = _b.Driver.FindElement(By.Id("no_vat"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", bezNds);
            Thread.Sleep(500);
        }

        public void ClickSave()
        {
            Thread.Sleep(2000); // Увеличенная задержка перед поиском

            // Прокручиваем страницу вниз, чтобы кнопка стала видимой
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(500);

            IWebElement saveBtn = null;

            // Пробуем разные локаторы
            try
            {
                saveBtn = _b.Driver.FindElement(By.XPath("//button[contains(@class,'btn-red')]"));
                Console.WriteLine("✅ Кнопка найдена по классу btn-red");
            }
            catch
            {
                try
                {
                    saveBtn = _b.Driver.FindElement(By.XPath("//button[@type='submit']"));
                    Console.WriteLine("✅ Кнопка найдена по type=submit");
                }
                catch
                {
                    try
                    {
                        saveBtn = _b.Driver.FindElement(By.XPath("//button[contains(text(),'Создать')]"));
                        Console.WriteLine("✅ Кнопка найдена по тексту 'Создать'");
                    }
                    catch
                    {
                        try
                        {
                            saveBtn = _b.Driver.FindElement(By.XPath("//button[contains(@class,'btn')]//span[contains(text(),'Создать')]"));
                            Console.WriteLine("✅ Кнопка найдена по классу btn и тексту");
                        }
                        catch { }
                    }
                }
            }

            if (saveBtn != null)
            {
                // Клик через JavaScript (гарантированно сработает)
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", saveBtn);
                Thread.Sleep(2000);
                Console.WriteLine("✅ Организация сохранена");
            }
            else
            {
                // Последний вариант: ищем через JavaScript
                IJavaScriptExecutor js = (IJavaScriptExecutor)_b.Driver;
                js.ExecuteScript(@"
                    var btns = document.querySelectorAll('button');
                    for(var i = 0; i < btns.length; i++) {
                        if(btns[i].innerText.includes('Создать')) {
                            btns[i].click();
                            break;
                        }
                    }
                ");
                Thread.Sleep(2000);
                Console.WriteLine("✅ Организация сохранена через JavaScript поиск");
            }
        }

        public void ClickClient(string clientName1, string clientName2)
        {
            Thread.Sleep(1000);

            // Сначала ищем первого клиента
            var clientRow = _b.Driver.FindElements(By.XPath($"//p[contains(@class,'link') and contains(@class,'pointer') and contains(text(),'{clientName1}')]"));

            // Если первого нет, ищем второго
            if (clientRow.Count == 0)
            {
                clientRow = _b.Driver.FindElements(By.XPath($"//p[contains(@class,'link') and contains(@class,'pointer') and contains(text(),'{clientName2}')]"));
            }

            // Если нашли - кликаем
            if (clientRow.Count > 0)
            {
                clientRow[0].Click();
                Thread.Sleep(1500);
            }
            else
            {
                throw new Exception($"Клиенты '{clientName1}' и '{clientName2}' не найдены");
            }
        }

        public void ClickEditOrganization()
        {
            Thread.Sleep(500);
            var editBtn = _b.WaitClickable(By.XPath("//button[contains(.,'Редактировать организацию')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", editBtn);
            Thread.Sleep(500);
            editBtn.Click();
            Thread.Sleep(1500);
        }

        public void SelectLegalForm(string formType)
        {
            Thread.Sleep(500);

            if (formType.Contains("ИП"))
            {
                var ipRadio = _b.Driver.FindElement(By.Id("legal_form_option-ip"));
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", ipRadio);
            }
            else
            {
                var legalRadio = _b.Driver.FindElement(By.Id("legal_form_option-legal"));
                ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", legalRadio);
            }

            Thread.Sleep(500);
        }

        public void EditRequisites(string newInn, string newOgrn, string newName)
        {
            Thread.Sleep(1000);

            // ИНН
            var innInput = _b.Driver.FindElement(By.Id("inn"));
            innInput.Click();
            innInput.Clear();
            Thread.Sleep(200);
            innInput.SendKeys(newInn);
            Thread.Sleep(300);

            // ОГРН — ищем по label "ОГРН / ОГРНИП"
            var ogrnLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'ОГРН')]"));
            var ogrnInput = ogrnLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            ogrnInput.Click();
            ogrnInput.Clear();
            Thread.Sleep(200);
            ogrnInput.SendKeys(newOgrn);
            Thread.Sleep(300);

            // Название — ищем внутри div#name textarea
            var nameBlock = _b.Driver.FindElement(By.Id("name"));
            var nameTextarea = nameBlock.FindElement(By.TagName("textarea"));
            nameTextarea.Click();
            nameTextarea.Clear();
            Thread.Sleep(200);
            nameTextarea.SendKeys(newName);
            Thread.Sleep(300);
        }

        public void EditContacts(string newName, string newEmail, string newPhone)
        {
            Thread.Sleep(1000);

            // Имя Фамилия
            var nameLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Имя')]"));
            var nameInput = nameLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            nameInput.Click();
            nameInput.Clear();
            Thread.Sleep(200);
            nameInput.SendKeys(newName);
            Thread.Sleep(300);

            // Email
            var emailLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'почт')]"));
            var emailInput = emailLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            emailInput.Click();
            emailInput.Clear();
            Thread.Sleep(200);
            emailInput.SendKeys(newEmail);
            Thread.Sleep(300);

            // Телефон
            var phoneLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Телефон')]"));
            var phoneInput = phoneLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            phoneInput.Click();
            phoneInput.Clear();
            Thread.Sleep(200);
            phoneInput.SendKeys(newPhone);
            Thread.Sleep(300);
        }

        public void EditDescription(string newDescription)
        {
            Thread.Sleep(500);
            var allTextareas = _b.Driver.FindElements(By.TagName("textarea"));
            if (allTextareas.Count > 0)
            {
                var last = allTextareas[allTextareas.Count - 1];
                last.Click();
                last.Clear();
                Thread.Sleep(200);
                last.SendKeys(newDescription);
                Thread.Sleep(300);
            }
        }

        public void EditEquipmentType(string newType)
        {
            Thread.Sleep(500);
            // Исправлен локатор: "Тип обслуживаемого оборудования" вместо "Тип оборудования"
            var equipLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Тип обслуживаемого оборудования')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", equipLabel);
            Thread.Sleep(500);
            var equipInput = equipLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            equipInput.Click();
            equipInput.Clear();
            Thread.Sleep(200);
            equipInput.SendKeys(newType);
            Thread.Sleep(300);
            Console.WriteLine($"✅ Тип оборудования изменён на: {newType}");
        }

        public void EditBankRequisites(string newBik, string newBankName, string newAccount, string newCorrAccount)
        {
            Thread.Sleep(1000);

            // Скроллим до банковских реквизитов
            var bankHeader = _b.Driver.FindElement(By.XPath("//span[contains(text(),'Банковские')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", bankHeader);
            Thread.Sleep(500);

            var allInputs = _b.Driver.FindElements(By.CssSelector("input[type='text']"));
            var visibleInputs = new List<IWebElement>();
            foreach (var inp in allInputs)
            {
                if (inp.Displayed && inp.Enabled)
                    visibleInputs.Add(inp);
            }

            int count = visibleInputs.Count;
            if (count >= 4)
            {
                visibleInputs[count - 4].Click();
                visibleInputs[count - 4].Clear();
                visibleInputs[count - 4].SendKeys(newBik);
                Thread.Sleep(200);

                visibleInputs[count - 3].Click();
                visibleInputs[count - 3].Clear();
                visibleInputs[count - 3].SendKeys(newBankName);
                Thread.Sleep(200);

                visibleInputs[count - 2].Click();
                visibleInputs[count - 2].Clear();
                visibleInputs[count - 2].SendKeys(newAccount);
                Thread.Sleep(200);

                visibleInputs[count - 1].Click();
                visibleInputs[count - 1].Clear();
                visibleInputs[count - 1].SendKeys(newCorrAccount);
                Thread.Sleep(200);
            }
        }

        public void SelectNDSOnly()
        {
            Thread.Sleep(500);
            var sNds = _b.Driver.FindElement(By.Id("with_vat"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", sNds);
            Thread.Sleep(300);
        }

        public void ClickSaveOrganization()
        {
            Thread.Sleep(500);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_b.Driver;
            js.ExecuteScript("var form = document.querySelector('form'); if (form) { var btn = form.querySelector('button[type=\"submit\"]'); if (btn) { btn.click(); } }");
            Thread.Sleep(2000);
        }

        public void ClickCreateRequest(string clientName = "Колледж Связи №54")
        {
            Thread.Sleep(500);

            var btn = _b.Driver.FindElement(By.XPath(
                $"//p[contains(text(),'{clientName}')]/ancestor::tr//button[contains(.,'Создать заявку')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", btn);
            Thread.Sleep(1500);
        }

        public void SelectMaster()
        {
            Thread.Sleep(1000);

            // Клик по дропдауну
            var dropdown = _b.Driver.FindElement(By.XPath("//input[@placeholder='Не выбран']"));
            dropdown.Click();
            Thread.Sleep(800);

            // Ищем опцию по ID как в SelectByIndex
            var option = _b.Driver.FindElement(By.XPath("//*[contains(@id,'__option-1')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", option);
            Thread.Sleep(500);

            // Срок исполнения — открыть календарь
            var dateInput = _b.Driver.FindElement(By.XPath("//input[@placeholder='Выбрать срок исполнения']"));
            dateInput.Click();
            Thread.Sleep(500);

            // Выбрать сегодняшнее число
            var today = DateTime.Now.Day;
            var days = _b.Driver.FindElements(By.CssSelector("button.day"));
            foreach (var day in days)
            {
                var span = day.FindElement(By.CssSelector("span.number"));
                if (span.Text.Trim() == today.ToString())
                {
                    day.Click();
                    Thread.Sleep(300);
                    break;
                }
            }
        }

        public void FillObjectDetails(string name, string address)
        {
            Thread.Sleep(500);

            // Название объекта
            var nameLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Название')]"));
            var nameInput = nameLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            nameInput.Click();
            nameInput.SendKeys(name);
            Thread.Sleep(300);

            // Адрес — input#map
            var addressInput = _b.Driver.FindElement(By.Id("map"));
            addressInput.Click();
            addressInput.Clear();
            addressInput.SendKeys(address);
            Thread.Sleep(1000);
            addressInput.SendKeys(Keys.ArrowDown);
            Thread.Sleep(300);
            addressInput.SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            // Скроллим к парковке
            var parkingLabel = _b.Driver.FindElement(By.XPath("//*[contains(text(),'Парковка')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", parkingLabel);
            Thread.Sleep(300);

            // Парковка
            var parkingSelected = _b.Driver.FindElement(By.XPath("//*[contains(text(),'Парковка')]/following-sibling::*//span[@class='vs__selected']"));
            parkingSelected.Click();
            Thread.Sleep(800);
            var option = _b.Driver.FindElement(By.XPath("//*[contains(@id,'__option-2')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", option);
            Thread.Sleep(500);

            // Тип заявки — "Тех. обслуживание"
            var typeLabel = _b.Driver.FindElement(By.XPath("//*[contains(text(),'Тип заявки')]"));
            var typeDropdown = typeLabel.FindElement(By.XPath("./following-sibling::*//span[@class='vs__selected']"));
            typeDropdown.Click();
            Thread.Sleep(800);
            var typeOption = _b.Driver.FindElement(By.XPath("//*[contains(@id,'__option-1')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", typeOption);
            Thread.Sleep(500);

            // Приоритетность — "Текущая"
            var priorityLabel = _b.Driver.FindElement(By.XPath("//*[contains(text(),'Приоритетность')]"));
            var priorityDropdown = priorityLabel.FindElement(By.XPath("./following-sibling::*//span[@class='vs__selected']"));
            priorityDropdown.Click();
            Thread.Sleep(800);
            var priorityOption = _b.Driver.FindElement(By.XPath("//*[contains(@id,'__option-1')]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", priorityOption);
            Thread.Sleep(500);

            // Желаемое время работ
            var workTimeLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Желаемый день')]"));
            var workTimeInput = workTimeLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", workTimeInput);
            Thread.Sleep(300);
            workTimeInput.Clear();
            var today = DateTime.Now;
            var time = today.ToString("dd.MM.yyyy, HH:mm") + "-" + today.AddHours(2).ToString("HH:mm");
            workTimeInput.SendKeys(time);
            Thread.Sleep(300);

            // Заявка на*
            var requestForLabel = _b.Driver.FindElement(By.XPath("//label[contains(text(),'Заявка на')]"));
            var requestForInput = requestForLabel.FindElement(By.XPath("./following-sibling::input[1]"));
            requestForInput.Click();
            requestForInput.SendKeys("Тестовая заявка");
            Thread.Sleep(300);

            // Описание задачи
            var taskTextarea = _b.Driver.FindElement(By.XPath("//textarea[contains(@placeholder,'механику')]"));
            taskTextarea.Click();
            taskTextarea.SendKeys("Тестовое описание задачи для механика");
            Thread.Sleep(300);

            // Фото
            try
            {
                var fileInput = _b.Driver.FindElement(By.Id("upload"));
                fileInput.SendKeys(Path.GetFullPath(@"C:\testfiles\photo.jpg"));
                Thread.Sleep(1000);
            }
            catch { }

            // Отправить заявку
            var submitBtn = _b.Driver.FindElement(By.Id("submit"));
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitBtn);
            Thread.Sleep(300);
            ((IJavaScriptExecutor)_b.Driver).ExecuteScript("arguments[0].click();", submitBtn);
            Thread.Sleep(2000);


        }

        public void GoToOrdersTab()
        {
            // Переход во вкладку Заявки
            var ordersLink = _b.Driver.FindElement(By.XPath("//a[@href='/orders']"));
            ordersLink.Click();
            Thread.Sleep(2000);

            // Нажать на вкладку Внешние клиенты
            var externalClientsTab = _b.WaitClickable(By.XPath("//div[contains(@class,'tabs__tab') and contains(text(),'Внешние клиенты')]"));
            externalClientsTab.Click();
            Thread.Sleep(2000);
        }


    }
}