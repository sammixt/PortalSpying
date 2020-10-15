using NibssPortalScrapper.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibssPortalScrapper.Steps
{
    class LoginStep
    {
        IWebDriver WebDriver;
        LoginPage loginPage ;
        private string UserName;
        private string Password;
        private string BankCode;
        public LoginStep(IWebDriver webDriver)
        {
            WebDriver = webDriver;
            loginPage = new LoginPage(WebDriver);
        }

        void SetUsername(string username) => UserName = username;
        void SetPassword(string password) => Password = password;
        void SetBankCode(string code) => BankCode = code;

        public void SetUserDetails(string username, string password, string code)
        {
            SetUsername(username);
            SetPassword(password);
            SetBankCode(code);
        }
        public void EnterLoginDetails()
        {
            loginPage.Login(UserName, Password, BankCode);
        }

        public void ClickLoginBtn()
        {
            loginPage.ClickLogin();
        }

        //Lu
    }
}
