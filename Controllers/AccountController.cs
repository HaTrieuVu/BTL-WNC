﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;
using System;
using BTL.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AspNetCoreHero.ToastNotification.Abstractions;
using BTL.ModelsViews;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BTL.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly BTLContext _context;
        public INotyfService _notyfService { get; }

        public AccountController(BTLContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        //public IActionResult ValidatePhone(string Phone)
        //{
        //    try
        //    {
        //        var khachhang = _context.Users.AsNoTracking()
        //                     .SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
        //        if (khachhang != null)
        //        {
        //            return Json(data: "Số điện thoại: " + Phone + " đã được sử dụng<br/>");
        //        }
        //        return Json(data: true);

        //    }
        //    catch
        //    {
        //        return Json(data: true);
        //    }
        //}

        //public IActionResult ValidateEmail(string Email)
        //{
        //    try
        //    {
        //        var khachhang = _context.Users.AsNoTracking()
        //                     .SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
        //        if (khachhang != null) {
        //            return Json(data: "Email: " + Email + " đã được sử dụng<br/>");
        //        }
        //        return Json(data: true);

        //    }
        //    catch {
        //        return Json(data: true);
        //    }
        //}


        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("UserId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Users.AsNoTracking()
                    .SingleOrDefault(x => x.UserId == Convert.ToInt32(taikhoanID));
                if(khachhang != null)
                {
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public async Task<IActionResult> Register(RegisterViewModel taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User user = new User()
                    {
                        FullName = taikhoan.FullName,
                        Phone = taikhoan.Phone,
                        Email = taikhoan.Email,
                        PasswordHash = taikhoan.Password,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        RoleId = 3
                    };
                    try
                    {
                        _context.Add(user);
                        await _context.SaveChangesAsync();

                        
                        //lưu session khách hàng
                        HttpContext.Session.SetString("UserId", user.UserId.ToString());
                        var taikhoanID = HttpContext.Session.GetString("UserId");

                        //Indentity
                        var claims = new List<Claim>
                        {

                            new Claim(ClaimTypes.Name, user.FullName),
                            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                            new Claim(ClaimTypes.Role, user.Role.RoleName),  // Lưu Role vào Claim
                            new Claim("CompanyId", user.CompanyId.ToString()) // Thêm CompanyId vào Claims
                        };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        _notyfService.Success("Đăng ký thành công!");

                        return RedirectToAction("Dashboard", "Account"); 

                    }
                    catch
                    {
                        return RedirectToAction("Register", "Account");
                    }
                }
                else
                {
                    return View(taikhoan);
                }
            }
            catch
            {
                return View(taikhoan);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public IActionResult Login(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("UserId");
            if (taikhoanID != null)
            {
                return RedirectToAction("Dashboard", "Account");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public async Task<IActionResult> Login(LoginViewModel user, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tìm người dùng trong cơ sở dữ liệu
                    var khachhang = _context.Users
                                        .AsNoTracking()
                                        .Include(x => x.Role)
                                        .FirstOrDefault(x => x.Email.Trim() == user.UserName.Trim());

                    if (khachhang == null) 
                    {
                        return RedirectToAction("Register", "Account");
                    }

                    string pass = user.Password.ToString();

                    if (khachhang.PasswordHash != pass)
                    {
                        _notyfService.Error("Tài khoản hoặc mật khẩu không chính xác!");
                        return View(user);
                    }

                    //kiểm tra khách hàng có bị disable không
                    if (khachhang.IsActive == false)
                    {
                        return RedirectToAction("ThongBao", "Account");
                    }

                    //Lưu Session KH
                    HttpContext.Session.SetString("UserId", khachhang.UserId.ToString());
                    HttpContext.Session.SetString("UserRole", khachhang.Role.RoleName);  // Lưu role nếu cần
                    HttpContext.Session.SetString("UserName", khachhang.FullName);
                    var taikhoanID = HttpContext.Session.GetString("UserId");

                    //Indentity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, khachhang.FullName),
                        new Claim(ClaimTypes.NameIdentifier, khachhang.UserId.ToString()),
                        new Claim(ClaimTypes.Role, khachhang.Role.RoleName),  // Lưu Role vào Claim
                        new Claim("CompanyId", khachhang.CompanyId.ToString()) // Thêm CompanyId vào Claims
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Đăng nhập và tạo cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    _notyfService.Success("Đăng nhập thành công!");

                    return RedirectToAction("Index", "Home");

                }
            }
            catch
            {
                return RedirectToAction("Register", "Account");
            }

            return View(user);
        }

        [HttpGet]
        [Route("dang-xuat.html", Name ="Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("UserId");

            return RedirectToAction("Index", "Home");
        }

        [Route("/hanchetruycap.html")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
