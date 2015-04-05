﻿using Caliburn.Micro;
using Frontend.Core.Commands;
using Frontend.Core.Converting.Operations;
using Frontend.Core.Model;
using Frontend.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Core.Converting
{
    public class RunOperationsCommand : CommandBase
    {
        private IConverterOptions options;
        private IOperationProcessor processor;
        private IOperationProvider provider;
        private bool busy;
        private Action<int> reportProgress;

        public RunOperationsCommand(
            IEventAggregator eventAggregator, 
            IConverterOptions options, 
            IOperationProcessor processor, 
            IOperationProvider provider,
            Action<int> reportProgress)
            : base(eventAggregator)
        {
            this.options = options;
            this.processor = processor;
            this.provider = provider;
            this.reportProgress = reportProgress;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return !this.busy || this.provider.Operations.All(operation => operation.CanRun());
        }

        protected override void OnExecute(object parameter)
        {
            this.busy = true;
            var progressIndicator = new Progress<int>(this.reportProgress);
            this.processor.ProcessQueue(this.provider.Operations, progressIndicator);
            this.busy = false;
        }
    }
}