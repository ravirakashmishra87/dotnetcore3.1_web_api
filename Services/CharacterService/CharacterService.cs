using System.Collections.Generic;
using coreAPI.Controllers;
using coreAPI.Models;
using System.Linq;
using System.Threading.Tasks;
using coreAPI.Dtos.Character;
using AutoMapper;
using System;
using coreAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace coreAPI.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        #region VARIABLES
        private readonly IMapper _mapper;
        private readonly DataContext _datacontext;

        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region CONSTRUCTOR
        public CharacterService(IMapper mapper, DataContext datacontext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _datacontext = datacontext;
            _httpContextAccessor = httpContextAccessor;

        }
        #endregion

        #region ADD DATA
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {

                Character character = _mapper.Map<Character>(newCharacter);

                character.User = await _datacontext.Users.FirstOrDefaultAsync(u => u.Id == GetUserID());
                await _datacontext.Characters.AddAsync(character);
                await _datacontext.SaveChangesAsync();

                // serviceResponse.Data = (characters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
                //serviceResponse.Data = _datacontext.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
                List<Character> dbCharacters = await _datacontext.Characters.Where(c => c.User.Id == GetUserID()).ToListAsync();
                serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
                serviceResponse.Message = "Record inserted successfully";


            }
            catch (Exception ex)
            {
                serviceResponse.Message = "Fail to insert record - " + ex.InnerException;
                serviceResponse.Success = false;
            }
            return serviceResponse;
        }


        #endregion

        #region GET ALL DATA
        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                List<Character> dbCharacters = await _datacontext.Characters.Where(c => c.User.Id == GetUserID()).ToListAsync();

                if (dbCharacters != null)
                {
                    serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
                }
                else
                {

                    serviceResponse.Message = "Record not found.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        #endregion

        #region GET DATA BY ID
        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                // serviceResponse.Data = _mapper.Map<GetCharacterDto>(characters.FirstOrDefault(c => c.Id == id));
                Character dbCharacter = await _datacontext.Characters
                .Include(c=>c.User)
                .Include(c=>c.Weapon)
                .Include(c=>c.CharacterSkills).ThenInclude(cs=>cs.Skill)
                .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserID());
               
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        #endregion

        #region  UPDATE DATA        
        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();

            try
            {  // Character character = characters.FirstOrDefault(c => c.Id == updatedCharacter.Id);
                Character dbCharacter = await _datacontext.Characters.Include(c=>c.User).FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                                                                                            //&& c.User.Id == GetUserID());
                if (dbCharacter != null)
                {
                    dbCharacter.Class = updatedCharacter.Class;
                    dbCharacter.Name = updatedCharacter.Name;
                    dbCharacter.Defense = updatedCharacter.Defense;
                    dbCharacter.HitPoints = updatedCharacter.HitPoints;
                    dbCharacter.Intelligence = updatedCharacter.Intelligence;
                    dbCharacter.Strength = updatedCharacter.Strength;

                    await _datacontext.SaveChangesAsync();
                    serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);

                    serviceResponse.Message = "Record updated successfully";

                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No record found to update";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        #endregion

        #region DELETE RECORD
        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                Character dbCharacter = await _datacontext.Characters.FirstOrDefaultAsync(c => c.Id == id &&
                                                                                        c.User.Id == GetUserID());
                if (dbCharacter != null)
                {
                    _datacontext.Characters.Remove(dbCharacter);
                    await _datacontext.SaveChangesAsync();

                    serviceResponse.Data = (_datacontext.Characters.Where(c => c.User.Id == GetUserID())
                                                                   .Select(c => _mapper.Map<GetCharacterDto>(c)))
                                                                   .ToList();
                    serviceResponse.Message = "Record deleted successfully";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Record not found";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        #endregion

        #region  GET CURRENT USER
        private int GetUserID() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        #endregion
    }
}