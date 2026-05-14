using OpenQA.Selenium;
using RingAutoTests.Helpers;
using System;
using System.Linq;

namespace RingAutoTests.Pages
{
    public class RequestCreatePage
    {
        private BrowserHelper _b;

        private By CreateRequestBtn = By.XPath("//li[contains(@class,'sidebar__button') and contains(text(),'Создать заявку')]");
        private By NextButton = By.XPath("//button[contains(.,'Далее')]");
        private By DescriptionField = By.CssSelector("textarea");
        private By FileInput = By.CssSelector("input[type='file']");
        private By SubmitButton = By.XPath("//button[contains(text(),'Создать')]");

        public RequestCreatePage(BrowserHelper browser)
        {
            _b = browser;
        }

        // === СМЕНА ЮРЛИЦА ===
        public void SwitchLegalEntity(string entityName)
        {
            var dropdown = _b.Driver.FindElement(By.CssSelector("div.dropdown__value"));
            dropdown.Click();
            Thread.Sleep(1000);

            var options = _b.Driver.FindElements(By.CssSelector("div.dropdown-options__item"));

            foreach (var opt in options)
            {
                var text = opt.Text.Trim().Replace("\"", "");
                if (opt.Displayed && text.Contains(entityName, StringComparison.OrdinalIgnoreCase))
                {
                    opt.Click();
                    Thread.Sleep(500);
                    return;
                }
            }

            throw new Exception($"Юрлицо '{entityName}' не найдено среди {options.Count} опций.");
        }

        // === СОЗДАНИЕ ЗАЯВКИ ===
        public void OpenCreateForm()
        {
            _b.WaitClickable(CreateRequestBtn).Click();
        }

        // Шаг 1
        private void SelectByIndex(int index, string optionText)
        {
            var dropdowns = _b.Driver.FindElements(By.CssSelector("div.vs__dropdown-toggle"));
            if (dropdowns.Count > index)
            {
                dropdowns[index].Click();
                Thread.Sleep(800);

                var allOptions = _b.Driver.FindElements(By.CssSelector("[id*='__option-']"));

                foreach (var opt in allOptions)
                {
                    var text = opt.Text.Trim();
                    if (text.Contains(optionText))
                    {
                        opt.Click();
                        Thread.Sleep(300);
                        return;
                    }
                }

                throw new Exception($"Опция '{optionText}' не найдена. Доступно: {string.Join(", ", allOptions.Select(o => o.Text.Trim()))}");
            }
        }

        public void Step1_SelectSystem(string name) => SelectByIndex(0, name);
        public void Step1_SelectType(string name) => SelectByIndex(1, name);
        public void Step1_SelectRoom(string name) => SelectByIndex(2, name);

        public void ClickNext()
        {
            Thread.Sleep(500);
            _b.WaitClickable(NextButton).Click();
        }

        // Шаг 2
        public void Step2_SelectAndNext()
        {
            Thread.Sleep(1000);

            try
            {
                var arrow = _b.Driver.FindElement(By.CssSelector("svg.dropdown-icon-blue"));
                arrow.Click();
                Thread.Sleep(1000);
            }
            catch { }

            try
            {
                var items = _b.Driver.FindElements(By.XPath("//li//div[contains(@class,'list__ite')]"));
                if (items.Count >= 2)
                {
                    items[1].Click();
                    Thread.Sleep(500);
                }
                else if (items.Count > 0)
                {
                    items[0].Click();
                    Thread.Sleep(500);
                }
            }
            catch { }

            Thread.Sleep(500);
            _b.WaitClickable(NextButton).Click();
        }

        // Шаг 3
        public void Step3_SelectPriority(string priorityName)
        {
            Thread.Sleep(500);

            try
            {
                var label = _b.Driver.FindElement(By.XPath("//*[contains(text(),'Приоритетность')]"));
                var dropdown = label.FindElement(By.XPath("./following-sibling::*//div[contains(@class,'vs__dropdown-toggle')]"));
                dropdown.Click();
                Thread.Sleep(500);

                var options = _b.Driver.FindElements(By.CssSelector("[id*='__option-']"));
                foreach (var opt in options)
                {
                    if (opt.Text.Trim().Contains(priorityName))
                    {
                        opt.Click();
                        Thread.Sleep(300);
                        return;
                    }
                }
            }
            catch { }
        }

