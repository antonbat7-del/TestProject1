using Xunit;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System.Threading;

namespace RingAutoTests.Tests
{
    public class ExternalClientsTests
    {
        [Fact]
        public void CreateExternalClient_Test()
        {
            var browser = new BrowserHelper();

            try
            {
                new LoginPage(browser).Login("79262266015", "QwertY100");
                Thread.Sleep(1000);

                var clients = new ExternalClientsPage(browser);
                clients.GoToExternalClients();
                clients.ClickCreateOrganization();
                clients.FillRequisites("7705513734", "770501001", "1057705003619", "ООО Тестовый Клиент", "115172, город Москва, ул. Большие Каменщики, д.7");
                clients.FillContacts("Бот Ботович", "BotTobBot@yandex.ru", "1111111111");
                clients.ClickBankRequisites();
                clients.FillBankRequisites("123456789", "Супер Банк", "12345678901234567890", "09876543210987654321");
                clients.SelectNDS();
                clients.ClickSave();

                Assert.True(true);
            }
            finally { browser.Close(); }
        }
    }
}