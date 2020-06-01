using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
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
        public void Run_CallsSync()
        {
            _job.Run();

            _mockStore.Verify(mock => mock.Sync(_mockCrm.Object), Times.Once);
        }
    }
}