using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using coreAPI.Dtos.Character;
using coreAPI.Models;
using coreAPI.Services.CharacterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace coreAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        public CharacterController(ICharacterService characterService)
        {
            this._characterService = characterService;
        }


        // GET: api/Character
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
           // int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int id)
        {
            return Ok(await _characterService.GetCharacterById(id));
        }

        // POST: api/Character
        [HttpPost]
        public async Task<IActionResult> AddCharacter(AddCharacterDto newCharacter)
        {

            return Ok(await _characterService.AddCharacter(newCharacter));
        }

        // PUT: api/Character/5
        [HttpPut]
        public async Task<IActionResult> UpdateCharacter(UpdateCharacterDto newCharacter)
        {
            ServiceResponse<GetCharacterDto> response = await _characterService.UpdateCharacter(newCharacter);
            if (response.Data == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
             ServiceResponse<List<GetCharacterDto>> response = await _characterService.DeleteCharacter(id);
            if (response.Data == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
