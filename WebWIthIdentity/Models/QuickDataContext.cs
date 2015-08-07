namespace ApiServerV1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;

    public class QuickDataContext : DbContext
    {
        // Your context has been configured to use a 'QuickData' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'DatabaseWriteTest.Models.QuickData' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'QuickData' 
        // connection string in the application configuration file.
        public QuickDataContext()
            : base("name=QuickData.tt")
        {

        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Human> Humans { get; set; }
        public DbSet<Field> Fields { get; set; }


    }

    public class Address
    {
        [Key]
        public long AddressId { get; set; } //PK - Not Null

        /*You can give an address an arbitrary name if you wnat*/
        public String Name { get; set; }

        /*house Number*/
        public int Number { get; set; }

        /*three lines for address, one should be mandatory*/
        public String Address1 { get; set; } //Not Null
        public String Address2 { get; set; }
        public String Address3 { get; set; }

        public String City { get; set; } //Not Null

        public String Country { get; set; } //Not Null

        public String Postcode { get; set; } //Not Null

    }


    public class Human
    {
        public Human()
        {

        }

        public Human(String name, byte[] PasswordHash, long CountryCode, long PhoneNumber)
        {

            Name = name;

            Addresses = new List<Address>();
            Fields = new List<Field>();
            PWDhash = PasswordHash;
            phoneCountryCode = CountryCode;
            phoneNumber = PhoneNumber;

            created = DateTime.Now;
            lastLoginSuccess = created.AddMonths(-12);
            lastLoginFailure = created.AddMonths(-12);

        }

        [Key]
        public long HumanId { get; set; }
        public String Name { get; set; }

        public ICollection<Address> Addresses { get; set; }
        public ICollection<Field> Fields { get; set; }

        public long phoneCountryCode { get; set; }
        public long phoneNumber { get; set; }

        public string email { get; set; }

        public DateTime created { get; set; }

        public DateTime lastLoginSuccess { get; set; }
        public DateTime lastLoginFailure { get; set; }
        public int bruteForceCounter { get; set; }
        public byte[] nonce = new byte[32];
        public byte[] lastNonce = new byte[32];



        public byte[] PWDhash = new byte[32];
        public byte[] Salt = new byte[32];

        public bool Disabled { get; set; }


    }

    public class Field
    {
        [Key]
        public long FieldId { get; set; }

        public Address address { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public double lattitude { get; set; }
        public double longitude { get; set; }

    }
}