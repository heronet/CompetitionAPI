using CompetitionAPI.DTO;
using CompetitionAPI.Models;
using CompetitionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CompetitionAPI.Utilities.Constants;

namespace CompetitionAPI.Controllers
{
    public class AccountController : DefaultController
    {
        private readonly UserManager<Teacher> _userManager;
        private readonly TokenService _tokenService;
        private readonly SignInManager<Teacher> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(
            UserManager<Teacher> userManager,
            SignInManager<Teacher> signInManager,
            RoleManager<IdentityRole> roleManager,
            TokenService tokenService
        )
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userManager = userManager;
        }
        /// <summary>
        /// POST api/account/register
        /// </summary>
        /// <param name="registerDTO"></param>
        /// <returns><see cref="TeacherAuthDTO" /></returns>
        [HttpPost("register")]
        public async Task<ActionResult<TeacherAuthDTO>> RegisterUser(RegisterDTO registerDTO)
        {
            var memberRoleExists = await _roleManager.RoleExistsAsync(Roles.Teacher);
            if (!memberRoleExists)
            {
                var role = new IdentityRole(Roles.Teacher);
                var roleResult = await _roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                    return BadRequest("Can't add to Teacher");
            }

            var user = new Teacher
            {
                Name = registerDTO.Name.Trim(),
                UserName = registerDTO.Email.ToLower().Trim(),
                Email = registerDTO.Email.ToLower().Trim(),
                PhoneNumber = registerDTO.Phone,
                Subject = registerDTO.Subject,
                NcpscId = registerDTO.NcpscId
            };

            var result = await _userManager.CreateAsync(user, password: registerDTO.Password);
            if (!result.Succeeded) return BadRequest(result);

            var addToRoleResult = await _userManager.AddToRoleAsync(user, Roles.Teacher);
            if (addToRoleResult.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return await TeacherToDto(user, roles.ToList());
            }

            return BadRequest("Can't add user");
        }

        /// <summary>
        /// POST api/account/register
        /// </summary>
        /// <param name="registerDTO"></param>
        /// <returns><see cref="TeacherAuthDTO" /></returns>
        [HttpPost("register/admin")]
        // [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<TeacherAuthDTO>> RegisterAdmin(RegisterDTO registerDTO)
        {
            var memberRoleExists = await _roleManager.RoleExistsAsync(Roles.Admin);
            if (!memberRoleExists)
            {
                var role = new IdentityRole(Roles.Admin);
                var roleResult = await _roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                    return BadRequest("Can't add to Admin");
            }

            var user = new Teacher
            {
                Name = registerDTO.Name.Trim(),
                UserName = registerDTO.Email.ToLower().Trim(),
                Email = registerDTO.Email.ToLower().Trim(),
                PhoneNumber = registerDTO.Phone,
                Subject = registerDTO.Subject,
                NcpscId = registerDTO.NcpscId
            };

            var result = await _userManager.CreateAsync(user, password: registerDTO.Password);
            if (!result.Succeeded) return BadRequest(result);

            var addToRoleResult = await _userManager.AddToRoleAsync(user, Roles.Admin);
            if (addToRoleResult.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return await TeacherToDto(user, roles.ToList());
            }

            return BadRequest("Can't add user");
        }

        /// <summary>
        /// POST api/account/login
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns><see cref="TeacherAuthDTO" /></returns>
        [HttpPost("login")]
        public async Task<ActionResult<TeacherAuthDTO>> LoginUser(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email.ToLower().Trim());

            // Return If user was not found
            if (user == null) return BadRequest("Invalid Email");

            var result = await _signInManager.CheckPasswordSignInAsync(user, password: loginDTO.Password, false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return await TeacherToDto(user, roles.ToList());
            }

            return BadRequest("Invalid Password");
        }
        /// <summary>
        /// POST api/account/refresh
        /// </summary>
        /// <param name="userAuthDTO"></param>
        /// <returns><see cref="TeacherAuthDTO" /></returns>
        [Authorize]
        [HttpPost("refresh")]
        public async Task<ActionResult<TeacherAuthDTO>> RefreshToken(TeacherAuthDTO userAuthDTO)
        {

            var user = await _userManager.FindByIdAsync(userAuthDTO.Id);

            // Return If user was not found
            if (user == null) return BadRequest("Invalid User");

            var roles = await _userManager.GetRolesAsync(user);
            return await TeacherToDto(user, roles.ToList());
        }

        /// <summary>
        /// Utility Method.
        /// Converts a WhotUser to an AuthUserDto
        /// </summary>
        /// <param name="user"></param>
        /// <returns><see cref="TeacherAuthDTO" /></returns>
        private async Task<TeacherAuthDTO> TeacherToDto(Teacher user, List<string> roles)
        {
            return new TeacherAuthDTO
            {
                Name = user.Name,
                Token = await _tokenService.GenerateToken(user),
                Id = user.Id,
                Roles = roles
            };
        }
    }
}