﻿using ARWNI2S.Caching;
using ARWNI2S.Core.Builder;
using ARWNI2S.Core.Caching;
using ARWNI2S.Core.Configuration;
using ARWNI2S.Core.Models;
using ARWNI2S.Diagnostics;
using ARWNI2S.Engine.Configuration;
using ARWNI2S.Engine.Object;
using ARWNI2S.Engine.Parts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Buffers;

namespace ARWNI2S.Core
{
    internal static class NI2SCoreServiceCollectionExtensions
    {
        internal static INiisCoreBuilder AddNI2SCore(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));
            INodeHostEnvironment serviceFromCollection = GetServiceFromCollection<INodeHostEnvironment>(services);
            EnginePartManager applicationPartManager = GetEnginePartManager(services, serviceFromCollection);
            services.TryAddSingleton(applicationPartManager);
            ConfigureDefaultProviders(applicationPartManager);
            ConfigureDefaultServices(services);
            AddNI2SCoreServices(services);
            return new NI2SCoreBuilder(services, applicationPartManager);
        }

        private static void ConfigureDefaultProviders(EnginePartManager manager)
        {
            if (!manager.FeatureProviders.OfType<ActorFeatureProvider>().Any())
            {
                manager.FeatureProviders.Add(new ActorFeatureProvider());
            }
        }

        private static EnginePartManager GetEnginePartManager(IServiceCollection services, INodeHostEnvironment environment)
        {
            EnginePartManager applicationPartManager = GetServiceFromCollection<EnginePartManager>(services);
            if (applicationPartManager == null)
            {
                applicationPartManager = new EnginePartManager();
                string text = environment?.ApplicationName;
                if (string.IsNullOrEmpty(text))
                {
                    return applicationPartManager;
                }
                applicationPartManager.PopulateDefaultParts(text);
            }
            return applicationPartManager;
        }

        private static T GetServiceFromCollection<T>(IServiceCollection services)
        {
            return (T)(services.LastOrDefault((d) => d.ServiceType == typeof(T))?.ImplementationInstance);
        }

        public static INiisCoreBuilder AddNI2SCore(this IServiceCollection services, Action<NI2SOptions> setupAction)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));
            ArgumentNullException.ThrowIfNull(setupAction, nameof(setupAction));
            INiisCoreBuilder result = services.AddNI2SCore();
            services.Configure(setupAction);
            return result;
        }

        internal static void AddNI2SCoreServices(IServiceCollection services)
        {
            services.AddSingleton<IObjectFactory<NI2SObject>, DefaultObjectFactory>();

            // Registrar el servicio genérico de resolución de factorías
            services.AddTransient(typeof(IObjectFactory<>), sp =>
            {
                var requestedType = sp.GetService(typeof(IObjectFactory<>))?.GetType();
                var specificFactory = sp.GetService(requestedType);
                return specificFactory ?? sp.GetRequiredService<IObjectFactory<NI2SObject>>();
            });


            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<NI2SOptions>, NI2SCoreNI2SOptionsSetup>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IPostConfigureOptions<NI2SOptions>, NI2SCoreNI2SOptionsSetup>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<BehaviorOptions>, BehaviorOptionsSetup>());
            //services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<NetwoekingOptions>, NI2SCoreNetwoekingOptionsSetup>());

            //services.TryAddSingleton<EngineModelFactory>();
            //services.TryAddEnumerable(ServiceDescriptor.Transient<IEngineModelProvider, DefaultEngineModelProvider>());
            //services.TryAddEnumerable(ServiceDescriptor.Transient<IEngineModelProvider, AiBehaviorEngineModelProvider>());

            //services.TryAddEnumerable(ServiceDescriptor.Transient<IActionDescriptorProvider, ActorActionDescriptorProvider>());
            //services.TryAddSingleton<IActionDescriptorCollectionProvider, DefaultActionDescriptorCollectionProvider>();
            //services.TryAddSingleton<IActionSelector, ActionSelector>();
            //services.TryAddSingleton<ActionConstraintCache>();
            //services.TryAddEnumerable(ServiceDescriptor.Transient<IActionConstraintProvider, DefaultActionConstraintProvider>());
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, ActionConstraintMatcherPolicy>());

            //services.TryAddSingleton<IActorFactory, DefaultActorFactory>();
            //services.TryAddTransient<IActorActivator, DefaultActorActivator>();
            //services.TryAddSingleton<IActorFactoryProvider, ActorFactoryProvider>();
            //services.TryAddSingleton<IActorActivatorProvider, ActorActivatorProvider>();

            services.TryAddEnumerable(ServiceDescriptor.Transient<IActorStateActivator, DefaultActorStateActivator>());

            //services.TryAddSingleton<IActionInvokerFactory, ActionInvokerFactory>();
            //services.TryAddEnumerable(ServiceDescriptor.Transient<IActionInvokerProvider, ActorActionInvokerProvider>());
            //services.TryAddSingleton<ActorActionInvokerCache>();
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IActionFilterProvider, DefaultFilterProvider>());
            //services.TryAddSingleton<IActionResultTypeMapper, ActionResultTypeMapper>();

            ////services.TryAddTransient<RequestSizeLimitFilter>();
            ////services.TryAddTransient<DisableRequestSizeLimitFilter>();Update
            ////services.TryAddTransient<RequestFormLimitsFilter>();
            services.TryAddSingleton<IModelMetadataProvider, DefaultModelMetadataProvider>();
            services.TryAdd(ServiceDescriptor.Transient((Func<IServiceProvider, ICompositeMetadataDetailsProvider>)((s) => new DefaultCompositeMetadataDetailsProvider(s.GetRequiredService<NI2SOptions>().ModelMetadataDetailsProviders))));
            services.TryAddSingleton<IModelBinderFactory, ModelBinderFactory>();
            services.TryAddSingleton((Func<IServiceProvider, IObjectModelValidator>)delegate (IServiceProvider s)
            {
                var options = s.GetRequiredService<NI2SOptions>();
                return new DefaultObjectValidator(s.GetRequiredService<IModelMetadataProvider>(), options.ModelValidatorProviders, options);
            });
            ////services.TryAddSingleton<ClientValidatorCache>();
            //services.TryAddSingleton<ParameterBinder>();
            //services.TryAddSingleton<NI2SMarkerService, NI2SMarkerService>();
            services.TryAddSingleton<ITypeActivatorCache, TypeActivatorCache>();

            ////services.TryAddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            ////services.TryAddSingleton<IHttpRequestStreamReaderFactory, MemoryPoolHttpRequestStreamReaderFactory>();
            ////services.TryAddSingleton<IHttpResponseStreamWriterFactory, MemoryPoolHttpResponseStreamWriterFactory>();
            services.TryAddSingleton(ArrayPool<byte>.Shared);
            services.TryAddSingleton(ArrayPool<char>.Shared);
            ////services.TryAddSingleton<OutputFormatterSelector, DefaultOutputFormatterSelector>();
            ////services.TryAddSingleton<IActionResultExecutor<ObjectResult>, ObjectResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<PhysicalFileResult>, PhysicalFileResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<VirtualFileResult>, VirtualFileResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<FileStreamResult>, FileStreamResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<FileContentResult>, FileContentResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<RedirectResult>, RedirectResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<LocalRedirectResult>, LocalRedirectResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<RedirectToActionResult>, RedirectToActionResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<RedirectToRouteResult>, RedirectToRouteResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<RedirectToPageResult>, RedirectToPageResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<ContentResult>, ContentResultExecutor>();
            ////services.TryAddSingleton<IActionResultExecutor<JsonResult>, SystemTextJsonResultExecutor>();
            services.TryAddSingleton<IRemoteErrorFactory, ProblemDetailsRemoteErrorFactory>();
            ////services.TryAddSingleton<MvcRouteHandler>();
            ////services.TryAddTransient<MvcAttributeRouteHandler>();

            //services.TryAddSingleton<ActorActionUpdateDataSourceFactory>();
            //services.TryAddSingleton<OrderedUpdatesSequenceProviderCache>();
            //services.TryAddSingleton<ActorActionUpdateDataSourceIdProvider>();
            //services.TryAddSingleton<ActionUpdateFactory>();
            //services.TryAddSingleton<DynamicActorUpdateSelectorCache>();
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, DynamicActorUpdateMatcherPolicy>());
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IUpdateDelegateFactory, ActorUpdateDelegateFactory>());

            ////services.TryAddSingleton<MiddlewareFilterConfigurationProvider>();
            ////services.TryAddSingleton<MiddlewareFilterBuilder>();
            ////services.TryAddEnumerable(ServiceDescriptor.Singleton<IStartupFilter, MiddlewareFilterBuilderStartupFilter>());
            services.TryAddSingleton<ProblemDetailsFactory, DefaultProblemDetailsFactory>();
            ////services.TryAddEnumerable(ServiceDescriptor.Singleton<IProblemDetailsWriter, DefaultApiProblemDetailsWriter>());
        }

        private static void ConfigureDefaultServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddMemoryCache();
            services.AddDistributedCache();
        }
    }
}
