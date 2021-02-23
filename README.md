# Rebus.GooglePubSub

[![install from nuget](https://img.shields.io/nuget/v/AMain.Rebus.GooglePubSub.svg?style=flat-square)](https://www.nuget.org/packages/AMain.Rebus.GooglePubSub)


Provides a Google Pub/Sub based transport implementation for [Rebus](https://github.com/rebus-org/Rebus).

It's just

**TODO!! FIX THIS!!**

```csharp
var storageClient = StorageClient.Create();

Configure.With(...)
	.Transport(t => t.UseGooglePubSub(storageClient, "my-project-id", "your_queue"))
	.(...)
	.Start();
```

or

```csharp
var storageClient = StorageClient.Create();

var bus Configure.With(...)
	.Transport(t => t.UseGooglePubSubAsOneWayClient(storageClient))
	.(...)
	.Start();
```

and off you go! :rocket:

![](https://raw.githubusercontent.com/rebus-org/Rebus/master/artwork/little_rebusbus2_copy-200x200.png)

---
