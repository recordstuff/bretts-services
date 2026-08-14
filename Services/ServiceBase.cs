using bretts_services.Models.Entities;
using Microsoft.Data.SqlClient;

namespace bretts_services.Services;

public abstract class ServiceBase
{
    private const int CannotInsertDuplicateKey = 2601;
    private const int CannotInsertDuplicateKeyInUniqueIndex = 2627;

    protected readonly BrettsAppContext _brettsAppContext;

    protected ServiceBase(BrettsAppContext brettsAppContext)
    {
        _brettsAppContext = brettsAppContext;
    }

    protected async Task<bool> TrySaveChanges(string uniqueIndexName)
    {
        try
        {
            await _brettsAppContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException exception)
            when (IsDuplicateKeyForIndex(exception, uniqueIndexName))
        {
            return false;
        }
    }

    private static bool IsDuplicateKeyForIndex(DbUpdateException exception, string uniqueIndexName)
    {
        if (exception.InnerException is not SqlException sqlException)
        {
            return false;
        }

        var isDuplicateKey = sqlException.Number == CannotInsertDuplicateKey
                          || sqlException.Number == CannotInsertDuplicateKeyInUniqueIndex;

        if (!isDuplicateKey)
        {
            return false;
        }

        return sqlException.Message.Contains(uniqueIndexName, StringComparison.OrdinalIgnoreCase);
    }
}
