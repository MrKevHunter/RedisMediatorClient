using System;
using System.IO;
using System.Linq;
using CommonServiceLocator.NinjectAdapter.Unofficial;
using MediatR;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Planning.Bindings;
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

            var openGenericRequestHandler = typeof (IRequestHandler<,>);
            var mediatorPipeline = typeof(MediatorPipeline<,>);
            kernel.Bind(
                x =>
                {
                    x.FromAssemblyContaining<ExpensiveRequest>()
                        .SelectAllClasses()
                        .InheritedFrom(openGenericRequestHandler)
                        .BindAllInterfaces()
                        .Configure(syntax => syntax.WhenInjectedInto(mediatorPipeline));
                });


            kernel.Bind(openGenericRequestHandler).To(mediatorPipeline);

            
            kernel.Bind(
                x =>
                    x.FromAssemblyContaining<ExpensiveRequest>()
                        .SelectAllClasses()
                        .InheritedFrom(typeof (IPreRequestHandler<>)).BindAllInterfaces()
                );
            kernel.Bind(
                x =>
                    x.FromAssemblyContaining<ExpensiveRequest>()
                        .SelectAllClasses()
                        .InheritedFrom(typeof (IPostRequestHandler<,>)).BindAllInterfaces()
                );


            kernel.Bind<TextWriter>().ToConstant(Console.Out);

            var serviceLocator = new NinjectServiceLocator(kernel);
            var serviceLocatorProvider = new ServiceLocatorProvider(() => serviceLocator);
            kernel.Bind<ServiceLocatorProvider>().ToConstant(serviceLocatorProvider);

            var mediator = serviceLocator.GetInstance<IMediator>();

            return mediator;
        }
    }
}