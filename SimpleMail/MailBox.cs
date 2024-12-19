using MimeKit;

namespace SimpleMail
{
    /// <summary>
    /// Represents a mailbox with an email address and an optional display name.
    /// </summary>
    public class MailBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MailBox"/> class with the specified email address.
        /// </summary>
        /// <param name="emailAddress">The email address of the mailbox.</param>
        public MailBox(string emailAddress)
        {
            emailAddress ??= emailAddress?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(emailAddress))
                return;
            // Check to see if we can parse display name from the email address (e.g. "John Doe <jdoe@example.com>")
            MailboxAddress = MailboxAddress.Parse(emailAddress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailBox"/> class with the specified display name and email address.
        /// </summary>
        /// <param name="displayName">The display name of the mailbox.</param>
        /// <param name="emailAddress">The email address of the mailbox.</param>
        public MailBox(string displayName, string emailAddress)
        {
            displayName ??= displayName?.Trim() ?? "";
            emailAddress ??= emailAddress?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(emailAddress))
                return;
            MailboxAddress = new MailboxAddress(displayName, emailAddress);
        }

        /// <summary>
        /// Gets or sets the display name of the mailbox.
        /// </summary>
        public string DisplayName
        {
            get => MailboxAddress?.Name ?? "";
            set
            {
                if (MailboxAddress == null)
                {
                    MailboxAddress = new MailboxAddress(value, EmailAddress);
                }
                else
                {
                    MailboxAddress.Name = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the email address of the mailbox.
        /// </summary>
        public string EmailAddress
        {
            get => MailboxAddress?.Address ?? "";
            set
            {
                if (MailboxAddress == null)
                {
                    MailboxAddress = new MailboxAddress(DisplayName, value);
                }
                else
                {
                    MailboxAddress.Address = value;
                }
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="MailboxAddress"/> object.
        /// </summary>
        internal MailboxAddress? MailboxAddress { get; set; }

        /// <summary>
        /// Implicitly converts a string to a <see cref="MailBox"/> object.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        public static implicit operator MailBox(string value) => new(value);

        /// <summary>
        /// Returns a string representation of the mailbox.
        /// </summary>
        /// <returns>A string representation of the mailbox.</returns>
        public override string ToString() => DisplayName.Length > 0 ? $"{DisplayName} <{EmailAddress}>" : EmailAddress;
    }
}