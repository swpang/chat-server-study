﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Concurrent;

namespace chatTest3_Server
{
    class StaticDefine
    {
        public const int SHOW_CURRENT_CLIENT = 1;
        public const int SHOW_ACCESS_LOG = 2;
        public const int SHOW_CHATTING_LOG = 3;
        public const int ADD_ACCESS_LOG = 5;
        public const int ADD_CHATTING_LOG = 6;
        public const int EXIT = 0;
    }
}
