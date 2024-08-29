#define CRASH_SOURCE

namespace ChartTools.IO;

internal abstract class FileParser<T>
{
    public bool ResultReady { get; private set; }
    public abstract object? Result { get; }

    public async Task StartAsyncParse(IEnumerable<T> items)
    {
        await Task.Run(() => ParseBase(items));

#if CRASH_SOURCE
        FinaliseParse();
#else
        try { FinaliseParse(); }
        catch (Exception e) { throw GetFinalizeException(e); }
#endif
    }
    public void Parse(IEnumerable<T> items)
    {
        ParseBase(items);

#if CRASH_SOURCE
        FinaliseParse();
#else
        try { FinaliseParse(); }
        catch (Exception e) { throw GetFinalizeException(e); }
#endif
    }
    private void ParseBase(IEnumerable<T> items)
    {
        foreach (var item in items)
#if CRASH_SOURCE
            HandleItem(item);
#else
            try { HandleItem(item); }
            catch (Exception e) { throw GetHandleException(item, e); }
#endif
    }

    protected abstract void HandleItem(T item);

    protected virtual void FinaliseParse() => ResultReady = true;

    protected TResult GetResult<TResult>(TResult result) => ResultReady ? result : throw new Exception("Result is not ready.");

    protected abstract Exception GetHandleException(T item, Exception innerException);
    protected abstract Exception GetFinalizeException(Exception innerException);
}
