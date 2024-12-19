using Xunit;

namespace SimpleMail.Tests
{
    public class MailBoxTests
    {
        public MailBoxTests()
        {
            _emailAddress = "TestValue840840065";
            _displayName = "TestValue758283532";
            _testClass = new MailBox(_displayName, _emailAddress);
        }

        private readonly string _displayName;
        private readonly string _emailAddress;
        private readonly MailBox _testClass;

        [Fact]
        public void CanCallToString()
        {
            // Act
            var result = _testClass.ToString();

            // Assert
            Assert.NotNull(result);
            _ = Assert.IsType<string>(result);
        }

        [Fact]
        public void CanConstruct()
        {
            // Act
            var instance = new MailBox(_emailAddress);

            // Assert
            Assert.NotNull(instance);

            // Act
            instance = new MailBox(_displayName, _emailAddress);

            // Assert
            Assert.NotNull(instance);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CanConstructWithInvalidDisplayName(string value) => _ = new MailBox(value, _emailAddress);

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CanConstructWithInvalidEmailAddress(string value)
        {
            _ = new MailBox(value);
            _ = new MailBox(_displayName, value);
        }

        [Fact]
        public void CanSetAndGetDisplayName()
        {
            // Arrange
            var testValue = "TestValue1149489993";

            // Act
            _testClass.DisplayName = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.DisplayName);
        }

        [Fact]
        public void CanSetAndGetEmailAddress()
        {
            // Arrange
            var testValue = "TestValue1213480930";

            // Act
            _testClass.EmailAddress = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.EmailAddress);
        }

        [Fact]
        public void DisplayNameIsInitializedCorrectly() => Assert.Equal(_displayName, _testClass.DisplayName);

        [Fact]
        public void EmailAddressIsInitializedCorrectly()
        {
            var instance = new MailBox(_emailAddress);
            Assert.Equal(_emailAddress, instance.EmailAddress);
            instance = new MailBox(_displayName, _emailAddress);
            Assert.Equal(_emailAddress, instance.EmailAddress);
        }
    }
}