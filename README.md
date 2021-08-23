# CSRequest
Http client helper methods for C#. </br>
Internally, this library uses HttpRequestMessage, keeping the state of the HttpClient on every request. </br>


## Initialization:
By default, the ```Request``` class will use a ```HttpClient``` singleton to send requests. So, out of the box, you can use it like this:
```cs
using var response = await new Request(@"http://foobar.com").GetAsync();
```
Or, if you prefer, you can inject the HttpClient in the constructor:
```cs
using var client = new HttpClient();
var request = new Request(@"https://foobar.com", client);
``` 
Or you can define the HttpClient statically in you app initialization:
```cs
// in you app initialization...
var client = new HttpClient();
RequestConfiguration.SetHttpClientFactory(_ => client);

// ...or
Request.SetHttpClientFactory(url => 
{
    if(url == @"https://foobar.com")
    {
        // return some HttpClient
    }
    // return some other HttpClient
});

// then, anywhere else
var request = new Request(@"https://foobar.com");

```
### Executing requests
Use any of the following methods:
```cs
// async
await new Request(@"https://foobar.com").GetAsync();
await new Request(@"https://foobar.com").PostAsync();
await new Request(@"https://foobar.com").PutAsync();
await new Request(@"https://foobar.com").PatchAsync();
await new Request(@"https://foobar.com").DeleteAsync();

// sync
new Request().Get();
new Request().Post();
new Request().Put();
new Request().Patch();
new Request().Delete();
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

