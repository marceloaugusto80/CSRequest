# CSRequest
Http client helper methods for .Net </br>
Internally, this library uses HttpRequestMessage, keeping the HttpClient's state on every request. </br>

## Installation
```shell
> dotnet add package CSRequest
```
```shell
PM> Install-Package CSRequest
```

## Initialization:
By default, the ```Request``` class will use an internal lazy loaded ```HttpClient``` singleton to send requests. So, out of the box, you can make requests like this:
```cs
using CSRequest;

//...

using var response = await new Request("https://foobar.com").GetAsync();
```
Or, if you prefer, you can inject the HttpClient in the constructor:
```cs
using var client = new HttpClient();
var request = new Request("https://foobar.com", client);
``` 
Also, you can define the HttpClient statically in you app initialization:
```cs
// in you app initialization...
var client = new HttpClient();
RequestConfiguration.SetHttpClientFactory(_ => client);

// ...or
Request.SetHttpClientFactory(url => 
{
    if(url == "https://foobar.com")
    {
        // return some HttpClient
    }
    // return some other HttpClient
});

// then, anywhere else
var request = new Request("https://foobar.com");

```
Although `Request.SetHttpClientFactory` is thread-safe, for performance reasons, is recomended to call it only once in you app initialization.
### Fluent interface
Concat all request configurations.
Example:
```cs
using HttpResponseMessage response = await new Request("https://foobar.com")
    .WithSegments("foo", "bar") // url becomes https://foobar.com/foo/bar
    .WithQuery(new { q1 = 1, q2 = 2 }) // url becomes https://foobar.com/foo/bar?q1=1&q2=2
    .WithHeader(new { Content_Type = "application/json" }) // adds header Content-Type: application/json
    .WithBearerTokenAuthentication("some-token") // sets header Authorization: Bearer some-token
    .WithBasicAuthentication("myuser", "mypass") // sets header Authorization: Basic bXl1c2VyOm15cGFzcw==
    .WithFormData(new { name = "Jonh", age = 30 }) // sets multipart form data
    .AddFormFile(someStream) // sends form file
    .PostAsync() // executes a post request

// do something with the response...
```
In some configuration methods, for the sake of convenience, the parameter can be either an `object`, a `Dictionary<string, object>` or variadic `(string, string)` tuples.
In the example below, all requests will produce the same result:
```cs
new Request().WithHeader(
    ("header1", "value1"),
    ("header2", "2"),
    //...
);

new Request().WithHeader(new Dictionary<string, object>(){
    { "header1", "value1" },
    { "header2", 2},
    //...
});

new Request.WithHeader(new {
    header1 = "value1",
    header2 = 2,
    //...
});
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


### Response extensions
There are some HttpResponseMessage extension methods defined out of the box. They are both implemented synchronously and asynchronously.
Examples:
```cs
dynamic myObject = await new Request().Get().ReadJsonAsync();

var fooBar = await new Request().Get().ReadJsonAsync<FooBar>();

string myString = await new Request().Get().ReadStringAsync();

using var stream = await new Request().Get().ReadStreamAsync();
```
