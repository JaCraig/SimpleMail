namespace SimpleMail.Example
{
    /// <summary>
    /// Example program to send an email using SimpleMail
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static async Task Main(string[] args)
        {
            // Create a new email
            Email email = new Email
            {
                // Email address to send the email from
                From = "system@example.com",
                // Email address to send the email to
                To = "someone@example.com",
                // Subject of the email
                Subject = "Example Subject",
                // Body of the email
                Body = "Example Body",
                // Password of the email account
                Password = "password",
                // Email server to send the email
                Server = "smtp.example.com",
                // Username of the email account
                UserName = "username",
                // Port of the email server (default is 25)
                Port = 587,
                // Use SSL to encrypt the connection
                UseSSL = true,
                // Priority of the email
                Priority = MimeKit.MessagePriority.Urgent
            };
            // Send the email
            await email.SendAsync().ConfigureAwait(false);
        }
    }
}