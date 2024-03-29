﻿using DragonSpark.Model.Selection;

namespace Scrutor.Decorate.Model;

public class Select<TIn, TOut> : ISelect<TIn, TOut>
{
	readonly Func<TIn, TOut> _source;

	public Select(ISelect<TIn, TOut> select) : this(select.Get) {}

	public Select(Func<TIn, TOut> select) => _source = select;

	public TOut Get(TIn parameter) => _source(parameter);
}