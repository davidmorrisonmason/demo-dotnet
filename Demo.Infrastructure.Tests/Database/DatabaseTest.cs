using Demo.DomainServices.Encryption;
using Demo.DomainServices.Context;
using Demo.Infrastructure.Data;
using Demo.Model.UnitTests.Model;
using Microsoft.EntityFrameworkCore;
using TestContext = Demo.Model.UnitTests.TestContext;

namespace Demo.Infrastructure.Tests.Database
{
    public class DatabaseTest : ModelTest
    {
        protected DbContextOptions<ApplicationDbContext> DbContextOptions { get; private set; }
        public RequestContext TestRequestContext { get; }
        public static string TestApiKeyPlainText = "test-api-key";
        public static string TestApiKeyHashedText = "";
        public static int TestClientId
        {
            get => TestContext.TestClientId;
            set => TestContext.TestClientId = value;
        }


        /// <summary>
        /// Flag to indicate if a Client corresponding to the request context test client should be inserted into the database
        /// </summary>
        protected virtual bool AddRequestContextTestClient => true;

        public DatabaseTest(BaseDatabaseFixture databaseFixture)
        {
            databaseFixture.ResetDatabase();

            DbContextOptions = databaseFixture.DbContextOptions;
            SetUpDatabaseContext(DbContextOptions);

            TestApiKeyHashedText = new EncryptionService().OneWayHash(TestApiKeyPlainText);

            if (AddRequestContextTestClient)
            {
                var client = BuilderFactory.NewClientBuilder()
                    .With(x => x.ApiKey, TestApiKeyHashedText)
                    .BuildAndPersist();
                TestClientId = client.Id;
            }

            TestRequestContext = CreateTestRequestContext();
        }

        private RequestContext CreateTestRequestContext()
        {
            var requestContext = new RequestContext();
            requestContext.SetClient(new Demo.Model.Domain.Client(TestClientId, "Test Client", TestApiKeyHashedText));
            return requestContext;
        }
    }
}
