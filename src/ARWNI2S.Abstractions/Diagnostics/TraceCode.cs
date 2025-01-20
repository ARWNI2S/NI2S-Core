namespace ARWNI2S.Diagnostics
{
    public enum TraceCode : int
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
