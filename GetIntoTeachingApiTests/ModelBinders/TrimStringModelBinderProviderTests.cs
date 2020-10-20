using FluentAssertions;
using GetIntoTeachingApi.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Moq;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.ModelBinders
{
    public class TrimStringModelBinderProviderTests
    {
        private readonly TrimStringModelBinderProvider _provider;

        public TrimStringModelBinderProviderTests()
        {
            _provider = new TrimStringModelBinderProvider();
        }

        [Fact]
        public void GetBinder_ModelTypeString_ReturnsTrimStringModelBinder()
        {
            var mockContext = new Mock<ModelBinderProviderContext>();
            var mockMetadata = new Mock<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(string)));
            mockContext.Setup(m => m.Metadata).Returns(mockMetadata.Object);

            var binder = _provider.GetBinder(mockContext.Object);

            binder.Should().BeOfType(typeof(TrimStringModelBinder));
        }

        [Fact]
        public void GetBinder_ModelTypeNotString_ReturnsNull()
        {
            var mockContext = new Mock<ModelBinderProviderContext>();
            var mockMetadata = new Mock<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(double)));
            mockContext.Setup(m => m.Metadata).Returns(mockMetadata.Object);

            _provider.GetBinder(mockContext.Object).Should().BeNull();
        }

        [Fact]
        public void GetBinder_ContextIsNull_ThrowsArgumentNullException()
        {
            _provider.Invoking(p => p.GetBinder(null)).Should().Throw<ArgumentNullException>();
        }
    }
}
