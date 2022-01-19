using System.IO;

namespace CSRequest.Test.Helpers
{
    public static class Generators
    {
        public static Stream GenerateStream()
        {
            return new MemoryStream(new byte[] { 0x1, 0x2, 0x3 });
        }

        public static IEnumerable<Stream> GenerateStream(int count)
        {
            for (int i = 0; i < count; i++)
                yield return GenerateStream();
        }
    }
}
