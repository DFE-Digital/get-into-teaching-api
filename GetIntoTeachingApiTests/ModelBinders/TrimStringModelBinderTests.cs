using FluentAssertions;
using GetIntoTeachingApi.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GetIntoTeachingApiTests.ModelBinders
{
    public class TrimStringModelBinderTests
    {
        private readonly TrimStringModelBinder _binder;
        private readonly Mock<ModelBindingContext> _mockContext;
        private readonly Mock<IValueProvider> _mockValueProvider;

        public TrimStringModelBinderTests()
        {
            _binder = new TrimStringModelBinder();
            _mockContext = new Mock<ModelBindingContext>();
            _mockValueProvider = new Mock<IValueProvider>();

            _mockContext.Setup(m => m.ModelName).Returns("key");
            _mockContext.Setup(m => m.ValueProvider).Returns(_mockValueProvider.Object);
        }

        [Fact]
        public void BindModelAsync_WithNullContext_ThrowsArgumentNullException()
        {
            _binder.Awaiting(b => b.BindModelAsync(null)).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BindModelAsync_WhenValueProviderResultIsNone_CompletesTask()
        {
            _mockValueProvider.Setup(m => m.GetValue("key")).Returns(ValueProviderResult.None);

            var task = _binder.BindModelAsync(_mockContext.Object);

            task.Wait();
            task.Should().Be(Task.CompletedTask);
        }

        [Fact]
        public void BindModelAsync_WhenValueIsString_TrimsStringAndCompletesTask()
        {
            var modelState = new ModelStateDictionary();
            var valueProviderResult = new ValueProviderResult("  value  ");
            _mockValueProvider.Setup(m => m.GetValue("key")).Returns(valueProviderResult);
            _mockContext.Setup(m => m.ModelState).Returns(modelState);

            var task = _binder.BindModelAsync(_mockContext.Object);

            task.Wait();
            task.Should().Be(Task.CompletedTask);
            _mockContext.VerifySet(m => m.Result = ModelBindingResult.Success("value"));
        }
    }
}
