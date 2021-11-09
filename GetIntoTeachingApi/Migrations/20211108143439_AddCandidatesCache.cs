using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GetIntoTeachingApi.Migrations
{
    public partial class AddCandidatesCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhoneCall",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<string>(type: "text", nullable: true),
                    ChannelId = table.Column<int>(type: "integer", nullable: true),
                    DestinationId = table.Column<int>(type: "integer", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Telephone = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    IsAppointment = table.Column<bool>(type: "boolean", nullable: false),
                    AppointmentRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsDirectionCode = table.Column<bool>(type: "boolean", nullable: false),
                    TalkingPoints = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneCall", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PreferredTeachingSubjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    SecondaryPreferredTeachingSubjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwningBusinessUnitId = table.Column<Guid>(type: "uuid", nullable: true),
                    MasterId = table.Column<Guid>(type: "uuid", nullable: true),
                    PreferredEducationPhaseId = table.Column<int>(type: "integer", nullable: true),
                    InitialTeacherTrainingYearId = table.Column<int>(type: "integer", nullable: true),
                    ChannelId = table.Column<int>(type: "integer", nullable: true),
                    HasGcseEnglishId = table.Column<int>(type: "integer", nullable: true),
                    HasGcseMathsId = table.Column<int>(type: "integer", nullable: true),
                    HasGcseScienceId = table.Column<int>(type: "integer", nullable: true),
                    PlanningToRetakeGcseEnglishId = table.Column<int>(type: "integer", nullable: true),
                    PlanningToRetakeGcseMathsId = table.Column<int>(type: "integer", nullable: true),
                    PlanningToRetakeGcseScienceId = table.Column<int>(type: "integer", nullable: true),
                    ConsiderationJourneyStageId = table.Column<int>(type: "integer", nullable: true),
                    TypeId = table.Column<int>(type: "integer", nullable: true),
                    AssignmentStatusId = table.Column<int>(type: "integer", nullable: true),
                    AdviserEligibilityId = table.Column<int>(type: "integer", nullable: true),
                    AdviserRequirementId = table.Column<int>(type: "integer", nullable: true),
                    PreferredPhoneNumberTypeId = table.Column<int>(type: "integer", nullable: true),
                    PreferredContactMethodId = table.Column<int>(type: "integer", nullable: true),
                    GdprConsentId = table.Column<int>(type: "integer", nullable: true),
                    MagicLinkTokenStatusId = table.Column<int>(type: "integer", nullable: true),
                    AdviserStatusId = table.Column<int>(type: "integer", nullable: true),
                    RegistrationStatusId = table.Column<int>(type: "integer", nullable: true),
                    FindApplyStatusId = table.Column<int>(type: "integer", nullable: true),
                    FindApplyPhaseId = table.Column<int>(type: "integer", nullable: true),
                    StatusIsWaitingToBeAssignedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Merged = table.Column<bool>(type: "boolean", nullable: false),
                    FindApplyId = table.Column<string>(type: "text", nullable: true),
                    FindApplyUpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FindApplyCreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    SecondaryEmail = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MobileTelephone = table.Column<string>(type: "text", nullable: true),
                    AddressTelephone = table.Column<string>(type: "text", nullable: true),
                    AddressLine1 = table.Column<string>(type: "text", nullable: true),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    AddressLine3 = table.Column<string>(type: "text", nullable: true),
                    AddressCity = table.Column<string>(type: "text", nullable: true),
                    AddressStateOrProvince = table.Column<string>(type: "text", nullable: true),
                    AddressPostcode = table.Column<string>(type: "text", nullable: true),
                    Telephone = table.Column<string>(type: "text", nullable: true),
                    SecondaryTelephone = table.Column<string>(type: "text", nullable: true),
                    HasDbsCertificate = table.Column<bool>(type: "boolean", nullable: true),
                    DbsCertificateIssuedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ClassroomExperienceNotesRaw = table.Column<string>(type: "text", nullable: true),
                    TeacherId = table.Column<string>(type: "text", nullable: true),
                    EligibilityRulesPassed = table.Column<string>(type: "text", nullable: true),
                    DoNotBulkEmail = table.Column<bool>(type: "boolean", nullable: true),
                    DoNotBulkPostalMail = table.Column<bool>(type: "boolean", nullable: true),
                    DoNotEmail = table.Column<bool>(type: "boolean", nullable: true),
                    DoNotPostalMail = table.Column<bool>(type: "boolean", nullable: true),
                    DoNotSendMm = table.Column<bool>(type: "boolean", nullable: true),
                    OptOutOfSms = table.Column<bool>(type: "boolean", nullable: true),
                    OptOutOfGdpr = table.Column<bool>(type: "boolean", nullable: true),
                    IsNewRegistrant = table.Column<bool>(type: "boolean", nullable: false),
                    MagicLinkToken = table.Column<string>(type: "text", nullable: true),
                    MagicLinkTokenExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasTeacherTrainingAdviserSubscription = table.Column<bool>(type: "boolean", nullable: true),
                    TeacherTrainingAdviserSubscriptionChannelId = table.Column<int>(type: "integer", nullable: true),
                    TeacherTrainingAdviserSubscriptionStartAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TeacherTrainingAdviserSubscriptionDoNotBulkEmail = table.Column<bool>(type: "boolean", nullable: true),
                    TeacherTrainingAdviserSubscriptionDoNotBulkPostalMail = table.Column<bool>(type: "boolean", nullable: true),
                    TeacherTrainingAdviserSubscriptionDoNotEmail = table.Column<bool>(type: "boolean", nullable: true),
                    TeacherTrainingAdviserSubscriptionDoNotPostalMail = table.Column<bool>(type: "boolean", nullable: true),
                    TeacherTrainingAdviserSubscriptionDoNotSendMm = table.Column<bool>(type: "boolean", nullable: true),
                    HasMailingListSubscription = table.Column<bool>(type: "boolean", nullable: true),
                    MailingListSubscriptionChannelId = table.Column<int>(type: "integer", nullable: true),
                    MailingListSubscriptionStartAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MailingListSubscriptionDoNotBulkEmail = table.Column<bool>(type: "boolean", nullable: true),
                    MailingListSubscriptionDoNotBulkPostalMail = table.Column<bool>(type: "boolean", nullable: true),
                    MailingListSubscriptionDoNotEmail = table.Column<bool>(type: "boolean", nullable: true),
                    MailingListSubscriptionDoNotPostalMail = table.Column<bool>(type: "boolean", nullable: true),
                    MailingListSubscriptionDoNotSendMm = table.Column<bool>(type: "boolean", nullable: true),
                    HasEventsSubscription = table.Column<bool>(type: "boolean", nullable: true),
                    EventsSubscriptionChannelId = table.Column<int>(type: "integer", nullable: true),
                    EventsSubscriptionTypeId = table.Column<int>(type: "integer", nullable: true),
                    EventsSubscriptionStartAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EventsSubscriptionDoNotBulkEmail = table.Column<bool>(type: "boolean", nullable: true),
                    EventsSubscriptionDoNotBulkPostalMail = table.Column<bool>(type: "boolean", nullable: true),
                    EventsSubscriptionDoNotEmail = table.Column<bool>(type: "boolean", nullable: true),
                    EventsSubscriptionDoNotPostalMail = table.Column<bool>(type: "boolean", nullable: true),
                    EventsSubscriptionDoNotSendMm = table.Column<bool>(type: "boolean", nullable: true),
                    PhoneCallId = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidates_PhoneCall_PhoneCallId",
                        column: x => x.PhoneCallId,
                        principalTable: "PhoneCall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidatePastTeachingPosition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectTaughtId = table.Column<Guid>(type: "uuid", nullable: true),
                    EducationPhaseId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatePastTeachingPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatePastTeachingPosition_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidatePrivacyPolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcceptedPolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsentReceivedById = table.Column<int>(type: "integer", nullable: false),
                    MeanOfConsentId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatePrivacyPolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatePrivacyPolicy_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateQualification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: true),
                    UkDegreeGradeId = table.Column<int>(type: "integer", nullable: true),
                    DegreeStatusId = table.Column<int>(type: "integer", nullable: true),
                    DegreeSubject = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateQualification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateQualification_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeachingEventRegistration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<int>(type: "integer", nullable: true),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: true),
                    RegistrationNotificationSeen = table.Column<bool>(type: "boolean", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachingEventRegistration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeachingEventRegistration_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidatePastTeachingPosition_CandidateId",
                table: "CandidatePastTeachingPosition",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatePrivacyPolicy_CandidateId",
                table: "CandidatePrivacyPolicy",
                column: "CandidateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateQualification_CandidateId",
                table: "CandidateQualification",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_PhoneCallId",
                table: "Candidates",
                column: "PhoneCallId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingEventRegistration_CandidateId",
                table: "TeachingEventRegistration",
                column: "CandidateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidatePastTeachingPosition");

            migrationBuilder.DropTable(
                name: "CandidatePrivacyPolicy");

            migrationBuilder.DropTable(
                name: "CandidateQualification");

            migrationBuilder.DropTable(
                name: "TeachingEventRegistration");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "PhoneCall");
        }
    }
}
