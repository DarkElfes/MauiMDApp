namespace Results;
public static class ResultExtensions
{
    
    /// <summary>
    /// Executes a function on the value of a successful result, returning a new result based on that function's output.
    /// If the original result is a failure, it returns a failure result.
    /// </summary>
    /// <typeparam name="TIn">Represents the type of the value contained in the original result.</typeparam>
    /// <typeparam name="TOut">Represents the type of the value contained in the new result after applying the function.</typeparam>
    /// <param name="result">The original result that may contain a value or an error.</param>
    /// <param name="bind">A function that transforms the value of the original result into a new result.</param>
    /// <returns>A new result that is either the result of the function or a failure based on the original result.</returns>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bind)
        => result.IsSuccess
            ? bind(result.Value)
            : Result.Failure<TOut>(result.Error);




    public static Result<TIn> Ensure<TIn>(this Result<TIn> result, Func<TIn, Result> predicate, Error? error = null)
    {
        if (result.IsSuccess && predicate(result.Value) is { IsFailure: true } predicateResult)
            return predicateResult.Error;
        return result;
    }

    public static Result<TOut> TryCatch<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func, Error? error = null)
    {
        try
        {
            return result.IsSuccess
                ? Result.Success(func(result.Value))
                : Result.Failure<TOut>(result.Error);
        }
        catch(Exception e)
        {
            return Result.Failure<TOut>(error ?? new Error(e.Message));
        }
    }

    public static Result<TIn> Tap<TIn>(this Result<TIn> result, Action<TIn> action)
    {
        if (result.IsSuccess)
            action(result.Value);
        return result;
    }



    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Error, TOut> onFailure) 
    {
        return result.IsSuccess
            ? onSuccess(result.Value) 
            : onFailure(result.Error);
    }
}
