using System;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Xunit.Abstractions;
using LogLevel = OpenQA.Selenium.LogLevel;

namespace CommonFixtures
{
    internal class Selenium
    {
        public Uri SutUri { get; }
        private readonly ITestOutputHelper _output;
        private readonly bool _runHeadlessBrowser;

        public Selenium(string scheme, string host, int port, bool runHeadlessBrowser, ITestOutputHelper output) : this(scheme, host, port, runHeadlessBrowser)
        {
            _output = output;
        }

        public Selenium(string scheme, string host, int port, bool runHeadlessBrowser) : this(new Uri($"{scheme}://{host}:{port}"), runHeadlessBrowser)
        {
        }

        public Selenium(Uri sutUri, bool runHeadlessBrowser)
        {
            SutUri = sutUri;
            _runHeadlessBrowser = runHeadlessBrowser;
        }

        public ChromeDriver CreateBrowser(CancellationToken cancellationToken, bool captureBrowserMemory = false)
        {
            var options = new ChromeOptions();

            if (_runHeadlessBrowser)
            {
                options.AddArgument("--headless");
            }

            if (captureBrowserMemory)
            {
                options.AddArgument("--enable-precise-memory-info");
            }

            options.SetLoggingPreference(LogType.Browser, LogLevel.All);

            var attempt = 0;
            const int maxAttempts = 3;
            do
            {
                try
                {
                    var driver = new ChromeDriver(options);

                    driver
                        .Manage()
                        .Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

                    driver.Navigate().GoToUrl(SutUri);
                    
                    // Run in background.
                     _ = Task.Run(async () =>
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
                            var consoleLogs =  driver.Manage().Logs.GetLog(LogType.Browser);
                             foreach (var entry in consoleLogs)
                            {
                                _output?.WriteLine($"[Browser Log]: {entry.Timestamp}: {entry.Message}");
                            }
                        }
                    }, cancellationToken);

                    return driver;
                }
                catch (Exception ex)
                {
                    _output?.WriteLine($"Error initializing WebDriver: {ex.Message}");
                }

                attempt++;
            } while (attempt < maxAttempts);

            throw new InvalidOperationException("Couldn't create a Selenium Chrome driver client. The server is irresponsive");
        }
    }
}