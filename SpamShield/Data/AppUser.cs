using InstaHyreSDETest.Entities;
using InstaHyreSDETest.Enums;
using Microsoft.AspNetCore.Identity;
using static InstaHyreSDETest.Enums.NumberType;

namespace InstaHyreSDETest.Data
{
    public class AppUser : IdentityUser
    {

        public System.DateTime Created { get; set; } = DateTime.Now;
        public System.DateTime Modified { get; set; } = DateTime.Now;
        public NumberTypeEnum flag { get; set; } = NumberTypeEnum.Normal;
        public string? Name { get; set; }
        public ICollection<Contacts> Contacts{ get; set; }
    }
}
