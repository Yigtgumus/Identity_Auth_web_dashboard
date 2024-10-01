using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Identity_Auth.Helpers
{
    public class DatabaseHelper
    {
        public static async Task<DataTable> ExecuteQueryAsync(string connectionString, string query, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Parametreleri ekleyin
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }

            return dataTable;
        }
    }
}
