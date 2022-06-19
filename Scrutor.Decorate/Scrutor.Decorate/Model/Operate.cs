namespace Scrutor.Decorate.Model;

public delegate ValueTask<TOut> Operate<in TIn, TOut>(TIn parameter);