        public void Step3_FillDescription(string text)
        {
            Thread.Sleep(500);
            _b.WaitVisible(DescriptionField).SendKeys(text);
        }

        public void Step3_FillPhone(string phone)
        {
            Thread.Sleep(500);
            try
            {
                var phoneField = _b.Driver.FindElement(By.CssSelector("input[placeholder='Введите номер']"));
                phoneField.Click();
                Thread.Sleep(300);
                phoneField.Clear();
                phoneField.SendKeys(phone);
            }
            catch { }
        }

        public void Step3_AttachPhoto(string filePath)
        {
            try
            {
                var input = _b.Driver.FindElement(FileInput);
                input.SendKeys(Path.GetFullPath(filePath));
            }
            catch { }
        }

        public void Step3_FillDates()
        {
            Thread.Sleep(500);
            try
            {
                var today = DateTime.Today;
                var dayFrom = today.Day;
                var dayTo = today.AddDays(7).Day; // +7 дней

                var dateFields = _b.Driver.FindElements(By.CssSelector("input[placeholder*='___']"));

                // Открываем календарь "От"
                if (dateFields.Count >= 1)
                {
                    dateFields[0].Click();
                    Thread.Sleep(500);

                    var days = _b.Driver.FindElements(By.CssSelector("button.day"));
                    foreach (var day in days)
                    {
                        try
                        {
                            var span = day.FindElement(By.CssSelector("span.number"));
                            if (span.Text.Trim() == dayFrom.ToString())
                            {
                                day.Click();
                                Thread.Sleep(300);
                                break;
                            }
                        }
                        catch { }
                    }
                }

                // Открываем календарь "До"
                if (dateFields.Count >= 2)
                {
                    dateFields[1].Click();
                    Thread.Sleep(500);

                    var days = _b.Driver.FindElements(By.CssSelector("button.day"));
                    foreach (var day in days)
                    {
                        try
                        {
                            var span = day.FindElement(By.CssSelector("span.number"));
                            if (span.Text.Trim() == dayTo.ToString())
                            {
                                day.Click();
                                Thread.Sleep(300);
                                break;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        public void Step3_SelectResponsible(string name)
        {
            Thread.Sleep(500);
            try
            {
                var label = _b.Driver.FindElement(By.XPath("//*[contains(text(),'Ответственный')]"));
                var dropdown = label.FindElement(By.XPath("./following-sibling::*//div[contains(@class,'vs__dropdown-toggle')]"));
                dropdown.Click();
                Thread.Sleep(500);

                var options = _b.Driver.FindElements(By.CssSelector("[id*='__option-']"));
                foreach (var opt in options)
                {
                    if (opt.Text.Trim().Contains(name))
                    {
                        opt.Click();
                        Thread.Sleep(300);
                        return;
                    }
                }
            }
            catch { }
        }

        public void Step3_SelectExecutorCompany(string companyName)
        {
            Thread.Sleep(500);
            try
            {
                // 1. Переключаем на вкладку "Компании/самозанятому"
                var tab = _b.Driver.FindElement(By.XPath("//div[contains(text(),'Компании')]"));
                tab.Click();
                Thread.Sleep(500);

                // 2. Открываем дропдаун с компаниями
                var label = _b.Driver.FindElement(By.XPath("//*[contains(text(),'Компания')]"));
                var dropdown = label.FindElement(By.XPath("./following-sibling::*//div[contains(@class,'vs__dropdown-toggle')]"));
                dropdown.Click();
                Thread.Sleep(500);

                // 3. Выбираем нужную компанию
                var options = _b.Driver.FindElements(By.CssSelector("[id*='__option-']"));
                foreach (var opt in options)
                {
                    if (opt.Text.Trim().Contains(companyName))
                    {
                        opt.Click();
                        Thread.Sleep(300);
                        return;
                    }
                }
            }
            catch { }
        }

        public void Step3_SendToPartner()
        {
            Thread.Sleep(500);
            try
            {
                var btn = _b.Driver.FindElement(By.XPath("//button[contains(.,'Отправить партнеру')]"));
                btn.Click();
                Thread.Sleep(500);
            }
            catch { }
        }

        public void Step3_Submit()
        {
            try
            {
                _b.WaitClickable(SubmitButton).Click();
            }
            catch { }
        }
    }
}