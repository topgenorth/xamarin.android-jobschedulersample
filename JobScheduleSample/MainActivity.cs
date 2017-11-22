using System;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace JobScheduleSample
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public partial class MainActivity : Activity
    {
        static readonly string TAG = typeof(MainActivity).FullName;
        JobScheduler jobScheduler;
        FibonacciResultReciever receiver;
        TextView resultsTextView;
        EditText inputEditText;
        Button calculateButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            receiver = new FibonacciResultReciever(this);

            jobScheduler = (JobScheduler)GetSystemService(JobSchedulerService);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);


            resultsTextView = FindViewById<TextView>(Resource.Id.results_textview);
            inputEditText = FindViewById<EditText>(Resource.Id.fibonacci_start_value);

            calculateButton = FindViewById<Button>(Resource.Id.download_button);
            calculateButton.Click += ScheduleFibonacciCalculation;
        }

        protected override void OnResume()
        {
            base.OnResume();
            BaseContext.RegisterReceiver(receiver, new IntentFilter(JobSchedulerHelpers.FibonacciResultKey));

            IntentFilter filter = new IntentFilter();
            filter.AddAction(JobSchedulerHelpers.FibonacciJobActionKey);
            RegisterReceiver(receiver, filter);
        }

        protected override void OnPause()
        {
            BaseContext.UnregisterReceiver(receiver);
            base.OnPause();
        }

        void ScheduleFibonacciCalculation(object sender, EventArgs eventArgs)
        {
            int value = Int32.Parse(inputEditText.Text);

            JobInfo.Builder builder = this.CreateJobInfoBuilderForFibonnaciCalculation(value)
                .SetPersisted(false)
                .SetMinimumLatency(1000)    // Wait at least 1 second
                .SetOverrideDeadline(5000)  // But no longer than 5 seconds
                .SetRequiredNetworkType(NetworkType.Unmetered);

            int result = jobScheduler.Schedule(builder.Build());
            if (result == JobScheduler.ResultSuccess)
            {
                calculateButton.Enabled = false;
                resultsTextView.SetText(Resource.String.fibonacci_calculation_in_progress);
                Log.Debug(TAG, "Job started!");
            }
            else
            {
                Log.Warn(TAG, "Problem starting the job " + result);
            }
        }

    }
}