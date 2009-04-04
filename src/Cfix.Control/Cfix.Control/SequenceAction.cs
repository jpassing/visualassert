//using System;
//using System.Diagnostics;
//using System.Collections.Generic;
//using Cfixctl;

//namespace Cfix.Control
//{
//    /// <summary>
//    /// Allows multiple actions to be run in sequence.
//    /// 
//    /// Threadsafe.
//    /// </summary>
//    internal class SequenceAction : IAction
//    {
//        //
//        // Direct ancestor for all actions.
//        //
//        private readonly ITestItem item;
//        private readonly IResultItem result;
//        private readonly IList<IAction> actions;

//        private volatile bool started = false;

//        //
//        // Once stop is true, no further sub-action may be run.
//        //
//        private volatile bool stopped = false;
//        private volatile IAction currentAction = null;

//        public SequenceAction(
//            ITestItem item,
//            IList<IAction> actions
//            )
//        {
//            Debug.Assert( item != null );
//            Debug.Assert( actions != null );

//            this.actions = actions;
//            this.item = item;

//            this.result = item.CreateResultItem(
//                parentResult,
//                events,
//                ExecutionStatus.Pending );
//        }

//        /*----------------------------------------------------------------------
//         * IAction.
//         */

//        public IResultItem Result
//        {
//            get { return this.result; }
//        }

//        public ITestItem Item
//        {
//            get { return this.item; }
//        }
		
//        public void Run()
//        {
//            if ( this.started )
//            {
//                throw new InvalidOperationException( "Already started" );
//            }

//            this.started = true;

//            foreach ( IAction action in this.actions )
//            {
//                if ( stopped )
//                {
//                    break;
//                }

//                //
//                // Save reference to current action to allow stopping.
//                //
//                this.currentAction = action;

//                action.Run();
//            }
//        }

//        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.InvalidOperationException.#ctor(System.String)" )]
//        public void Stop()
//        {
//            if ( !this.started )
//            {
//                throw new InvalidOperationException( "Not started" );
//            }

//            this.stopped = true;

//            IAction action = this.currentAction;
//            if ( action != null )
//            {
//                action.Stop();
//            }
//        }

//        public void Dispose()
//        {
//            foreach ( IAction action in this.actions )
//            {
//                action.Dispose();
//            }
//        }
//    }
//}
