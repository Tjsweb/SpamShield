using Azure;
using InstaHyreSDETest.Data;
using InstaHyreSDETest.DTO;
using InstaHyreSDETest.Entities;
using InstaHyreSDETest.Models;
using InstaHyreSDETest.Repository;
using InstaHyreSDETest.Repository.Authentication;
using InstaHyreSDETest.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;
using static InstaHyreSDETest.Enums.NumberType;

namespace InstaHyreSDETest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext applicationDBContext;
        private readonly AppUser appUser;
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IContactListRepo contactListRepo;
        private readonly IUserAuthRepo userAuthRepo;

        public AuthController(ApplicationDBContext applicationDBContext, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IContactListRepo contactListRepo, IUserAuthRepo userAuthRepo)
        {
            this.applicationDBContext = applicationDBContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.contactListRepo = contactListRepo;
            this.userAuthRepo = userAuthRepo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            AResponse<string> AResponse = new AResponse<string>();

            try
            {
               
                var newUser = new AppUser ();

                if(registerModel.Email == null || registerModel.Email == "")
                {
                    newUser = new AppUser { UserName = registerModel.PhoneNumber, PhoneNumber = registerModel.PhoneNumber, flag = NumberTypeEnum.Normal, Name = registerModel.Name };
                }else
                {
                    newUser = new AppUser { UserName = registerModel.PhoneNumber, Email = registerModel.Email, PhoneNumber = registerModel.PhoneNumber, flag = NumberTypeEnum.Normal, Name = registerModel.Name };
                }

                var result = await userManager.CreateAsync(newUser, registerModel.Password);


                if (result.Succeeded)
                {
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    await userManager.UpdateAsync(newUser);

                    code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                    var confirmEmailResult = await userManager.ConfirmEmailAsync(newUser, code);

                    if (!confirmEmailResult.Succeeded)
                    {
                        throw new InvalidOperationException("Error confirming email");
                    }

                    List<Contacts> registeredUser = await contactListRepo.GetAllContacts(registerModel.PhoneNumber);

                    if(registeredUser != null)
                    {
                        // update the userId in the contact list
                        foreach (var contact in registeredUser)
                        {
                            contact.UserId = newUser.Id;
                            await contactListRepo.UpdateContact(contact);
                        }
                    }

                    Contacts globalContact = await contactListRepo.CreateContact(new Contacts
                    {
                        PhoneNumber = registerModel.PhoneNumber,
                        Name = registerModel.Name,
                        UserId = newUser.Id,
                    });


                    AResponse.Data = registerModel.PhoneNumber;
                    AResponse.Message = "Successfully registered user";
                    AResponse.Success = true;

                }
                else
                {
                    throw new InvalidOperationException(result.Errors.FirstOrDefault().Description);
                }

                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;
                AResponse.Success = false;
                return Ok(AResponse);

            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            AResponse<JWTResponseDTO> AResponse = new AResponse<JWTResponseDTO>();
            try
            {
                var user = await userManager.FindByNameAsync(loginModel.UserName);
                if (user == null)
                {
                    AResponse.Message = "User not found";
                    AResponse.Success = false;
                    return Ok(AResponse);
                }

                var result = await signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);

                if (result.Succeeded)
                {

                    JWTResponseDTO jWTResponseDTO = await userAuthRepo.createJWTToken(loginModel.UserName);


                    AResponse.Data = jWTResponseDTO;
                    AResponse.Message = "Successfully logged in";
                    AResponse.Success = true;
                }
                else
                {
                    AResponse.Message = "Invalid login attempt";
                    AResponse.Success = false;
                }
                return Ok(AResponse);
            }
            catch (Exception ex)
            {
                AResponse.Message = ex.Message;
                AResponse.Success = false;
                return Ok(AResponse);
            }
        }
    }
}
