using Microsoft.Data.SqlClient;

namespace bretts_services.Utilities;

public static class SqlExceptionHelper
{
    private const int CannotInsertDuplicateKey = 2601;
    private const int CannotInsertDuplicateKeyInUniqueIndex = 2627;

    public static bool IsDuplicateKeyForIndex(DbUpdateException exception, string indexName)
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

        return sqlException.Message.Contains(indexName, StringComparison.OrdinalIgnoreCase);
    }
}
