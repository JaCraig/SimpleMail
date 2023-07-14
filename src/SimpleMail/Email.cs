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
    public class Email
    {
        /// <summary>
        /// Gets the attachments.
        /// </summary>
        /// <value>The attachments.</value>
        public List<Attachment> Attachments { get; } = new List<Attachment>();

        /// <summary>
        /// Gets or sets the BCC.
        /// </summary>
        /// <value>The BCC.</value>
        public string Bcc { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the cc.
        /// </summary>
        /// <value>The cc.</value>
        public string Cc { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to [ignore server certificate issues].
        /// </summary>
        /// <value>
        ///   <c>true</c> if it should [ignore server certificate issues]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreServerCertificateIssues { get; set; }

        /// <summary>
        /// Gets or sets the local domain.
        /// </summary>
        /// <value>The local domain.</value>
        public string LocalDomain { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

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
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use SSL].
        /// </summary>
        /// <value><c>true</c> if [use SSL]; otherwise, <c>false</c>.</value>
        public bool UseSSL { get; set; }

        /// <summary>
        /// Sends this email
        /// </summary>
        public void Send()
        {
            var Message = SetupMessage();
            using (var Client = new SmtpClient())
            {
                if (IgnoreServerCertificateIssues)
                {
                    Client.ServerCertificateValidationCallback = (object _, X509Certificate __, X509Chain ___, SslPolicyErrors ____) => true;
                }
                Client.LocalDomain = LocalDomain;
                Client.Connect(Server, Port, UseSSL);
                if (!string.IsNullOrEmpty(UserName))
                {
                    Client.Authenticate(UserName, Password);
                }
                Client.Send(Message);
                Client.Disconnect(true);
            }
        }

        /// <summary>
        /// Sends the email asynchronously.
        /// </summary>
        /// <returns>The resulting Task</returns>
        public async Task SendAsync()
        {
            var Message = SetupMessage();
            using (var Client = new SmtpClient())
            {
                if (IgnoreServerCertificateIssues)
                {
                    Client.ServerCertificateValidationCallback = (object _, X509Certificate __, X509Chain ___, SslPolicyErrors ____) => true;
                }
                Client.LocalDomain = LocalDomain;
                await Client.ConnectAsync(Server, Port, UseSSL).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(UserName))
                {
                    await Client.AuthenticateAsync(UserName, Password).ConfigureAwait(false);
                }
                await Client.SendAsync(Message).ConfigureAwait(false);
                await Client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Splits the recipients.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="mailboxes">The mailboxes.</param>
        private static void SplitRecipients(InternetAddressList list, string mailboxes)
        {
            if (string.IsNullOrEmpty(mailboxes) || list is null)
                return;
            var SplitMailboxes = mailboxes?.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            for (int i = 0; i < SplitMailboxes.Length; i++)
            {
                list.Add(new MailboxAddress("", SplitMailboxes[i]));
            }
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
            InternalMessage.From.Add(new MailboxAddress(string.Empty, From));
            SplitRecipients(InternalMessage.To, To);
            SplitRecipients(InternalMessage.Bcc, Bcc);
            SplitRecipients(InternalMessage.Cc, Cc);
            MimeEntity Content = new TextPart(TextFormat.Html) { Text = Body };
            if (Attachments.Count > 0)
            {
                var TempContent = new Multipart("mixed")
                {
                    Content
                };
                Content = TempContent;
                foreach (var Attachment in Attachments)
                {
                    TempContent.Add(Attachment.Convert());
                }
            }
            InternalMessage.Body = Content;
            return InternalMessage;
        }
    }
}