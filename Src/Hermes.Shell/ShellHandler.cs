﻿using System;

using Hermes.Messages;
using Hermes.Tests.Messages;

namespace Hermes.Shell
{
    public class ShellHandler : IHandleMessage<OrderShipped>
    {
        public void Handle(OrderShipped command)
        {
            Console.WriteLine("Shoes sold {0}", command.OrderNumber);
        }
    }
}