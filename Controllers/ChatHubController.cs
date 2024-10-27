using AspNetCoreHero.ToastNotification.Abstractions;
using BTL.Models;
using BTL.ModelsViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BTL.Controllers
{
    public class ChatHubController : Controller
    {
        private readonly BTLContext _context;
        public INotyfService _notyfService { get; }

        public ChatHubController(BTLContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [Route("chat.html")]
        public IActionResult Index()
        {
            var currentUserId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(currentUserId);

            var khachhang = _context.Users.AsNoTracking()
                .SingleOrDefault(x => x.UserId == userId);

            if (khachhang != null)
            {
                ViewBag.UserProfile = khachhang;

                // Lấy danh sách các cuộc trò chuyện gần đây của người dùng hiện tại
                var recentChats = _context.Messages
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .AsEnumerable()
                    .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                    .Select(g => new RecentChatModel
                    {
                        // Lấy người tham gia cuộc trò chuyện (người nhận hoặc người gửi)
                        User = _context.Users.FirstOrDefault(u => u.UserId == g.Key),
                        // Lấy tin nhắn cuối cùng của cuộc trò chuyện
                        LastMessage = g.OrderByDescending(m => m.SentAt).FirstOrDefault(),
                        // Đếm số tin nhắn chưa đọc từ người dùng hiện tại
                        UnreadCount = g.Count(m => m.ReceiverId == userId && !m.IsRead)
                    })
                    .ToList();

                // Lấy danh sách các nhóm mà người dùng tham gia
                var userGroups = _context.GroupMembers
                    .Where(gm => gm.UserId == userId)
                    .Select(gm => new GroupModel
                    {
                        GroupName = gm.Group.GroupName,
                        UnreadCount = _context.Messages
                            .Where(m => m.GroupId == gm.GroupId && m.SenderId != userId && !m.IsRead)
                            .Count() // Đếm số lượng tin nhắn chưa đọc trong group
                    })
                    .ToList();

                var viewModel = new ChatSidebarViewModel
                {
                    RecentChats = recentChats,
                    Groups = userGroups
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

    }
}
