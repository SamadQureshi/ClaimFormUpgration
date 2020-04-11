using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Services
{
    public class UserService : IUserService
    {
        private IBaseRepository<User> _userRepository;
        
        public UserService(IBaseRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public List<UserVM> GetAllUsers()
        {
            var allUsers = _userRepository.GetQueryable().ToList();
            return Mapper.Map<List<UserVM>>(allUsers);
        }
        
    }
}
