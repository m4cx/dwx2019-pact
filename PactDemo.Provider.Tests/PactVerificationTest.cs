using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;
using PactDemo.Provider;
using PactDemo.Provider.Tests;
using PactNet;
using System;
using System.Threading.Tasks;

namespace Tests
{
    public class PactVerificationTest
    {
        private string pactServiceUri;
        private IWebHost pactServiceHost;
        private string providerServiceUri;
        private IWebHost providerServiceHost;

        [SetUp]
        public async Task Setup()
        {
            // Stat the pact service and the provider-states middleware
            pactServiceUri = "http://localhost:5002";
            pactServiceHost = WebHost.CreateDefaultBuilder()
                .UseUrls(pactServiceUri)
                .UseStartup<TestStartup>()
                .Build();
            await pactServiceHost.StartAsync();

            providerServiceUri = "http://localhost:5000";
            providerServiceHost = WebHost.CreateDefaultBuilder()
                .UseUrls(providerServiceUri)
                .UseStartup<Startup>()
                .Build();

            
            await providerServiceHost.StartAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await pactServiceHost.StopAsync();
            pactServiceHost.Dispose();

            await providerServiceHost.StopAsync();
            providerServiceHost.Dispose();
        }

        [Test]
        public void EnsurePactDemoProviderHobnoursPactWithPactDemoConsumer()
        {
            var config = new PactVerifierConfig
            {
                Verbose = true
            };
            Console.WriteLine($"Current: {Environment.CurrentDirectory}");
            var pactVerifier = new PactVerifier(config);
            pactVerifier.ProviderState($"{pactServiceUri}/provider-states")
                .ServiceProvider("PactDemo.Provider", $"{providerServiceUri}")
                .HonoursPactWith("PactDemo.Consumer")
                .PactUri(@"..\..\..\..\..\pacts\pactdemo.consumer-pactdemo.provider.json")
                .Verify();
        }

        [Test]
        [Ignore("PACT BROKER")]
        public void EnsurePactDemoProviderHobnoursPactWithPactDemoConsumerUsingPactBroker()
        {
            var config = new PactVerifierConfig
            {
                ProviderVersion = "1.0.0",
                PublishVerificationResults = true,
                Verbose = true
            };

            Console.WriteLine($"Current: {Environment.CurrentDirectory}");
            var pactVerifier = new PactVerifier(config);
            pactVerifier.ProviderState($"{pactServiceUri}/provider-states")
                .ServiceProvider("PactDemo.Provider", $"{providerServiceUri}")
                .HonoursPactWith("PactDemo.Consumer")
                .PactUri("http://localhost:8090/pacts/provider/PactDemo.Provider/consumer/PactDemo.Consumer/latest")
                .Verify();
        }
    }
}