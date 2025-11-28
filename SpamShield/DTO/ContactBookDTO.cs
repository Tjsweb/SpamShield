using InstaHyreSDETest.Models;

namespace InstaHyreSDETest.DTO
{
    public class ContactBookDTO
    {
        public string SelfPhoneNumber{ get; set; }
        public List<GlobalContactModel> PersonalContacts { get; set; }
    }
}
