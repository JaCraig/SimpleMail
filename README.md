# SimpleMail

[![.NET Publish](https://github.com/JaCraig/SimpleMail/actions/workflows/dotnet-publish.yml/badge.svg)](https://github.com/JaCraig/SimpleMail/actions/workflows/dotnet-publish.yml)

SimpleMail is a C# library designed to simplify sending emails. It provides a convenient and straightforward interface for sending email messages.

## Features

- Easy-to-use API for sending emails and attachments
- Compatible with .NET 8 and above

## Installation

SimpleMail can be easily installed via NuGet:

```
dotnet add package SimpleMail
```

Alternatively, you can search for "SimpleMail" in the NuGet Package Manager UI and install it from there.

## Getting Started

To use SimpleMail in your project, follow these simple steps:

1. Add a reference to the SimpleMail namespace in your code file:

```csharp
    using SimpleMail;
```

2. Create an instance of the `EmailMessage` class:

```csharp
    // Create a new email
    Email email = new Email
    {
        // Email address to send the email from
        From = "system@example.com",
        // Email address to send the email to
        To = ["someone@example.com"],
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
        Priority = MimeKit.MessagePriority.Urgent,
        // Reply-to address
        ReplyTo = ["replyto@example.com"]
    };
```

Or if you want to supply your own SmtpClient, you can do so. This is useful if you want to use a custom SmtpClient with specific settings:

```csharp
    // Create a new email
    Email email = new Email(new SmtpClient())
    {
        // Email address to send the email from
        From = "system@example.com",
        ...
    };
```

Note that the SmtpClient instance is not disposed of by SimpleMail, so you will need to dispose of it yourself when you are done with it.
    

3. Send the email:

```csharp
await email.SendAsync().ConfigureAwait(false);
```

That's it! You have successfully sent an email using SimpleMail.

## Compatibility

SimpleMail is compatible with .NET 8 and above.

## Contributing

We welcome contributions to SimpleMail! If you encounter any issues, have suggestions for improvements, or would like to contribute new features, please feel free to submit a pull request.

## License

SimpleMail is released under the [Apache 2 License](https://github.com/JaCraig/SimpleMail/blob/master/LICENSE).