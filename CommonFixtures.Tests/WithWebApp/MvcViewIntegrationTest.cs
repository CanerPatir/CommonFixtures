using System;
using CommonFixtures.SampleWebApp;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace CommonFixtures.Tests.WithWebApp
{
    public class MvcViewIntegrationTest : WithWebApp<Startup>
    {
        protected override bool SeleniumEnabled => true;
        protected override bool SeleniumHeadless => false;

        
        private Func<IWebDriver, bool> WaitUntilElementIsEnabled(By locator)
        {
            bool Condition(IWebDriver d)
            {
                IWebElement e = d.FindElement(locator);
                return e.Displayed && e.Enabled;
            }

            return Condition;
        }
        
        [Fact(Skip = "Skipping to keep github actions pipeline consist")]
        public void Counter_Test()
        { 
            // Arrange
            var waitForElement = new WebDriverWait(Selenium, TimeSpan.FromSeconds(10));
            waitForElement.Until(WaitUntilElementIsEnabled(By.Id("counter")));
            
            var button = Selenium.FindElement(By.Id("btn"));
            var counterSpan = Selenium.FindElement(By.Id("counter"));
            
            // Act
            Assert.Equal(0, int.Parse(counterSpan.Text));
            button.Click();

            // Assert
            counterSpan = Selenium.FindElement(By.Id("counter"));
            Assert.Equal(1, int.Parse(counterSpan.Text));
        }
    }
}