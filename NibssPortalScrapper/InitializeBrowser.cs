using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibssPortalScrapper
{
    public class InitializeBrowser
    {
        private static IWebDriver driver;

        public static bool IsInternetExplorer = false;
        public static bool IsFireFox = false;
        public static bool IsChrome = false;

        public static IWebDriver UserFireFox(string url, string downloadPath)
        {
            //Firefox Profile

            FirefoxProfile profile = new FirefoxProfile();
            profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/octet-stream;application/xml;text/xml;");

            profile.SetPreference("browser.helperApps.alwaysAsk.force", false);

            // profile.SetPreference("browser.download.manager.showWhenStarting", false);

            profile.SetPreference("browser.download.folderList", 2);

            profile.SetPreference("browser.download.dir", downloadPath);

            FirefoxOptions opt = new FirefoxOptions() { Profile = profile, };
            driver = new FirefoxDriver(opt);
            driver.Url = url;
            IsFireFox = true;
            return driver;
        }

        public static IWebDriver UserChrome(string url, string downloadPath, bool headless)
        {
            string browserpath = AppDomain.CurrentDomain.BaseDirectory + "\\chrome";
            var chromeOptions = new ChromeOptions();
            if (headless)
            {
                chromeOptions.AddArgument("--headless");
            }
            chromeOptions.AddUserProfilePreference("download.default_directory", downloadPath);
            chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "false");
            chromeOptions.AddUserProfilePreference(preferenceName: "download.prompt_for_download", "false");
            driver = new ChromeDriver(browserpath, chromeOptions);
            //IWebDriver driver = new ChromeDriver(@"\\Mac\Home\Movies\chromedriver_win32", chromeOptions);
            driver.Url = url;
            driver.Manage().Window.Maximize();
            IsChrome = true;
            return driver;
        }

        public static IWebDriver UseInternetExplorer(string url)
        {
            // create a web reference for the web driver
            var ieOptions = new InternetExplorerOptions()
            {
                InitialBrowserUrl = url,
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                IgnoreZoomLevel = true,
                EnableNativeEvents = true,
                EnsureCleanSession = true
                //Proxy = seleniumProxy

            };
            string IeDriverLocation = AppDomain.CurrentDomain.BaseDirectory;
            driver = new InternetExplorerDriver($"{IeDriverLocation}\\IEDriverServer_Win32_2.39.0", ieOptions);
            IsInternetExplorer = true;
            driver.Navigate().GoToUrl(url);
            return driver;
        }
    }
}
