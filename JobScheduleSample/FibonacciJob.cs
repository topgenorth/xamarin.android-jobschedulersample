using System;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Util;

namespace JobScheduleSample
{
    [Service(Name = "JobScheduleSample.FibonacciJob", Permission = "android.permission.BIND_JOB_SERVICE")]
    public class FibonacciJob : JobService
    {
        public static readonly string TAG = typeof(FibonacciJob).FullName;
        long fibonacciValue;
        SimpleFibonacciCalculatorTask calculator;
        JobParameters parameters;

        public FibonacciJob()
        {
        }

        public override bool OnStartJob(JobParameters @params)
        {
            fibonacciValue = @params.Extras.GetLong(JobSchedulerHelpers.FibonacciValueKey, -1);
            if (fibonacciValue < 0)
            {
                Log.Debug(TAG, "Invalid value - must be > 0.");
                return false;
            }

            parameters = @params;
            calculator = new SimpleFibonacciCalculatorTask(this);

            calculator.Execute(fibonacciValue);
            return true; // No more work to do!
        }

        public override bool OnStopJob(JobParameters @params)
        {
            Log.Debug(TAG, "System halted the job.");
            if (calculator != null && !calculator.IsCancelled)
            {
                calculator.Cancel(true);
            }
            calculator = null;
            return false; // Don't reschedule the job.
        }


        class SimpleFibonacciCalculatorTask : AsyncTask<long, Java.Lang.Void, long>
        {
            readonly FibonacciJob jobService;
            long fibonacciValue = -1;

            public SimpleFibonacciCalculatorTask(FibonacciJob jobService)
            {
                this.jobService = jobService;
            }

            long GetFibonacciFor(long value)
            {
                if (value == 0) 
                {
                    return 0;
                }
                if ((value == 1) || (value == 2))
                {
                    return 1;
                }

                long result = 0;
                long n1 = 0;
                long n2 = 1;
                for (long i = 2; i <= value; i++)
                {
                    result = n1 + n2;
                    n1 = n2;
                    n2 = result;
                }

                return result;
            }

            protected override long RunInBackground(params long[] @params)
            {
                fibonacciValue = -1;
                long value = @params[0];
                return GetFibonacciFor(value);
            }

            protected override void OnPostExecute(long result)
            {

                base.OnPostExecute(result);

                fibonacciValue = result;

                BroadcastResults(result);

                jobService.JobFinished(jobService.parameters, false);

                Log.Debug(TAG, "Finished with fibonacci calculation: " + result);

            }

            void BroadcastResults(long result)
            {

                Context c = jobService.BaseContext;

                Intent i = new Intent(JobSchedulerHelpers.FibonacciJobActionKey);
                i.PutExtra(JobSchedulerHelpers.FibonacciResultKey, result);
                jobService.BaseContext.SendBroadcast(i);
            }
        }
    }
}