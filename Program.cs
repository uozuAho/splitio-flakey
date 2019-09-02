using System;
using Splitio.Services.Client.Classes;
using Splitio.Services.Client.Interfaces;

namespace splitio_flakey
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("usage: exe <apikey> <feature name> <feature key>");
                return;
            }

            var apiKey = args[0];
            var featureName = args[1];
            var featureKey = args[2];

            TryClient(apiKey, featureKey, featureName);
        }

        private static void TryClient(string apiKey, string featureKey, string featureName)
        {
            Console.WriteLine("Creating split client...");

            var splitClient = CreateSplitClient(apiKey);

            Console.WriteLine("Getting feature value...");

            var featureValue = splitClient.GetTreatment(featureKey, featureName);

            Console.WriteLine($"Feature value: {featureValue}");
        }

        private static ISplitClient CreateSplitClient(string apiKey)
        {
            var factory = new SplitFactory(apiKey);

            var client = factory.Client();
            client.BlockUntilReady(5000);

            return client;
        }

        // todo: make this reliable. The problem:
        // Sometimes an exception is thrown on client.BlockUntilReady.
        // The factory then seems unable to produce a working client
        // until the whole process is restarted. A similar thing is seen
        // if client.Destroy() is called. Do this, then, as per docs, create a new
        // factory and client. This client will _always_ fail on BlockUntilReady
        private static ISplitClient ReliablyCreateSplitClient(string apiKey)
        {
            SplitFactory factory = null;
            ISplitClient client = null;

            for (var i = 0; i < 5; i++)
            {
                try
                {
                    if (factory == null)
                    {
                        factory = new SplitFactory(apiKey);
                    }
                    client = factory.Client();
                    client.BlockUntilReady(5000);

                    return client;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Creating client failed:");
                    Console.WriteLine();
                    Console.WriteLine(e);
                    Console.WriteLine();
                    Console.WriteLine("Retrying...");

                    client?.Destroy();
                    factory = null;
                }
            }

            throw new Exception("couldn't initialise client after 5 tries");
        }
    }
}
