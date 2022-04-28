namespace HR.BrightspaceConnector.Utilities
{
    /// <summary>
    /// Provides support for returning both a value and out parameter from an async method for the purpose of implementing the Try-method pattern.
    /// </summary>
    /// <typeparam name="TReturn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <seealso href="https://stackoverflow.com/a/57707700">How to write an async method with out parameter?</seealso>
    public struct AsyncOutResult<TReturn, TOut>
    {
        private readonly TReturn returnValue;
        private readonly TOut result;

        public AsyncOutResult(TReturn returnValue, TOut result)
        {
            this.returnValue = returnValue;
            this.result = result;
        }

        public TReturn Out(out TOut result)
        {
            result = this.result;
            return returnValue;
        }

        public static implicit operator AsyncOutResult<TReturn, TOut>((TReturn ReturnValue, TOut Result) tuple)
            => new(tuple.ReturnValue, tuple.Result);
    }
}
