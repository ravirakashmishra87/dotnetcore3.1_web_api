using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using coreAPI.Data;
using coreAPI.Dtos.Character;
using coreAPI.Dtos.CharacterSkill;
using coreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace coreAPI.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly DataContext _datacontext;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly IMapper _mapper;
        public CharacterSkillService(DataContext dataContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this._datacontext = dataContext;
            this._httpcontextAccessor = httpContextAccessor;
            this._mapper = mapper;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();

            try
            {
                Character character = await _datacontext.Characters
                                    .Include(c => c.User)
                                    .Include(c => c.Weapon)
                                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId &&
                                    c.User.Id == int.Parse(_httpcontextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));

                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }
                Skill skill = await _datacontext.Skills
                    .FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);
                if (skill == null)
                {
                    response.Success = false;
                    response.Message = "Skill not found.";
                    return response;
                }
                CharacterSkill characterSkill = new CharacterSkill
                {
                    Character = character,
                    Skill = skill
                };

                await _datacontext.CharacterSkills.AddAsync(characterSkill);
                await _datacontext.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);
                response.Message = "Character skill added successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = string.Format("Exception {0} and Inner Eception {1}", ex.Message, ex.InnerException);
            }
            return response;
        }
    }
}