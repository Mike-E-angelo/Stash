using DragonSpark.Application.Entities.Design;
using DragonSpark.Compose;
using DragonSpark.Model.Commands;
using EfCore6.Model.Entities;
using JetBrains.Annotations;
using System;

namespace EfCore6.Model.Design
{
	[UsedImplicitly]
	sealed class StorageBuilder : StorageBuilder<ApplicationState>
	{
		public StorageBuilder() : this(A.Type<StorageBuilder>()) {}

		public StorageBuilder(Type owner)
			: base(SqlServerConfigurations<ApplicationState>.Default.Get(owner), ApplicationStates.Default.Get) {}
	}
}