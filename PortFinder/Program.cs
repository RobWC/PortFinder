using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;

namespace PortFinder
{
    class Program
    {
        static void Main(string[] args)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var counter = 0;
            var tracker = new Dictionary<int, int>();
            var firstWatch = System.Diagnostics.Stopwatch.StartNew();
            while (true)
            {
                int port = 0;
                Socket sock = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                sock.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0)); // Pass 0 here.

                port = ((IPEndPoint) sock.LocalEndPoint).Port;

                if (tracker.ContainsKey(port))
                {
                    firstWatch.Stop();
                    tracker[port]++;
                }
                else
                {
                    tracker[port] = 1;
                }
                
                sock.Dispose();
                counter++;
                
                if (counter > 500000)
                {
                    break;
                }
            }

            var sortedTracker = tracker.ToImmutableSortedDictionary();
            var totalUsedPorts = 0;
            var lowestPort = 0;
            var higestPort = 0;
            var maxUsed = new KeyValuePair<int,int>();
            var minUsed = new KeyValuePair<int,int>();
            var loopCounter = 0;
            foreach (KeyValuePair<int, int> port in sortedTracker)
            {
                if (totalUsedPorts == 0)
                {
                    lowestPort = port.Key;
                }
                totalUsedPorts = totalUsedPorts + port.Value;
                Console.WriteLine($"Port: {port.Key} Usages: {port.Value}");

                if (port.Value > maxUsed.Value)
                {
                    maxUsed = new KeyValuePair<int, int>(port.Key, port.Value);

                }
                
                if (minUsed.Value >= port.Value || minUsed.Value == 0)
                {
                    minUsed = new KeyValuePair<int, int>(port.Key, port.Value);

                }

                if (loopCounter == sortedTracker.Count -1)
                {
                    higestPort = port.Key;
                }

                loopCounter++;
            }
            watch.Stop();
            Console.WriteLine($"Total time to run: {watch.ElapsedMilliseconds} First collision: {firstWatch.ElapsedMilliseconds}");
            Console.WriteLine($"Lowest port: {lowestPort} Highest port: {higestPort}");
            Console.WriteLine($"Total Ports found: {sortedTracker.Count}");
            Console.WriteLine($"Total ports used: {totalUsedPorts}");
            Console.WriteLine($"Highest used: {maxUsed.Key} used: {sortedTracker[maxUsed.Key]}");
            Console.WriteLine($"Lowest used: {minUsed.Key} used: {sortedTracker[minUsed.Key]}");
            Console.WriteLine($"Average usages: {totalUsedPorts / sortedTracker.Count}");
        }
    }
}