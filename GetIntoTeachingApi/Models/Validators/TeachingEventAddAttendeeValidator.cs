﻿using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingEventAddAttendeeValidator : AbstractValidator<TeachingEventAddAttendee>
    {
        public TeachingEventAddAttendeeValidator(IStore store)
        {
            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.Email).NotEmpty();
            RuleFor(request => request.EventId).NotEmpty();
            RuleFor(request => request.AcceptedPolicyId).NotEmpty();

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }
    }
}