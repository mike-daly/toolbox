using System.Data.SqlClient;
/// <summary>
/// container to encapsulate database connection and operations.
/// </summary>
class database
{
    string connectionString = @"Server=LocalHost\SQLExpress; Database=dupe_files; Integrated Security=SSPI;";
    SqlConnection myConnection;
    SqlCommand totalRowCountCommand;            // all of the rows in the table
    SqlCommand rowCountCommand;                 // rows that match checksum and path (for upsert)
    SqlCommand upsertRowCommand;                // do the insert....could be turned into an sproc call some day TODO

    public database()
    {
        this.myConnection = new SqlConnection(this.connectionString);
        this.myConnection.Open();

        this.totalRowCountCommand = this.myConnection.CreateCommand();
        this.totalRowCountCommand.CommandText = $"SELECT COUNT(*) FROM dbo.checksum_file";

        this.rowCountCommand = this.myConnection.CreateCommand();
        this.rowCountCommand.CommandText = $"SELECT COUNT(*) FROM dbo.checksum_file WHERE sha1_checksum = @sha1_checksum AND file_path = @file_path";

        this.upsertRowCommand = this.myConnection.CreateCommand();
        this.upsertRowCommand.CommandText = $"INSERT INTO dbo.checksum_file (sha1_checksum, file_path) VALUES (@sha1_checksum, @file_path)";

    }

    public void Close()
    {
        this.myConnection.Close();
    }

    public void insertSha1Path(string sha1, string path)
    {
        //this.rowCountCommand.CommandText = $"SELECT COUNT(*) FROM dbo.checksum_file WHERE sha1_checksum and file_path";
        this.rowCountCommand.Parameters.Clear();
        this.rowCountCommand.Parameters.AddWithValue("@sha1_checksum", sha1);
        this.rowCountCommand.Parameters.AddWithValue("@file_path", path);

        Int32 count = (Int32)this.rowCountCommand.ExecuteScalar();
        //Console.WriteLine($"match count before:  {count}");

        if (count == 0)
        {
            this.upsertRowCommand.Parameters.Clear();
            this.upsertRowCommand.Parameters.AddWithValue("@sha1_checksum", sha1);
            this.upsertRowCommand.Parameters.AddWithValue("@file_path", path);
            //Console.WriteLine($"upsert stmt:  {this.upsertRowCommand.CommandText}");
            this.upsertRowCommand.ExecuteNonQuery();
        }
        /*
        else
        {
            Console.WriteLine("already got it");
        }
        */

        /*
        count = (Int32)this.rowCountCommand.ExecuteScalar();
        Console.WriteLine($"match count after:  {count}");

        Console.WriteLine($"current total row count:  {(Int32)this.totalRowCountCommand.ExecuteScalar()}");
        */

    }
}
