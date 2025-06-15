using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UsersController : ControllerBase
    {
        private readonly IUserDAO _userDAO;
        private readonly IConfiguration _configuration;

        public UsersController(IUserDAO userDAO, IConfiguration configuration)
        {
            _userDAO = userDAO;
            _configuration = configuration;
        }


        [HttpGet]
        public ActionResult<IEnumerable<UserEntity>> Get()
        {
            return Ok(_userDAO.Select());
        }

        [HttpGet("{id:guid}")]
        public ActionResult<UserEntity> Get(Guid id)
        {
            if (id != Guid.Empty)
            {
                var user = _userDAO.Select(id);

                if (user != null)
                {
                    return Ok(user);
                }
                return NotFound("User not found");
            }
            return BadRequest("Invalid user identifier");
        }


        [AllowAnonymous] 
        [HttpPost]
        public ActionResult<UserEntity> Post(UserEntity user)
        {
            if (user == null || user.UserId == Guid.Empty)
                return BadRequest("Invalid User entity or UserId.");

            UserEntity existingUser = _userDAO.SelectByUsername(user.Username);

            if (existingUser.UserId != Guid.Empty)
            {
                return BadRequest($"User exist with this name {user.Username}");
            }
            else
            {
                PasswordHasher<UserEntity> passwordHasher = new();
                user.Password = passwordHasher.HashPassword(user, user.Password);

                _userDAO.Insert(user);

                return CreatedAtAction(nameof(Get), new { id = user.UserId }, user);
            }    
           
        }


        [AllowAnonymous] 
        [HttpPost("LogIn")]
        public ActionResult<object> LogIn(LoginEntity login)
        {
            if (login != null)
            {
                var user = _userDAO.Select(login);
                if (user != null && user.UserId != Guid.Empty)
                {
                    var passwordHasher = new PasswordHasher<UserEntity>();

                    var result = passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);

                    if (result == PasswordVerificationResult.Success)
                    {
                        var token = GenerateJwtToken(user);

                        var response = new
                        {
                            Token = token, 
                        };

                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest("Wrong credentials");
                    }
                }
                else
                {
                    return BadRequest("Wrong credentials");
                }
            }
            return BadRequest("Login entity is null");
        }

        [HttpPut("EditUser")]
        public ActionResult<EditUserEntity> Put(EditUserEntity user)
        {
            if (user == null || user.UserId == Guid.Empty)
                return BadRequest("Invalid UserEntity or UserID.");

            _userDAO.Update(user);

            return CreatedAtAction(nameof(Get), new { id = user.UserId }, user);
        }

        [HttpPut("EditUserData")]
        public ActionResult<UserEntity> Put(UserEntity user)
        {
            if (user == null || user.UserId == Guid.Empty)
                return BadRequest("Invalid UserEntity or UserID.");

            PasswordHasher<UserEntity> passwordHasher = new();
            user.Password = passwordHasher.HashPassword(user, user.Password);


            _userDAO.UpdateAll(user);

            return CreatedAtAction(nameof(Get), new { id = user.UserId }, user);
        }


        [HttpDelete("{id:guid}")]
        public ActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid UserId.");

            _userDAO.Delete(id);
            return NoContent();
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), 
                new Claim("Username", user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email), 
                new Claim("UserType", user.UserType), 
                new Claim("ProfilePicture", user.ProfilePicture ?? ""), 
                new Claim("PhoneNumber", user.PhoneNumber ?? ""), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
