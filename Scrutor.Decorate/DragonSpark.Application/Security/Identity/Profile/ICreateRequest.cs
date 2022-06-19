﻿using DragonSpark.Model.Operations;
using Microsoft.AspNetCore.Identity;

namespace DragonSpark.Application.Security.Identity.Profile;

public interface ICreateRequest : ISelecting<ExternalLoginInfo, IdentityResult> {}