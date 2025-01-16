namespace ARWNI2S.Context
{
    internal class WorkingContextAccessor : IWorkingContextAccessor
    {
        private static readonly AsyncLocal<WorkingContextHolder> _contextCurrent = new();

        /// <inheritdoc/>
        public IWorkingContext WorkingContext
        {
            get
            {
                return _contextCurrent.Value?.Context;
            }
            set
            {
                var holder = _contextCurrent.Value;
                if (holder != null)
                {
                    // Clear current HttpContext trapped in the AsyncLocals, as its done.
                    holder.Context = null;
                }

                if (value != null)
                {
                    // Use an object indirection to hold the HttpContext in the AsyncLocal,
                    // so it can be cleared in all ExecutionContexts when its cleared.
                    _contextCurrent.Value = new WorkingContextHolder { Context = value };
                }
            }
        }

        private sealed class WorkingContextHolder
        {
            public IWorkingContext Context;
        }
    }
}
