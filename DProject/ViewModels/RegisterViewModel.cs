﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Введите Ваше имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан эмейл")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }
    }
}
