using System;
using System.Net.Http;

namespace CommonFixtures
{
    public interface IWebAppFactory<TStartup>  : IDisposable
        where TStartup : class
    {
        HttpClient HttpClient { get; }
        Uri RootUri { get; }
        IServiceProvider Services { get; }
    }
}