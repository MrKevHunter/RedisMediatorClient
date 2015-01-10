using System;

namespace RedisMediatorClient
{
    public interface IPreRequestHandler<in TRequest>
    {
        void Handle(TRequest request);
    }

    public class AuthorisationHandler : IPreRequestHandler<ExpensiveRequest>
    {
        public void Handle(ExpensiveRequest request)
        {
            Console.WriteLine("Creating request");
        }
    }
}