using System;
using System.IO;
using System.Net.NetworkInformation;
using MediatR;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace RedisMediatorClient
{
    public class BootstrapStructuremap
    {
        public static IMediator BuildMediator()
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<ExpensiveRequest>();
                    scanner.AssemblyContainingType<IMediator>();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof (IRequestHandler<,>));
                    scanner.AddAllTypesOf(typeof (IAsyncRequestHandler<,>));
                    scanner.AddAllTypesOf(typeof (INotificationHandler<>));
                    scanner.AddAllTypesOf(typeof (IAsyncNotificationHandler<>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IPreRequestHandler<>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IPostRequestHandler<,>));

                });
                cfg.For<TextWriter>().Use(Console.Out);

                var handlerType = cfg.For(typeof(IRequestHandler<,>));
                handlerType.DecorateAllWith(typeof(CachingHandler<,>));
                
                cfg.For(typeof(IRequestHandler<,>)).DecorateAllWith(typeof(MediatorPipeline<,>));
                
            });
            var whatDoIHave = container.WhatDoIHave();
            var serviceLocator = new StructureMapServiceLocator(container);
            var serviceLocatorProvider = new ServiceLocatorProvider(() => serviceLocator);
            container.Configure(cfg => cfg.For<ServiceLocatorProvider>().Use(serviceLocatorProvider));
            var mediator = serviceLocator.GetInstance<IMediator>();

            return mediator;
        }
    }
}