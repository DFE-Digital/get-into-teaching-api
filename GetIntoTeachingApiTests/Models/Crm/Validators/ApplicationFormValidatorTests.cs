﻿using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.Validators
{
    public class ApplicationFormValidatorTests
    {
        private readonly ApplicationFormValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public ApplicationFormValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new ApplicationFormValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new PickListItem { Id = 123 };

            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_applyapplicationform", "dfe_applyphase"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_applyapplicationform", "dfe_applystatus"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_applyapplicationform", "dfe_recruitmentyear"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var form = new ApplicationForm()
            {
                CandidateId = Guid.NewGuid(),
                ApplyId = "67890",
                PhaseId = mockPickListItem.Id,
                StatusId = mockPickListItem.Id,
                RecruitmentCycleYearId = mockPickListItem.Id,
            };

            var result = _validator.TestValidate(form);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ApplyIdIsEmpty_HasError()
        {
            var form = new ApplicationForm() { ApplyId = "" };
            var result = _validator.TestValidate(form);

            result.ShouldHaveValidationErrorFor("ApplyId");
        }

        [Fact]
        public void Validate_OptionSetIsNotValid_HasError()
        {
            var form = new ApplicationForm() { PhaseId = 456, StatusId = 789, RecruitmentCycleYearId = 876 };
            var result = _validator.TestValidate(form);

            result.ShouldHaveValidationErrorFor("PhaseId");
            result.ShouldHaveValidationErrorFor("StatusId");
            result.ShouldHaveValidationErrorFor("RecruitmentCycleYearId");
        }

        [Fact]
        public void Validate_RequiredAttributeIsNull_HasError()
        {
            var form = new ApplicationForm() { PhaseId = null, StatusId = null, RecruitmentCycleYearId = null };
            var result = _validator.TestValidate(form);

            result.ShouldHaveValidationErrorFor("PhaseId");
            result.ShouldHaveValidationErrorFor("StatusId");
            result.ShouldHaveValidationErrorFor("RecruitmentCycleYearId");
        }
    }
}
