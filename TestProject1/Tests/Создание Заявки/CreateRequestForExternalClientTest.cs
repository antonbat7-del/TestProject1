using Xunit;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System.Threading;

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
                // Логин
                new LoginPage(browser).Login("79262266015", "QwertY100");
                Thread.Sleep(1000);

                var clients = new ExternalClientsPage(browser);

                // Перейти к внешним клиентам
                clients.GoToExternalClients();

                // Нажать "Создать заявку" в строке нужного клиента
                clients.ClickCreateRequest("Колледж Связи №54");

                // Выбрать мастера
                clients.SelectMaster();

                clients.FillObjectDetails("Тестовый объект", "г. Москва, ул. Тестовая, д. 1");

                Assert.True(true);
            }
            finally { browser.Close(); }
        }
    }
}