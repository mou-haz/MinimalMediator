using System.Reflection;
using MinimalMediator.Abstractions.LazyHelpers;
using MinimalMediator.Mediator;

namespace MinimalMediator.Helpers;

public static class MinimalMediatorExtension
{
    internal static ServiceLifetime DefaultLifeTime = ServiceLifetime.Scoped;

    public static void AddMinimalMediator(this IServiceCollection serviceDescriptors,
        Func<Type, bool>? typeFilter = null, bool registerInfrastructure = false,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, params Assembly[] assemblies)
    {
        var minimalMediatorOpenTypes = new[]
        {
            typeof(IRequestHandlerBase<>),
            typeof(IRequestHandlerBase<,>),
            typeof(IStreamHandler<,>),
            typeof(IInterceptPipeline<>),
            typeof(IInterceptPipeline<,>),
            typeof(IInterceptStreamPipeline<,>),
        };

        Func<Type, Type, IServiceCollection> registrationMethod = serviceLifetime switch
        {
            ServiceLifetime.Singleton => serviceDescriptors.AddSingleton,
            ServiceLifetime.Transient => serviceDescriptors.AddTransient,
            ServiceLifetime.Scoped => serviceDescriptors.AddScoped,
            _ => serviceDescriptors.AddScoped,
        };

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.DefinedTypes)
            {
                RegisterType(type);
            }
        }

        if (registerInfrastructure)
        {
            DefaultLifeTime = serviceLifetime;

            serviceDescriptors.AddSingleton(typeof(LazyRequestHandler<>), typeof(LazyRequestHandler<>));

            var concreteMediatorType = serviceLifetime switch
            {
                ServiceLifetime.Singleton => typeof(SingeltonMinimalMediator),
                _ => typeof(Mediator.MinimalMediator)
            };

            registrationMethod(typeof(IMediator), concreteMediatorType);
            
            registrationMethod(typeof(Mediator.MinimalMediator), typeof(Mediator.MinimalMediator));
            registrationMethod(typeof(SingeltonMinimalMediator), typeof(SingeltonMinimalMediator));

            registrationMethod(typeof(IHandlerWrapper<>), typeof(HandlerWrapper<>));
            registrationMethod(typeof(HandlerWrapper<>), typeof(HandlerWrapper<>));

            registrationMethod(typeof(IHandlerWrapper<,>), typeof(HandlerWrapper<,>));
            registrationMethod(typeof(HandlerWrapper<,>), typeof(HandlerWrapper<,>));

            registrationMethod(typeof(StreamHandlerWrapper<,>), typeof(StreamHandlerWrapper<,>));
            registrationMethod(typeof(IStreamHandlerWrapper<,>), typeof(StreamHandlerWrapper<,>));
        }

        void RegisterType(Type type)
        {
            if (typeFilter is not null && !typeFilter(type))
            {
                return;
            }

            var registerType = false;

            //using AsSpan() here causing error in wasm ?
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!interfaceType.IsGenericType)
                {
                    continue;
                }

                var genericInterface = interfaceType.GetGenericTypeDefinition();

                if (minimalMediatorOpenTypes.Contains(genericInterface))
                {
                    registrationMethod(interfaceType, type);
                    registerType = true;
                }
            }

            if (registerType)
            {
                registrationMethod(type, type);
            }
        }
    }
}