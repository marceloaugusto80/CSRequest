using System.Reflection;

namespace CSRequest.Internal
{
    public class PropertyInfoCache_Test
    {
        [Fact]
        public void Dynamic_objects_test_1()
        {
            var objs = new dynamic[]
            {
                new { A = 1     , B = 2    },
                new { A = 2     , B = "B"  },
                new { A = "A"   , B = true },
            };
            var cache = new PropertyInfoCache();

            // no duplicate cache entries

            foreach (var obj in objs)
            {
                cache.GetProperties(obj);
            }
            cache.Size.Should().Be(1);

            // result is correct

            var props = cache.GetProperties(objs.First()) as PropertyInfo[];
            props.Select(p => p.Name).Should().BeEquivalentTo(new string[] { "A", "B" });


        }

        [Fact]
        public void Dynamic_objects_test_2()
        {
            var objs = new dynamic[]
            {
                new { A = 1, B = 2 },
                new { B = 1, C = 2 },
                new { C = 1, D = 2 },

            };
            var cache = new PropertyInfoCache();

            // cache identifies different dynamic objects

            foreach (var obj in objs)
            {
                cache.GetProperties(obj);
            }
            cache.Size.Should().Be(objs.Length);

            // result is correct

            (cache.GetProperties(objs[0]) as PropertyInfo[]).Select(x => x.Name).Should().BeEquivalentTo(new string[] { "A", "B" });
            (cache.GetProperties(objs[1]) as PropertyInfo[]).Select(x => x.Name).Should().BeEquivalentTo(new string[] { "B", "C" });
            (cache.GetProperties(objs[2]) as PropertyInfo[]).Select(x => x.Name).Should().BeEquivalentTo(new string[] { "C", "D" });

        }



    }
}
