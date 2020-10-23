# CSRequest
Http client helper methods for C#. </br>
Internally, this library uses HttpRequestMessage, keeping the state of the HttpClient on every request. </br>
Ideal for testing web api's. </br>

## Usage:
You must define the HttpClient statically...
```
var client = new HttpClient()
{
    BaseAddress = new Uri("http://postman-echo.com/")
};

Request.DefaultClientFactory = () => client;

```

... and/or, define the HttpClient in the request.
```
// overrides the default client

using HttpResponseMessage response = await new Request()
    .InjectClient(() => new HttpClient() { BaseAddress = new Uri(@"https://foobar.com/") })
    .WithQuery(new { someKey = "somevalue" })
    .GetAsync();
```
### Fluent interface
Concat all request configurations:
```
using HttpResponseMessage response = new Request()
    .WithSegments("foo", "bar")
    .WithHeader(new { h1 = "value1", h2 = "value2" })
    .WithQuery(new { q1 = 1, q2 = 2 })
    //..
    .GetAsync()

// do something with the response...
```
### Response extensions
There are some HttpResponseMessage extension methods defined.
An example:
```
class FooBar 
{
    public string Foo { get; set; }
    public int Bar { get; set; }
}

//...

var fooBar = await new Request()
    .WithSegments("a", "b")
    .Get()
    .ReadJsonAsync<FooBar>();
```