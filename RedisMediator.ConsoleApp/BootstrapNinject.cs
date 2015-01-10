using System;
using System.IO;
using CommonServiceLocator.NinjectAdapter.Unofficial;
using MediatR;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Conventions;
using Ninject.Planning.Bindings.Resolvers;
using RedisMediatorClient;

namespace RedisMediator.ConsoleApp
{
    internal class BootstrapNinject
    {
        public static IMediator BuildMediator()
        {
            IKernel kernel = new StandardKernel();
            kernel.Components.Add<IBindingResolver, ContravariantBindingResolver>();
            kernel.Bind(scan => scan.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());


            kernel.Bind(
                        x =>
                            x.FromAssemblyContaining<ExpensiveRequest>()
                                .SelectAllClasses()
                                .InheritedFrom(typeof(IRequestHandler<,>)).BindAllInterfaces().Configure(syntax => syntax.WhenInjectedInto(typeof(MediatorPipeline<,>))

                        ));

            kernel.Bind(typeof (IRequestHandler<,>)).To(typeof (MediatorPipeline<,>));

            kernel.Bind<TextWriter>().ToConstant(Console.Out);

            var serviceLocator = new NinjectServiceLocator(kernel);
            var serviceLocatorProvider = new ServiceLocatorProvider(() => serviceLocator);
            kernel.Bind<ServiceLocatorProvider>().ToConstant(serviceLocatorProvider);


            var mediator = serviceLocator.GetInstance<IMediator>();

            return mediator;
        }

        private static void Action(IContext context, object o)
        {
            new MediatorPipeline<ExpensiveRequest, ExpensiveRequestResponse>(
                (IRequestHandler<ExpensiveRequest, ExpensiveRequestResponse>) o, null, null);
        }

        /* 
            var openListType = typeof (MediatorPipeline<,>);
            var requestType = context.Request.Service.GenericTypeArguments[0];
            var responseType = context.Request.Service.GenericTypeArguments[1];

            var preRequestType = typeof (IPreRequestHandler<>).MakeGenericType(requestType);
            var postRequestType = typeof (IPostRequestHandler<,>).MakeGenericType(requestType, responseType);

            try
            {

                var preRequests = context.Kernel.GetAll(preRequestType);

                var postRequests = context.Kernel.GetAll(postRequestType);

                var preRequestArrayInstance = Array.CreateInstance(preRequestType,preRequests.Count());

                var postRequestArrayInstance = Array.CreateInstance(postRequestType,postRequests.Count());
                for (int i = 0; i < preRequests.Count(); i++)
                {
                    dynamic arr = preRequestArrayInstance;
                    dynamic elementAt = preRequests.ElementAt(i);
                    arr[i] = elementAt;
                }

                var genericListType = openListType.MakeGenericType(requestType, responseType);
                Activator.CreateInstance(genericListType, o, preRequestArrayInstance, postRequestArrayInstance);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }*/

        public static dynamic ChangeTheType(dynamic source, Type dest)
        {
            return Convert.ChangeType(source, dest);
        }
    }
}