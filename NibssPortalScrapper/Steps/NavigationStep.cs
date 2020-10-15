using NibssPortalScrapper.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibssPortalScrapper.Steps
{
    public class NavigationStep
    {

        IWebDriver WebDriver;
        HomePage homePage;
        public NavigationStep(IWebDriver webDriver)
        {
            WebDriver = webDriver;
            homePage = new HomePage(WebDriver);
        }

        public void SwitchToHeader() => WebDriver.SwitchTo().Frame("header");

        public bool IsHomePage() => homePage.AssertHomePage("ECOBANK PLC");

        public void SwitchToDefault() => WebDriver.SwitchTo().DefaultContent();

        public void SwitchToSidebar() => WebDriver.SwitchTo().Frame("left");

        public void ClickReportDropDown() => homePage.ClickDropDown();
        public void ClickAllTransReportLink() => homePage.ClickAllTransactionReport();
        public void SwitchToContent() => WebDriver.SwitchTo().Frame("right");

        public void EnterStartDate(string day, string month, string year)
        {
            homePage.SelectStartDay(day);
            homePage.SelectStartMonth(month);
            homePage.SelectStartYear(year);
        }

        public void EnterEndDate(string day, string month, string year)
        {
            homePage.SelectEndDay(day);
            homePage.SelectEndMonth(month);
            homePage.SelectEndYear(year);
        }

        public void SelectDownloadFormat()
        {
            homePage.CheckTransactionList();
            homePage.CheckExcel();
        }

        public void ClickDownload() => homePage.ClickDownloadBtn();

        public void SignOut() => homePage.ClickSignOut();


    }
}
