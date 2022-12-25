using Hangfire;
using System.Linq.Expressions;

namespace User_Infrastructure.Interface
{
    public class BackgroundJob : IBackgroundJob
    {
        private readonly IBackgroundJobClient _backgroundClient;

        public BackgroundJob(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundClient = backgroundJobClient;
        }

        public string AddEnque(Expression<Action> methodCall)
        {
            return _backgroundClient.Enqueue(methodCall);
        }

        public string AddEnque<T>(Expression<Action<T>> methodCall)
        {
            return _backgroundClient.Enqueue<T>(methodCall);
        }

        public string AddContinuations(Expression<Action> methodCall, string jobid)
        {
            return _backgroundClient.ContinueJobWith(jobid, methodCall);
        }

        public string AddContinuations<T>(Expression<Action<T>> methodCall, string jobid)
        {
            return _backgroundClient.ContinueJobWith<T>(jobid, methodCall);
        }

        public string AddSchedule(Expression<Action> methodCall, RecuringTime recuringTime, double time)
        {
            return recuringTime switch
            {
                RecuringTime.Milliseconds => _backgroundClient.Schedule(methodCall, TimeSpan.FromMilliseconds(time)),
                RecuringTime.Seconds => _backgroundClient.Schedule(methodCall, TimeSpan.FromSeconds(time)),
                RecuringTime.Minutes => _backgroundClient.Schedule(methodCall, TimeSpan.FromMinutes(time)),
                RecuringTime.Hours => _backgroundClient.Schedule(methodCall, TimeSpan.FromHours(time)),
                RecuringTime.Day => _backgroundClient.Schedule(methodCall, TimeSpan.FromDays(time)),
                _ => _backgroundClient.Schedule(methodCall, TimeSpan.FromMinutes(time)),
            };
        }

        public string AddSchedule<T>(Expression<Action<T>> methodCall, RecuringTime recuringTime, double time)
        {
            return recuringTime switch
            {
                RecuringTime.Milliseconds => _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromMilliseconds(time)),
                RecuringTime.Seconds => _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromSeconds(time)),
                RecuringTime.Minutes => _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromMinutes(time)),
                RecuringTime.Hours => _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromHours(time)),
                RecuringTime.Day => _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromDays(time)),
                _ => _backgroundClient.Schedule<T>(methodCall, TimeSpan.FromMinutes(time)),
            };
        }
    }
}