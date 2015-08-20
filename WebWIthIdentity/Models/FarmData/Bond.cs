using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebWIthIdentity.Models.FarmData
{
    /// <summary>
    /// Bidnings link people to Farms, the type of binding dictates if it's an agicultural worker or owner of the property
    /// </summary>
    public class Bond
    {
        public Bond()
        {
            Created = DateTime.Now;
        }

        /// <summary>
        /// Recommended constructor to use when creating a bond through a user
        /// </summary>
        /// <param name="Farm"></param>
        /// <param name="BondType"></param>
        public Bond(Farm farm, BondType type)
        {
            Created = DateTime.Now;
            Farm = farm;
            Type = type;
        }

        public string PersonID { get; set; }

        public virtual ApplicationUser Person { get; set; }

        public long FarmID { get; set; }

        public virtual Farm Farm { get; set; }

        public DateTime Created { get; set; }

        public BondType Type { get; set; }
        
        public bool Accepted { get; set; }
    }

    /// <summary> Explains the relationship between the person and the farm. THere are Mangers, Agronomists and Crew. ALl have different privilages.  </summary>
    public enum BondType
    {
        /// <summary>Any kind of bond, should not be assigned as a value to anybody in the DB, is only used for quering</summary>
        Any = 0,
        /// <summary> A worker on the farm, he may accept jobs and complete them </summary>
        Crew = 1,
        /// <summary> An agronomist may create orders to be done on the farm </summary>
        Agrinomist = 2, 
        /// <summary> Manager can assign tasks to spesific people, and he is the only one that can create tasks done in the past </summary>
        Manager = 3,
        /// <summary> Same as manager, except he can add and remove managers. There is only one owner at any given time</summary>
        Owner = 4,
        /// <summary> Another Value used for spesific cases</summary>
        NotApplicable = 100
    }
}