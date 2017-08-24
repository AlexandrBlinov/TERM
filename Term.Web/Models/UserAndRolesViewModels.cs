using System;
using System.ComponentModel.DataAnnotations;
using Yst.DropDowns;
using Term.DAL;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using PagedList;
using Yst.ViewModels;
using Term.Web.Views.Resources;

namespace YstIdentity.Models 
{
   
    public class ManageUserViewModel
    {
      //123
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        

        [Display(Name = "ФИО")]
        [StringLength(100)]
        public string ContactFIO { get; set; }

        [Display(Name = "Номер телефона")]
        [StringLength(12)]
        public string PhoneNumber { get; set; }

        [Display(Name= "Партнер")]
        public bool IsPartner { get; set; }

        [StringLength(7)]
        public string PartnerId { get; set; }

        public int? PartnerPointId { get; set; }

        public int? SupplierId { get; set; }

     /*   public bool IsSupplier { get; set; }

        public bool IsSaleRep { get; set; } */
        
        public int? DepartmentId { get; set; }
        

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        
       
    }

    public class ListUserViewModel
    {
        public string UserName { get; set; }
        
        [Display(Name = "Номер телефона")]
        [StringLength(12)]
        public string PhoneNumber { get; set; }

        [Display(Name = "ФИО")]
        [StringLength(100)]
        public string ContactFIO { get; set; }

        public bool IsPartner { get; set; }

        // связь пользователя и партнера, чтобы потом привязываться к нему
        [StringLength(7)]
        public string PartnerId { get; set; }

    }

    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email",ResourceType =typeof(AccountTexts))]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(AccountTexts))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword",  ResourceType = typeof(AccountTexts))]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }



    public class ForgotPasswordViewModel
    {
        [Required]        
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }

    public class RoleViewModel
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// DTO model for managing roles (with users)
    /// </summary>
    public class RoleViewModelWithUsers
    {
        public RoleViewModelWithUsers()
        {
            Users = new List<string>();
        }
        public string Name { get; set; }
        public string Id { get; set; }

        public IList<string> Users { get; set; }
    }

    public enum OrderByUserRole
    {
        NameAsc,
        NameDesc
    }

    public class BaseRoleUserModel
    {
        public BaseRoleUserModel()
        {
            ItemsPerPage = 50;
            PageNumber = 1;
        }

        public int ItemsPerPage { get; set; }
        public int PageNumber { get; set; }

        public string FilterByName { get; set; }

        public OrderByUserRole OrderBy { get; set; }
    }

    public class UsersViewModel : BaseRoleUserModel
    {
        public IPagedList<ApplicationUser> UsersPaged { get; set; }

    }

    /// <summary>
    /// Модель для редактирования пользователя
    /// </summary>
    public class EditUserViewModel
    {
        // не меняем поле, а только отображаем его
        //   [Required]
        //[Display(Name = "Имя пользователя (логин)")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Display(Name = "ФИО")]
        [StringLength(100)]
        public string ContactFIO { get; set; }

        [Display(Name = "Номер телефона")]
        [StringLength(12)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Партнер")]
        public bool IsPartner { get; set; }

        [StringLength(7)]
        public string PartnerId { get; set; }

        public int? PartnerPointId { get; set; }

        public int? DepartmentId { get; set; }

        public int? SupplierId { get; set; }


        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}
