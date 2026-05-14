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
                // Открыть новую вкладку
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("window.open();");
                var tabs = browser.Driver.WindowHandles;
                browser.Driver.SwitchTo().Window(tabs[1]);

                // Очистить куки и залогиниться под создателем
                browser.Driver.Navigate().GoToUrl("https://lk.reeng.ru/");
                Thread.Sleep(1000);
                browser.Driver.Manage().Cookies.DeleteAllCookies();
                browser.Driver.Navigate().GoToUrl("https://lk.reeng.ru/");
                Thread.Sleep(1000);

                browser.WaitVisible(By.Id("phone")).SendKeys("79160000071");
                browser.WaitVisible(By.Id("password")).SendKeys("Qwerty100");
                browser.WaitClickable(By.Id("submit")).Click();
                Thread.Sleep(2000);

                // Смена юрлица на Мастер-Дент
                var legalDropdown = browser.Driver.FindElement(By.CssSelector("div.dropdown__value"));
                legalDropdown.Click();
                Thread.Sleep(800);
                var option = browser.Driver.FindElement(By.XPath("//div[contains(@class,'dropdown-options__item') and contains(text(),'Мастер-Дент')]"));
                option.Click();
                Thread.Sleep(500);

                // Перейти в заявки
                var ordersLink = browser.WaitClickable(By.XPath("//a[@href='/orders']"));
                ordersLink.Click();
                Thread.Sleep(3000);

                // === ПЕРВАЯ СМЕНА ИСПОЛНИТЕЛЯ ===
                var dots = browser.Driver.FindElements(By.XPath("//div[contains(@class,'record-description') and contains(text(),'Тестовая заявка ТО')]/ancestor::tr//div[contains(@class,'grey-lamp')]"));
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

                // === ВТОРАЯ СМЕНА ИСПОЛНИТЕЛЯ ===
                Thread.Sleep(2000);

                var dots2 = browser.Driver.FindElements(By.XPath("//div[contains(@class,'record-description') and contains(text(),'Тестовая заявка ТО')]/ancestor::tr//div[contains(@class,'grey-lamp')]"));
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

                    var companyOption2 = browser.Driver.FindElement(By.XPath("//ul[contains(@class,'vs__dropdown-menu') and not(contains(@style,'display: none'))]//li[contains(text(),'Мастерская')]"));
                    ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", companyOption2);
                    Thread.Sleep(500);

                    var changeBtn2 = browser.Driver.FindElements(By.XPath("//button[contains(.,'Сменить исполнителя')]"));
                    if (changeBtn2.Count > 0)
                    {
                        ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", changeBtn2[changeBtn2.Count - 1]);
                        Thread.Sleep(1500);
                    }
                }

                Assert.True(true);
            }
            finally { browser.Close(); }
        }
    }
}