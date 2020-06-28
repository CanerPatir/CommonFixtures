using System;
using CommonFixtures.SampleWebApp;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using static OpenQA.Selenium.Support.UI.ExpectedConditions;

namespace CommonFixtures.Tests.WithWebApp
{
    public class MvcViewIntegrationTest : WithWebApp<Startup>
    {
        protected override bool SeleniumEnabled => true;
        protected override bool SeleniumHeadless => true;

        [Fact]
        public void Counter_Test()
        {
            // Arrange
            WebDriverWait waitForElement = new WebDriverWait(Selenium, TimeSpan.FromSeconds(10));
            waitForElement.Until(ElementIsVisible(By.Id("counter")));
            
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