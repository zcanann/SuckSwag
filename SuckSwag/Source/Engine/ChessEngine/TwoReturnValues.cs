namespace SuckSwag.Source.ChessEngine
{
    /// <summary>
    /// A small helper class that makes it possible to return two values from a function.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class TwoReturnValues<T1, T2>
    {
        public T1 first;
        public T2 second;

        public TwoReturnValues(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }
    }
    //// End class
}
//// End namespace