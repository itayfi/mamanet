﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Files
{
    public interface INotifyFileChange
    {
        void NotifyFileChange(TaskScheduler taskScheduler);
    }
}
