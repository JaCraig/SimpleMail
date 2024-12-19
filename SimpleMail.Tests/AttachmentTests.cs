using Xunit;

namespace SimpleMail.Tests
{
    public class AttachmentTests
    {
        public AttachmentTests()
        {
            _fileName = "TestValue235574086";
            _mimeType = "TestValue1196560625";
            _content = new byte[] { 11, 36, 30, 120 };
            _testClass = new Attachment(_fileName, _mimeType, _content);
        }

        private readonly byte[] _content;
        private readonly string _fileName;
        private readonly string _mimeType;
        private readonly Attachment _testClass;

        [Fact]
        public void CanConstruct()
        {
            // Act
            var instance = new Attachment(_fileName, _mimeType, _content);

            // Assert
            Assert.NotNull(instance);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CanConstructWithInvalidFileName(string value) => _ = new Attachment(value, _mimeType, _content);

        [Fact]
        public void CanConstructWithNullContent() => _ = new Attachment(_fileName, _mimeType, default(byte[]));

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CannoonstructWithInvalidMimeType(string value) => _ = new Attachment(_fileName, value, _content);

        [Fact]
        public void CanSetAndGetContentId()
        {
            // Arrange
            var testValue = "TestValue818042934";

            // Act
            _testClass.ContentId = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.ContentId);
        }

        [Fact]
        public void ContentIsInitializedCorrectly() => Assert.NotNull(_testClass.Content);

        [Fact]
        public void FileNameIsInitializedCorrectly() => Assert.Equal(_fileName, _testClass.FileName);

        [Fact]
        public void MimeTypeIsInitializedCorrectly() => Assert.Equal(_mimeType, _testClass.MimeType);
    }
}