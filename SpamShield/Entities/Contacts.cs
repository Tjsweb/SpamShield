using InstaHyreSDETest.Data;
using InstaHyreSDETest.Enums;
using System.ComponentModel.DataAnnotations;
using static InstaHyreSDETest.Enums.NumberType;

namespace InstaHyreSDETest.Entities
{
    public class Contacts
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Phone]
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public NumberTypeEnum flag { get; set; } = NumberTypeEnum.Normal;

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        #region Related Data
        public AppUser ContactUser { get; set; }   // User who owns this contact
        public string? ContactUserId { get; set; }

        public AppUser? User { get; set; }     // If it's a registered user
        public string? UserId { get; set; }
        #endregion
    }
}
