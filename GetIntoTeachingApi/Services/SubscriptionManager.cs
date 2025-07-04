﻿using System;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Services
{
    public static class SubscriptionManager
    {
        public static void SubscribeToMailingList(Candidate candidate, DateTime utcNow)
        {
            candidate.HasMailingListSubscription = true;
            candidate.MailingListSubscriptionChannelId = (int)Candidate.SubscriptionChannel.Subscribed;
            candidate.MailingListSubscriptionStartAt = utcNow;
            candidate.MailingListSubscriptionDoNotEmail = false;
            candidate.MailingListSubscriptionDoNotBulkEmail = false;
            candidate.MailingListSubscriptionDoNotBulkPostalMail = true;
            candidate.MailingListSubscriptionDoNotPostalMail = true;
            candidate.MailingListSubscriptionDoNotSendMm = false;

            candidate.OptOutOfSms = ConsentValue(candidate.OptOutOfSms, false);
            candidate.DoNotBulkEmail = ConsentValue(candidate.DoNotBulkEmail, false);
            candidate.DoNotEmail = ConsentValue(candidate.DoNotEmail, false);
            candidate.DoNotBulkPostalMail = ConsentValue(candidate.DoNotBulkPostalMail, true);
            candidate.DoNotPostalMail = ConsentValue(candidate.DoNotPostalMail, true);
            candidate.DoNotSendMm = ConsentValue(candidate.DoNotSendMm, false);
        }

        public static void SubscribeToEvents(Candidate candidate, DateTime utcNow)
        {
            candidate.HasEventsSubscription = true;
            candidate.EventsSubscriptionChannelId = (int)Candidate.SubscriptionChannel.Subscribed;
            candidate.EventsSubscriptionStartAt = utcNow;
            candidate.EventsSubscriptionDoNotEmail = false;
            candidate.EventsSubscriptionDoNotBulkEmail = false;
            candidate.EventsSubscriptionDoNotBulkPostalMail = true;
            candidate.EventsSubscriptionDoNotPostalMail = true;
            candidate.EventsSubscriptionDoNotSendMm = false;

            if (string.IsNullOrWhiteSpace(candidate.AddressPostcode))
            {
                candidate.EventsSubscriptionTypeId = (int)Candidate.SubscriptionType.SingleEvent;
            }
            else
            {
                candidate.EventsSubscriptionTypeId = (int)Candidate.SubscriptionType.LocalEvent;
            }

            candidate.OptOutOfSms = ConsentValue(candidate.OptOutOfSms, false);
            candidate.DoNotBulkEmail = ConsentValue(candidate.DoNotBulkEmail, false);
            candidate.DoNotEmail = ConsentValue(candidate.DoNotEmail, false);
            candidate.DoNotBulkPostalMail = ConsentValue(candidate.DoNotBulkPostalMail, true);
            candidate.DoNotPostalMail = ConsentValue(candidate.DoNotPostalMail, true);
            candidate.DoNotSendMm = ConsentValue(candidate.DoNotSendMm, true);
        }

        public static void SubscribeToTeacherTrainingAdviser(Candidate candidate, DateTime utcNow)
        {
            candidate.HasTeacherTrainingAdviserSubscription = true;
            candidate.TeacherTrainingAdviserSubscriptionChannelId = (int)Candidate.SubscriptionChannel.Subscribed;
            candidate.TeacherTrainingAdviserSubscriptionStartAt = utcNow;
            candidate.TeacherTrainingAdviserSubscriptionDoNotEmail = false;
            candidate.TeacherTrainingAdviserSubscriptionDoNotBulkEmail = candidate.IsReturningToTeaching();
            candidate.TeacherTrainingAdviserSubscriptionDoNotBulkPostalMail = true;
            candidate.TeacherTrainingAdviserSubscriptionDoNotPostalMail = true;
            candidate.TeacherTrainingAdviserSubscriptionDoNotSendMm = candidate.IsReturningToTeaching();

            candidate.OptOutOfSms = ConsentValue(candidate.OptOutOfSms, false);
            candidate.DoNotBulkEmail = ConsentValue(candidate.DoNotBulkEmail, candidate.IsReturningToTeaching());
            candidate.DoNotEmail = ConsentValue(candidate.DoNotEmail, false);
            candidate.DoNotBulkPostalMail = ConsentValue(candidate.DoNotBulkPostalMail, true);
            candidate.DoNotPostalMail = ConsentValue(candidate.DoNotPostalMail, true);
            candidate.DoNotSendMm = ConsentValue(candidate.DoNotSendMm, candidate.IsReturningToTeaching());
        }

        private static bool ConsentValue(bool? currentValue, bool desiredValue)
        {
            // Never opt out if already consented.
            if (currentValue == false)
            {
                return false;
            }

            return desiredValue;
        }
    }
}
