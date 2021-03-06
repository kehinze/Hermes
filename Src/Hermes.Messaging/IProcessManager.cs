﻿namespace Hermes.Messaging
{
    public interface IProcessManager<out T> where T : class, IContainProcessManagerData
    {
        IPersistProcessManagers ProcessManagerPersistence { get; set; }
        IMessageBus Bus { get; set; }
    }
}