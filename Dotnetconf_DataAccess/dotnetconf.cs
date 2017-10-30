namespace Dotnetconf_DataAccess
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;

    public class dotnetconf : DbContext
    {
        // Your context has been configured to use a 'dotnetconf' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Dotnetconf_DataAccess.dotnetconf' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'dotnetconf' 
        // connection string in the application configuration file.
        public dotnetconf()
            : base("name=dotnetconf")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Attendes> MyEntities { get; set; }
    }

    public class Attendes
    {
      
        [Key]

        public int Id { get; set; }
        public string Name { get; set; }
    }
}