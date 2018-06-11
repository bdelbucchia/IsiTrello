﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsiTrello.Infrastructures
{
    public interface IDependencyService
    {
        T Get<T>() where T : class;
    }
}
