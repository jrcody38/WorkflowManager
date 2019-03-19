using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using System.Configuration;

namespace WorkflowManager
{
   public class WorkflowHandler
    {
        private ConcurrentBag<Tuple<string, Exception>> _threadExceptions;
        private static EventWaitHandle _eventWaitHandle;
        private static long _remainingThreads;
        private static long _blockedThreads;
       
        public WorkflowHandler()
        {
            flush();
        }

        void flush()
        {
            setUpOutputDirectory();
            _threadExceptions = new ConcurrentBag<Tuple<string, Exception>>();
            _remainingThreads = 0;
            _blockedThreads = 0;
            
        }
        public void Execute(Workflow workflow)
        {
           
            using (_eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset))
            {

                var threads = new List<Thread>();

                _remainingThreads = Convert.ToInt64(workflow.Sequences.Count);

                Console.WriteLine("Starting Workflow -> " + workflow.Name + "...");

                foreach (var sequence in workflow.Sequences)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(x => executeSequences(sequence)));
                    threads.Add(t);
                    t.Start();
                }

                foreach (Thread thread in threads)
                {
                    thread.Join();
                }

                if (_threadExceptions.Count > 0)
                {
                    Console.WriteLine("All Sequences have been executed but there were exceptions during execution. Please see below. Press any key to terminate");
                    foreach (var tuple in _threadExceptions)
                    {
                        Console.WriteLine("EXCEPTION-> " + tuple.Item1 + "-> " + tuple.Item2);
                    }
                }

                else
                {
                    Console.WriteLine("All Sequences have been executed sucessfully. Press any key to terminate");
                }
            }
        }

      
         void executeSequences(ExecutionSequence sequence)
        {
            try
            {
                foreach (var task in sequence.Tasks)
                {
                   
                    if (task.Name.ToLower() == Enums.Commands.ReadFile.ToString().ToLower())
                    {
                        sequence.CurrentText = FileManager.ReadFile((string)task.Parameter);
                    }

                    else if (task.Name.ToLower() == Enums.Commands.WriteFile.ToString().ToLower())
                    {
                        FileManager.WriteFile(sequence.CurrentText, sequence.Name + "_"  + (string)task.Parameter);
                    }

                    else if (task.Name.ToLower() == Enums.Commands.Grep.ToString().ToLower())
                    {
                        sequence.CurrentText = FileManager.Grep(sequence.CurrentText, (string)task.Parameter);
                    }

                    else if (task.Name.ToLower() == Enums.Commands.Sort.ToString().ToLower())
                    {
                        sequence.CurrentText = FileManager.Sort(sequence.CurrentText);
                    }

                    else if (task.Name.ToLower() == Enums.Commands.Replace.ToString().ToLower())
                    {
                        var paramsArr = task.Parameter.ToString().Split(' ');
                        sequence.CurrentText = FileManager.Replace(sequence.CurrentText, paramsArr[0], paramsArr[1]);
                    }

                    else if (task.Name.ToLower() == Enums.Commands.Synch.ToString().ToLower())
                    {
                        synch(sequence);
                    }
                }

               
            }

            catch(Exception ex)
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


        void synch(ExecutionSequence sequence)
        {
            Interlocked.Increment(ref _blockedThreads);

            if (_blockedThreads == _remainingThreads)
            {
                _eventWaitHandle.Set();
            }
            
            Console.WriteLine(sequence.Name + " thread is waiting...");
            _eventWaitHandle.WaitOne();
            Console.WriteLine(sequence.Name + " thread is released...");
            Interlocked.Decrement(ref _blockedThreads);

        }


        void setUpOutputDirectory()
        {
            var outputFolder = AppDomain.CurrentDomain.BaseDirectory + "\\" + ConfigurationManager.AppSettings["OutputFolder"];

            if (Directory.Exists(outputFolder))
            {
                purgeDirectory(outputFolder);
            }

            else
            {
                System.IO.Directory.CreateDirectory(outputFolder);
            }

        }

        void purgeDirectory(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);


            for (int x = 0; x < 10; x++)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {

                    try
                    {
                        dir.Delete(true);
                    }

                    catch (Exception)
                    {
                        Thread.Sleep(1);
                        dir.Delete(true);

                    }


                }
            }
        }
    }
}
