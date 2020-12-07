using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coreAPI.Dtos.Fight;
using coreAPI.Services.FightService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace coreAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;
        public FightController(IFightService fightService)
        {
            this._fightService = fightService;
        }

        [HttpPost("Weapon")]
        public async Task<IActionResult> WeaponAttack(WeaponAttackDto request)
        {
            return Ok(await _fightService.WeaponAttack(request));
        }

        [HttpPost("Skill")]
        public async Task<IActionResult> SkillAttack(SkillAttackDto request)
        {
            return Ok(await _fightService.SkillAttack(request));
        }
        [HttpPost]
        public async Task<IActionResult>Fight(FightRequestDto request)
        {

            return Ok(await _fightService.Fight(request));
        }
    }
}
