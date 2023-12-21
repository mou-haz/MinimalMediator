using System.Reflection;
using MinimalMediator.Abstractions.LazyHelpers;
using MinimalMediator.Mediator;

namespace MinimalMediator.Helpers;

public static class CustomMediatorExtension
{
    public static void AddCustomMediator(this IServiceCollection serviceDescriptors,
        Func<Type, bool>? typeFilter = null, bool registerInfrastructure = false,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, Assembly[] assemblies = null)
    {
        var customMediatorOpenTypes = new[]
        {
            typeof(IRequestHandlerBase<>),
            typeof(IRequestHandlerBase<,>),
            typeof(IStreamHandler<,>),
            typeof(IInterceptPipeline<>),
            typeof(IInterceptPipeline<,>),
            typeof(IInterceptStreamPipeline<,>),
        };

        Func<Type, Type, IServiceCollection> registerationMethod = serviceLifetime switch
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
            serviceDescriptors.AddSingleton(typeof(LazyRequestHandler<>), typeof(LazyRequestHandler<>));

            registerationMethod(typeof(IMediator), typeof(CustomMediator));

            registerationMethod(typeof(CustomMediator), typeof(CustomMediator));

            registerationMethod(typeof(IHandlerWrapper<>), typeof(HandlerWrapper<>));
            registerationMethod(typeof(HandlerWrapper<>), typeof(HandlerWrapper<>));

            registerationMethod(typeof(IHandlerWrapper<,>), typeof(HandlerWrapper<,>));
            registerationMethod(typeof(HandlerWrapper<,>), typeof(HandlerWrapper<,>));

            registerationMethod(typeof(StreamHandlerWrapper<,>), typeof(StreamHandlerWrapper<,>));
            registerationMethod(typeof(IStreamHandlerWrapper<,>), typeof(StreamHandlerWrapper<,>));
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

                if (customMediatorOpenTypes.Contains(genericInterface))
                {
                    registerationMethod(interfaceType, type);
                    registerType = true;
                }
            }

            if (registerType)
            {
                registerationMethod(type, type);
            }
        }
    }
}