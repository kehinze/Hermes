﻿using System;

using Hermes.Messaging.Configuration;

namespace Hermes.Messaging.Transports
{
    public static class MessageRuleValidation
    {
        public static void ValidateIsCommandType(object message)
        {
            if (message == null)
                throw new InvalidOperationException("Cannot send a null message.");

            if (Settings.IsCommandType == null)
            {
                throw new InvalidOperationException("No rules have been configured for command message types. Use the DefineCommandAs function during endpoint configuration to configure this rule.");
            }

            if (!Settings.IsCommandType(message.GetType()))
            {
                var error = String.Format("Send is reserved for messages that have been defined as commands using the DefineCommandAs" +
                    " function during endpoing configuration. Message {0} does not comply with the current rule.", message.GetType().FullName);
                throw new InvalidOperationException(error);
            }
        }

        public static void ValidateIsEventType(object message)
        {
            if (message == null)
                throw new InvalidOperationException("Cannot send a null message.");

            if (Settings.IsEventType == null)
            {
                throw new InvalidOperationException("No rules have been configured for event message types. Use the DefineEventAs function during endpoint configuration to configure this rule.");
            }

            if (!Settings.IsEventType(message.GetType()))
            {
                var error = String.Format("Publish is reserved for messages that have been defined as events using the DefineEventAs" +
                    " function during endpoing configuration. Message {0} does not comply with the current rule.", message.GetType().FullName);
                throw new InvalidOperationException(error);
            }
        }

        public static void ValidateIsMessageType(object message)
        {
            if (message == null)
                throw new InvalidOperationException("Cannot send a null message.");

            if (Settings.IsMessageType == null)
            {
                throw new InvalidOperationException("No rules have been configured for reply message types. Use the DefineMessageAs function during endpoint configuration to configure this rule.");
            }

            if (!Settings.IsMessageType(message.GetType()))
            {
                var error = String.Format("Reply is reserved for messages that have been defined as normal messages using the DefineMessageAs" +
                    " function during endpoing configuration. Message {0} does not comply with the current rule.", message.GetType().FullName);
                throw new InvalidOperationException(error);
            }
        }
    }
}