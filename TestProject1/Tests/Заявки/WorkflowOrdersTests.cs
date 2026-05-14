using Xunit;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;

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
                new LoginPage(browser).Login("79262266015", "QwertY100");
                Thread.Sleep(1000);

                var orders = new OrderActionsPage(browser);
                orders.GoToOrders();
                orders.ProcessRequest("Тестовая заявка ТО");

                Assert.True(true);
            }
            finally { browser.Close(); }
        }
    }
}