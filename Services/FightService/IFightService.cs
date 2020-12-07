using System.Threading.Tasks;
using coreAPI.Dtos.Fight;
using coreAPI.Models;

namespace coreAPI.Services.FightService
{
    public interface IFightService
    {

        public Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto newWeaponattack);
        public Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request);
        public Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request);
    }
}