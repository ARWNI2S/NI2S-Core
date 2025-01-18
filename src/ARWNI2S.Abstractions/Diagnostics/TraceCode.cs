namespace ARWNI2S.Diagnostics
{
    internal enum TraceCode : int
    {
        LifecycleStagesReport,

        LifecycleStartFailure,
        LifecycleStopFailure,

        StartPerformanceMeasure,
        StopPerformanceMeasure,

        HeartbeatError,
        NodeRuntime_Error_100325,
    }
}
