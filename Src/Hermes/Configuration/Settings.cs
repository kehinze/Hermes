﻿using System;
using System.Collections.Generic;

using Hermes.Ioc;

namespace Hermes.Configuration
{
    /// <summary>
    /// Used to store all infrastructure configuration settings
    /// </summary>
    public static class Settings
    {
        private static readonly Dictionary<string,object> settings = new Dictionary<string, object>();
        private static readonly Address defermentEndpoint = Address.Parse("Deferment");

        private static IContainerBuilder builder;
        private static Address thisEndpoint = Address.Undefined;
        private static int numberOfWorkers = 1;

        public static int NumberOfWorkers
        {
            get { return numberOfWorkers; }
            internal set { numberOfWorkers = value; }
        }

        public static IContainerBuilder Builder
        {
            get
            {
                if (builder == null)
                    throw new InvalidOperationException("Please add a call to Configure.DefaultBuilder() or any of the other supported builders to set one up");

                return builder;
            }

            internal set { builder = value; }
        } 

        public static IContainer RootContainer
        {
            get
            {
                if (builder == null)
                    throw new InvalidOperationException("Please add a call to Configure.DefaultBuilder() or any of the other supported builders to set one up");

                return builder.Container;
            } 
        }

        public static Address DefermentEndpoint
        {
            get { return defermentEndpoint; }
        }

        public static Address ThisEndpoint
        {
            get { return thisEndpoint; }
            internal set { thisEndpoint = value; }
        }

        public static IMessageBus MessageBus
        {
            get { return builder.Container.GetInstance<IMessageBus>(); }
        }

        public static IManageSubscriptions Subscriptions
        {
            get { return builder.Container.GetInstance<IManageSubscriptions>(); }
        }
 
        public static T GetSetting<T>(string settingKey)
        {
            if (settings.ContainsKey(settingKey))
            {
                return (T)settings[settingKey];
            }

            throw new ConfigurationSettingNotFoundException(settingKey);
        }

        public static void AddSetting(string settingKey, object value)
        {
            if (settings.ContainsKey(settingKey))
            {
                settings.Remove(settingKey);
            }

            settings.Add(settingKey, value);
        }              
    }
}