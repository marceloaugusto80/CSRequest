using CSRequest.Test.Helpers;
using FluentAssertions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CSRequest.Internal
{
    public class Transforms_Tests
    {
        [Fact]
        public void CookieRequestTransform_tests()
        {
            using var request = new HttpRequestMessage();
            var cookie = new
            {
                key1 = "value1",
                key2 = "value2"
            };

            new CoockieRequestTransform(new ArgList(cookie)).Transform(request);

            request.Headers.GetValues("Cookie").First().Should().Be("key1=value1;key2=value2");
        }

        [Fact]
        public void FormDataRequestTransform_tests()
        {
            using var request = new HttpRequestMessage();
            var formData = new
            {
                key1 = "value1",
                key2 = "value2"
            };

            new FormDataRequestTransform(new ArgList(formData)).Transform(request);

            request.Content.Headers.GetValues("Content-Type").First().Should().Contain("multipart/form-data;");
        }

        [Fact]
        public void FormFileRequestTransform_tests()
        {
            using var request = new HttpRequestMessage();
            using var file = Generators.GenerateStream();

            new FormFileRequestTransform(file, "field", "file").Transform(request);

            request.Content.Headers.GetValues("Content-Type").First().Should().Contain("multipart/form-data;");
        }

        [Fact]
        public async Task JsonContentRequestTransform_tests()
        {
            using var request = new HttpRequestMessage();
            var obj = new { foo = "bar", bar = "foo" };
            
            new JsonContentRequestTransform(obj).Transform(request);

            request.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            (await request.Content.ReadAsStringAsync()).Should().Be(@"{""foo"":""bar"",""bar"":""foo""}");
        }

        [Fact]
        public void HeaderRequestTransform_tests()
        {
            using var request = new HttpRequestMessage();
            var header = new
            {
                foo = "bar",
                bar = "foo"
            };

            new HeaderRequestTransform(new ArgList(header)).Transform(request);

            request.Headers.GetValues("foo").First().Should().Be("bar");
            request.Headers.GetValues("bar").First().Should().Be("foo");
        }

        [Fact]
        public void UrlQueryRequestTransform_tests()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, @"http://foobar.com");
            var query = new
            {
                foo = "bar",
                bar = "foo"
            };

            new UrlQueryRequestTransform(new ArgList(query)).Transform(request);

            request.RequestUri.AbsoluteUri.Should().Be("http://foobar.com/?foo=bar&bar=foo");
        }

        [Fact]
        public void UrlSegmentsRequestTransform_tests()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, @"http://foobar.com");

            new UrlSegmentsRequestTransform("foo", "bar").Transform(request);

            request.RequestUri.AbsoluteUri.Should().Be("http://foobar.com/foo/bar");
        }
    }
}
