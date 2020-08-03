using CSScriptLib;
using CsvHelper;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Integration
{
    public class TeacherTrainingAdviserSignUpTests
    {
        [Fact]
        public async void SignUp_WithFixtures_BehaveAsExpected()
        {
            using var reader = new StreamReader("./Fixtures/teacher_training_adviser_sign_ups.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var rowsToSkip = new[] { "Firstname", "identity[first_name]", "" };
            csv.Configuration.ShouldSkipRecord = row => rowsToSkip.Contains(row[2]);

            await csv.ReadAsync();
            csv.ReadHeader();

            var headers = csv.Context.HeaderRecord.Where(h => !string.IsNullOrEmpty(h));

            while (await csv.ReadAsync())
            {
                var signUp = CreateTeacherTrainingAdviserSignUp(csv, headers);

            }
        }

        private TeacherTrainingAdviserSignUp CreateTeacherTrainingAdviserSignUp(CsvReader csv, IEnumerable<string> headers)
        {
            var signUp = new TeacherTrainingAdviserSignUp();

            headers.ForEach(h =>
            {
                var propertyName = h.Substring(0, 1).ToUpper() + h.Substring(1);
                var property = signUp.GetType().GetProperty(propertyName);

                property.Should().NotBeNull($"Unrecognized property: {propertyName}");

                property.SetValue(signUp, csv.GetField(h), null);
            });

            return signUp;
        }
    }
}
