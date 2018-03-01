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
using System.Threading.Tasks;

namespace SimpleMail
{
    /// <summary>
    /// The class used to send an email.
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Email"/> class.
        /// </summary>
        public Email()
        {
            Priority = MessagePriority.Normal;
            Port = 25;
        }

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
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public MessagePriority Priority { get; set; }

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

        private MimeMessage SetupMessage()
        {
            var InternalMessage = new MimeMessage();
            InternalMessage.From.Add(new MailboxAddress(From));
            var ToSplit = To?.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            foreach (var Item in ToSplit)
            {
                InternalMessage.To.Add(new MailboxAddress(Item));
            }
            var BccSplit = Bcc?.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            foreach (var Item in BccSplit)
            {
                InternalMessage.Bcc.Add(new MailboxAddress(Item));
            }
            var CcSplit = Cc?.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            foreach (var Item in CcSplit)
            {
                InternalMessage.Cc.Add(new MailboxAddress(Item));
            }
            InternalMessage.Subject = Subject;
            InternalMessage.Body = new TextPart(TextFormat.Html) { Text = Body };
            InternalMessage.Priority = Priority;
            InternalMessage.Importance = MessageImportance.Normal;
            return InternalMessage;
        }
    }
}