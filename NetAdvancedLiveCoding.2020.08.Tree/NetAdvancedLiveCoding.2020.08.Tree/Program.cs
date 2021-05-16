using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetAdvancedLiveCoding._2020._08.Tree
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Console.ReadLine();

            var fileSystemQueue = new BlockingCollection<object>();
            var consoleQueue = new BlockingCollection<object>();
            var jsonQueue = new BlockingCollection<object>();
            var task1 = FileSystemProducer(path, fileSystemQueue).ContinueWith(t => fileSystemQueue.CompleteAdding());
            var task15 = PrepareForMultipleConsumers(fileSystemQueue, consoleQueue, jsonQueue);
            var task2 = ConsoleConsumer(consoleQueue);
            var task3 = ConsumeForJSON(jsonQueue);
            Task.WaitAll(task1, task15, task2, task3);
        }

        private static async Task PrepareForMultipleConsumers(BlockingCollection<object> fileSystemQueue, params BlockingCollection<object>[] consumerCollections)
        {
            await Task.Run(() =>
            {
                foreach (var item in fileSystemQueue.GetConsumingEnumerable())
                {
                    if (item is BlockingCollection<object> nested)
                    {
                        var collections = new List<BlockingCollection<object>>();
                        foreach (var col in consumerCollections)
                        {
                            var current = new BlockingCollection<object>();
                            collections.Add(current);
                            col.Add(current);
                        }
                        PrepareForMultipleConsumers(nested, collections.ToArray());
                    }
                    foreach (var col in consumerCollections)
                        col.Add(item);
                }
                foreach (var col in consumerCollections)
                    col.CompleteAdding();
            });
        }

        private static async Task ConsumeForJSON(BlockingCollection<object> blockingCollection)
        {
            using (var file = new StreamWriter(File.OpenWrite("file.json")))
            {
                using (var writer = new JsonTextWriter(file))
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await ConsumeForJSONInternal(blockingCollection, writer);
                        }
                        catch (OperationCanceledException) { }
                    });
                }
            }
        }

        private static async Task ConsumeForJSONInternal(BlockingCollection<object> blockingCollection, JsonTextWriter writer)
        {
            bool hasContent = false;
            foreach (var item in blockingCollection.GetConsumingEnumerable())
            {
                if (item is BlockingCollection<object> dirCollection)
                    try
                    {
                        await ConsumeForJSONInternal(dirCollection, writer);
                    }
                    catch (OperationCanceledException) { }
                else if (item is FileInfo[] files)
                {
                    hasContent = true;
                    foreach (var file in files)
                        writer.WriteValue(file.Name);
                }
                else if (item is DirectoryInfo di)
                {
                    hasContent = true;
                    writer.WriteStartObject();
                    writer.WritePropertyName("Name");
                    writer.WriteValue(di.Name);
                    writer.WritePropertyName("Content");
                    writer.WriteStartArray();
                }
            }
            if (hasContent)
            {
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }

        private static async Task ConsoleConsumer(BlockingCollection<object> fileSystemQueue, string offset = "-")
        {
            await Task.Run(async () =>
            {
                foreach (var item in fileSystemQueue.GetConsumingEnumerable())
                {
                    if (item is BlockingCollection<object> dirCollection)
                        await ConsoleConsumer(dirCollection, offset + "-");
                    else if (item is FileInfo[] files)
                    {
                        foreach (var file in files)
                        {
                            Console.Write(offset + "-");
                            Console.WriteLine(file.Name);
                        }
                    }
                    else if (item is DirectoryInfo di)
                    {
                        Console.WriteLine(offset + di.Name + " <dir>");
                    }
                }
            });
        }

        private static async Task FileSystemProducer(string path, BlockingCollection<object> queue)
        {
            var di = new DirectoryInfo(path);
            var dirCollection = new BlockingCollection<object>();
            queue.Add(dirCollection);
            dirCollection.Add(di);
            dirCollection.Add(di.GetFiles());
            await Task.Run(async () =>
            {
                foreach (var dir in di.GetDirectories())
                    await FileSystemProducer(dir.FullName, dirCollection);
                dirCollection.CompleteAdding();
            });
        }
    }
}
