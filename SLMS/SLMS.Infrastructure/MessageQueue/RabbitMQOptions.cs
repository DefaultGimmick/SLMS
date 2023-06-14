﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Infrastructure.MessageQueue
{
    public class RabbitMQOptions
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
    }
}
