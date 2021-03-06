﻿using System;
using System.Collections.Generic;

using Microsoft.Practices.ServiceLocation;

namespace Hermes.Ioc
{
    public class DisposedProvider : ServiceLocatorImplBase
    {
        protected override object DoGetInstance(Type serviceType, string key)
        {
            throw new ObjectDisposedException("Unable to resolve service {0} as the service provider is currently disposed.");
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            throw new ObjectDisposedException("Unable to resolve service {0} as the service provider is currently disposed.");
        }
    }
}