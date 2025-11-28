using System.ComponentModel.DataAnnotations;
using static InstaHyreSDETest.Enums.NumberType;

namespace InstaHyreSDETest.Models
{

    public class ContactModel
    {
        public string SelfPhoneNumber{ get; set; }
        public List<GlobalContactModel> PersonalContacts { get; set; }
    }
    public class GlobalContactModel
    {
        [Phone]
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public bool isSpam { get; set; } = false;
    }
}
