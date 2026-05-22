using Xunit;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System.Threading;
using OpenQA.Selenium;

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
                // 1. Логин
                new LoginPage(browser).Login("79262266015", "QwertY100");
                Thread.Sleep(1000);

                var clients = new ExternalClientsPage(browser);

                // 2. Создание организации
                clients.GoToExternalClients();
                clients.ClickCreateOrganization();
                clients.FillRequisites("7705513734", "770501001", "1057705003619", "ООО Тестовый Клиент", "115172, город Москва, ул. Большие Каменщики, д.7");
                clients.FillContacts("Бот Ботович", "BotTobBot@yandex.ru", "1111111111");
                clients.ClickBankRequisites();
                clients.FillBankRequisites("123456789", "Супер Банк", "12345678901234567890", "09876543210987654321");
                clients.SelectNDS();
                clients.ClickSave();

                // Обновляем страницу перед проверкой
                Thread.Sleep(2000);
                browser.Driver.Navigate().Refresh();
                Thread.Sleep(2000);

                // ✅ ПРОВЕРКА: Организация появилась в списке
                clients.GoToExternalClients();
                Thread.Sleep(1000);
                var clientInList = browser.Driver.FindElements(By.XPath("//*[contains(text(),'ГБПОУ КС № 54')]"));
                Assert.True(clientInList.Count > 0, "❌ Организация не найдена в списке");

                Console.WriteLine("✅ Тест успешно пройден! Организация создана.");
            }
            finally
            {
                browser.Close();
            }
        }
    }
}