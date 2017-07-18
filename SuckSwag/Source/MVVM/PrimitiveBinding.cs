namespace SuckSwag.Source.Mvvm
{
    /// <summary>
    /// Display class to allow MVVM binding for ObservableCollection of primitive types, which is normally not allowed.
    /// </summary>
    /// <typeparam name="T">The primitive type.</typeparam>
    public class PrimitiveBinding<T> where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveBinding{T}" /> class.
        /// </summary>
        /// <param name="value">The primitive value.</param>
        public PrimitiveBinding(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the primitive value.
        /// </summary>
        public T Value { get; set; }
    }
    //// End class
}
//// End namespace