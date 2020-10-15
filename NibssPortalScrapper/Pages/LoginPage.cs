using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibssPortalScrapper.Pages
{
    public class LoginPage
    {
        public IWebDriver WebDriver { get;  }
        public LoginPage(IWebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        public IWebElement txtUsername => WebDriver.FindElement(By.Name("X1"));
        public IWebElement txtPassword => WebDriver.FindElement(By.Name("X2"));
        public IWebElement txtAggregatorBankId => WebDriver.FindElement(By.Name("X3"));

        public IWebElement loginBtn => WebDriver.FindElement(By.XPath("//div[@class='signInButton']//a"));

        public void ClickLogin() => loginBtn.Click();

        public void Login(string userName, string password, string aggregatorBankId)
        {
            txtUsername.SendKeys(userName);
            txtPassword.SendKeys(password);
            txtAggregatorBankId.SendKeys(aggregatorBankId);
        }

       
    }
}
