using System;

namespace GetIntoTeachingApi.Models
{
    public static class SubscriptionConfigurer
    {
        public static void ConfigureSubscriptions(
            SubscriptionTypes subscriptionType,
            Candidate candidate,
            int? channelId = null)
        {
            if (subscriptionType.HasFlag(SubscriptionTypes.TeachingEvent))
            {
                ConfigureEventsSubscriptions(candidate);
            }

            if (subscriptionType.HasFlag(SubscriptionTypes.MailingList))
            {
                ConfigureMailingListSubscriptions(candidate);
            }

            if (subscriptionType.HasFlag(SubscriptionTypes.TeacherTrainingAdviser))
            {
                ConfigureTeacherTrainingAdviserSubscriptions(candidate);
            }

            if (channelId.HasValue)
            {
                SetChannelId(candidate, subscriptionType, channelId);
            }
        }

        private static void SetChannelId(Candidate candidate, SubscriptionTypes subscriptionType, int? channelId)
        {
            if (subscriptionType.HasFlag(SubscriptionTypes.TeachingEvent))
            {
                candidate.EventsSubscriptionChannelId = channelId ?? (int)Candidate.SubscriptionChannel.Events;
            }

            if (subscriptionType.HasFlag(SubscriptionTypes.MailingList))
            {
                candidate.MailingListSubscriptionChannelId = channelId ?? (int)Candidate.SubscriptionChannel.MailingList;
            }
        }

        private static void ConfigureEventsSubscriptions(Candidate candidate)
        {
            candidate.HasEventsSubscription = true;
            candidate.EventsSubscriptionChannelId = (int)Candidate.SubscriptionChannel.Events;
            candidate.EventsSubscriptionStartAt = DateTime.UtcNow;
            candidate.EventsSubscriptionDoNotEmail = false;
            candidate.EventsSubscriptionDoNotBulkEmail = false;
            candidate.EventsSubscriptionDoNotBulkPostalMail = true;
            candidate.EventsSubscriptionDoNotPostalMail = true;
            candidate.EventsSubscriptionDoNotSendMm = false;

            SetEventsSubscriptionType(candidate);
        }

        private static void SetEventsSubscriptionType(Candidate candidate)
        {
            candidate.EventsSubscriptionTypeId = string.IsNullOrWhiteSpace(candidate.AddressPostcode)
                ? (int)Candidate.SubscriptionType.SingleEvent
                : (int)Candidate.SubscriptionType.LocalEvent;
        }

        private static void ConfigureMailingListSubscriptions(Candidate candidate)
        {
            candidate.HasMailingListSubscription = true;
            candidate.MailingListSubscriptionChannelId = (int)Candidate.SubscriptionChannel.MailingList;
            candidate.MailingListSubscriptionStartAt = DateTime.UtcNow;
            candidate.MailingListSubscriptionDoNotEmail = false;
            candidate.MailingListSubscriptionDoNotBulkEmail = false;
            candidate.MailingListSubscriptionDoNotBulkPostalMail = true;
            candidate.MailingListSubscriptionDoNotPostalMail = true;
            candidate.MailingListSubscriptionDoNotSendMm = false;
        }

        private static void ConfigureTeacherTrainingAdviserSubscriptions(Candidate candidate)
        {
            candidate.HasTeacherTrainingAdviserSubscription = true;
            candidate.TeacherTrainingAdviserSubscriptionChannelId = (int)Candidate.SubscriptionChannel.TeacherTrainingAdviser;
            candidate.TeacherTrainingAdviserSubscriptionStartAt = DateTime.UtcNow;
            candidate.TeacherTrainingAdviserSubscriptionDoNotEmail = false;
            candidate.TeacherTrainingAdviserSubscriptionDoNotBulkEmail = candidate.IsReturningToTeaching();
            candidate.TeacherTrainingAdviserSubscriptionDoNotBulkPostalMail = candidate.IsReturningToTeaching();
            candidate.TeacherTrainingAdviserSubscriptionDoNotPostalMail = candidate.IsReturningToTeaching();
            candidate.TeacherTrainingAdviserSubscriptionDoNotSendMm = candidate.IsReturningToTeaching();
        }

        [Flags]
        public enum SubscriptionTypes
        {
            None = 0,
            MailingList = 1,
            TeachingEvent = 2,
            TeacherTrainingAdviser = 4,
        }
    }
}
