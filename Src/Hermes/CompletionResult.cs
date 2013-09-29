﻿using System.Collections.Generic;

namespace Hermes
{
    public class CompletionResult
    {
        public int ErrorCode { get; set; }
        public IReadOnlyCollection<object> Messages { get; set; }
        public object State { get; set; }
    }
}