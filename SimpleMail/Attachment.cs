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

using MimeKit;
using MimeKit.Utils;
using System.IO;

namespace SimpleMail
{
    /// <summary>
    /// Attachment for the message
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Attachment"/> class.
    /// </remarks>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="mimeType">Type of the MIME.</param>
    /// <param name="content">The content.</param>
    public class Attachment(string? fileName, string? mimeType, Stream? content)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Attachment"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="content">The content.</param>
        public Attachment(string fileName, string mimeType, byte[] content)
            : this(fileName, mimeType, content is null ? null : new MemoryStream(content))
        {
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public Stream? Content { get; } = content;

        /// <summary>
        /// Gets or sets the content identifier (used for embedding images, etc.).
        /// </summary>
        /// <value>The content identifier.</value>
        public string ContentId { get; set; } = MimeUtils.GenerateMessageId();

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; } = fileName ?? "Attachment";

        /// <summary>
        /// Gets the type of the MIME.
        /// </summary>
        /// <value>The type of the MIME.</value>
        public string MimeType { get; } = mimeType ?? "application/octet-stream";

        /// <summary>
        /// Converts this instance.
        /// </summary>
        /// <returns></returns>
        internal MimePart Convert()
        {
            return new MimePart(MimeType)
            {
                Content = new MimeContent(Content),
                ContentId = ContentId,
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = FileName,
            };
        }
    }
}