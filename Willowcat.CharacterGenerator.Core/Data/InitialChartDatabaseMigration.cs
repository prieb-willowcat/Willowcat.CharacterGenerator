using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.Core.Models;

namespace Willowcat.CharacterGenerator.Core.Data
{
    public class InitialChartDatabaseMigration : IDatabaseMigration<ChartContext>
    {
        public Task BringDownAsync(ChartContext chartDatabaseService, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<bool> BringUpAsync(ChartContext context, CancellationToken cancellationToken)
        {
            context.Report(new ChartSetupMessage(this, "Loading charts from resource files"));
            var builder = new ChartCollectionBuilder();
            await builder.AddChartsFromResourcesAsync(context.Configuration, context.ProgressReporter);
            cancellationToken.ThrowIfCancellationRequested();

            bool canContinue = true;

            var parentChartCollections = builder.BuildCollections();
            ClearChartsFromCollections(parentChartCollections);
            if (canContinue)
            {
                canContinue = await InitializeCollections(context, parentChartCollections, cancellationToken);
            }

            var charts = builder.BuildCharts();
            var tags = ExtractTags(charts.Values);
            var options = ExtractOptions(charts.Values);
            if (canContinue)
            {
                await context.SaveChangesAsync();
                canContinue = await InitializeTags(context, tags.Values, cancellationToken);
            }

            if (canContinue)
            {
                await context.SaveChangesAsync();
                canContinue = await InitializeCharts(context, charts.Values, cancellationToken);
            }

            if (canContinue)
            {
                await context.SaveChangesAsync();
                canContinue = await InitializeOptions(context, options, cancellationToken);
            }

            if (canContinue)
            {
                await context.SaveChangesAsync();
            }

            return canContinue;
        }

        private static void ClearChartsFromCollections(IEnumerable<ChartCollectionModel> collections)
        {
            foreach (var collection in collections)
            {
                collection.Charts.Clear();
                ClearChartsFromCollections(collection.SubCollections);
            }
        }

        private async Task<bool> InitializeCollections(ChartContext context, List<ChartCollectionModel> collections, CancellationToken cancellationToken)
        {
            var succeeded = true;
            foreach (var collection in collections)
            {
                var existing = context.ChartCollections.Find(collection.CollectionId);
                if (existing != null)
                {
                    Debug.WriteLine($"Chart {collection.CollectionName} with key {collection.CollectionId} already exists");
                }
                else
                {
                    try
                    {
                        if (collection.ParentCollection != null)
                        {
                            await context.ChartCollections.AddAsync(collection.ParentCollection);
                        }
                        else
                        {
                            await context.ChartCollections.AddAsync(collection);
                        }
                    }
                    catch (Exception ex)
                    {
                        succeeded = false;
                        context.Report(new ChartSetupMessage(this, $"Failed to add {collection.CollectionName}", ex));
                        break;
                    }
                }
            }
            return succeeded;
        }

        private async Task<bool> InitializeCharts(ChartContext context, IEnumerable<ChartModel> charts, CancellationToken cancellationToken)
        {
            var succeeded = true;
            var chartsProcessed = 0;
            var totalCount = charts.Count();
            context.Report(new ChartSetupMessage(this, "Inserting charts into database", chartsProcessed, totalCount));
            foreach (var chart in charts)
            {
                chartsProcessed++;
                var existing = context.Charts.Find(chart.Key);
                if (existing == null && string.IsNullOrEmpty(chart.ParentKey))
                {
                    Debug.WriteLine($"Adding chart {chart.ChartName} ({chart.Key})");
                    foreach (var sub in chart.SubCharts)
                    {
                        Debug.WriteLine($"-- sub chart {sub.ChartName} ({sub.Key}) {sub.SubCharts.Count} more charts");
                    }
                    try
                    {
                        await context.Charts.AddAsync(chart);
                        //await context.SaveChangesAsync();
                        context.Report(new ChartSetupMessage(this, chartsProcessed));
                    }
                    catch (Exception ex)
                    {
                        succeeded = false;
                        context.Report(new ChartSetupMessage(this, $"Failed to add {chart.ChartName} ({chart.Key})", ex));
                        break;
                    }
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
            context.Report(new ChartSetupMessage(this, "Done", totalCount, totalCount));
            return succeeded;
        }

        private async Task<bool> InitializeOptions(ChartContext context, IEnumerable<OptionModel> options, CancellationToken cancellationToken)
        {
            var succeeded = true;
            var optionsProcessed = 0;
            var totalCount = options.Count();
            context.Report(new ChartSetupMessage(this, "Inserting options into database", optionsProcessed, totalCount));
            foreach (var option in options)
            {
                optionsProcessed++;
                var existing = context.ChartOptions.Find(option.OptionId);
                if (existing == null)
                {
                    var chart = context.Charts.Find(option.ChartKey);
                    if (chart == null) 
                    {
                        Debug.WriteLine($"{option.ChartKey}\t{option.ChartKey}\t{option.Range}\t{option.Description}");
                    }
                    if (!string.IsNullOrEmpty(option.GoToChartKey))
                    {
                        var gotoChart = context.Charts.Find(option.GoToChartKey);
                        if (gotoChart == null)
                        {
                            Debug.WriteLine($"{option.GoToChartKey}\t{option.ChartKey}\t{option.Range}\t{option.Description}");
                        }
                    }

                    try
                    {
                        await context.ChartOptions.AddAsync(option);
                        //await context.SaveChangesAsync();
                    }
                    catch (MissingItemException)
                    {
                        // ignore
                    }
                    catch (Exception ex)
                    {
                        succeeded = false;
                        context.Report(new ChartSetupMessage(this, $"Failed to add {option.Range} {option.Description} ({option.ChartKey})", ex));
                        break;
                    }
                }
                context.Report(new ChartSetupMessage(this, optionsProcessed));
                cancellationToken.ThrowIfCancellationRequested();
            }
            context.Report(new ChartSetupMessage(this, "Done", totalCount, totalCount));
            return succeeded;
        }

        private async Task<bool> InitializeTags(ChartContext context, IEnumerable<TagModel> tags, CancellationToken cancellationToken)
        {
            var succeeded = true;
            var tagsProcessed = 0;
            var totalCount = tags.Count();
            context.Report(new ChartSetupMessage(this, "Inserting tags into database", tagsProcessed, totalCount));
            foreach (var tag in tags)
            {
                tagsProcessed++;
                var existing = context.Tags.Find(tag.TagId);
                if (existing == null)
                { 
                    try
                    {
                        await context.Tags.AddAsync(tag);
                    }
                    catch (Exception ex)
                    {
                        succeeded = false;
                        context.Report(new ChartSetupMessage(this, $"Failed to add {tag.Name}", ex));
                        break;
                    }
                }
                context.Report(new ChartSetupMessage(this, tagsProcessed));
                cancellationToken.ThrowIfCancellationRequested();
            }
            context.Report(new ChartSetupMessage(this, "Done", totalCount, totalCount));
            return succeeded;
        }

        private List<OptionModel> ExtractOptions(IEnumerable<ChartModel> charts, List<OptionModel> options = null)
        {
            if (options == null)
            {
                options = new List<OptionModel>();
            }

            foreach (var chart in charts)
            {
                options.AddRange(chart.Options);
                chart.Options.Clear();
                options = ExtractOptions(chart.SubCharts, options);
            }

            return options;
        }

        private Dictionary<string, TagModel> ExtractTags(IEnumerable<ChartModel> charts, Dictionary<string, TagModel> tags = null)
        {
            if (tags == null)
            {
                tags = new Dictionary<string, TagModel>(StringComparer.CurrentCultureIgnoreCase);
            }

            foreach (var chart in charts)
            {
                foreach (var tagName in chart.ParsedTags)
                {
                    TagModel tag = null;
                    if (!tags.TryGetValue(tagName, out tag))
                    {
                        tag = new TagModel()
                        {
                            //TagId = tags.Count,
                            Name = tagName,
                        };
                        tags.Add(tagName, tag);
                    }
                    chart.Tags.Add(tag);
                }
                tags = ExtractTags(chart.SubCharts, tags);
            }

            return tags;
        }
    }
}
