using System;
using System.Data.SqlClient;

namespace _02.VillainNames
{
    class Program
    {
        const string CONNECTION_STRING = @"Server=.;Database=MinionsDB;Integrated Security=True";
        static void Main(string[] args)
        {
            SqlConnection sqlConnection = new SqlConnection(CONNECTION_STRING);

            using (sqlConnection)
            {
                sqlConnection.Open();

                string selectQuery = @"SELECT *
	                                    FROM (
	                                    	  SELECT Villains.Name,
	                                    	         COUNT(*) AS [MinionsCount]
	                                    	      FROM Villains
	                                    	 	  JOIN MinionsVillains ON Villains.Id = MinionsVillains.VillainId
	                                    	      GROUP BY Villains.Name
	                                    	 ) AS TEMP
	                                    WHERE [MinionsCount] > 3
	                                    ORDER BY [MinionsCount] DESC";

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);

                SqlDataReader reader = selectCommand.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        string villainName = (string)reader["Name"];
                        int numberOfMinions = (int)reader["MinionsCount"];

                        Console.WriteLine(villainName + " - " + numberOfMinions);
                    }
                }
            }
        }
    }
}