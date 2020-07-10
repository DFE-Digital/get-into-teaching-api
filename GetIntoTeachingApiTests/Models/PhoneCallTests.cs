﻿using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class PhoneCallTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(PhoneCall);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "phonecall");

            type.GetProperty("ChannelId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_channelcreation" && a.Type == typeof(OptionSetValue));

            type.GetProperty("ScheduledAt").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "scheduledstart");
            type.GetProperty("Telephone").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "phonenumber");
            type.GetProperty("Subject").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "subject");
            type.GetProperty("IsAppointment").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_appointmentflag");
            type.GetProperty("AppointmentRequired").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_appointmentrequired");
            type.GetProperty("IsDirectionCode").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "directioncode");
        }

        [Fact]
        public void PopulateWithCandidate_SetsTelephoneAndSubject()
        {
            var phoneCall = new PhoneCall();
            var candidate = new Candidate() { FirstName = "John", LastName = "Doe", Telephone = "123456789" };

            phoneCall.PopulateWithCandidate(candidate);

            phoneCall.Telephone.Should().Be(candidate.Telephone);
            phoneCall.Subject.Should().Be("Scheduled phone call requested by John Doe");
        }

        [Fact]
        public void IsAppointment_DefaultValue_IsCorrect()
        {
            new PhoneCall().IsAppointment.Should().BeFalse();
        }

        [Fact]
        public void AppointmentRequired_DefaultValue_IsCorrect()
        {
            new PhoneCall().AppointmentRequired.Should().BeFalse();
        }

        [Fact]
        public void IsDirectionCode_DefaultValue_IsCorrect()
        {
            new PhoneCall().IsDirectionCode.Should().BeTrue();
        }
    }
}
