/*
Copyright 2016 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SimpleMail
{
    /// <summary>
    /// The class used to send an email.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Email"/> class with the specified SMTP client.
    /// </remarks>
    /// <param name="client">
    /// The SMTP client to use for sending emails. If null, a new instance of <see
    /// cref="SmtpClient"/> will be created.
    /// </param>
    public class Email(SmtpClient? client) : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Email"/> class.
        /// </summary>
        public Email()
            : this(null)
        {
        }

        /// <summary>
        /// Gets the attachments.
        /// </summary>
        /// <value>The attachments.</value>
        public List<Attachment> Attachments { get; } = [];

        /// <summary>
        /// Gets or sets the BCC.
        /// </summary>
        /// <value>The BCC.</value>
        public List<MailBox> Bcc { get; set; } = [];

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        public string? Body { get; set; }

        /// <summary>
        /// Gets or sets the cc.
        /// </summary>
        /// <value>The cc.</value>
        public List<MailBox> Cc { get; set; } = [];

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public MailBox? From { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to [ignore server certificate issues].
        /// </summary>
        /// <value><c>true</c> if it should [ignore server certificate issues]; otherwise, <c>false</c>.</value>
        public bool IgnoreServerCertificateIssues { get; set; }

        /// <summary>
        /// Gets or sets the local domain.
        /// </summary>
        /// <value>The local domain.</value>
        public string? LocalDomain { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; } = 25;

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public MessagePriority Priority { get; set; } = MessagePriority.Normal;

        /// <summary>
        /// Gets or sets the reply-to address.
        /// </summary>
        /// <value>The reply-to address.</value>
        public List<MailBox> ReplyTo { get; set; } = [];

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public string? Server { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string? Subject { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        public List<MailBox> To { get; set; } = [];

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use SSL].
        /// </summary>
        /// <value><c>true</c> if [use SSL]; otherwise, <c>false</c>.</value>
        public bool UseSSL { get; set; }

        /// <summary>
        /// Gets or sets the SMTP client.
        /// </summary>
        private SmtpClient? SmtpClient { get; set; } = client ?? new SmtpClient();

        /// <summary>
        /// Determines if the client is externally owned (don't dispose if it is)
        /// </summary>
        private readonly bool _ClientExternallyOwned = client is not null;

        /// <summary>
        /// Releases the resources used by the <see cref="Email"/> class.
        /// </summary>
        /// <remarks>If the SMTP client is externally owned, it will not be disposed.</remarks>
        public void Dispose()
        {
            if (_ClientExternallyOwned)
                return;

            SmtpClient?.Dispose();
            SmtpClient = null;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sends this email
        /// </summary>
        public void Send()
        {
            if (SmtpClient is null)
                throw new InvalidOperationException("The SMTP client is null.");
            if (From is null)
                throw new InvalidOperationException("No sender specified.");
            MimeMessage Message = SetupMessage();
            if (Message.To.Count == 0 && Message.Cc.Count == 0 && Message.Bcc.Count == 0)
                throw new InvalidOperationException("No valid recipients specified.");
            SetClientSettings(SmtpClient);
            Connect();
            Authenticate();
            _ = SmtpClient.Send(Message);
            Disconnect();
            CleanupClient();
        }

        /// <summary>
        /// Sends the email asynchronously.
        /// </summary>
        /// <returns>The resulting Task</returns>
        public async Task SendAsync()
        {
            if (SmtpClient is null)
                throw new InvalidOperationException("The SMTP client is null.");
            if (From is null)
                throw new InvalidOperationException("No sender specified.");
            MimeMessage Message = SetupMessage();
            if (Message.To.Count == 0 && Message.Cc.Count == 0 && Message.Bcc.Count == 0)
                throw new InvalidOperationException("No valid recipients specified.");
            SetClientSettings(SmtpClient);
            await ConnectAsync().ConfigureAwait(false);
            await AuthenticateAsync().ConfigureAwait(false);
            _ = await SmtpClient.SendAsync(Message).ConfigureAwait(false);
            await DisconnectAsync().ConfigureAwait(false);
            CleanupClient();
        }

        /// <summary>
        /// Adds a list of mailboxes to the specified InternetAddressList.
        /// </summary>
        /// <param name="list">The InternetAddressList to add the mailboxes to.</param>
        /// <param name="addressesToAdd">The list of MailBox objects to add to the InternetAddressList.</param>
        private static void AddMailboxesToList(InternetAddressList list, List<MailBox> addressesToAdd)
        {
            if (list is null || addressesToAdd is null)
                return;
            foreach (MailBox TempAddress in addressesToAdd)
            {
                if (TempAddress.MailboxAddress is null)
                    continue;
                list.Add(TempAddress.MailboxAddress);
            }
        }

        /// <summary>
        /// Authenticates the client.
        /// </summary>
        private void Authenticate()
        {
            if (string.IsNullOrEmpty(UserName) || SmtpClient?.IsAuthenticated != false)
                return;
            SmtpClient.Authenticate(UserName, Password);
        }

        /// <summary>
        /// Authenticates the client asynchronously.
        /// </summary>
        /// <returns>The resulting Task</returns>
        private Task AuthenticateAsync()
        {
            return string.IsNullOrEmpty(UserName) || SmtpClient?.IsAuthenticated != false
                ? Task.CompletedTask
                : SmtpClient.AuthenticateAsync(UserName, Password);
        }

        /// <summary>
        /// Cleans up the client. Clears out the To, Cc, Bcc, Attachments, and ReplyTo lists.
        /// </summary>
        private void CleanupClient()
        {
            if (_ClientExternallyOwned)
                return;
            To.Clear();
            Cc.Clear();
            Bcc.Clear();
            Attachments.Clear();
            ReplyTo.Clear();
            Subject = "";
            Body = "";
        }

        /// <summary>
        /// Connects the client.
        /// </summary>
        private void Connect()
        {
            if (SmtpClient?.IsConnected != false)
                return;
            SmtpClient.Connect(Server, Port, UseSSL);
        }

        /// <summary>
        /// Connects the client asynchronously.
        /// </summary>
        /// <returns>The resulting Task</returns>
        private Task ConnectAsync() => SmtpClient?.IsConnected != false ? Task.CompletedTask : SmtpClient.ConnectAsync(Server, Port, UseSSL);

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        private void Disconnect()
        {
            if (SmtpClient?.IsConnected != true || _ClientExternallyOwned)
                return;
            SmtpClient.Disconnect(true);
        }

        /// <summary>
        /// Disconnects the client asynchronously.
        /// </summary>
        /// <returns>The resulting Task</returns>
        private Task DisconnectAsync() => SmtpClient?.IsConnected != true || _ClientExternallyOwned ? Task.CompletedTask : SmtpClient.DisconnectAsync(true);

        /// <summary>
        /// Sets the client settings based on the current SMTP client configuration.
        /// </summary>
        /// <param name="client">The SMTP client to configure.</param>
        private void SetClientSettings(SmtpClient? client)
        {
            if (client is null || _ClientExternallyOwned)
                return;
            if (IgnoreServerCertificateIssues)
            {
                client.ServerCertificateValidationCallback = (object _, X509Certificate? __, X509Chain? ___, SslPolicyErrors ____) => true;
            }
            client.LocalDomain = LocalDomain;
        }

        /// <summary>
        /// Setups the message.
        /// </summary>
        /// <returns></returns>
        private MimeMessage SetupMessage()
        {
            var InternalMessage = new MimeMessage()
            {
                Subject = Subject,
                Priority = Priority,
                Importance = MessageImportance.Normal
            };
            if (From is null)
            {
                throw new ArgumentNullException(nameof(From));
            }
            InternalMessage.From.Add(From.MailboxAddress);
            Email.AddMailboxesToList(InternalMessage.To, To);
            Email.AddMailboxesToList(InternalMessage.Cc, Cc);
            Email.AddMailboxesToList(InternalMessage.Bcc, Bcc);
            Email.AddMailboxesToList(InternalMessage.ReplyTo, ReplyTo);

            MimeEntity Content = new TextPart(TextFormat.Html) { Text = Body };
            if (Attachments.Count > 0)
            {
                var TempContent = new Multipart("mixed")
                {
                    Content
                };
                Content = TempContent;
                foreach (Attachment Attachment in Attachments)
                {
                    TempContent.Add(Attachment.Convert());
                }
            }
            InternalMessage.Body = Content;
            return InternalMessage;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        ~Email()
        {
            Dispose();
        }
    }
}