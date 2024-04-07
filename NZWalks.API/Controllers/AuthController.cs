using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }
        /// <summary>
        /// This method will be used to register the user
        /// </summary>
        /// <param name="registerUserRequestDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequestDto registerUserRequestDto)
        {
            var identityUser = new IdentityUser()
            {
                UserName = registerUserRequestDto.UserName,
                Email  = registerUserRequestDto.UserName,
            };
            var identityResult = await userManager.CreateAsync(identityUser, registerUserRequestDto.Password);
            if (identityResult.Succeeded)
            {
                //Add roles to the user
                if(registerUserRequestDto.Roles != null && registerUserRequestDto.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerUserRequestDto.Roles);
                    if(identityResult.Succeeded)
                    {
                        return Ok("User was Registered ! Please Login.");
                    }
                }
                    
            }
            return BadRequest("Something went wrong.");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> UserLogin([FromBody] UserLoginRequestDto userLoginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(userLoginRequestDto.UserName);
            if(user != null) {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user,userLoginRequestDto.Password);
                if(checkPasswordResult)
                {
                    //get roles from user 
                    var roles = await userManager.GetRolesAsync(user);
                    if(roles != null)
                    {
                        //generate token 
                        string jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());
                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };
                        return Ok(response);
                    }
                    
                }
            }
            return BadRequest("UserName or Password Incorrect.");
        }
    }
}
