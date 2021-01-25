using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Benchmarking
{
    public class NewtonsoftVSSystemTextBenchmark
    {

        private const string PATH = @"C:\Users\andrii.podkolzin\source\repos\Benchmarking\Benchmarking\json\";

        [Benchmark]
        public void SystemTextJson()
        {
            var files = Directory.EnumerateFiles(@$"{PATH}\*.json");
            foreach(var f in files)
            {
                var text = File.ReadAllText(f);
                JsonDocument.Parse(text);
            }
        }

        [Benchmark]
        public void NewtonsoftJson()
        {
            foreach(var file in new DirectoryInfo(@$"{PATH}").EnumerateFiles())
            {
                using (var stream = file.OpenText())
                {
                    using (var reader = new JsonTextReader(stream))
                    {
                        JToken.Load(reader);
                    }
                }
            }
        }
    }
}