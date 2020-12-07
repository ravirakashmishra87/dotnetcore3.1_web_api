using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coreAPI.Dtos.CharacterSkill;
using coreAPI.Services.CharacterSkillService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace coreAPI.Controllers
{
    
    [Route("[controller]")]
    [ApiController]
    public class CharacterSkillController : ControllerBase
    {
        private readonly ICharacterSkillService _charracterSkillService;
        public CharacterSkillController(ICharacterSkillService characterSkillService)
        {
            this._charracterSkillService = characterSkillService;

        }

        [HttpPost]
        public async Task<IActionResult> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {

            return Ok(await _charracterSkillService.AddCharacterSkill(newCharacterSkill));
        }

    }
}
