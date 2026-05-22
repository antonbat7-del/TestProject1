using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System;
using System.Threading;

namespace RingAutoTests.Tests
{
    public class ChatOnlyTest
    {
        [Fact]
        public void SecondWindow_AcceptRequest()
        {
            var browser = new BrowserHelper();

            try
            {
                Console.WriteLine("TEST STARTED - ChatOnlyTest");

                // 1. Открыть новую вкладку
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("window.open();");
                var tabs = browser.Driver.WindowHandles;
                browser.Driver.SwitchTo().Window(tabs[1]);
                Console.WriteLine("✅ Новая вкладка открыта");
                Assert.Equal(2, tabs.Count); // ✅ ПРОВЕРКА: открыто 2 вкладки

                // 2. Очистить куки и залогиниться под создателем
                browser.Driver.Navigate().GoToUrl("https://lk.reeng.ru/");
                Thread.Sleep(1000);
                browser.Driver.Manage().Cookies.DeleteAllCookies();
                browser.Driver.Navigate().GoToUrl("https://lk.reeng.ru/");
                Thread.Sleep(1000);

                browser.WaitVisible(By.Id("phone")).SendKeys("79160000071");
                browser.WaitVisible(By.Id("password")).SendKeys("Qwerty100");
                browser.WaitClickable(By.Id("submit")).Click();
                Thread.Sleep(2000);
                Console.WriteLine("✅ Логин выполнен");

                // ✅ ПРОВЕРКА: логин успешен
                Assert.True(browser.Driver.Url.Contains("lk") || browser.Driver.PageSource.Contains("личный кабинет"), "❌ Логин не выполнен");

                // 3. Смена юрлица на Мастер-Дент
                var legalDropdown = browser.Driver.FindElement(By.CssSelector("div.dropdown__value"));
                legalDropdown.Click();
                Thread.Sleep(800);
                var option = browser.Driver.FindElement(By.XPath("//div[contains(@class,'dropdown-options__item') and contains(text(),'Мастер-Дент')]"));
                option.Click();
                Thread.Sleep(500);
                Console.WriteLine("✅ Юрлицо сменено на Мастер-Дент");

                // ✅ ПРОВЕРКА: юрлицо сменилось
                Assert.True(browser.Driver.PageSource.Contains("Мастер-Дент"), "❌ Юрлицо не сменилось");

                // 4. Перейти в заявки
                var ordersLink = browser.WaitClickable(By.XPath("//a[@href='/orders']"));
                ordersLink.Click();
                Thread.Sleep(3000);
                Console.WriteLine("✅ Переход в заявки");

                // ✅ ПРОВЕРКА: страница заявок открыта
                Assert.Contains("/orders", browser.Driver.Url);

                // === ПЕРВАЯ СМЕНА ИСПОЛНИТЕЛЯ ===
                var dots = browser.Driver.FindElements(By.XPath("//div[contains(@class,'record-description') and contains(text(),'Тестовая заявка ТО')]/ancestor::tr//div[contains(@class,'grey-lamp')]"));
                Assert.True(dots.Count > 0, "❌ Кнопка 'три точки' не найдена"); // ✅ ПРОВЕРКА
                if (dots.Count > 0 && dots[0].Displayed)
                {
                    dots[0].Click();
                    Thread.Sleep(500);

                    var changeExecutor = browser.Driver.FindElement(By.XPath("//span[contains(text(),'Сменить исполнителя')]"));
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", changeExecutor);
                    Thread.Sleep(1500);

                    var nextBtn = browser.Driver.FindElement(By.XPath("//button[contains(.,'Далее')]"));
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", nextBtn);
                    Thread.Sleep(1500);

                    var companyDropdown = browser.Driver.FindElement(By.XPath("//*[contains(text(),'Компания')]/following-sibling::*//div[contains(@class,'vs__dropdown-toggle')]"));
                    companyDropdown.Click();
                    Thread.Sleep(1000);

                    var companyOption = browser.Driver.FindElement(By.XPath("//li[contains(@class,'vs__dropdown-option') and contains(.,'Торговый')]"));
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", companyOption);
                    Thread.Sleep(500);

                    var changeBtn = browser.Driver.FindElements(By.XPath("//button[contains(.,'Сменить исполнителя')]"));
                    if (changeBtn.Count > 0)
                    {
                        ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", changeBtn[changeBtn.Count - 1]);
                        Thread.Sleep(1500);
                    }
                }

                // ✅ ПРОВЕРКА: первая смена исполнителя прошла
                Assert.True(browser.Driver.PageSource.Contains("Торговый"), "❌ Исполнитель не изменён на 'Торговый'");
                Console.WriteLine("✅ Первая смена исполнителя: Торговый");

                // === ВТОРАЯ СМЕНА ИСПОЛНИТЕЛЯ ===
                Thread.Sleep(2000);

                var dots2 = browser.Driver.FindElements(By.XPath("//div[contains(@class,'record-description') and contains(text(),'Тестовая заявка ТО')]/ancestor::tr//div[contains(@class,'grey-lamp')]"));
                Assert.True(dots2.Count > 0, "❌ Кнопка 'три точки' не найдена для второй смены"); // ✅ ПРОВЕРКА
                if (dots2.Count > 0 && dots2[0].Displayed)
                {
                    dots2[0].Click();
                    Thread.Sleep(500);

                    var changeExecutor2 = browser.Driver.FindElement(By.XPath("//span[contains(text(),'Сменить исполнителя')]"));
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", changeExecutor2);
                    Thread.Sleep(1500);

                    var nextBtn2 = browser.Driver.FindElement(By.XPath("//button[contains(.,'Далее')]"));
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", nextBtn2);
                    Thread.Sleep(1500);

                    var companyDropdown2 = browser.Driver.FindElement(By.XPath("//*[contains(text(),'Компания')]/following-sibling::*//div[contains(@class,'vs__dropdown-toggle')]"));
                    companyDropdown2.Click();
                    Thread.Sleep(1000);

                    // Поиск "Мастерской" по части текста
                    var allOptions = browser.Driver.FindElements(By.XPath("//li[contains(@class,'vs__dropdown-option')]"));
                    IWebElement companyOption2 = null;
                    foreach (var opt in allOptions)
                    {
                        if (opt.Text.Contains("Мастерская"))
                        {
                            companyOption2 = opt;
                            break;
                        }
                    }

                    if (companyOption2 != null)
                    {
                        ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", companyOption2);
                        Thread.Sleep(500);
                    }
                    else if (allOptions.Count > 0)
                    {
                        ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", allOptions[0]);
                        Thread.Sleep(500);
                    }

                    var changeBtn2 = browser.Driver.FindElements(By.XPath("//button[contains(.,'Сменить исполнителя')]"));
                    if (changeBtn2.Count > 0)
                    {
                        ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", changeBtn2[changeBtn2.Count - 1]);
                        Thread.Sleep(1500);
                    }
                }

                // ✅ ПРОВЕРКА: вторая смена исполнителя прошла
                Assert.True(browser.Driver.PageSource.Contains("Мастерская"), "❌ Исполнитель не изменён на 'Мастерская'");
                Console.WriteLine("✅ Вторая смена исполнителя: Мастерская");

                Console.WriteLine("TEST COMPLETED SUCCESSFULLY!");
                Assert.True(true);
            }
            finally
            {
                browser.Close();
                Console.WriteLine("Browser closed");
            }
        }
    }
}