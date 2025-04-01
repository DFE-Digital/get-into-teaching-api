using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Common;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators;
using GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles;
using System;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference
{
    public sealed class DegreeStatusDomainServiceTests
    {
        [Fact]
        public void GetInferredDegreeStatusFromGraduationYear_OneYearTillGraduation_InfersFinalYearStatus()
        {
            // arrange
            IEnumerable<IChainEvaluationHandler<
                DegreeStatusInferenceRequest, DegreeStatus>> degreeStatusInferenceHandlers
                    = ChainEvaluationHandlerStub.ChainEvaluationHandlersStub<DegreeStatusInferenceRequest, DegreeStatus>();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 27));

            GraduationYear graduationYear = new(year: 2025, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);
            DegreeStatusDomainService service = new(degreeStatusInferenceHandlers);

            // act
            int? degreeStatusId =
                service.GetInferredDegreeStatusFromGraduationYear(degreeStatusInferenceRequest);

            Assert.NotNull(degreeStatusId);
            Assert.Equal(222750001, degreeStatusId);
        }

        [Fact]
        public void GetInferredDegreeStatusFromGraduationYear_TwoYearsTillGraduation_InfersSecondYearStatus()
        {
            // arrange
            IEnumerable<IChainEvaluationHandler<
                DegreeStatusInferenceRequest, DegreeStatus>> degreeStatusInferenceHandlers
                    = ChainEvaluationHandlerStub.ChainEvaluationHandlersStub<DegreeStatusInferenceRequest, DegreeStatus>();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 03, 27));

            GraduationYear graduationYear = new(year: 2026, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);
            DegreeStatusDomainService service = new(degreeStatusInferenceHandlers);

            // act
            int? degreeStatusId =
                service.GetInferredDegreeStatusFromGraduationYear(degreeStatusInferenceRequest);

            Assert.NotNull(degreeStatusId);
            Assert.Equal(222750002, degreeStatusId);
        }

        [Fact]
        public void GetInferredDegreeStatusFromGraduationYear_ThreeYearsTillGraduation_InfersFirstYearStatus()
        {
            // arrange
            IEnumerable<IChainEvaluationHandler<
                DegreeStatusInferenceRequest, DegreeStatus>> degreeStatusInferenceHandlers
                    = ChainEvaluationHandlerStub.ChainEvaluationHandlersStub<DegreeStatusInferenceRequest, DegreeStatus>();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 03, 27));

            GraduationYear graduationYear = new(year: 2027, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);
            DegreeStatusDomainService service = new(degreeStatusInferenceHandlers);

            // act
            int? degreeStatusId =
                service.GetInferredDegreeStatusFromGraduationYear(degreeStatusInferenceRequest);

            Assert.NotNull(degreeStatusId);
            Assert.Equal(222750003, degreeStatusId);
        }

        [Fact]
        public void GetInferredDegreeStatusFromGraduationYear_GreatherThanThreeYearsTillGraduation_InfersFirstYearStatus()
        {
            // arrange
            IEnumerable<IChainEvaluationHandler<
                DegreeStatusInferenceRequest, DegreeStatus>> degreeStatusInferenceHandlers
                    = ChainEvaluationHandlerStub.ChainEvaluationHandlersStub<DegreeStatusInferenceRequest, DegreeStatus>();

            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 03, 27));

            GraduationYear graduationYear = new(year: 2038, currentYearProvider);
            DegreeStatusInferenceRequest degreeStatusInferenceRequest = new(graduationYear, currentYearProvider);
            DegreeStatusDomainService service = new(degreeStatusInferenceHandlers);

            // act
            int? degreeStatusId =
                service.GetInferredDegreeStatusFromGraduationYear(degreeStatusInferenceRequest);

            Assert.NotNull(degreeStatusId);
            Assert.Equal(222750003, degreeStatusId);
        }
    }
}
