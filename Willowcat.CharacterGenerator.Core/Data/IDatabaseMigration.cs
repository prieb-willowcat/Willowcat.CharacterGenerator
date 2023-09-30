﻿using System.Threading;
using System.Threading.Tasks;

namespace Willowcat.CharacterGenerator.Core.Data
{
    public interface IDatabaseMigration<T>
    {
        Task BringDownAsync(T context, CancellationToken cancellationToken);
        Task<bool> BringUpAsync(T context, CancellationToken cancellationToken);
    }
}
