using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coreAPI.Data;
using coreAPI.Dtos.user;
using coreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace coreAPI.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            this._authRepository = authRepository;
        }

        
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            ServiceResponse<int> response = await _authRepository.Register(
                    new User { Username = request.Username }, request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

/// <summary>
/// All user to login to the application with valid login credentials
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
           ServiceResponse<string> response = await _authRepository.Login(request.Username, request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}
