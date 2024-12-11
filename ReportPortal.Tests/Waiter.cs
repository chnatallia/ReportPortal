using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Modules.BrowsingContext;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ReportPortal.Tests
{
    public enum Conditions
    {
        AlertIsPresent = 1,
        ElementIsVisible = 2,
        ElementExists = 3,
        ElementToBeClickableBy = 4,
        ElementToBeClickableWebElement = 5,
        ElementToBeSelected = 6,
        TitleContains = 7,
        UrlMatches = 8,
        TextToBePresentInElementValue = 9,
        TextToBePresentInElement= 10
    }

    public static class Waiter
    {

        public static void ImplicitWait(IWebDriver driver, double seconds)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(seconds);
        }
        public static void ExplicitWaitElementExist(IWebDriver driver, By locator, double seconds)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(seconds)).Until(ExpectedConditions.ElementExists(locator));
            }
            catch(WebDriverException exception)
            {
                throw new NotFoundException($"Cannot find element:{locator}", exception);
            }
            
        }
        public static void ExplicitWaitElementIsVisible(IWebDriver driver, By locator, double seconds)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(seconds)).Until(ExpectedConditions.ElementIsVisible(locator));
            }
            catch (WebDriverException exception)
            { 
                throw new ElementNotVisibleException($"Element is not visible:{locator}", exception);
            }

        }
        public static void ExplicitWaitAlertIsPresent(IWebDriver driver, double seconds = 10)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(seconds)).Until(ExpectedConditions.AlertIsPresent());
            }
            catch(WebDriverException exception)
            {
                throw new NoAlertPresentException("Alert is not present", exception);
            }

        }
        public static void ExplicitWaitElementToBeClickableBy(IWebDriver driver, double seconds, By locator)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(seconds)).Until(ExpectedConditions.ElementToBeClickable(locator));
            }
            catch (WebDriverException exception)
            {
                throw new NoAlertPresentException($"Element is not clickable:{locator}", exception);
            }

        }
        public static void ExplicitWaitElementToBeSelected(IWebDriver driver, double seconds, By locator)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(seconds)).Until(ExpectedConditions.ElementToBeSelected(locator));
            }
            catch (WebDriverException exception)
            {
                throw new NoAlertPresentException($"Element is not selectable:{locator}", exception);
            }

        }

        public static void ExplicitWaitTitleContains(IWebDriver driver, double seconds, string title)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(seconds)).Until(ExpectedConditions.TitleContains(title));
            }
            catch (WebDriverException exception)
            {
                throw new NoAlertPresentException($"Title does not contain:{title}", exception);
            }
        }
        public static void ExplicitWaitUrlMatches(IWebDriver driver, double seconds, string url)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(seconds)).Until(ExpectedConditions.UrlMatches(url));
            }
            catch (WebDriverException exception)
            {
                throw new NoAlertPresentException($"Url is not:{url}", exception);
            }
        }
        public static void ExplicitWaitTextToBePresentInElementValue(IWebDriver driver, double seconds, string text, By locator)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(seconds)).Until(ExpectedConditions.TextToBePresentInElementValue(locator,text));
            }
            catch (WebDriverException exception)
            {
                throw new NoAlertPresentException($"Text is not present in element value:{text}", exception);
            }
        }
        public static void ExplicitWaitTextToBePresentInElement(IWebDriver driver, double seconds, string text, By locator)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(seconds)).Until(ExpectedConditions.TextToBePresentInElementValue(locator, text));
            }
            catch (WebDriverException exception)
            {
                throw new NoAlertPresentException($"Text is not present in element:{text}", exception);
            }

        }

        public static void FluentWaitElementIsVisible(IWebDriver driver, By locator, double seconds, double polling)
        {
            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver)
            {
                Timeout = TimeSpan.FromSeconds(seconds),
                PollingInterval = TimeSpan.FromMilliseconds(polling),
            };
            fluentWait.Until(d =>
            {
                IWebElement element = d.FindElement(locator);
                return element.Displayed;
            });
        }
        
    }




























}
