using System;

namespace PlaygroundSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            LogProcessing_OneContractManyEventsAsync.Main(null).Wait();
            Console.WriteLine("Hello World!");
        }
    }
}
