using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using HiveServer.Models.FarmData;
using HiveServer.DTO;

namespace HiveServer.Models
{
    /// <summary>
    /// Records form a contact book. Friendship book is a list of people each user knows on the system. 
    /// </summary>
    public class ContactDb : _Entity
    {
        [Index("ForIndex", Order = 1)]
        [Index("RevIndex", Order = 2)]
        public long Person1Id { get; set; }

        
        public virtual ApplicationUser Person1 { get; set; }

        [Index("ForIndex", Order = 2)]
        [Index("RevIndex", Order = 1)]
        public long Person2Id { get; set; }

        public virtual ApplicationUser Person2 { get; set; }

        /// <summary>
        /// There are several valid states that need to be prosesed:
        /// P1 - pending for user1 to respond, P2, pending for user2 to respond, F - friends,
        /// B1 - user 1 was blocked, B2 - user 2 was blocked, NA- not applicable
        /// </summary>
        [Index("ForIndex", Order = 3)]
        [Index("RevIndex", Order = 3)]
        public string  State { get; set; }

        public const string StateFriend = "Fr";
        public const string StatePendingP1 = "P1";
        public const string StatePendingP2 = "P2";
        public const string StateBlockedP1 = "B1";
        public const string StateBlockedP2 = "B2";
        public static readonly string[] ValidStates = { StateFriend, StatePendingP1, StatePendingP2, StateBlockedP1, StateBlockedP2 };




        /// <summary>
        /// Changes this friendship int the Data transfer object
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Contact ToContact(long userId)
        {
            var friendDto = new Contact
            {
                createdOn = CreatedOn,
                updatedOn = UpdatedOn,
                id = Id,
                markedDeleted = MarkedDeleted,
                version = Version
            };

            //determine which of the two users are we creating the DTO for
            if (Person1Id == userId)
            {
                friendDto.firstName = Person2.FirstName;
                friendDto.lastName = Person2.LastName;
                friendDto.phone = Person2.PhoneNumber;
                friendDto.friendID = Person2Id;
                friendDto.state = State;
            }
            else if (Person2Id == userId)
            {
                friendDto.firstName = Person1.FirstName;
                friendDto.lastName = Person1.LastName;
                friendDto.phone = Person1.PhoneNumber;
                friendDto.friendID = Person1Id;

                //Flip around pending expression  
                friendDto.state = FlipState(State);
            }
            else
            {
                throw new Exception(
                    "Can't convert Friendship to friend DTO, provided userId is not present in this relationship");
            }

            return friendDto;
        }

        /// <summary>
        /// if the user in this relationship is user 2, then the state markers need to be flipped
        /// </summary>
        /// <param name="state"></param>
        /// <returns>Returnes string, which is the state, but flipped around</returns>
        public static string FlipState(string state)
        {
            switch (state)
            {
                case StateFriend:
                    state = StateFriend;
                    break;
                case StatePendingP1:
                    state = StatePendingP2;
                    break;
                case StatePendingP2:
                    state = StatePendingP1;
                    break;
                case StateBlockedP1:
                    state = StateBlockedP2;
                    break;
                case StateBlockedP2:
                    state = StateBlockedP1;
                    break;
                default:
                    throw new Exception("State of Friendship is not valid");
            }

            return state;
        }

    }

}
/// <summary>   To use this fucntion, The contact and it's detail must be loaded into memory from SQL database </summary>
/// <returns>Returns Viewmodel of the contact referred in this Friendship</returns>






