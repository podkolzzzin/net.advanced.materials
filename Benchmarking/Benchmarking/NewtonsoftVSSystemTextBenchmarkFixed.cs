using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Benchmarking
{
    public class NewtonsoftVSSystemTextBenchmarkFixed
    {
        private const string PATH = @"C:\Users\andrii.podkolzin\source\repos\Benchmarking\Benchmarking\json\";
        private readonly Dictionary<string, string> data;

        public NewtonsoftVSSystemTextBenchmarkFixed()
        {
            data = Directory.EnumerateFiles(PATH).ToDictionary(x => Path.GetFileName(x), x => File.ReadAllText(x));
        }

        [ParamsSource(nameof(ItemSource))]
        public string FileName;

        public IEnumerable<string> ItemSource()
        {
            return Directory.EnumerateFiles(PATH).Select(x => Path.GetFileName(x));
        }

        [Benchmark]
        public void SystemTextJson()
        {
            var text = data[FileName];
            JsonDocument.Parse(text);
        }

        [Benchmark]
        public void NewtonsoftJson()
        {
            var text = data[FileName];
            JToken.Parse(text);
        }
    }
}
