﻿using System;

namespace Hermes.Storage.EntityFramework.ProcessManagager
{
    public class ProcessManagerEntity
    {
        public Guid Id { get; set; }
        public Byte[] TimeStamp { get; set; }
        public byte[] State { get; set; }
    }
}