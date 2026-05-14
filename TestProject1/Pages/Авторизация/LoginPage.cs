using OpenQA.Selenium;
using RingAutoTests.Helpers;

namespace RingAutoTests.Pages
{
    public class LoginPage
    {
        private BrowserHelper _b;

        private By PhoneField = By.Id("phone");
        private By PasswordField = By.Id("password");
        private By SubmitButton = By.Id("submit");

        public LoginPage(BrowserHelper browser)
        {
            _b = browser;
        }

        public void Login(string phone, string password)
        {
            _b.GoToUrl("https://lk.reeng.ru/");
            _b.WaitVisible(PhoneField).Clear();
            _b.WaitVisible(PhoneField).SendKeys(phone);
            _b.WaitVisible(PasswordField).Clear();
            _b.WaitVisible(PasswordField).SendKeys(password);
            _b.WaitClickable(SubmitButton).Click();
        }
    }
}