using System.Collections.Generic;
using System.Text.Json.Serialization;
using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Mvc.Controllers;
using Xunit;

namespace GetIntoTeachingApiTests.Utils
{
    public class LoggableMessageComposerTests
    {
        [Fact]
        public void LogMessageForObject__WithLoggable_SerializesLoggableAttributes()
        {
            var message = LoggableMessageComposer.LogMessageForObject(new StubLoggable());

            message.Should().Be("{\"Name\":\"Ross\"}");
        }

        [Fact]
        public void LogMessageForObject_WithArrayOfLoggable_SerializesAllObjectLoggableAttributes()
        {
            var objs = new List<StubLoggable>() { new StubLoggable("Ross"), new StubLoggable("James") };

            var message = LoggableMessageComposer.LogMessageForObject(objs);

            message.Should().Be("[{\"Name\":\"Ross\"},{\"Name\":\"James\"}]");
        }

        [Fact]
        public void LogMessageForObject_WithUnloggable_ReturnsNull()
        {
            var message = LoggableMessageComposer.LogMessageForObject(new StubUnloggable());

            message.Should().BeNull();
        }

        [Fact]
        public void LogMessageForObject_WithPrimitive_ReturnsNull()
        {
            var message = LoggableMessageComposer.LogMessageForObject(123);

            message.Should().BeNull();
        }

        [Fact]
        public void LogMessage_WithSingleMessage_FormatsCorrectly()
        {
            var messages = new[] { "message" };
            var descriptor = new ControllerActionDescriptor()
            {
                ActionName = "Action",
                ControllerName = "Controller"
            };

            var message = LoggableMessageComposer.LogMessage("Prefix", descriptor, messages);

            message.Should().Be("Prefix Controller:Action - message");
        }

        [Fact]
        public void LogMessage_WithMultipleMessages_FormatsCorrectly()
        {
            var messages = new[] { "message1", "message2" };
            var descriptor = new ControllerActionDescriptor()
            {
                ActionName = "Action",
                ControllerName = "Controller"
            };

            var message = LoggableMessageComposer.LogMessage("Prefix", descriptor, messages);

            message.Should().Be("Prefix Controller:Action - message1\nmessage2");
        }

        [Fact]
        public void LogMessage_WithNullMessages_FormatsCorrectly()
        {
            var messages = new[] { null as string };
            var descriptor = new ControllerActionDescriptor()
            {
                ActionName = "Action",
                ControllerName = "Controller"
            };

            var message = LoggableMessageComposer.LogMessage("Prefix", descriptor, messages);

            message.Should().Be("Prefix Controller:Action - ");
        }
    }

    [Loggable]
    public class StubLoggable
    {
        public string Name { get; set; } = "Ross";
        [SensitiveData]
        public string Password { get; set; } = "sensitive";
        [JsonIgnore]
        public string Ignored { get; set; } = "ignored";

        public StubLoggable() { }

        public StubLoggable(string name)
        {
            Name = name;
        }
    }

    public class StubUnloggable
    {
        public string Name { get; set; } = "Ross";
    }
}
