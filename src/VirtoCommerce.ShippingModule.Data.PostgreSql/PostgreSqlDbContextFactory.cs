using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.ShippingModule.Data.Repositories;

namespace VirtoCommerce.ShippingModule.Data.PostgreSql
{
    public class PostgreSqlDbContextFactory : IDesignTimeDbContextFactory<ShippingDbContext>
    {
        public ShippingDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ShippingDbContext>();
            var connectionString = args.Any() ? args[0] : "User ID = postgres; Password = password; Host = localhost; Port = 5432; Database = virtocommerce3;";

            builder.UseNpgsql(
                connectionString,
                db => db.MigrationsAssembly(typeof(PostgreSqlDbContextFactory).Assembly.GetName().Name));

            return new ShippingDbContext(builder.Options);
        }
    }
}
