using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ProjectManagerAPI.Data;

public class DataContextDapper
{
    // Configuration object that contains the Connection String for the Database
    private readonly IConfiguration _config;

    // Constructor for DataContextDapper object
    public DataContextDapper(IConfiguration config)
    {
        _config = config;
    }

    // Function to get data from the Database based of an SQL Query
    public IEnumerable<T> LoadData<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.Query<T>(sql);
    }

    // Function to get a single object from the Database based of an SQL Query
    public T LoadSingleData<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.QuerySingle<T>(sql);
    }

    // Function used to execute a custom SQL Query
    public bool ExecuteSql(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(sql) > 0;
    }

    // Function used to execute a custom SQL Query that also returns the number of rows that were affected
    public int ExecuteSqlWithRowCount(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(sql);
    }

    // Function used to execute a custom SQL Query based on custom parameters
    public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
    {
        SqlCommand commandWithParams = new(sql);
        foreach (SqlParameter parameter in parameters)
        {
            commandWithParams.Parameters.Add(parameter);
        }

        SqlConnection dbConnection = new(_config.GetConnectionString("DefaultConnection"));
        dbConnection.Open();

        commandWithParams.Connection = dbConnection;

        int rowsAffected = commandWithParams.ExecuteNonQuery();

        dbConnection.Close();

        return rowsAffected > 0;
    }
}