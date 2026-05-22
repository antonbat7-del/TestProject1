using Xunit;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System.Threading;
using OpenQA.Selenium;

namespace RingAutoTests.Tests
{
    public class CreateRequestForExternalClientTest
    {
        [Fact]
        public void CreateRequest_ForExternalClient_Test()
        {
            var browser = new BrowserHelper();

            try
            {
                Console.WriteLine("TEST STARTED - CreateRequestForExternalClientTest");

                // Логин
                new LoginPage(browser).Login("79262266015", "QwertY100");
                Thread.Sleep(1000);
                Console.WriteLine("✅ Логин выполнен");

                var clients = new ExternalClientsPage(browser);

                // Перейти к внешним клиентам
                clients.GoToExternalClients();
                Console.WriteLine("✅ Переход к внешним клиентам");

                // Нажать "Создать заявку" в строке нужного клиента
                clients.ClickCreateRequest("Колледж Связи №54");
                Console.WriteLine("✅ Форма создания заявки открыта");

                // Выбрать мастера
                clients.SelectMaster();
                Console.WriteLine("✅ Мастер выбран");

                // Заполнить данные объекта
                clients.FillObjectDetails("Тестовый объект", "г. Москва, ул. Тестовая, д. 1");
                Console.WriteLine("✅ Данные объекта заполнены, заявка отправлена");

                // Ждём отправки
                Thread.Sleep(4000);

                // Переход в раздел Заявки
                var ordersLink = browser.Driver.FindElement(By.XPath("//a[@href='/orders']"));
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", ordersLink);
                Thread.Sleep(2000);
                Console.WriteLine("✅ Переход в раздел Заявки");

                // Нажать на вкладку "Внешние клиенты"
                var externalClientsTab = browser.Driver.FindElement(By.XPath("//div[contains(@class,'tabs__tab') and contains(text(),'Внешние клиенты')]"));
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", externalClientsTab);
                Thread.Sleep(2000);
                Console.WriteLine("✅ Выбрана вкладка 'Внешние клиенты'");

                // Проверка: заявка появилась
                var requestInList = browser.Driver.FindElements(By.XPath("//*[contains(text(),'Тестовый объект')]"));
                Assert.True(requestInList.Count > 0, "❌ Заявка не создалась");
                Console.WriteLine("✅ Проверка: заявка найдена");

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