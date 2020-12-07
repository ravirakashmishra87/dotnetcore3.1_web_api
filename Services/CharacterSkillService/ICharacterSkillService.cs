using System.Threading.Tasks;
using coreAPI.Dtos.Character;
using coreAPI.Dtos.CharacterSkill;
using coreAPI.Models;

namespace coreAPI.Services.CharacterSkillService
{
    public interface ICharacterSkillService
    {
         Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
    }
}