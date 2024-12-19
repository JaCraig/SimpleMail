using MailKit.Net.Smtp;
using MimeKit;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SimpleMail.Tests
{
    public class EmailTests
    {
        public EmailTests()
        {
            // Create substitute for SMTP client using NSubstitute
            _smtpClient = Substitute.For<SmtpClient>();
            _testClass = new Email(_smtpClient);
        }

        private readonly SmtpClient _smtpClient;
        private readonly Email _testClass;

        [Fact]
        public void CanConstruct()
        {
            // Act
            var instance = new Email();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void CanConstructWithSmtpClient()
        {
            // Act
            var instance = new Email(_smtpClient);

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void CanSendEmail()
        {
            // Arrange
            _testClass.From = new MailBox("from@example.com");
            _testClass.To.Add(new MailBox("to@example.com"));
            _testClass.Subject = "Test Subject";
            _testClass.Body = "Test Body";
            _testClass.Server = "smtp.example.com";
            _testClass.UserName = "username";
            _testClass.Password = "password";
            _testClass.Port = 587;
            _testClass.UseSSL = true;

            // Act
            _testClass.Send();
        }

        [Fact]
        public async Task CanSendEmailAsync()
        {
            // Arrange
            _testClass.From = new MailBox("from@example.com");
            _testClass.To.Add(new MailBox("to@example.com"));
            _testClass.Subject = "Test Subject";
            _testClass.Body = "Test Body";
            _testClass.Server = "smtp.example.com";
            _testClass.UserName = "username";
            _testClass.Password = "password";
            _testClass.Port = 587;
            _testClass.UseSSL = true;

            // Act
            await _testClass.SendAsync();
        }

        [Fact]
        public void CanSetAndGetAttachments()
        {
            // Arrange
            var testValue = new List<Attachment> { new("file.txt", "text/plain", new byte[] { 1, 2, 3 }) };

            // Act
            _testClass.Attachments.AddRange(testValue);

            // Assert
            Assert.Equal(testValue, _testClass.Attachments);
        }

        [Fact]
        public void CanSetAndGetBcc()
        {
            // Arrange
            var testValue = new List<MailBox> { new("test@example.com") };

            // Act
            _testClass.Bcc = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.Bcc);
        }

        [Fact]
        public void CanSetAndGetBody()
        {
            // Arrange
            var testValue = "Test Body";

            // Act
            _testClass.Body = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.Body);
        }

        [Fact]
        public void CanSetAndGetCc()
        {
            // Arrange
            var testValue = new List<MailBox> { new("test@example.com") };

            // Act
            _testClass.Cc = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.Cc);
        }

        [Fact]
        public void CanSetAndGetFrom()
        {
            // Arrange
            var testValue = new MailBox("test@example.com");

            // Act
            _testClass.From = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.From);
        }

        [Fact]
        public void CanSetAndGetIgnoreServerCertificateIssues()
        {
            // Arrange
            var testValue = true;

            // Act
            _testClass.IgnoreServerCertificateIssues = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.IgnoreServerCertificateIssues);
        }

        [Fact]
        public void CanSetAndGetLocalDomain()
        {
            // Arrange
            var testValue = "localhost";

            // Act
            _testClass.LocalDomain = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.LocalDomain);
        }

        [Fact]
        public void CanSetAndGetPassword()
        {
            // Arrange
            var testValue = "password";

            // Act
            _testClass.Password = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.Password);
        }

        [Fact]
        public void CanSetAndGetPort()
        {
            // Arrange
            var testValue = 587;

            // Act
            _testClass.Port = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.Port);
        }

        [Fact]
        public void CanSetAndGetPriority()
        {
            // Arrange
            MessagePriority testValue = MessagePriority.Urgent;

            // Act
            _testClass.Priority = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.Priority);
        }

        [Fact]
        public void CanSetAndGetReplyTo()
        {
            // Arrange
            var testValue = new List<MailBox> { new("test@example.com") };

            // Act
            _testClass.ReplyTo = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.ReplyTo);
        }

        [Fact]
        public void CanSetAndGetServer()
        {
            // Arrange
            var testValue = "smtp.example.com";

            // Act
            _testClass.Server = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.Server);
        }

        [Fact]
        public void CanSetAndGetSubject()
        {
            // Arrange
            var testValue = "Test Subject";

            // Act
            _testClass.Subject = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.Subject);
        }

        [Fact]
        public void CanSetAndGetTo()
        {
            // Arrange
            var testValue = new List<MailBox> { new("test@example.com") };

            // Act
            _testClass.To = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.To);
        }

        [Fact]
        public void CanSetAndGetUserName()
        {
            // Arrange
            var testValue = "username";

            // Act
            _testClass.UserName = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.UserName);
        }

        [Fact]
        public void CanSetAndGetUseSSL()
        {
            // Arrange
            var testValue = true;

            // Act
            _testClass.UseSSL = testValue;

            // Assert
            Assert.Equal(testValue, _testClass.UseSSL);
        }
    }
}