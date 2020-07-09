﻿using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class TeacherTrainingAdviserSignUpTests
    {
        [Fact]
        public void Constructor_WithCandidate_MapsCorrectly()
        {
            var latestQualification = new CandidateQualification()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.AddDays(10),
                DegreeStatusId = 1,
                UkDegreeGradeId = 2,
                Subject = "English"
            };

            var qualifications = new List<CandidateQualification>()
            {
                new CandidateQualification() { Id = Guid.NewGuid(), CreatedAt = DateTime.Now.AddDays(3) },
                latestQualification,
                new CandidateQualification() { Id = Guid.NewGuid(), CreatedAt = DateTime.Now.AddDays(5) },
            };

            var latestPastTeachingPosition = new CandidatePastTeachingPosition()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.AddDays(10),
                SubjectTaughtId = Guid.NewGuid(),
            };

            var pastTeachingPositions = new List<CandidatePastTeachingPosition>()
            {
                new CandidatePastTeachingPosition() { Id = Guid.NewGuid(), CreatedAt = DateTime.Now.AddDays(3) },
                latestPastTeachingPosition,
                new CandidatePastTeachingPosition() { Id = Guid.NewGuid(), CreatedAt = DateTime.Now.AddDays(5) },
            };

            var candidate = new Candidate()
            {
                Id = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                CountryId = Guid.NewGuid(),
                InitialTeacherTrainingYearId = 1,
                PreferredEducationPhaseId = 2,
                HasGcseEnglishId = 3,
                HasGcseMathsId = 4,
                HasGcseScienceId = 5,
                PlanningToRetakeGcseEnglishId = 6,
                PlanningToRetakeGcseMathsId = 7,
                PlanningToRetakeCgseScienceId = 8,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now,
                Telephone = "1234567",
                TeacherId = "abc123",
                AddressLine1 = "Address 1",
                AddressLine2 = "Address 2",
                AddressLine3 = "Address 3",
                AddressCity = "City",
                AddressState = "State",
                AddressPostcode = "KY11 9YU",
                Qualifications = qualifications,
                PastTeachingPositions = pastTeachingPositions,
            };

            var response = new TeacherTrainingAdviserSignUp(candidate);

            response.CandidateId.Should().Be(candidate.Id);
            response.PreferredTeachingSubjectId.Should().Be(candidate.PreferredTeachingSubjectId);
            response.CountryId.Should().Be(candidate.CountryId);
            response.InitialTeacherTrainingYearId.Should().Be(candidate.InitialTeacherTrainingYearId);
            response.PreferredEducationPhaseId.Should().Be(candidate.PreferredEducationPhaseId);
            response.HasGcseEnglishId.Should().Be(candidate.HasGcseEnglishId);
            response.HasGcseMathsId.Should().Be(candidate.HasGcseMathsId);
            response.HasGcseScienceId.Should().Be(candidate.HasGcseScienceId);
            response.PlanningToRetakeCgseScienceId.Should().Be(candidate.PlanningToRetakeCgseScienceId);
            response.PlanningToRetakeGcseEnglishId.Should().Be(candidate.PlanningToRetakeGcseEnglishId);
            response.PlanningToRetakeGcseMathsId.Should().Be(candidate.PlanningToRetakeGcseMathsId);
            response.Email.Should().Be(candidate.Email);
            response.FirstName.Should().Be(candidate.FirstName);
            response.LastName.Should().Be(candidate.LastName);
            response.TeacherId.Should().Be(candidate.TeacherId);
            response.Telephone.Should().Be(candidate.Telephone);
            response.AddressLine1.Should().Be(candidate.AddressLine1);
            response.AddressLine2.Should().Be(candidate.AddressLine2);
            response.AddressLine3.Should().Be(candidate.AddressLine3);
            response.AddressCity.Should().Be(candidate.AddressCity);
            response.AddressState.Should().Be(candidate.AddressState);
            response.AddressPostcode.Should().Be(candidate.AddressPostcode);

            response.QualificationId.Should().Be(latestQualification.Id);
            response.DegreeStatusId.Should().Be(latestQualification.DegreeStatusId);
            response.UkDegreeGradeId.Should().Be(latestQualification.UkDegreeGradeId);
            response.DegreeSubject.Should().Be(latestQualification.Subject);

            response.PastTeachingPositionId.Should().Be(latestPastTeachingPosition.Id);
            response.SubjectTaughtId.Should().Be(latestPastTeachingPosition.SubjectTaughtId);

            response.AlreadySubscribedToTeacherTrainingAdviser.Should().BeTrue();
        }

        [Fact]
        public void Candidate_MapsCorrectly()
        {
            var request = new TeacherTrainingAdviserSignUp()
            {
                CandidateId = Guid.NewGuid(),
                QualificationId = Guid.NewGuid(),
                SubjectTaughtId = Guid.NewGuid(),
                PastTeachingPositionId = Guid.NewGuid(),
                PreferredTeachingSubjectId = Guid.NewGuid(),
                CountryId = Guid.NewGuid(),
                AcceptedPolicyId = Guid.NewGuid(),
                UkDegreeGradeId = 1,
                DegreeStatusId = 1,
                InitialTeacherTrainingYearId = 3,
                PreferredEducationPhaseId = 4,
                HasGcseEnglishId = 5,
                HasGcseMathsId = 6,
                HasGcseScienceId = 7,
                PlanningToRetakeGcseEnglishId = 7,
                PlanningToRetakeGcseMathsId = 8,
                PlanningToRetakeCgseScienceId = 9,
                Email = "email@address.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now,
                Telephone = "1234567",
                TeacherId = "abc123",
                DegreeSubject = "Maths",
                AddressLine1 = "Address 1",
                AddressLine2 = "Address 2",
                AddressLine3 = "Address 3",
                AddressCity = "City",
                AddressState = "State",
                AddressPostcode = "KY11 9YU",
                PhoneCallScheduledAt = DateTime.Now,
            };

            var candidate = request.Candidate;

            candidate.Id.Should().Equals(request.CandidateId);
            candidate.PreferredTeachingSubjectId.Should().Equals(request.PreferredTeachingSubjectId);
            candidate.CountryId.Should().Equals(request.CountryId);
            candidate.InitialTeacherTrainingYearId.Should().Equals(request.InitialTeacherTrainingYearId);
            candidate.PreferredEducationPhaseId.Should().Equals(request.PreferredEducationPhaseId);
            candidate.HasGcseEnglishId.Should().Equals(request.HasGcseEnglishId);
            candidate.HasGcseMathsId.Should().Equals(request.HasGcseMathsId);
            candidate.HasGcseScienceId.Should().Equals(request.HasGcseScienceId);
            candidate.PlanningToRetakeGcseEnglishId.Should().Equals(request.PlanningToRetakeGcseEnglishId);
            candidate.PlanningToRetakeGcseMathsId.Should().Equals(request.PlanningToRetakeGcseMathsId);
            candidate.PlanningToRetakeCgseScienceId.Should().Equals(request.PlanningToRetakeCgseScienceId);
            candidate.AdviserRequirementId.Should().BeNull();
            candidate.AdviserEligibilityId.Should().BeNull();
            candidate.StatusId.Should().BeNull();
            candidate.Email.Should().Be(request.Email);
            candidate.FirstName.Should().Be(request.FirstName);
            candidate.LastName.Should().Be(request.LastName);
            candidate.DateOfBirth.Should().Be(request.DateOfBirth);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.Telephone.Should().Be(request.Telephone);
            candidate.TeacherId.Should().Be(request.TeacherId);
            candidate.AddressLine1.Should().Be(request.AddressLine1);
            candidate.AddressLine2.Should().Be(request.AddressLine2);
            candidate.AddressLine3.Should().Be(request.AddressLine3);
            candidate.AddressCity.Should().Be(request.AddressCity);
            candidate.AddressState.Should().Be(request.AddressState);
            candidate.AddressPostcode.Should().Be(request.AddressPostcode);
            candidate.ChannelId.Should().BeNull();
            candidate.EligibilityRulesPassed.Should().Be("false");
            candidate.OptOutOfSms.Should().BeFalse();
            candidate.DoNotBulkEmail.Should().BeFalse();
            candidate.DoNotEmail.Should().BeFalse();
            candidate.DoNotBulkPostalMail.Should().BeFalse();
            candidate.DoNotPostalMail.Should().BeFalse();
            candidate.DoNotSendMm.Should().BeFalse();

            candidate.PrivacyPolicy.AcceptedPolicyId.Should().Be((Guid)request.AcceptedPolicyId);

            candidate.PhoneCall.ScheduledAt.Should().Be((DateTime)request.PhoneCallScheduledAt);
            candidate.PhoneCall.Telephone.Should().Be(request.Telephone);
            candidate.PhoneCall.ChannelId.Should().Be((int)PhoneCall.Channel.CallbackRequest);

            candidate.PastTeachingPositions.First().Id.Should().Be(request.PastTeachingPositionId);
            candidate.PastTeachingPositions.First().SubjectTaughtId.Should().Be(request.SubjectTaughtId);
            candidate.PastTeachingPositions.First().EducationPhaseId.Should().Be((int)CandidatePastTeachingPosition.EducationPhase.Secondary);

            candidate.Qualifications.First().Id.Should().Be(request.QualificationId);
            candidate.Qualifications.First().UkDegreeGradeId.Should().Be(request.UkDegreeGradeId);
            candidate.Qualifications.First().DegreeStatusId.Should().Be(request.DegreeStatusId);
            candidate.Qualifications.First().Subject.Should().Be(request.DegreeSubject);

            candidate.Subscriptions.First().TypeId.Should().Be((int)Subscription.ServiceType.TeacherTrainingAdviser);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNull_IsTeacherTrainingAdviser()
        {
            var request = new TeacherTrainingAdviserSignUp() { CandidateId = null };

            request.Candidate.ChannelId.Should().Be((int)Candidate.Channel.TeacherTrainingAdviser);
        }

        [Fact]
        public void Candidate_ChannelIdWhenCandidateIdIsNotNull_IsNull()
        {
            var request = new TeacherTrainingAdviserSignUp() { CandidateId = Guid.NewGuid() };

            request.Candidate.ChannelId.Should().BeNull();
        }

        [Fact]
        public void Candidate_PhoneCallScheduledAtIsNull_NoPhoneCallIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { PhoneCallScheduledAt = null };

            request.Candidate.PhoneCall.Should().BeNull();
        }

        [Fact]
        public void Candidate_SubjectTaughtIdIsNull_NoPastTeachingPositionIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { SubjectTaughtId = null };

            request.Candidate.PastTeachingPositions.Should().BeEmpty();
        }

        [Fact]
        public void Candidate_QualificationFieldsAreNull_NoQualificationIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { UkDegreeGradeId = null, DegreeStatusId = null, DegreeSubject = null };

            request.Candidate.Qualifications.Should().BeEmpty();
        }

        [Fact]
        public void Candidate_UkDegreeGradeIdIsPresent_QualificationIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { UkDegreeGradeId = 1, DegreeStatusId = null, DegreeSubject = null };

            request.Candidate.Qualifications.Count.Should().Be(1);
        }

        [Fact]
        public void Candidate_DegreeStatusIdIsPresent_QualificationIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { UkDegreeGradeId = null, DegreeStatusId = 1, DegreeSubject = null };

            request.Candidate.Qualifications.Count.Should().Be(1);
        }

        [Fact]
        public void Candidate_DegreeSubjectIdIsPresent_QualificationIsCreated()
        {
            var request = new TeacherTrainingAdviserSignUp() { UkDegreeGradeId = null, DegreeStatusId = null, DegreeSubject = "Maths" };

            request.Candidate.Qualifications.Count.Should().Be(1);
        }
    }
}
