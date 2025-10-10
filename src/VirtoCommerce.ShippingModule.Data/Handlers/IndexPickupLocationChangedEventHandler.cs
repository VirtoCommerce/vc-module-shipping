using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.BackgroundJobs;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Events;
using VirtoCommerce.ShippingModule.Core.Extensions;
using VirtoCommerce.ShippingModule.Data.Search.Indexed;

namespace VirtoCommerce.ShippingModule.Data.Handlers;

public class IndexPickupLocationChangedEventHandler(
    ISettingsManager settingsManager,
    IConfiguration configuration,
    IIndexingJobService indexingJobService,
    IEnumerable<IndexDocumentConfiguration> indexingConfigurations)
    : IEventHandler<PickupLocationChangedEvent>
{
    public async Task Handle(PickupLocationChangedEvent message)
    {
        if (!configuration.IsPickupLocationFullTextSearchEnabled() || !await settingsManager.GetValueAsync<bool>(ModuleConstants.Settings.EventBasedIndexation))
        {
            return;
        }

        var indexEntries = message?.ChangedEntries
            .Select(x => new IndexEntry { Id = x.OldEntry.Id, EntryState = x.EntryState, Type = ModuleConstants.PickupLocationIndexDocumentType })
            .ToArray() ?? Array.Empty<IndexEntry>();

        indexingJobService.EnqueueIndexAndDeleteDocuments(indexEntries, JobPriority.Normal, indexingConfigurations.GetDocumentBuilders(ModuleConstants.PickupLocationIndexDocumentType, typeof(PickupLocationChangesProvider)).ToList());
    }
}
