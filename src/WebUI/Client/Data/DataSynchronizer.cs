using System.Data.Common;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using WebUI.Client.Services;

namespace WebUI.Client.Data;

// This service synchronizes the Sqlite DB with both the backend server and the browser's IndexedDb storage
class DataSynchronizer
{
    public const string SqliteDbFilename = "app.db";
    private readonly Task _firstTimeSetupTask;
    private readonly IDbContextFactory<ClientSideDbContext> _dbContextFactory;
    private readonly IMessagesClient _messagesClient;
    private bool _isSynchronizing;

    public DataSynchronizer(IJSRuntime js, IDbContextFactory<ClientSideDbContext> dbContextFactory, IMessagesClient messagesClient)
    {
        this._dbContextFactory = dbContextFactory;
        _messagesClient = messagesClient;
        _firstTimeSetupTask = FirstTimeSetupAsync(js);
    }

    public int SyncCompleted { get; private set; }

    public int SyncTotal { get; private set; }

    public async Task<ClientSideDbContext> GetPreparedDbContextAsync()
    {
        await _firstTimeSetupTask;
        return await _dbContextFactory.CreateDbContextAsync();
    }

    public void SynchronizeInBackground()
    {
        _ = EnsureSynchronizingAsync();
    }

    public event Action? OnUpdate;
    public event Action<Exception>? OnError;

    private async Task FirstTimeSetupAsync(IJSRuntime js)
    {
        var module = await js.InvokeAsync<IJSObjectReference>("import", "./dbstorage.js");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser")))
        {
            await module.InvokeVoidAsync("synchronizeFileWithIndexedDb", SqliteDbFilename);
        }

        using var db = await _dbContextFactory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
    }

    private async Task EnsureSynchronizingAsync()
    {
        // Don't run multiple syncs in parallel. This simple logic is adequate because of single-threadedness.
        if (_isSynchronizing)
        {
            return;
        }

        try
        {
            _isSynchronizing = true;
            SyncCompleted = 0;
            SyncTotal = 0;

            // Get a DB context
            using var db = await GetPreparedDbContextAsync();
            db.ChangeTracker.AutoDetectChangesEnabled = false;
            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            // Begin fetching any updates to the dataset from the backend server
            var mostRecentUpdate = db.SentMessages.OrderByDescending(p => p.Id).FirstOrDefault()?.Id;

            var connection = db.Database.GetDbConnection();
            await connection.OpenAsync();

            while (true)
            {
                var response = await _messagesClient.GetSentMessagesInBatchAsync(1000, mostRecentUpdate ?? -1);
                var syncRemaining = response.ModifiedCount - response.SentMessages.Count;
                SyncCompleted += response.SentMessages.Count;
                SyncTotal = SyncCompleted + syncRemaining;

                if (response.SentMessages.Count == 0)
                {
                    break;
                }
                else
                {
                    mostRecentUpdate = response.SentMessages.Last().Id;
                    BulkInsert(connection, response.SentMessages);
                    OnUpdate?.Invoke();
                }
            }
        }
        catch (Exception ex)
        {
            // TODO: use logger also
            OnError?.Invoke(ex);
        }
        finally
        {
            _isSynchronizing = false;
        }
    }

    private static void BulkInsert(DbConnection connection, IEnumerable<SentMessageBatchDto> sentMessages)
    {
        // Since we're inserting so much data, we can save a huge amount of time by dropping down below EF Core and
        // using the fastest bulk insertion technique for Sqlite.
        using (var transaction = connection.BeginTransaction())
        {
            var command = connection.CreateCommand();
            var messsageId = AddNamedParameter(command, "$Id");
            var messageThreadId = AddNamedParameter(command, "$MessageThreadId");
            var toAddress = AddNamedParameter(command, "$ToAddress");
            var fromAddress = AddNamedParameter(command, "$FromAddress");
            var summaryContent = AddNamedParameter(command, "$SummaryContent");
            var created = AddNamedParameter(command, "$Created");

            command.CommandText =
                $"INSERT OR REPLACE INTO SentMessages (Id, MessageThreadId, ToAddress, FromAddress, SummaryContent, Created) " +
                $"VALUES ({messsageId.ParameterName}, {messageThreadId.ParameterName}, {toAddress.ParameterName}, {fromAddress.ParameterName}, {summaryContent.ParameterName}, {created.ParameterName})";

            foreach (var sentMessage in sentMessages)
            {
                messsageId.Value = sentMessage.Id;
                messageThreadId.Value = sentMessage.MessageThreadId;
                toAddress.Value = sentMessage.ToAddress;
                fromAddress.Value = sentMessage.FromAddress;
                summaryContent.Value = sentMessage.SummaryContent;
                created.Value = sentMessage.Created;
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        static DbParameter AddNamedParameter(DbCommand command, string name)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}
