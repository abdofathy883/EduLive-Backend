using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configrations
{
    internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<E_LearningDbContext>
    {
        public E_LearningDbContext CreateDbContext(string[] args)
        {
            var currentDir = Directory.GetCurrentDirectory();

            // Go up one level to the solution directory
            var parentDir = Directory.GetParent(currentDir)?.FullName;

            // Navigate to the Client API project where appsettings.json is located
            var clientApiDir = Path.Combine(parentDir, "Client API");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(clientApiDir)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<E_LearningDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnectionString");

            builder.UseSqlServer(connectionString);

            return new E_LearningDbContext(builder.Options);
        }
    }
}
