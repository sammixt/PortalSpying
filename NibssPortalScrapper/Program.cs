using NibssPortalScrapper.Steps;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NibssPortalScrapper
{
    class Program
    {
        static IWebDriver WebDriver;
       static Logger log = new Logger();
        static void Main(string[] args)
        {
            bool output = false;
            Console.WriteLine($"Web scrapper started @ {DateTime.Now}");
            //Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Blue;
            //string screenshotFolder = AppDomain.CurrentDomain.BaseDirectory + "\\Screenshot";
            InputParams inputParams = new InputParams(args);
            WebDriver = InitializeBrowser.UserChrome(inputParams.WebUrl, inputParams.DownloadPath, inputParams.Headless);
            try
            {
                DeleteFiles(inputParams.DownloadPath);
                LoginStep steps = new LoginStep(WebDriver);
                NavigationStep navSteps = new NavigationStep(WebDriver);
                Thread.Sleep(4000);
                log.Info("Entering users details");
                steps.SetUserDetails(inputParams.Username, inputParams.Password, inputParams.BankCode);
                steps.EnterLoginDetails();
                steps.ClickLoginBtn();
                Thread.Sleep(4000);

                navSteps.SwitchToSidebar();
                navSteps.ClickReportDropDown();
                navSteps.ClickAllTransReportLink();
                log.Info("Loading All Transactions Page");
                Thread.Sleep(7000);
                navSteps.SwitchToDefault();
                navSteps.SwitchToContent();

                log.Info("Entering Date");
                navSteps.EnterStartDate(inputParams.StartDay, inputParams.StartMonth, inputParams.StartYear);
                Thread.Sleep(2000);
                navSteps.EnterEndDate(inputParams.EndDay, inputParams.EndMonth, inputParams.EndYear);

                log.Info("Selecting Download format");
                navSteps.SelectDownloadFormat();
                Thread.Sleep(2000);
                navSteps.ClickDownload();
                bool wasMaxTimeExceeded = false;
                DirectoryInfo directory = new DirectoryInfo(inputParams.DownloadPath);
                //var checkForDownloadFile = directory.GetFiles("echequesummary*.crdownload");
                wasMaxTimeExceeded = BeginDownload(directory, "echequesummary");

                log.Info("Waiting for download to Commence.. 60secs delay");

                if (!wasMaxTimeExceeded)
                {
                    navSteps.SwitchToDefault();
                    navSteps.SwitchToSidebar();
                    log.Info("Bank Settlement Report Page");
                    navSteps.ClicReconDropDown();
                    navSteps.ClickBankSettlementReport();
                    Thread.Sleep(7000);
                    navSteps.SwitchToDefault();
                    navSteps.SwitchToContent();

                    navSteps.DownloadReconciliationReport(inputParams.Session);
                    wasMaxTimeExceeded = BeginDownload(directory, "settlementreport");
                }

                //Thread.Sleep(30000);
                int size;
                //DirectoryInfo directory = new DirectoryInfo(inputParams.DownloadPath);
                if (!wasMaxTimeExceeded)
                {
                    do
                    {
                        var files = directory.GetFiles("*.crdownload");
                        size = files.Length;
                        if (size >= 1) { output = true; }
                        Thread.Sleep(10000);

                    } while (size != 0);
                    output = true;
                    Console.WriteLine("Download Completed");
                }
                else
                {
                    output = false;
                }

                //output = true;

                navSteps.SwitchToContent();
                navSteps.SwitchToHeader();
                navSteps.SignOut();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                WebDriver.Quit();
            }

            WriteOutput(output, inputParams.DownloadPath);
            log.Info(output.ToString());


        }

        private static bool BeginDownload(DirectoryInfo directory, string search)
        {
            bool hasDownloadStarted = false;
            bool wasDownloadStarted = false;
            bool wasMaxTimeExceeded = false;
            int maxCount = 150, startCount = 1;
            do
            {
                var checkForDownloadFile = directory.GetFiles($"{search}*.crdownload");
                if (checkForDownloadFile.Length >= 1)
                {
                    hasDownloadStarted = true;
                }
                else
                {
                    Thread.Sleep(2000);
                    startCount++;
                }

                if (hasDownloadStarted | (startCount == maxCount))
                {
                    if (startCount == maxCount)
                    {
                        wasMaxTimeExceeded = true;
                    }
                    wasDownloadStarted = true;

                }
                Console.Write(".");
            } while (!wasDownloadStarted);

            return wasMaxTimeExceeded;
        }

        public static void WriteOutput(bool result, string downloadPath)
        {
            if (File.Exists(downloadPath))
            {
                log.Info("Found an existing file.......");
                log.Info("Deleting existing file from location.........");
                File.Delete($"{downloadPath}NibsResult.txt");
                File.WriteAllText($"{downloadPath}NibsResult.txt", result.ToString());
            }
            else
            {
                File.WriteAllText($"{downloadPath}NibsResult.txt", result.ToString());
            }
        }

        public static void WriteOutput(string result, string downloadPath)
        {
            if (File.Exists(downloadPath))
            {
                log.Info("Found an existing file.......");
                log.Info($"Deleting existing file from location {downloadPath}");
                File.Delete($"{downloadPath}downloadoutcome.txt");
                File.WriteAllText($"{downloadPath}downloadoutcome.txt", result);
            }
            else
            {
                File.WriteAllText($"{downloadPath}downloadoutcome.txt", result);
            }
        }

        public static void DeleteFiles(string downloadPath)
        {
            DirectoryInfo directory = new DirectoryInfo(downloadPath);
            var files = directory.GetFiles("echequesummary*");
            if (files.Any())
            {
                foreach(var file in files)
                {
                    file.Delete();
                    log.Info($"Deleting files:  {file.Name}");
                }
            }
        }

    }
}
