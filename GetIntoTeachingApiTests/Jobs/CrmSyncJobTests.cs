using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class CrmSyncJobTests
    {
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IStore> _mockStore;
        private readonly CrmSyncJob _job;

        public CrmSyncJobTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _mockStore = new Mock<IStore>();
            _job = new CrmSyncJob(_mockCrm.Object, _mockStore.Object);
        }

        [Fact]
        public async void RunAsync_CallsSync()
        {
            await _job.RunAsync();

            _mockStore.Verify(mock => mock.SyncAsync(_mockCrm.Object), Times.Once);
        }
    }
}