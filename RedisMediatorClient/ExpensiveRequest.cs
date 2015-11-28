using System;
using System.Threading;
using MediatR;
namespace RedisMediatorClient
{
    public class ExpensiveRequest :IRequest<ExpensiveRequestResponse>,IRedisCachable
    {
        public string GenerateKey()
        {
            return "b";
        }
    }

    public interface IRedisCachable
    {
        string GenerateKey();
    }


    public class ExpensiveRequestHandler : IRequestHandler<ExpensiveRequest,ExpensiveRequestResponse>
    {
        public ExpensiveRequestResponse Handle(ExpensiveRequest message)
        {
            Thread.Sleep(5000);
            // Added some text
            return new ExpensiveRequestResponse();
        }
    }

    public class ExpensiveRequestResponse
    {
        public ExpensiveRequestResponse()
        {
            Created = DateTime.Now;
        }

        public DateTime Created { get; set; }
    }
}
