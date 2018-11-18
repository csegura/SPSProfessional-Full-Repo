using System;
using System.Threading;
using System.Windows.Forms;

namespace SPSProfessional.ActionDataBase.Generator
{
    /// <summary>
    /// Summary description for WorkerClass.
    /// </summary>
    public class WorkerClass
    {
        /// <summary>
        /// Usually a form or a winform control that implements "Invoke/BeginInvode"
        /// </summary>
        private ContainerControl _sender = null;

        /// <summary>
        /// The delegate method (callback) on the sender to call
        /// </summary>
        private Delegate _senderDelegate = null;

        /// <summary>
        /// Messages sent in - this is implementation specific
        /// </summary>
        private int _totalMessages = 0;

        /// <summary>
        /// Constructor used by caller using ThreadPool
        /// </summary>
        public WorkerClass()
        {
        }

        /// <summary>
        /// Constructor called by calle using ThreadPool OR ThreadStart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="totalMessages"></param>
        /// <param name="senderDelegate"></param>
        public WorkerClass(ContainerControl sender, Delegate senderDelegate, int totalMessages)
        {
            _sender = sender;
            _senderDelegate = senderDelegate;
            _totalMessages = totalMessages;
        }

        /// <summary>
        /// Another constructor using the params array pattern. Used by ThreadPool or ThreadStart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="senderDelegate"></param>
        /// <param name="list"></param>
        public WorkerClass(ContainerControl sender, Delegate senderDelegate, params object[] list)
        {
            _sender = sender;
            _senderDelegate = senderDelegate;
            _totalMessages = (int) list.GetValue(0);
        }

        /// <summary>
        /// Method for ThreadPool QueueWorkerItem
        /// </summary>
        /// <param name="obj"></param>
        public void RunProcess(object obj)
        {
            Thread.CurrentThread.IsBackground = true; //make them a daemon
            object[] objArray = (object[]) obj;
            _sender = (Form) objArray[0];
            _senderDelegate = (Delegate) objArray[1];
            _totalMessages = (int) objArray[2];

            LocalRunProcess();
        }

        /// <summary>
        /// Method for ThreadStart delegate
        /// </summary>
        public void RunProcess()
        {
            Thread.CurrentThread.IsBackground = true; //make them a daemon
            LocalRunProcess();
        }

        /// <summary>
        /// Local Method for the actual work.
        /// </summary>
        private void LocalRunProcess()
        {
            int i = 0;
            for (; i < _totalMessages; i++)
            {
                Thread.Sleep(50);
                _sender.BeginInvoke(_senderDelegate, new object[] {_totalMessages, i, false});
            }
            _sender.BeginInvoke(_senderDelegate, new object[] {_totalMessages, i, true});
        }
    }
}