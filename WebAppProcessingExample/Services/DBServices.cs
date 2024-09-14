using Dapper;
using System.Data.SqlClient;
using WebAppProcessingExample.Models;

namespace WebAppProcessingExample.Services
{
    public class DBServices
    {


        public async Task<List<AnalysisModel>> GetAnalysisAsync()
        {
            using (var connection = new SqlConnection("Server=localhost;Database=Processingdb;Trusted_Connection=True;"))
            {
                await connection.OpenAsync();
                var res = await connection.QueryAsync<AnalysisModel>("SELECT * FROM AnalysisProjecttbl");
                return res.ToList();
            }
        }


        public async Task InsertAnalysisAsync(AnalysisModel analysis)
        {
            using (var connection = new SqlConnection("Server=localhost;Database=Processingdb;Trusted_Connection=True;"))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync("INSERT INTO AnalysisProjecttbl (PKId, Name, ImageUrl, IsProcessed, Result) VALUES (@PKId, @Name, @ImageUrl, @IsProcessed, @Result)", analysis);
            }
        }

    }
}
