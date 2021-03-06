﻿using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Practices.ServiceLocation;

namespace Hermes.Ioc
{
    public class ServiceLocator : ServiceLocatorImplBase
    {
        private static readonly ThreadLocal<ServiceLocator> instance = new ThreadLocal<ServiceLocator>();
        private static readonly object SyncRoot = new Object();
        private IServiceLocator serviceProvider;

        private ServiceLocator()
        {
            SetCurrentLifetimeScope(new DisposedProvider());
        }

        public static ServiceLocator Current
        {
            get { return GetInstance(); }
        }

        private static ServiceLocator GetInstance()
        {
            if (!instance.IsValueCreated)
            {
                lock (SyncRoot)
                {
                    if (!instance.IsValueCreated)
                    {
                        instance.Value = new ServiceLocator();
                    }
                }
            }

            return instance.Value;
        }

        public void SetCurrentLifetimeScope(IServiceLocator provider)
        {
            serviceProvider = provider ?? new DisposedProvider();
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return serviceProvider.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return serviceProvider.GetAllInstances(serviceType);
        }

        public TService GetService<TService>() where TService : class
        {
            return (TService)GetService(typeof(TService));
        }

        public bool HasServiceProvider()
        {
            return serviceProvider != null && !(serviceProvider is DisposedProvider);
        }
    }
}