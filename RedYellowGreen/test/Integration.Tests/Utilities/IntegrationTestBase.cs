using Testcontainers.PostgreSql;

namespace Integration.Tests.Utilities;

[TestClass]
public abstract class IntegrationTestBase
{
    // this will setup a database container per test, which can get a bit slow
    // could optimize by creating a database in the same container per test
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:18")
        .Build();

    protected ApiClient Client = null!;
    private ApiFactory _factory = null!;

    [TestInitialize]
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _factory = new ApiFactory(_postgres.GetConnectionString());
        Client = new ApiClient(_factory.CreateClient(), _factory);
    }

    [TestCleanup]
    public async Task DisposeAsync()
    {
        Client.Dispose();
        await _postgres.DisposeAsync();
    }
}