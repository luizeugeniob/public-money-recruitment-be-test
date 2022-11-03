using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using VacationRental.Api;
using Xunit;

namespace VacationRental.Tests.Common
{
    [CollectionDefinition("Integration")]
    public sealed class IntegrationFixture : IDisposable, ICollectionFixture<IntegrationFixture>
    {
        private readonly WebApplicationFactory<Program> _server;

        public HttpClient Client { get; }

        public IntegrationFixture()
        {
            _server = new WebApplicationFactory<Program>();

            Client = _server.CreateClient();
        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}