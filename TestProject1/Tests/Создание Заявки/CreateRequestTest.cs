using Xunit;
using RingAutoTests.Helpers;
using RingAutoTests.Pages;

namespace RingAutoTests.Tests
{
    public class CreateRequestTest
    {
        [Fact]
        public void CreateRequest_TO_ShouldSucceed()
        {
            var browser = new BrowserHelper();

            try
            {
                var login = new LoginPage(browser);
                login.Login("79160000071", "Qwerty100");
                Thread.Sleep(1000);

                var request = new Pages.RequestCreatePage(browser);
                request.SwitchLegalEntity("Мастер-Дент");
                Thread.Sleep(500);

                request.OpenCreateForm();
                Thread.Sleep(1000);

                // Шаг 1
                request.Step1_SelectSystem("Канализация");
                request.Step1_SelectType("Тех. обслуживание");
                request.Step1_SelectRoom("Радиокомната");
                request.ClickNext();

                // Шаг 2
                request.Step2_SelectAndNext();

                // Шаг 3
                request.Step3_SelectPriority("Плановая");
                request.Step3_FillDescription("Тестовая заявка ТО");
                request.Step3_FillPhone("9160000071");
                request.Step3_AttachPhoto(@"C:\testfiles\photo.jpg");
                request.Step3_FillDates();
                request.Step3_SelectResponsible("Арбузов");
                request.Step3_SelectExecutorCompany("Мастерская");
                request.Step3_SendToPartner();
                request.Step3_Submit();

                Thread.Sleep(3000);
                Assert.True(true);
            }
            finally
            {
                browser.Close();
            }
        }
    }
}