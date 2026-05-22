using Xunit;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;
using System.Threading;
using OpenQA.Selenium;

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
                Console.WriteLine("TEST STARTED - EditExternalClientTest");

                // Логин
                new LoginPage(browser).Login("79262266015", "QwertY100");
                Thread.Sleep(1000);
                Console.WriteLine("✅ Логин выполнен");

                var clients = new ExternalClientsPage(browser);

                // Перейти к внешним клиентам и открыть созданного
                clients.GoToExternalClients();
                Console.WriteLine("✅ Переход к внешним клиентам");

                clients.ClickClient("ГБПОУ КС № 54", "Колледж Связи №54");
                Console.WriteLine("✅ Клиент выбран");

                // Нажать "Редактировать организацию"
                clients.ClickEditOrganization();
                Console.WriteLine("✅ Форма редактирования открыта");

                // Сменить форму на ИП
                clients.SelectLegalForm("Индивидуальный предприниматель (ИП)");
                Console.WriteLine("✅ Форма изменена на ИП");

                // Изменить реквизиты
                clients.EditRequisites("540427024806", "1125476199090", "Колледж Связи №54");
                Console.WriteLine("✅ Реквизиты изменены");

                // Изменить контакты
                clients.EditContacts("Ботов Ботович Updated", "updated@test.ru", "2222222222");
                Console.WriteLine("✅ Контакты изменены");

                // Изменить описание
                clients.EditDescription("Тестовое описание организации 12345678910");
                Console.WriteLine("✅ Описание изменено");

                // Изменить тип оборудования
                clients.EditEquipmentType("Зонт");
                Console.WriteLine("✅ Тип оборудования изменён");

                // Раскрыть банковские реквизиты
                clients.ClickBankRequisites();
                Console.WriteLine("✅ Банковские реквизиты открыты");

                // Изменить банковские реквизиты
                clients.EditBankRequisites("987654321", "Мега Банк", "11111111111111111111", "22222222222222222222");
                Console.WriteLine("✅ Банковские реквизиты изменены");

                // Выбрать С НДС
                clients.SelectNDSOnly();
                Console.WriteLine("✅ Выбран НДС");

                // Сохранить
                clients.ClickSaveOrganization();
                Console.WriteLine("✅ Изменения сохранены");

                // Возврат на страницу списка
                Thread.Sleep(2000);
                browser.Driver.Navigate().Back();
                Thread.Sleep(2000);
                Console.WriteLine("✅ Возврат на страницу списка");

                // Проверка: организация всё ещё в списке
                clients.GoToExternalClients();
                Thread.Sleep(1000);
                var clientInList = browser.Driver.FindElements(By.XPath("//*[contains(text(),'Колледж Связи №54')]"));
                Assert.True(clientInList.Count > 0, "❌ Организация не найдена в списке после редактирования");
                Console.WriteLine("✅ Проверка: организация в списке");

                // Создать заявку
                clients.ClickCreateRequest();
                Console.WriteLine("✅ Создание заявки");

                // Выбрать мастера
                clients.SelectMaster();
                Console.WriteLine("✅ Мастер выбран");

                // Закрываем модальное окно, если оно открыто
                Thread.Sleep(1000);
                try
                {
                    var modalClose = browser.Driver.FindElements(By.XPath("//div[contains(@class,'modal__wrap')]//button[contains(@class,'close')] | //div[contains(@class,'modal')]//button[contains(@class,'close')]"));
                    if (modalClose.Count > 0 && modalClose[0].Displayed)
                    {
                        ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", modalClose[0]);
                        Thread.Sleep(500);
                        Console.WriteLine("✅ Модальное окно закрыто");
                    }
                }
                catch { }

                // ✅ ПРОВЕРКА: заявка появилась в списке
                Thread.Sleep(2000);
                var ordersLink = browser.Driver.FindElement(By.XPath("//a[@href='/orders']"));
                ((IJavaScriptExecutor)browser.Driver).ExecuteScript("arguments[0].click();", ordersLink);
                Thread.Sleep(2000);

                var requestInList = browser.Driver.FindElements(By.XPath("//*[contains(text(),'Тестовая заявка')]"));
                Assert.True(requestInList.Count > 0, "❌ Заявка не создалась");
                Console.WriteLine("✅ Проверка: заявка создана");

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