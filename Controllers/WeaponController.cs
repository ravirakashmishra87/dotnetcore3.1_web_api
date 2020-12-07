using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coreAPI.Dtos.Weapon;
using coreAPI.Services.WeaponService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace coreAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;
        public WeaponController(IWeaponService weaponService)
        {
            this._weaponService = weaponService;
        }
       
       [HttpPost]
        public async Task<ActionResult> AddWeapon(AddWeaponDto newWeapon)
        {
            return Ok(await _weaponService.AddWeapon(newWeapon));
        }

    }
}
