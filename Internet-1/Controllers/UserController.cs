using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using AutoMapper;
using Internet_1.Models;
using Internet_1.Repositories;
using Internet_1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.Collections.Specialized;

namespace Internet_1.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly INotyfService _notyf;
        public UserController(UserRepository userRepository, IMapper mapper, IConfiguration config, INotyfService notyf)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _config = config;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            var userModels = _mapper.Map<List<UserModel>>(users);
            return View(userModels);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserModel model)
        {
            if (_userRepository.Where(s => s.UserName == model.UserName).Count() > 0)
            {
                _notyf.Error("Girilen Kullanıcı Adı Kayıtlıdır!");
                return View(model);
            }
            if (_userRepository.Where(s => s.Email == model.Email).Count() > 0)
            {
                _notyf.Error("Girilen E-Posta Adresi Kayıtlıdır!");
                return View(model);
            }
            var user = new User();
            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Created = DateTime.Now;
            user.Updated = DateTime.Now;
            user.PhotoUrl = "no-image.png";
            user.Password = MD5Hash(model.Password);
            user.Role = model.Role;
            await _userRepository.AddAsync(user);
            _notyf.Success("Üye Eklendi");

            return RedirectToAction("Index");
        }
        public IActionResult Update()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }

        public string MD5Hash(string pass)
        {
            var salt = _config.GetValue<string>("AppSettings:MD5Salt");
            var password = pass + salt;
            var hashed = password.MD5();
            return hashed;
        }
    }
}

