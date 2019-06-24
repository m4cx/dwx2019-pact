using NUnit.Framework;
using PactNet;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PactDemo.Consumer.Tests
{
    [TestFixture]
    public class ProductTests : ConsumerProductPactTestBase
    {
        [SetUp]
        public void SetUp()
        {
            MockProviderService.ClearInteractions();
        }

        [Test]
        public async Task GetProduct_ReturnsExpectedProduct()
        {
            var guidRegex = "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
            var expectedProductId = Guid.Parse("E0C2E684-D83F-45C3-A6E5-D62A6F83A0BD");
            var expectedName = "Test";
            var expectedProduct = new
            {
                id = Match.Regex(expectedProductId.ToString(), $"^{guidRegex}$"),
                name = Match.Type(expectedName),
                description = Match.Type("A product for testing"),
            };

            MockProviderService
                .Given("A Product with expected structure") // Describe the state the provider needs to setup
                .UponReceiving("a GET request for a single product") // textual description - business case
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = Match.Regex($"/{expectedProductId}", $"^\\/{guidRegex}$"),
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, object>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = expectedProduct
                });

            var consumer = new ProductClient(MockServerBaseUri);
            var result = await consumer.Get(expectedProductId);

            Assert.AreEqual(expectedProductId, result.Id);
            Assert.AreEqual(expectedName, result.Name);

            MockProviderService.VerifyInteractions();
        }

        #region Pact-Broker

        [Test]
        [Ignore("PACT BROKER")]
        public async Task PublishToPactBroker()
        {
            var pactPublisher = new PactPublisher("http://localhost:8090");
            pactPublisher.PublishToBroker($"../../pacts/{ConsumerName.ToLower()}-{ProviderName.ToLower()}.json", "1.0.0");
        }

        #endregion
    }
}
