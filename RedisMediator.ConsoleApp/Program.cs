

using System;
using System.Diagnostics;
using MediatR;
using RedisMediatorClient;

namespace RedisMediator.ConsoleApp
{


    class Program
    {
        static void Main(string[] args)
        {

            IMediator mediator;
            if (1 == 2)
            {
                mediator = BootstrapStructuremap.BuildMediator();
            }
            else
            {
                mediator = BootstrapNinject.BuildMediator();
            }
            for (int i = 1; i <= 5; i++)
            {
                var sw = Stopwatch.StartNew();
                var expensiveRequestResponse = mediator.Send(new ExpensiveRequest());

                Console.WriteLine("request {1} returned in {0} seconds", sw.Elapsed.TotalSeconds,i);
                Console.WriteLine(expensiveRequestResponse.Created.Ticks);
            }
            Console.ReadLine();
        }
    }
}
