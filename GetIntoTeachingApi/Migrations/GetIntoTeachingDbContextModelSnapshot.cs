﻿// <auto-generated />
using System;
using GetIntoTeachingApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GetIntoTeachingApi.Migrations
{
    [DbContext(typeof(GetIntoTeachingDbContext))]
    partial class GetIntoTeachingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.Candidate", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("AddressCity")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine1")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine2")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine3")
                        .HasColumnType("text");

                    b.Property<string>("AddressPostcode")
                        .HasColumnType("text");

                    b.Property<string>("AddressStateOrProvince")
                        .HasColumnType("text");

                    b.Property<string>("AddressTelephone")
                        .HasColumnType("text");

                    b.Property<int?>("AdviserEligibilityId")
                        .HasColumnType("integer");

                    b.Property<int?>("AdviserRequirementId")
                        .HasColumnType("integer");

                    b.Property<int?>("AdviserStatusId")
                        .HasColumnType("integer");

                    b.Property<int?>("AssignmentStatusId")
                        .HasColumnType("integer");

                    b.Property<int?>("ChannelId")
                        .HasColumnType("integer");

                    b.Property<string>("ClassroomExperienceNotesRaw")
                        .HasColumnType("text");

                    b.Property<int?>("ConsiderationJourneyStageId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("CountryId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DbsCertificateIssuedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool?>("DoNotBulkEmail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("DoNotBulkPostalMail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("DoNotEmail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("DoNotPostalMail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("DoNotSendMm")
                        .HasColumnType("boolean");

                    b.Property<string>("EligibilityRulesPassed")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<int?>("EventsSubscriptionChannelId")
                        .HasColumnType("integer");

                    b.Property<bool?>("EventsSubscriptionDoNotBulkEmail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("EventsSubscriptionDoNotBulkPostalMail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("EventsSubscriptionDoNotEmail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("EventsSubscriptionDoNotPostalMail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("EventsSubscriptionDoNotSendMm")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("EventsSubscriptionStartAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("EventsSubscriptionTypeId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("FindApplyCreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FindApplyId")
                        .HasColumnType("text");

                    b.Property<int?>("FindApplyPhaseId")
                        .HasColumnType("integer");

                    b.Property<int?>("FindApplyStatusId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("FindApplyUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<int?>("GdprConsentId")
                        .HasColumnType("integer");

                    b.Property<bool?>("HasDbsCertificate")
                        .HasColumnType("boolean");

                    b.Property<bool?>("HasEventsSubscription")
                        .HasColumnType("boolean");

                    b.Property<int?>("HasGcseEnglishId")
                        .HasColumnType("integer");

                    b.Property<int?>("HasGcseMathsId")
                        .HasColumnType("integer");

                    b.Property<int?>("HasGcseScienceId")
                        .HasColumnType("integer");

                    b.Property<bool?>("HasMailingListSubscription")
                        .HasColumnType("boolean");

                    b.Property<bool?>("HasTeacherTrainingAdviserSubscription")
                        .HasColumnType("boolean");

                    b.Property<int?>("InitialTeacherTrainingYearId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsNewRegistrant")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("MagicLinkToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("MagicLinkTokenExpiresAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("MagicLinkTokenStatusId")
                        .HasColumnType("integer");

                    b.Property<int?>("MailingListSubscriptionChannelId")
                        .HasColumnType("integer");

                    b.Property<bool?>("MailingListSubscriptionDoNotBulkEmail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("MailingListSubscriptionDoNotBulkPostalMail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("MailingListSubscriptionDoNotEmail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("MailingListSubscriptionDoNotPostalMail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("MailingListSubscriptionDoNotSendMm")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("MailingListSubscriptionStartAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("MasterId")
                        .HasColumnType("uuid");

                    b.Property<bool>("Merged")
                        .HasColumnType("boolean");

                    b.Property<string>("MobileTelephone")
                        .HasColumnType("text");

                    b.Property<bool?>("OptOutOfGdpr")
                        .HasColumnType("boolean");

                    b.Property<bool?>("OptOutOfSms")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("OwningBusinessUnitId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PhoneCallId")
                        .HasColumnType("uuid");

                    b.Property<int?>("PlanningToRetakeGcseEnglishId")
                        .HasColumnType("integer");

                    b.Property<int?>("PlanningToRetakeGcseMathsId")
                        .HasColumnType("integer");

                    b.Property<int?>("PlanningToRetakeGcseScienceId")
                        .HasColumnType("integer");

                    b.Property<int?>("PreferredContactMethodId")
                        .HasColumnType("integer");

                    b.Property<int?>("PreferredEducationPhaseId")
                        .HasColumnType("integer");

                    b.Property<int?>("PreferredPhoneNumberTypeId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("PreferredTeachingSubjectId")
                        .HasColumnType("uuid");

                    b.Property<int?>("RegistrationStatusId")
                        .HasColumnType("integer");

                    b.Property<string>("SecondaryEmail")
                        .HasColumnType("text");

                    b.Property<Guid?>("SecondaryPreferredTeachingSubjectId")
                        .HasColumnType("uuid");

                    b.Property<string>("SecondaryTelephone")
                        .HasColumnType("text");

                    b.Property<DateTime?>("StatusIsWaitingToBeAssignedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("TeacherId")
                        .HasColumnType("text");

                    b.Property<int?>("TeacherTrainingAdviserSubscriptionChannelId")
                        .HasColumnType("integer");

                    b.Property<bool?>("TeacherTrainingAdviserSubscriptionDoNotBulkEmail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("TeacherTrainingAdviserSubscriptionDoNotBulkPostalMail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("TeacherTrainingAdviserSubscriptionDoNotEmail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("TeacherTrainingAdviserSubscriptionDoNotPostalMail")
                        .HasColumnType("boolean");

                    b.Property<bool?>("TeacherTrainingAdviserSubscriptionDoNotSendMm")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("TeacherTrainingAdviserSubscriptionStartAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Telephone")
                        .HasColumnType("text");

                    b.Property<int?>("TypeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PhoneCallId");

                    b.ToTable("Candidates");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.CandidatePastTeachingPosition", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CandidateId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("EducationPhaseId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("SubjectTaughtId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId");

                    b.ToTable("CandidatePastTeachingPosition");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.CandidatePrivacyPolicy", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("AcceptedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("AcceptedPolicyId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CandidateId")
                        .HasColumnType("uuid");

                    b.Property<int>("ConsentReceivedById")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("MeanOfConsentId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId")
                        .IsUnique();

                    b.ToTable("CandidatePrivacyPolicy");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.CandidateQualification", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CandidateId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("DegreeStatusId")
                        .HasColumnType("integer");

                    b.Property<string>("DegreeSubject")
                        .HasColumnType("text");

                    b.Property<int?>("TypeId")
                        .HasColumnType("integer");

                    b.Property<int?>("UkDegreeGradeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId");

                    b.ToTable("CandidateQualification");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.PhoneCall", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("AppointmentRequired")
                        .HasColumnType("boolean");

                    b.Property<string>("CandidateId")
                        .HasColumnType("text");

                    b.Property<int?>("ChannelId")
                        .HasColumnType("integer");

                    b.Property<int?>("DestinationId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsAppointment")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDirectionCode")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ScheduledAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Subject")
                        .HasColumnType("text");

                    b.Property<string>("TalkingPoints")
                        .HasColumnType("text");

                    b.Property<string>("Telephone")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PhoneCall");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.PrivacyPolicy", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PrivacyPolicies");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.TeachingEvent", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BuildingId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("EndAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("ProviderContactEmail")
                        .HasColumnType("text");

                    b.Property<string>("ProviderOrganiser")
                        .HasColumnType("text");

                    b.Property<string>("ProviderTargetAudience")
                        .HasColumnType("text");

                    b.Property<string>("ProviderWebsiteUrl")
                        .HasColumnType("text");

                    b.Property<string>("ProvidersList")
                        .HasColumnType("text");

                    b.Property<string>("ReadableId")
                        .HasColumnType("text");

                    b.Property<string>("ScribbleId")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("StatusId")
                        .HasColumnType("integer");

                    b.Property<string>("Summary")
                        .HasColumnType("text");

                    b.Property<int>("TypeId")
                        .HasColumnType("integer");

                    b.Property<string>("VideoUrl")
                        .HasColumnType("text");

                    b.Property<string>("WebFeedId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.ToTable("TeachingEvents");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.TeachingEventBuilding", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("AddressCity")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine1")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine2")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine3")
                        .HasColumnType("text");

                    b.Property<string>("AddressPostcode")
                        .HasColumnType("text");

                    b.Property<Point>("Coordinate")
                        .HasColumnType("geography");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("Venue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TeachingEventBuildings");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.TeachingEventRegistration", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CandidateId")
                        .HasColumnType("uuid");

                    b.Property<int?>("ChannelId")
                        .HasColumnType("integer");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.Property<bool?>("IsCancelled")
                        .HasColumnType("boolean");

                    b.Property<bool?>("RegistrationNotificationSeen")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId");

                    b.ToTable("TeachingEventRegistration");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Location", b =>
                {
                    b.Property<string>("Postcode")
                        .HasColumnType("text");

                    b.Property<Point>("Coordinate")
                        .HasColumnType("geography");

                    b.Property<int>("Source")
                        .HasColumnType("integer");

                    b.HasKey("Postcode");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.LookupItem", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("EntityName")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id", "EntityName");

                    b.ToTable("LookupItems");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.PickListItem", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("EntityName")
                        .HasColumnType("text");

                    b.Property<string>("AttributeName")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id", "EntityName", "AttributeName");

                    b.ToTable("PickListItems");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.Candidate", b =>
                {
                    b.HasOne("GetIntoTeachingApi.Models.Crm.PhoneCall", "PhoneCall")
                        .WithMany()
                        .HasForeignKey("PhoneCallId");

                    b.Navigation("PhoneCall");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.CandidatePastTeachingPosition", b =>
                {
                    b.HasOne("GetIntoTeachingApi.Models.Crm.Candidate", null)
                        .WithMany("PastTeachingPositions")
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.CandidatePrivacyPolicy", b =>
                {
                    b.HasOne("GetIntoTeachingApi.Models.Crm.Candidate", null)
                        .WithOne("PrivacyPolicy")
                        .HasForeignKey("GetIntoTeachingApi.Models.Crm.CandidatePrivacyPolicy", "CandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.CandidateQualification", b =>
                {
                    b.HasOne("GetIntoTeachingApi.Models.Crm.Candidate", null)
                        .WithMany("Qualifications")
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.TeachingEvent", b =>
                {
                    b.HasOne("GetIntoTeachingApi.Models.Crm.TeachingEventBuilding", "Building")
                        .WithMany("TeachingEvents")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Building");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.TeachingEventRegistration", b =>
                {
                    b.HasOne("GetIntoTeachingApi.Models.Crm.Candidate", null)
                        .WithMany("TeachingEventRegistrations")
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.Candidate", b =>
                {
                    b.Navigation("PastTeachingPositions");

                    b.Navigation("PrivacyPolicy");

                    b.Navigation("Qualifications");

                    b.Navigation("TeachingEventRegistrations");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.TeachingEventBuilding", b =>
                {
                    b.Navigation("TeachingEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
