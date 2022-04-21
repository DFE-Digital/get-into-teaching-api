using System;
using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class UpsertApplicationFormJobTests
    {
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly ApplicationForm _form;
        private readonly ApplicationReference _reference;
        private readonly ApplicationChoice _choice;
        private readonly ApplicationInterview _interview;
        private readonly IMetricService _metrics;
        private readonly UpsertApplicationFormJob _job;
        private readonly Mock<ILogger<UpsertApplicationFormJob>> _mockLogger;

        public UpsertApplicationFormJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockLogger = new Mock<ILogger<UpsertApplicationFormJob>>();
            _mockAppSettings = new Mock<IAppSettings>();
            _mockCrm = new Mock<ICrmService>();
            _metrics = new MetricService();
            _job = new UpsertApplicationFormJob(
                new Env(), new Mock<IRedisService>().Object, _mockContext.Object, _metrics, _mockCrm.Object,
                _mockLogger.Object, _mockAppSettings.Object);

            _metrics.HangfireJobQueueDuration.RemoveLabelled(new[] { "UpsertApplicationFormJob" });
            _mockContext.Setup(m => m.GetJobCreatedAt(null)).Returns(DateTime.UtcNow.AddDays(-1));

            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);

            _reference = new ApplicationReference() { FindApplyId = "2" };
            _interview = new ApplicationInterview() { FindApplyId = "4" };
            _choice = new ApplicationChoice() { FindApplyId = "3", Interviews = new List<ApplicationInterview> { _interview } };
            _form = new ApplicationForm()
            {
                FindApplyId = "1",
                References = new List<ApplicationReference> { _reference },
                Choices = new List<ApplicationChoice> { _choice },
            };
        }

        [Fact]
        public void Run_OnSuccessWithNewModels_InsertsApplicationFormAndRelatedModels()
        {
            var formId = Guid.NewGuid();
            var choiceId = Guid.NewGuid();
            var referenceId = Guid.NewGuid();
            var interviewId = Guid.NewGuid();

            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _mockCrm.Setup(m => m.GetFindApplyModels<ApplicationForm>(new[] { _form.FindApplyId })).Returns(Array.Empty<ApplicationForm>());
            _mockCrm.Setup(m => m.GetFindApplyModels<ApplicationChoice>(new[] { _choice.FindApplyId })).Returns(Array.Empty<ApplicationChoice>());
            _mockCrm.Setup(m => m.GetFindApplyModels<ApplicationInterview>(new[] { _interview.FindApplyId })).Returns(Array.Empty<ApplicationInterview>());
            _mockCrm.Setup(m => m.GetFindApplyModels<ApplicationReference>(new[] { _reference.FindApplyId })).Returns(Array.Empty<ApplicationReference>());

            _mockCrm.Setup(m => m.Save(It.IsAny<ApplicationForm>())).Callback<BaseModel>(f => f.Id = formId);
            _mockCrm.Setup(m => m.Save(It.IsAny<ApplicationChoice>())).Callback<BaseModel>(c => c.Id = choiceId);
            _mockCrm.Setup(m => m.Save(It.IsAny<ApplicationInterview>())).Callback<BaseModel>(i => i.Id = interviewId);
            _mockCrm.Setup(m => m.Save(It.IsAny<ApplicationReference>())).Callback<BaseModel>(r => r.Id = referenceId);

            var json = _form.SerializeChangeTracked();
            _job.Run(json, null);

            _form.Id = formId;
            _choice.Id = choiceId;
            _choice.ApplicationFormId = formId;
            _interview.Id = interviewId;
            _interview.ApplicationChoiceId = choiceId;
            _reference.Id = referenceId;
            _reference.ApplicationFormId = formId;

            _mockCrm.Verify(mock => mock.Save(It.Is<ApplicationForm>(f => IsMatch(_form, f))), Times.Once);
            _mockCrm.Verify(mock => mock.Save(It.Is<ApplicationChoice>(c => IsMatch(_choice, c))), Times.Once);
            _mockCrm.Verify(mock => mock.Save(It.Is<ApplicationInterview>(i => IsMatch(_interview, i))), Times.Once);
            _mockCrm.Verify(mock => mock.Save(It.Is<ApplicationReference>(r => IsMatch(_reference, r))), Times.Once);

            _mockLogger.VerifyInformationWasCalled("UpsertApplicationFormJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled($"UpsertApplicationFormJob - Payload {Redactor.RedactJson(json)}");
            _mockLogger.VerifyInformationWasCalled($"UpsertApplicationFormJob - Succeeded - {_form.Id}");
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertApplicationFormJob" }).Count.Should().Be(1);
        }

        [Fact]
        public void Run_OnSuccessWithExistingModels_UpdatesApplicationFormAndRelatedModels()
        {
            var formId = Guid.NewGuid();
            var choiceId = Guid.NewGuid();
            var referenceId = Guid.NewGuid();
            var interviewId = Guid.NewGuid();

            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _mockCrm.Setup(m => m.GetFindApplyModels<ApplicationForm>(new[] { _form.FindApplyId }))
                .Returns(new[] { new ApplicationForm() { Id = formId, FindApplyId = _form.FindApplyId } });
            _mockCrm.Setup(m => m.GetFindApplyModels<ApplicationChoice>(new[] { _choice.FindApplyId }))
                .Returns(new[] { new ApplicationChoice() { Id = choiceId, FindApplyId = _choice.FindApplyId } });
            _mockCrm.Setup(m => m.GetFindApplyModels<ApplicationInterview>(new[] { _interview.FindApplyId }))
                .Returns(new[] { new ApplicationInterview() { Id = interviewId, FindApplyId = _interview.FindApplyId } });
            _mockCrm.Setup(m => m.GetFindApplyModels<ApplicationReference>(new[] { _reference.FindApplyId }))
                .Returns(new[] { new ApplicationReference() { Id = referenceId, FindApplyId = _reference.FindApplyId } });

            var json = _form.SerializeChangeTracked();
            _job.Run(json, null);

            _form.Id = formId;
            _choice.Id = choiceId;
            _choice.ApplicationFormId = formId;
            _interview.Id = interviewId;
            _interview.ApplicationChoiceId = choiceId;
            _reference.Id = referenceId;
            _reference.ApplicationFormId = formId;

            _mockCrm.Verify(mock => mock.Save(It.Is<ApplicationForm>(f => IsMatch(_form, f))), Times.Once);
            _mockCrm.Verify(mock => mock.Save(It.Is<ApplicationChoice>(c => IsMatch(_choice, c))), Times.Once);
            _mockCrm.Verify(mock => mock.Save(It.Is<ApplicationInterview>(i => IsMatch(_interview, i))), Times.Once);
            _mockCrm.Verify(mock => mock.Save(It.Is<ApplicationReference>(r => IsMatch(_reference, r))), Times.Once);
        }

        [Fact]
        public void Run_OnFailure_Logs()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(23);

            _job.Run(_form.SerializeChangeTracked(), null);

            _mockCrm.Verify(mock => mock.Save(It.IsAny<ApplicationForm>()), Times.Never);

            _mockLogger.VerifyInformationWasCalled("UpsertApplicationFormJob - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("UpsertApplicationFormJob - Deleted");
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertApplicationFormJob" }).Count.Should().Be(1);
        }

        [Fact]
        public void Run_WhenCrmIntegrationPaused_Aborts()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);

            var json = _form.SerializeChangeTracked();
            Action action = () => _job.Run(json, null);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("UpsertApplicationFormJob - Aborting (CRM integration paused).");
        }

        private static bool IsMatch(object objectA, object objectB)
        {
            objectA.Should().BeEquivalentTo(objectB);
            return true;
        }
    }
}
