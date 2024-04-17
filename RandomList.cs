// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Security.Cryptography;

/*
   Sometimes your best isn't good enough!
   
   IA ChatBot suggests using 'RandomNumberGenerator.GetInt32(<maxval>)' 
   over 'Random.Next(<maxval>)'. 
   However, RandomNumberGenerator is slower. 
   This slowdown emerges when random numbers are queried multiple times 
   (e.g. Fisher-Yates sorting).
   
   IA ChatBot suggests using Fisher-Yates sorting over 'OrderBy(_ => Guid.NewGuid())' 
   on large collections. 
   However, Fisher-Yates + RandomNumberGenerator can be very slow even on small collections.
   
 */
namespace RandomList
{
    internal class Program
    {
        public class RandomListNewGuid : List<string>
        {

            private List<int>? _shuffledIndices = null;
            private int _lastIndex = 0;
            private readonly Random _random = new();

            public new void Add(string item)
            {
                base.Add(item);
                _shuffledIndices = null;
                _lastIndex = 0;
            }

            public new void Clear()
            {
                base.Clear();
                _shuffledIndices = null;
                _lastIndex = 0;
            }

            public string GetRandomString()
            {
                if (this.Count == 0)
                {
                    throw new InvalidOperationException("List is empty");
                }

                // if (_shuffledIndices == null) {
                //     _shuffledIndices = Enumerable.Range(0, this.Count).ToList();
                // }
                _shuffledIndices ??= Enumerable.Range(0, this.Count).ToList();

                if (_lastIndex >= _shuffledIndices.Count)
                {
                    _shuffledIndices = _shuffledIndices
                        .OrderBy(_ => Guid.NewGuid())
                        .ToList();
                    _lastIndex = 0;
                }

                var randomIndex = _shuffledIndices[_lastIndex++];
                return this[randomIndex];
            }
        }

        public class RandomListFisherYates : List<string>
        {

            private List<int>? _shuffledIndices = null;
            private int _lastIndex = 0;
            private readonly Random _random = new();

            public new void Add(string item)
            {
                base.Add(item);
                _shuffledIndices = null;
                _lastIndex = 0;
            }

            public new void Clear()
            {
                base.Clear();
                _shuffledIndices = null;
                _lastIndex = 0;
            }

            public string GetRandomString()
            {
                if (this.Count == 0)
                {
                    throw new InvalidOperationException("List is empty");
                }


                // if (_shuffledIndices == null) {
                //     _shuffledIndices = Enumerable.Range(0, this.Count).ToList();
                // }
                _shuffledIndices ??= Enumerable.Range(0, this.Count).ToList();

                if (_lastIndex >= _shuffledIndices.Count)
                {
                    // Fisher-Yates Shuffle - for true randomness
                    for (int i = _shuffledIndices.Count - 1; i > 0; i--)
                    {
                        //var swapIndex = RandomNumberGenerator.GetInt32(i + 1);
                        var swapIndex = _random.Next(i + 1);

                        // var tmp = _shuffledIndices[i];
                        // _shuffledIndices[i] = _shuffledIndices[swapIndex];
                        // _shuffledIndices[swapIndex] = tmp;

                        (_shuffledIndices[i], _shuffledIndices[swapIndex]) =
                            (_shuffledIndices[swapIndex], _shuffledIndices[i]);
                    }

                    _lastIndex = 0;
                }

                int randomIndex = _shuffledIndices[_lastIndex++];
                return this[randomIndex];
            }

        }

        public class RandomListSimple : List<string>
        {
            private readonly Random _random = new();

            public string GetRandomString()
            {
                if (this.Count == 0)
                {
                    throw new InvalidOperationException("List is empty");
                }

                //Super Slow
                //return this[RandomNumberGenerator.GetInt32(this.Count)];
                return this[_random.Next(this.Count)];
                
            }
        }

        static void Main(string[] args)
        {
            var iterations = 10_000;
            Console.WriteLine("Three different ways to obtain a random string from a list:");
            Console.WriteLine("With ideal randomness every string should appear {0} times",iterations);
            
            var def = new string[] { "aaaaaaa", "bbbbbbb", "ccccccc", "ddddddd", "eeeeeee", "fffffff" };
            var rl0 = new RandomListSimple();
            var rl1 = new RandomListNewGuid();
            var rl2 = new RandomListFisherYates();

            rl0.AddRange(def);
            rl1.AddRange(def);
            rl2.AddRange(def);


            var dr0 = def.ToDictionary(x => x, _ => 0);
            var dr1 = def.ToDictionary(x => x, _ => 0);
            var dr2 = def.ToDictionary(x => x, _ => 0);
            
            //Simple
            var sw0 = new Stopwatch();
            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
            
            sw0.Start();
            for (var i = 0; i < def.Length * iterations; i++)
            {
                var s = rl0.GetRandomString();
                dr0[s]++;
            }
            sw0.Stop();

            sw1.Start();
            for (var i = 0; i < def.Length * iterations; i++)
            {
                var s = rl1.GetRandomString();
                dr1[s]++;
            }
            sw1.Stop();
            
            sw2.Start();
            for (var i = 0; i < def.Length * iterations; i++)
            {
                var s = rl2.GetRandomString();
                dr2[s]++;
            }
            sw2.Stop();

            //Simple
            foreach (var kvp in dr0.OrderBy(kv => kv.Value))
                Console.WriteLine("RandomListSimple: {0} - {1}", kvp.Key, kvp.Value);
            Console.WriteLine("RandomListSimple: {0}ms", sw0.Elapsed.TotalMilliseconds);
            //Linq
            foreach (var kvp in dr1.OrderBy(kv => kv.Value))
                Console.WriteLine("RandomListNewGuid: {0} - {1}", kvp.Key, kvp.Value);
            Console.WriteLine("RandomListNewGuid: {0}ms", sw1.Elapsed.TotalMilliseconds);
            //YF
            foreach (var kvp in dr2.OrderBy(kv => kv.Value))
                Console.WriteLine("RandomListFisherYates: {0} - {1}", kvp.Key, kvp.Value);
            Console.WriteLine("RandomListFisherYates: {0}ms", sw2.Elapsed.TotalMilliseconds);

            


        }
    }
}
