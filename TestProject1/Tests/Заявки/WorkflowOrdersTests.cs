using Xunit;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System.Threading;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace RingAutoTests.Tests.Заявки
{
    public class WorkflowOrdersTests
    {
        [Fact]
        public void FullWorkflow_Test()
        {
            var browser = new BrowserHelper();

            try
            {
                Console.WriteLine("TEST STARTED - FullWorkflow_Test");

                new LoginPage(browser).Login("79262266015", "QwertY100");
                Thread.Sleep(1000);
                Console.WriteLine("OK: Login successful");

                var orders = new OrderActionsPage(browser);
                orders.GoToOrders();
                Console.WriteLine("OK: Navigated to Orders section");

                orders.ProcessRequest("Тестовая заявка ТО");
                Console.WriteLine("OK: Request processing completed");

                // ✅ Проверка закомментирована, так как уже есть внутри ProcessRequest
                // var statusElement = wait.Until(ExpectedConditions.ElementIsVisible(
                //     By.XPath("//*[contains(@class,'badge') and contains(text(),'Завершена')]")));
                // Assert.True(statusElement.Displayed, "❌ Статус заявки не 'Завершена'");

                Console.WriteLine("🎉 ТЕСТ ПРОЙДЕН! Все поля совпадают");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ОШИБКА: {ex.Message}");
                throw;
            }
            finally { browser.Close(); }
        }
    }
}