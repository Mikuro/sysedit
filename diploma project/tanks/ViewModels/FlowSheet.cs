using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xaml;
using System.IO;
using System.Xml;
using System.Windows.Threading;

using tanks.Models.Solver;
using System.Diagnostics;

namespace tanks.ViewModels
{
    public class FlowSheet : BaseSheetVM
    {
        public FlowDiagram flowDiagram { get; set; }
        public RuntimeData runtimeData { get; set; }

        public override void Stop()
        {
        }
        public override void Done()
        {
            //Task.Delay(1000).Wait();
        }
        public override void Initialize()
        {
        }

        void UpdateDisplay()
        {
            foreach (var item in flowDiagram.Items)
            {
                item.UpdateDisplay();
            }
        }

        private DispatcherTimer dispatcherTimer;
        private object locker = new object();
        Task taskToWait = null;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            bool acquiredLock = false;

            try
            {
                Monitor.Enter(locker, ref acquiredLock);

                Debug.Assert(taskToWait != null);

                var status = taskToWait.Status;

                if (status == TaskStatus.RanToCompletion)
                {
                    //var nonUiTask1 = Task.Factory.StartNew(() => { Task.Delay(100).Wait(); });
                    var uiTask2 =
                    //Task.Factory.ContinueWhenAll<Models.FlowDiagram>(new Task[1] { nonUiTask1 },
                    Task.Factory.StartNew<Models.FlowDiagram>(() =>
                    //(a) =>
                    {
                        return tanks.Models.DTOUtil.CreateFromFlowDiagram(flowDiagram);
                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

                    var nonUiTask3 = Task.Factory.ContinueWhenAll<Models.FlowDiagram>(new Task[1] { uiTask2 },
                        (a) =>
                        {
                            Debug.Assert(a[0].Status == TaskStatus.RanToCompletion);
                            Models.FlowDiagram fd = (a[0] as Task<Models.FlowDiagram>).Result;
                            SRKSolver.ProcessLinks(fd);
                            Flash.funcnt = 0;
                            Flash.fugcnt = 0;
                            SRKSolver.SolveStatic(fd);
                            DCSSolver.SolveInstrument(fd);
                            DCSSolver.OneStep(fd);
                            return fd;
                        });

                    var uiTask4 = Task.Factory.ContinueWhenAll(new Task[1] { nonUiTask3 },
                        (a) =>
                        {
                            Debug.Assert(a[0].Status == TaskStatus.RanToCompletion);
                            Models.FlowDiagram fd = (a[0] as Task<Models.FlowDiagram>).Result;
                            tanks.Models.DTOUtil.UpdateFlowDiagram(flowDiagram, fd);
                            runtimeData.UpdateData(flowDiagram);
                            UpdateDisplay();
                        }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

                    taskToWait = uiTask4;
                }
                else
                {
                    Debug.WriteLine(String.Format("task status {0}",status));
                }
            }
            finally
            {
                if (acquiredLock) Monitor.Exit(locker);
            }
        }

        public FlowSheet()
        {
            runtimeData = new RuntimeData();
            SheetContentTypeValue = SheetContentType.FlowDiagram;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            bool acquiredLock = false;

            try
            {
                Monitor.Enter(locker, ref acquiredLock);

                //var nonUiTask1 = Task.Factory.StartNew(() => { Task.Delay(100).Wait(); });
                var uiTask2 = Task.Factory.StartNew<Models.FlowDiagram>(() =>
                //Task.Factory.ContinueWhenAll<Models.FlowDiagram>(new Task[1] { nonUiTask1 },
                //(a) =>
                {
                    flowDiagram.Fix();
                    return tanks.Models.DTOUtil.CreateFromFlowDiagram(flowDiagram);
                }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

                var nonUiTask3 = Task.Factory.ContinueWhenAll<Models.FlowDiagram>(new Task[1] { uiTask2 },
                    (a) =>
                    {
                        Debug.Assert(a[0].Status == TaskStatus.RanToCompletion);
                        Models.FlowDiagram fd = (a[0] as Task<Models.FlowDiagram>).Result;
                        SRKSolver.ProcessLinks(fd);
                        Flash.funcnt = 0;
                        Flash.fugcnt = 0;
                        SRKSolver.SolveStatic(fd);
                        DCSSolver.SolveInstrument(fd);
                        return fd;
                    });

                var uiTask4 = Task.Factory.ContinueWhenAll(new Task[1] { nonUiTask3 },
                    (a) =>
                    {
                        Debug.Assert(a[0].Status == TaskStatus.RanToCompletion);
                        Models.FlowDiagram fd = (a[0] as Task<Models.FlowDiagram>).Result;
                        tanks.Models.DTOUtil.UpdateFlowDiagram(flowDiagram, fd);
                        runtimeData.buildData(flowDiagram);
                        UpdateDisplay();
                    }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

                taskToWait = uiTask4;
            }
            finally
            {
                if (acquiredLock) Monitor.Exit(locker);
                else
                {
                    Debug.Assert(false);
                }
            }
        }
    }
}
