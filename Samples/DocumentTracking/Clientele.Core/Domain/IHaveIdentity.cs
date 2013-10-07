﻿namespace Clientele.Core.Domain
{
    public interface IHaveIdentity
    {
        dynamic GetId();
        bool IsEmpty();
        string GetTag();
    }
}