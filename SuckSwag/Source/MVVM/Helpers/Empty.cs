namespace SuckSwag.Source.Mvvm.Helpers
{
    using System.Threading.Tasks;

    /// <summary>
    /// Helper class used when an async method is required, but the context is synchronous.
    /// </summary>
    public static class Empty
    {
        /// <summary>
        /// An empty task object.
        /// </summary>
        private static readonly Task ConcreteTask = new Task(() => { });

        /// <summary>
        /// Gets the empty task.
        /// </summary>
        public static Task Task
        {
            get
            {
                return ConcreteTask;
            }
        }
    }
    //// End class
}
//// End namespace