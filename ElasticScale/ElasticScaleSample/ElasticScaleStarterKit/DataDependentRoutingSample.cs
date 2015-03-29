/*
    Copyright 2014 Microsoft, Corp.

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;

namespace ElasticScaleStarterKit
{
    internal static class DataDependentRoutingSample
    {
        private static string[] tenantNames = new[]
        {
            "AdventureWorks Cycles", 
            "Contoso Ltd.", 
            "Vending Corp.", 
            "Northwind Traders", 
            "ProseWare, Inc.", 
            "Lucerne Publishing", 
            "Fabrikam, Inc.", 
            "Coho Winery", 
            "Alpine Ski House", 
            "Humongous Insurance"
        };

        private static string[] thingNames = new[]
        {
            "SÜ 2020", 
            "SÜ 2020 - Outdoor", 
            "Robimat 99", 
            "Robimat Serie - High Security", 
            "FS 1500", 
            "FS 2020 - Outdoor", 
            "SiCompact", 
            "SiCompact Duo", 
            "SC 202", 
            "SC 302"
        };

        private static Random r = new Random();

        public static void ExecuteDataDependentRoutingQuery(RangeShardMap<int> shardMap, string credentialsConnectionString)
        {
            // A real application handling a request would need to determine the request's customer ID before connecting to the database.
            // Since this is a demo app, we just choose a random key out of the range that is mapped. Here we assume that the ranges
            // start at 0, are contiguous, and are bounded (i.e. there is no range where HighIsMax == true)
            int currentMaxHighKey = shardMap.GetMappings().Max(m => m.Value.High);
            int regionId = 2;

            //insert 50 tenants with one associated thing
            var listOfInsertedTenants = new List<int>();
            while (listOfInsertedTenants.Count < 50)
            {
                int tenantId = GetTenantId(currentMaxHighKey);
                string tenantName = tenantNames[r.Next(tenantNames.Length)];

                if (!listOfInsertedTenants.Contains(tenantId))
                {
                    AddTenant(shardMap, credentialsConnectionString, tenantId, tenantName, regionId);
                    listOfInsertedTenants.Add(tenantId);

                    //Add one thing to tenant
                    AddThing(shardMap, credentialsConnectionString, tenantId, thingNames[r.Next(thingNames.Length)]);
                }
            }
        }

        /// <summary>
        /// Adds a customer to the tenants table (or updates the tenants if that id already exists).
        /// </summary>
        private static void AddTenant(
            ShardMap shardMap,
            string credentialsConnectionString,
            int tenantId,
            string name,
            int regionId)
        {
            // Open and execute the command with retry for transient faults. Note that if the command fails, the connection is closed, so
            // the entire block is wrapped in a retry. This means that only one command should be executed per block, since if we had multiple
            // commands then the first command may be executed multiple times if later commands fail.
            SqlDatabaseUtils.SqlRetryPolicy.ExecuteAction(() =>
            {
                // Looks up the key in the shard map and opens a connection to the shard
                using (SqlConnection conn = shardMap.OpenConnectionForKey(tenantId, credentialsConnectionString))
                {
                    // Create a simple command that will insert or update the customer information
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = @"INSERT INTO Tenants (TenantId, Name, RegionId)
                                        VALUES (@tenantId, @name, @regionId)";
                    cmd.Parameters.AddWithValue("@tenantId", tenantId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@regionId", regionId);
                    cmd.CommandTimeout = 60;

                    // Execute the command
                    cmd.ExecuteNonQuery();
                }
            });
        }

        /// <summary>
        /// Adds an order to the things table for the tenant.
        /// </summary>
        private static void AddThing(
            ShardMap shardMap,
            string credentialsConnectionString,
            int tenantId,
            string name)
        {
            SqlDatabaseUtils.SqlRetryPolicy.ExecuteAction(() =>
            {
                // Looks up the key in the shard map and opens a connection to the shard
                using (SqlConnection conn = shardMap.OpenConnectionForKey(tenantId, credentialsConnectionString))
                {
                    // Create a simple command that will insert a new order
                    SqlCommand cmd = conn.CreateCommand();

                    // Create a simple command
                    cmd.CommandText = @"INSERT INTO dbo.Things (TenantId, Name, Description)
                                        VALUES (@tenantId, @name, @description)";
                    cmd.Parameters.AddWithValue("@tenantId", tenantId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", name);
                    cmd.CommandTimeout = 60;

                    // Execute the command
                    cmd.ExecuteNonQuery();
                }
            });

            ConsoleUtils.WriteInfo("Inserted thing for tenant ID: {0}", tenantId);
        }

        /// <summary>
        /// Gets a tenant ID to insert into the tenants table.
        /// </summary>
        private static int GetTenantId(int maxid)
        {
            // If this were a real app and we were inserting tenant IDs, we would need a 
            // service that generates unique new tenant IDs.

            // Since this is a demo, just create a random tenant ID. To keep the numbers
            // manageable for demo purposes, only use a range of integers that lies within existing ranges.
            
            return r.Next(0, maxid);
        }
    }
}
