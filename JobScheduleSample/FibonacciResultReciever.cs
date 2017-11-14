using Android.Content;
using Android.Util;

namespace JobScheduleSample
{
    public partial class MainActivity
    {
        [BroadcastReceiver(Enabled = true, Exported = false)]
        protected internal class FibonacciResultReciever : BroadcastReceiver
        {
            MainActivity activity;

            public FibonacciResultReciever()
            {
                Log.Debug(TAG, "FibonacciResultReceiver");
            }

            public FibonacciResultReciever(MainActivity activity)
            {
                Log.Debug(TAG, "FibonacciResultReceiver(MainActivity)");
                this.activity = activity;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                Log.Debug(TAG, "Received broadcast");

                if (activity == null)
                {
                    Log.Warn(TAG, "There is no activity, ignoring the results.");
                }
                else
                {
                    long result = intent.Extras.GetLong(JobSchedulerHelpers.FibonacciResultKey, -1);
                    if (result > -1)
                    {
                        activity.resultsTextView.Text = activity.Resources.GetString(Resource.String.fibonacci_calculation_result, result);;
                    }
                    else
                    {
                        activity.resultsTextView.SetText(Resource.String.fibonacci_calculation_problem);
                    }
                    activity.calculateButton.Enabled = true;
                }
            }
        }
    }
}