using System.Data.SqlClient;
using MicroServiceUtilites;
using Microsoft.EntityFrameworkCore;

namespace MyService
{
    public class MyObjectDbContext : DbContext
    {
        public DbSet<Provider> Provider   { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Program._configuration["DatabaseConnectionString"]);
        }
          
        public static Response HandleDbUpdateException(DbUpdateException e)
        {
            var sqlException = e.GetBaseException() as SqlException;
            if (sqlException != null) {
                if (sqlException .Errors.Count > 0) {
                    switch (sqlException .Errors[0].Number) {
                        case 547: // Foreign Key violation
                            return new Response(ResponseCode.BUSINESS_ENTITY_NOT_EXIST, "BusinessEntityParent does not exist. Please validate BusinessEntityParentID");                            
                        case 8152: //Value woulc be truncated
                            return new Response(ResponseCode.BAD_REQUEST, "Bad request recieved. A value is too long and would be truncated.");                            
                        default:
                            System.Console.WriteLine(sqlException.Errors[0].Number);
                            break;
                    }
                }
            }
            return new Response(ResponseCode.UNCAUGHT_EXCEPTION, "An unknown error has occurred while performing an update. Please try again " + e.InnerException.Message);   
        }
    }
}