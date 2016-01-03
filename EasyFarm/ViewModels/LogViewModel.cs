﻿/*///////////////////////////////////////////////////////////////////
<EasyFarm, general farming utility for FFXI.>
Copyright (C) <2013>  <Zerolimits>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
*/
///////////////////////////////////////////////////////////////////

using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using EasyFarm.Infrastructure;
using EasyFarm.Logging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;

namespace EasyFarm.ViewModels
{
    [ViewModel("Log")]
    public class LogViewModel : ViewModelBase
    {
        private readonly SynchronizationContext _syncContext;

        public LogViewModel()
        {
            LoggedItems = new ObservableCollection<string>();
            EventListener = new ObservableEventListener();
            EventListener.EnableEvents(Logger.Write, EventLevel.Verbose);
            _syncContext = SynchronizationContext.Current;
            EventListener.LogToCollection(PublishLogItem);
            // Can only be called on the dispatcher's thread. 
        }

        public ObservableCollection<string> LoggedItems { get; set; }
        public StringSink EventSink { get; set; }
        public ObservableEventListener EventListener { get; set; }

        /// <summary>
        ///     Publish log item under the right thread context.
        /// </summary>
        /// <param name="message"></param>
        public void PublishLogItem(string message)
        {
            if (_syncContext == SynchronizationContext.Current)
                AddLogItem(message);
            else
                _syncContext.Send(o => AddLogItem(message), null);
        }

        /// <summary>
        /// Add message to log without while preventing a <see cref="OutOfMemoryException"/>. 
        /// </summary>
        /// <param name="message"></param>
        public void AddLogItem(string message)
        {
            LoggedItems.Add(message);

            // Limit list to only 1000 items: prevent system out of memory exception. 
            if (LoggedItems.Count > 1000)
            {
                LoggedItems.Remove(LoggedItems.Last());
            }
        }
    }
}