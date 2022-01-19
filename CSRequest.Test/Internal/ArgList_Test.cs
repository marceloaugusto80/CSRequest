namespace CSRequest.Internal
{
    public class ArgList_Test
    {
        [Fact]
        public void ToDictionary_tests()
        {
            var args = new ArgList(("foo", "bar"), ("bar", "foo"));
            args.ToDictionary().Should().Contain("foo", "bar").And.Contain("bar", "foo");

            args = new ArgList(new Dictionary<string, object>()
            {
                {"foo", "bar" },
                {"number", 1 }
            });
            args.ToDictionary().Should().Contain("foo", "bar").And.Contain("number", "1");

            args = new ArgList(new
            {
                foo = "bar",
                number = 1
            });
            args.ToDictionary().Should().Contain("foo", "bar").And.Contain("number", "1");

        }

    }
}
