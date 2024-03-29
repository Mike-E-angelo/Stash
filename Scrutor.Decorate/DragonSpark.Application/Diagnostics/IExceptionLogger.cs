﻿using DragonSpark.Model.Operations;
using System;

namespace DragonSpark.Application.Diagnostics;

public interface IExceptionLogger : ISelecting<ExceptionInput, Exception> {}