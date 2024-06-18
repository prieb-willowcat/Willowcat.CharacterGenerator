// See https://aka.ms/new-console-template for more information
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Willowcat.CharacterGenerator.ConsoleApp;
using Willowcat.CharacterGenerator.EntityFramework.Extension;
using Willowcat.CharacterGenerator.EntityFramework.Migration;
using Willowcat.CharacterGenerator.FlatFile.Extension;

int result = Parser.Default.ParseArguments<ConsoleArguments>(args)
    .MapResult(Initialize, errors => 1);

static int Initialize(ConsoleArguments args)
{
    try
    {
        var host = Bootstrapper.CreateApp(args);
        var databaseMigrationService = host.Services.GetRequiredService<DatabaseMigrationService>();
        var dbConfiguration = host.Services.GetService<IOptions<DatabaseConfiguration>>();
        var flatFileConfiguration = host.Services.GetService<IOptions<FlatFileConfiguration>>();

        if (dbConfiguration != null && flatFileConfiguration != null)
        {

            if (CanLoadDatabase(dbConfiguration.Value, flatFileConfiguration.Value))
            {
                if (args.DeleteExistingDatabase)
                {
                    DatabaseMigrationService.ClearOldDatabase(dbConfiguration.Value.DatabaseLocation);
                }

                var task = databaseMigrationService.InitializeAsync(CancellationToken.None);
                task.Wait();

                if (task.Result)
                {
                    Console.WriteLine("Database is ready.");
                    return 0;
                }
            }
            else
            {
                Console.WriteLine($"Unable to load database - resources directory {flatFileConfiguration.Value.ResourcesDirectory} could not be found");
            }
        }
        else
        {
            Console.WriteLine($"Unable to load database - missing DatabaseConfiguration and/or FlatFileConfiguration inside appsettings.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Unable to load data");
        Console.WriteLine(ex.ToString());
    }
    return 1;
}


static bool CanLoadDatabase(DatabaseConfiguration dbConfiguration, FlatFileConfiguration flatFileConfiguration)
{
    if (!File.Exists(dbConfiguration.DatabaseLocation))
    {
        var parentPath = Path.GetDirectoryName(dbConfiguration.DatabaseLocation);
        if (parentPath != null && !Directory.Exists(parentPath))
        {
            Directory.CreateDirectory(parentPath);
        }
    }

    return !string.IsNullOrEmpty(flatFileConfiguration.ResourcesDirectory) && Directory.Exists(flatFileConfiguration.ResourcesDirectory);
}
