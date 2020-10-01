using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using System.Configuration;
using WorkflowManager.Tasks;

namespace WorkflowManager
{
    public static class WorkflowHandler
    {
        private static ConcurrentBag<Tuple<string, Exception>> _threadExceptions;
        private static EventWaitHandle _eventWaitHandle;
        private static long _remainingThreads;
        private static long _blockedThreads;

        
        static void Main(string[] args)
        {
            
            Console.WriteLine("WORFLOW MANAGER IS RUNNING... \n");
            Console.WriteLine("TYPE 'e' AND HIT RETURN KEY TO EXIT... \n");
            Console.WriteLine("PLEASE PROVIDE FULL FILE PATH OF INPUT: \n");
           

            string userInput = Console.ReadLine();

            while (!userInput.Equals("E", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string inputText = string.Empty;
                    using (StreamReader streamReader = new StreamReader(userInput))
                    {
                        inputText = streamReader.ReadToEnd();
                    }

                    Execute(WorkflowParser.ParseInput(inputText));
                }

                catch (Exception ex)
                {
                    Console.WriteLine("EXCEPTION OCCURED: " + ex.Message.ToString() + "\n");
                }

                Console.WriteLine("OPERATION COMPLETE... \n");
                Console.WriteLine("PLEASE PROVIDE FULL FILE PATH OF INPUT: \n");
                userInput = Console.ReadLine();
            }
        }
   
        public static void Execute(Workflow workflow)
        {
            using (_eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                _threadExceptions = new ConcurrentBag<Tuple<string, Exception>>();
                _remainingThreads = 0;
                _blockedThreads = 0;
                FileManager.InitialiseOutputDirectory();

                var threads = new List<Thread>();

                _remainingThreads = Convert.ToInt64(workflow.Sequences.Count);

                Console.WriteLine("\nStarting Workflow -> " + workflow.Name + "...");

                foreach (var sequence in workflow.Sequences)
                {

                    Thread t = new Thread(new ParameterizedThreadStart(x => executeSequence(sequence)));
                    threads.Add(t);
                    t.Start();
                }

                foreach (Thread thread in threads)
                {
                    thread.Join();
                }

                if (_threadExceptions.Count > 0)
                {
                    Console.WriteLine("All Sequences have been executed but there were exceptions during execution. Please see below... \n");
                    foreach (var tuple in _threadExceptions)
                    {
                        Console.WriteLine("Exception -> " + tuple.Item1 + "-> " + tuple.Item2 + " \n");
                    }

                }

                else
                {
                    Console.WriteLine("All Sequences have been executed sucessfully... \n");
                }
            }
        }


        static void executeSequence(WorkflowExecutionSequence sequence)
        {
            try
            {
                WorkflowState workflowState = new WorkflowState();
                workflowState.ExecutionSequence = sequence.Name;
                workflowState.SyncTriggered += synch;

                foreach (var task in sequence.Tasks)
                {
                    task.Execute(ref workflowState);
                }
            }

            catch (Exception ex)
            {
                _threadExceptions.Add(Tuple.Create(sequence.Name, ex));
            }

            Console.WriteLine(sequence.Name + " is finished...");
            Interlocked.Decrement(ref _remainingThreads);

            if (_blockedThreads == _remainingThreads)
            {
                _eventWaitHandle.Set();
            }


        }

        static void synch(object sender, SynchEventArgs e)
        {
            Interlocked.Increment(ref _blockedThreads);

            if (_blockedThreads == _remainingThreads)
            {
                _eventWaitHandle.Set();
            }

            Console.WriteLine(e.CurrentSequence + " thread is waiting...");
            _eventWaitHandle.WaitOne();
            Console.WriteLine(e.CurrentSequence + " thread is released...");
            Interlocked.Decrement(ref _blockedThreads);
        }
       
        }
    }

