using System;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using Xunit;

namespace GetIntoTeachingApiTests.Models
{
    public class ClassroomExperienceNoteTests
    {
        [Fact]
        public void ToString_FormatsWithConsistentPadding()
        {
            var note = new ClassroomExperienceNote()
            {
                Action = "RECORDED",
                RecordedAt = new DateTime(2020, 1, 1),
                Date = new DateTime(2020, 3, 2),
                SchoolUrn = "123456",
                SchoolName = "John Doe Primary",
            };

            note.ToString().Should().Be("01/01/2020 RECORDED               02/03/2020 123456 John Doe Primary\r\n");

            note.Action = "CANCELLED BY SCHOOL";
            note.SchoolUrn = "123";
            note.SchoolName = "Test";

            note.ToString().Should().Be("01/01/2020 CANCELLED BY SCHOOL    02/03/2020 123    Test\r\n");
        }
    }
}
