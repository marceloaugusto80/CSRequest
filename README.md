# CSRequest
Http client helper methods for C#. </br>
Internally, this library uses HttpRequestMessage, keeping the state of the HttpClient on every request. </br>
Ideal for testing web api's. </br>

## Initialization:
You can inject the HttpClient in the constructor:
```cs
using var client = new HttpClient();
var request = new Request(client, @"https://foobar.com");
``` 
Or you can define the HttpClient statically in you app initialization:
```cs
// in you app initialization

var client = new HttpClient();
Request.SetHttpClientFactory(url => client);

//or

Request.SetHttpClientFactory(url => 
{
    if(url == @"https://foobar.com")
    {
        // return some HttpClient
    }
    // return some other HttpClient
});

// then, anywhere else
var request = new Request();

```
### Executing requests
Use any of the following methods:
```cs
// async
using var response = await new Request().GetAsync();
using var response = await new Request().PostAsync();
using var response = await new Request().PutAsync();
using var response = await new Request().PatchAsync();
using var response = await new Request().DeleteAsync();

// sync
using var response = new Request().Get();
using var response = new Request().Post();
using var response = new Request().Put();
using var response = new Request().Patch();
using var response = new Request().Delete();
```


### Fluent interface
Concat all request configurations:
```cs
using HttpResponseMessage response = await new Request()
    .WithSegments("foo", "bar")
    .WithQuery(new { q1 = 1, q2 = 2 })
    .WithHeader(new { Content_Type = "application/json" })
    .AddOABearerToken("some-token")
    .WithFormData(new { name = "Jonh", age = 30 })
    .AddFormFile(someStream, "some-file-name.jpg")
    .PostAsync()

// do something with the response...
```
### Response extensions
There are some HttpResponseMessage extension methods defined out of the box. They are both implemented in synchronously and asynchronously.
Examples:
```cs
var fooBar = await new Request().Get().ReadJsonAsync<FooBar>();
string myString = await new Request().Get().ReadStringAsync();
using var stream = await new Request().Get().ReadStreamAsync();
```

