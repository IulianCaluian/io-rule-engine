﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioRulesEngine.ConsoleApp.Utils
{
    public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs e);
}
