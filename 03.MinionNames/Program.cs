using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace _03.MinionNames
{
    class Program
    {
        const string CONNECTION_STRING = @"Server=.;Database=MinionsDB;Integrated Security=True";
        static async Task Main(string[] args)
        {
            SqlConnection sqlConnection = new SqlConnection(CONNECTION_STRING);

            await sqlConnection.OpenAsync();

            await using (sqlConnection)
            {
                int inputMinionId = int.Parse(Console.ReadLine());

                string firstSelectQuery = $@"SELECT Name FROM Villains WHERE Id = {inputMinionId}";

                SqlCommand firstSelectCommand = new SqlCommand(firstSelectQuery, sqlConnection);

                var villainName = await firstSelectCommand.ExecuteScalarAsync();

                if (villainName == null)
                {
                    Console.WriteLine($"No villain with ID {inputMinionId} exists in the database.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Villain: {villainName}");
                }

                string secondSelectQuery = $@"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                                         m.Name, 
                                                         m.Age
                                                    FROM MinionsVillains AS mv
                                                    JOIN Minions As m ON mv.MinionId = m.Id
                                                    WHERE mv.VillainId = {inputMinionId}
                                                    ORDER BY m.Name";

                SqlCommand secondSelectCommand = new SqlCommand(secondSelectQuery, sqlConnection);
                SqlDataReader reader = await secondSelectCommand.ExecuteReaderAsync();

                await using (reader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            long rowNum = reader.GetInt64(0);
                            string minionName = reader.GetString(1);
                            int age = reader.GetInt32(2);

                            Console.WriteLine($"{rowNum}. {minionName} {age}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("(no minions)");
                    }
                }
            }
        }
    }
}
