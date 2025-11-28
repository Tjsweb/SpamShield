using InstaHyreSDETest.Data;
using InstaHyreSDETest.DTO;
using InstaHyreSDETest.Entities;
using InstaHyreSDETest.Enums;
using InstaHyreSDETest.Models;
using InstaHyreSDETest.Repository;
using InstaHyreSDETest.Utilities;
using InstaHyreSDETest.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Principal;
using static InstaHyreSDETest.Enums.NumberType;

namespace InstaHyreSDETest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlobalContactsController : ControllerBase
    {
        private readonly IContactListRepo contactListRepo;
        private readonly UserManager<AppUser> userManager;
        private readonly string InstaHyreTokens = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;

        public GlobalContactsController(IContactListRepo contactListRepo, UserManager<AppUser> userManager)
        {
            this.contactListRepo = contactListRepo;
            this.userManager = userManager;
        }

        [HttpPost("CreateContact")]
        public async Task<IActionResult> CreateContact(GlobalContactModel contact)
        {
            AResponse<Contacts> AResponse = new AResponse<Contacts>();
            try
            {
                // get all AppUsers
                var users = await userManager.Users.ToListAsync();

                // get user by phone number
                var user = users.FirstOrDefault(x => x.PhoneNumber == contact.PhoneNumber);

                if (user != null) {
                    AResponse.Message = "Contact with this phone number already exists";
                    return Ok(AResponse);
                }

                Contacts globalContact = await contactListRepo.CreateContact(new Contacts
                {
                    PhoneNumber = contact.PhoneNumber,
                    Name = contact.Name,
                    flag = contact.isSpam ? NumberTypeEnum.Spam : NumberTypeEnum.Normal,
                });

                AResponse.Data = globalContact;
                AResponse.Message = "Contact created successfully";

                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;

                return Ok(AResponse);
            }
        }

        [HttpPost("AddPersonalContacts")]
        [Authorize(AuthenticationSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddPersonalContacts(ContactModel contactModel)
        {
            AResponse<ContactBookDTO> AResponse = new AResponse<ContactBookDTO>();
            try
            {
                AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);

                if (appUser == null)
                {
                    AResponse.Message = "Contact not found";
                    return Ok(AResponse);
                }

                var contacts = contactModel.PersonalContacts;

                if(contacts == null || contacts.Count == 0)
                {
                    AResponse.Message = "No contacts to add";
                    return Ok(AResponse);
                }

                foreach (var personalContact in contacts)
                {
                    // check if personal contact already registered
                    AppUser registeredAppUser = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == personalContact.PhoneNumber);

                    if (registeredAppUser != null)
                    {
                        Contacts contacts1 = await contactListRepo.CreateContact(new Contacts
                        {
                            PhoneNumber = personalContact.PhoneNumber,
                            Name = personalContact.Name,
                            flag = personalContact.isSpam ? NumberTypeEnum.Spam : NumberTypeEnum.Normal,
                            ContactUser = appUser,
                            ContactUserId = appUser.Id,
                            UserId = registeredAppUser.Id,
                        });
                    }else 
                    { 
                        Contacts personalContactEntity = await contactListRepo.CreateContact(new Contacts
                        {
                            PhoneNumber = personalContact.PhoneNumber,
                            Name = personalContact.Name,
                            flag = personalContact.isSpam ? NumberTypeEnum.Spam : NumberTypeEnum.Normal,
                            ContactUser = appUser,
                            ContactUserId = appUser.Id,
                        });
                    }
                }

                AResponse.Message = "Personal contacts added successfully";
                AResponse.Data = new ContactBookDTO
                {
                    SelfPhoneNumber = contactModel.SelfPhoneNumber,
                    PersonalContacts = contacts
                };

                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;
                return Ok(AResponse);
            }
        }

        [HttpGet("GetAllContacts")]
        public async Task<IActionResult> GetAllContacts()
        {
            AResponse<List<Contacts>> AResponse = new AResponse<List<Contacts>>();
            try
            {
                List<Contacts> contacts = await contactListRepo.GetAllContacts();
                AResponse.Data = contacts;
                AResponse.Message = "Contacts fetched successfully";

                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;

                return Ok(AResponse);
            }
        }

        [HttpGet("GetContact")]
        public async Task<IActionResult> GetContact(string phoneNumber)
        {
            AResponse<List<Contacts>> AResponse = new AResponse<List<Contacts>>();
            try
            {
                List<Contacts> contactList = await contactListRepo.GetAllContacts(phoneNumber);
                AResponse.Data = contactList;
                AResponse.Message = "Contact fetched successfully";
                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;
                return Ok(AResponse);
            }
        }

        [HttpPost("DeleteContact")]
        public async Task<IActionResult> DeleteContact(string phoneNumber)
        {
            AResponse<Contacts> AResponse = new AResponse<Contacts>();
            try
            {
                Contacts contact = await contactListRepo.DeleteContact(phoneNumber);
                AResponse.Data = contact;
                AResponse.Message = "Contact deleted successfully";
                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;
                return Ok(AResponse);
            }
        }

        [HttpPost("MarkSpam")]
        [Authorize(AuthenticationSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> MarkSpam(string phoneNumber)
        {
            AResponse<List<Contacts>> AResponse = new AResponse<List<Contacts>>();
            try
            {
                AppUser user = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

                if (user != null)
                {
                    user.flag = NumberTypeEnum.Spam;
                    await userManager.UpdateAsync(user);
                }

                List<Contacts> contacts = await contactListRepo.GetAllContacts(phoneNumber);

                foreach (var item in contacts)
                {
                    item.flag = NumberTypeEnum.Spam;
                    await contactListRepo.UpdateContact(item);
                }

                AResponse.Data = contacts;
                AResponse.Message = "Contact marked as spam successfully";

                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;

                return Ok(AResponse);
            }
        }

        [HttpPost("MarkNotSpam")]
        [Authorize(AuthenticationSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> MarkNotSpam(string phoneNumber)
        {
            AResponse<List<Contacts>> AResponse = new AResponse<List<Contacts>>();
            try
            {
                AppUser user = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

                if (user != null)
                {
                    user.flag = NumberTypeEnum.Normal;
                    await userManager.UpdateAsync(user);
                }

                List<Contacts> contacts = await contactListRepo.GetAllContacts(phoneNumber);

                foreach (var item in contacts)
                {
                    item.flag = NumberTypeEnum.Normal;
                    await contactListRepo.UpdateContact(item);
                }

                AResponse.Data = contacts;
                AResponse.Message = "Contact marked as not spam successfully";

                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;

                return Ok(AResponse);
            }
        }

        [HttpPost("SearchByContact")]
        [Authorize(AuthenticationSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchByContact(string phoneNumber)
        {
            AResponse<List<SearchResponseDTO>> AResponse = new AResponse<List<SearchResponseDTO>>();
            try
            {
                AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);

                bool isUserXInPersonalContacts = false;

                if (appUser != null) {
                    isUserXInPersonalContacts = await contactListRepo.IsUserXInPersonalContacts(phoneNumber, appUser.Id);
                    //if (!isUserXInPersonalContacts)
                    //{
                    //    AResponse.Message = "Phone no. is not in the contact list";
                    //    return Ok(AResponse);
                    //}
                }

                var users = await userManager.Users.ToListAsync();

                // get user by phone number
                var user = users.FirstOrDefault(x => x.PhoneNumber == phoneNumber);
                
                List<Contacts> contacts = await contactListRepo.GetAllContacts();
                List<Contacts> filteredContacts = contacts.Where(x => x.PhoneNumber.StartsWith(phoneNumber) && (x.ContactUserId == appUser.Id || x.ContactUserId == null)).ToList();

                List<SearchResponseDTO> result = new List<SearchResponseDTO>();

                if(user != null && isUserXInPersonalContacts)
                {
                    result.Add(new SearchResponseDTO
                    {
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        isSpam = user.flag == NumberTypeEnum.Spam ? true : false,
                    });

                    AResponse.Data = result;
                    AResponse.Message = "Personal Contact fetched successfully";

                    return Ok(AResponse);
                }

                foreach (var item in filteredContacts)
                {
                    result.Add(new SearchResponseDTO
                    {
                        Name = item.Name,
                        PhoneNumber = item.PhoneNumber,
                        isSpam = item.flag == NumberTypeEnum.Spam ? true : false,
                    });
                }

                AResponse.Data = result;
                AResponse.Message = "Contacts fetched successfully";
                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;
                return Ok(AResponse);
            }
        }

        [HttpPost("SearchByName")]
        [Authorize(AuthenticationSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchByName(string name)
        {
            AResponse<List<SearchResponseDTO>> AResponse = new AResponse<List<SearchResponseDTO>>();
            try
            {
                List<Contacts> contacts = await contactListRepo.GetAllContacts();

                // Results should first show people whose names start with the given name, and then people whose names contain but don’t start with the given name.
                List<Contacts> filteredContacts = contacts.Where(x => x.Name.ToLower().StartsWith(name.ToLower())).ToList();
                List<Contacts> filteredContacts2 = contacts.Where(x => x.Name.ToLower().Contains(name.ToLower()) && !x.Name.ToLower().StartsWith(name.ToLower())).ToList();

                List<Contacts> totalFilteredContacts = filteredContacts.Concat(filteredContacts2).ToList();

                List<SearchResponseDTO> result = new List<SearchResponseDTO>();

                foreach (var item in totalFilteredContacts)
                {
                    if (result.FirstOrDefault(x => x.PhoneNumber == item.PhoneNumber) == null)
                    {
                        result.Add(new SearchResponseDTO
                        {
                            Name = item.Name,
                            PhoneNumber = item.PhoneNumber,
                            isSpam = item.flag == NumberTypeEnum.Spam ? true : false,
                        });
                    }
                }

                AResponse.Data = result;
                AResponse.Message = "Contacts fetched successfully";
                
                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;
                return Ok(AResponse);
            }
        }

        [HttpGet("SearchAllSpamContacts")]
        [Authorize(AuthenticationSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchAllSpamContacts()
        {
            AResponse<List<SearchResponseDTO>> AResponse = new AResponse<List<SearchResponseDTO>>();
            try
            {
                List<Contacts> contacts = await contactListRepo.GetAllContacts();
                List<Contacts> spamContacts = contacts.Where(x => x.flag == NumberTypeEnum.Spam).ToList();

                List<SearchResponseDTO> result = new List<SearchResponseDTO>();

                foreach (var item in spamContacts)
                {
                    // add if not already added
                    if (result.FirstOrDefault(x => x.PhoneNumber == item.PhoneNumber) == null)
                    {
                        result.Add(new SearchResponseDTO
                        {
                            Name = item.Name,
                            PhoneNumber = item.PhoneNumber,
                            isSpam = item.flag == NumberTypeEnum.Spam ? true : false,
                        });
                    }
                }

                AResponse.Data = result;
                AResponse.Message = "Spam contacts fetched successfully";
                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;
                return Ok(AResponse);
            }
        }
    }
}
