namespace Thavyra.Rest.Services;

public abstract class SlugResult;
public abstract class SlugResult<T> : SlugResult
{
    public abstract void Deconstruct(out SlugResult slugResult, out T? result);
}

public class SlugFoundResult<T> : SlugResult<T>
{
    public SlugFoundResult(T result)
    {
        Result = result;
    }

    public T Result { get; }
    
    public override void Deconstruct(out SlugResult slugResult, out T? result)
    {
        slugResult = this;
        result = Result;
    }
}

public class SlugNotFound : SlugResult;

public class SlugNotFound<T> : SlugResult<T>
{
    public override void Deconstruct(out SlugResult slugResult, out T? result)
    {
        slugResult = this;
        result = default;
    }
}

public class SlugClaimMissing : SlugResult;

public class SlugClaimMissing<T> : SlugResult<T>
{
    public override void Deconstruct(out SlugResult slugResult, out T? result)
    {
        slugResult = this;
        result = default;
    }
}

public class SlugInvalid : SlugResult;

public class SlugInvalid<T> : SlugResult<T>
{
    public override void Deconstruct(out SlugResult slugResult, out T? result)
    {
        slugResult = this;
        result = default;
    }
}