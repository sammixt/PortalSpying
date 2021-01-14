using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibssPortalScrapper.Pages
{
    public class HomePage
    {
        public IWebDriver WebDriver { get; }
        public HomePage(IWebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        public IWebElement CompanySegment => WebDriver.FindElement(By.Id("seg_company"));

        public bool AssertHomePage(string assertiveName) => CompanySegment.Text.Contains(assertiveName);

        #region reconciliation
        public IWebElement ReconciliationDropDown => WebDriver.FindElement(By.XPath("//a[@headerindex = '4h']//span[@class = 'accordsuffix']//img"));
        public void ClickReconciliationDropDown() => ReconciliationDropDown.Click();

        public IWebElement BankSettlementReport => WebDriver.FindElement(By.XPath("//a[.//font[text()='BANK: Settlement Report']]"));
        public void ClickBankSettlementReport() => BankSettlementReport.Click();

        public IWebElement Session => WebDriver.FindElement(By.XPath("//input[@type='text'][@name='sessionno'][@id='sessionno']"));

        public void EnterSession(string sessionTime)
        {
            Session.SendKeys(sessionTime);
        }

        public IWebElement ViewType => WebDriver.FindElement(By.XPath("//input[@type='radio'][@name='viewType'][@id='viewType_detail']"));
        public void CheckViewType() => ViewType.Click();

        #endregion reconciliation
        public IWebElement ReportDropDown => WebDriver.FindElement(By.XPath("//a[@headerindex = '5h']//span[@class = 'accordsuffix']//img"));
        public void ClickDropDown() => ReportDropDown.Click();

        public IWebElement AllTransactionReport => WebDriver.FindElement(By.XPath("//a[.//font[text()='All Transactions Report']]"));
        public void ClickAllTransactionReport() => AllTransactionReport.Click();
        public IWebElement StartDay => WebDriver.FindElement(By.XPath("//select[@name='vday']"));
        public void SelectStartDay(string day) 
        {
            var selectStartDay = new SelectElement(StartDay);
            selectStartDay.SelectByText(day);
        }
        public IWebElement StartMonth => WebDriver.FindElement(By.XPath("//select[@name='vmonth']"));
        public void SelectStartMonth(string month)
        {
            var selectStartMonth = new SelectElement(StartMonth);
            selectStartMonth.SelectByText(month);
        }

        public IWebElement StartYear => WebDriver.FindElement(By.XPath("//select[@name='vyear']"));
        public void SelectStartYear(string year)
        {
            var selectStartYear = new SelectElement(StartYear);
            selectStartYear.SelectByText(year);
        }

        public IWebElement EndDay => WebDriver.FindElement(By.XPath("//select[@name='vday1']"));
        public void SelectEndDay(string day)
        {
            var selectEndDay = new SelectElement(EndDay);
            selectEndDay.SelectByText(day);
        }
        public IWebElement EndMonth => WebDriver.FindElement(By.XPath("//select[@name='vmonth1']"));
        public void SelectEndMonth(string month)
        {
            var selectEndMonth = new SelectElement(EndMonth);
            selectEndMonth.SelectByText(month);
        }

        public IWebElement EndYear => WebDriver.FindElement(By.XPath("//select[@name='vyear1']"));
        public void SelectEndYear(string year)
        {
            var selectEndYear = new SelectElement(EndYear);
            selectEndYear.SelectByText(year);
        }

        public IWebElement DisplayType => WebDriver.FindElement(By.XPath("//input[@type='radio'][@name='option_4'][@value='S']"));
        public void CheckTransactionList() => DisplayType.Click();

        public IWebElement DisplayFormat => WebDriver.FindElement(By.XPath("//input[@type='radio'][@name='format'][@value='xls']"));

        public void CheckExcel() => DisplayFormat.Click();

        public IWebElement DownloadBtn => WebDriver.FindElement(By.XPath("//input[@type='button'][@name='newWinn']"));
        public void ClickDownloadBtn() => DownloadBtn.Click();

        public IWebElement SignOut => WebDriver.FindElement(By.XPath("//a[.//font[contains(text(),'Sign Out')]]"));
        public void ClickSignOut() => SignOut.Click();
    }
}
