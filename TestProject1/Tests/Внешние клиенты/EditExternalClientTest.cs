using Xunit;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System.Threading;

namespace RingAutoTests.Tests
{
    public class EditExternalClientTest
    {
        [Fact]
        public void EditExternalClient_Test()
        {
            var browser = new BrowserHelper();

            try
            {
                // Логин
                new LoginPage(browser).Login("79262266015", "QwertY100");
                Thread.Sleep(1000);

                var clients = new ExternalClientsPage(browser);

                // Перейти к внешним клиентам и открыть созданного
                clients.GoToExternalClients();
                clients.ClickClient("ГБПОУ КС № 54", "Колледж Связи №54");

                // Нажать "Редактировать организацию"
                clients.ClickEditOrganization();

                // Сменить форму на ИП
                clients.SelectLegalForm("Индивидуальный предприниматель (ИП)");

                // Изменить реквизиты
                clients.EditRequisites("540427024806", "1125476199090", "Колледж Связи №54");

                // Изменить контакты
                clients.EditContacts("Ботов Ботович Updated", "updated@test.ru", "2222222222");

                // Изменить описание
                clients.EditDescription("Тестовое описание организации 12345678910");

                // Изменить тип оборудования
                clients.EditEquipmentType("Зонт");

                // Раскрыть банковские реквизиты
                clients.ClickBankRequisites();

                // Изменить банковские реквизиты
                clients.EditBankRequisites("987654321", "Мега Банк", "11111111111111111111", "22222222222222222222");

                // Выбрать С НДС
                clients.SelectNDSOnly();

                // Сохранить
                clients.ClickSaveOrganization();

                clients.ClickCreateRequest();

                clients.SelectMaster();

                Assert.True(true);
            }
            finally { browser.Close(); }
        }
    }
}