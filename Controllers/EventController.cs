using AspNetCoreHero.ToastNotification.Abstractions;
using BTL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace BTL.Controllers
{
	public class EventController : Controller
    {
        private readonly BTLContext _db;
        public INotyfService _notyfService { get; }
        public EventController(BTLContext db, INotyfService notyfService)
        {
            _db = db;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public int GetManagedGroupId(int userId)
        {
            // Truy vấn cơ sở dữ liệu để lấy GroupId của người dùng
            var user = _db.Users
                               .Where(u => u.UserId == userId)
                               .FirstOrDefault();

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Trả về GroupId nếu người dùng có nhóm, hoặc giá trị mặc định nếu không có
            return user.GroupId ?? -1; // Trả về -1 nếu không có nhóm
        }
        // Kiểm tra nhóm được quản lý bởi Manager
        private bool IsManagedGroup(int? groupId, string managerId)
        {
            // Giả định: Có bảng Groups để lưu thông tin nhóm và manager
            return _db.GroupInCompanies.Any(g => g.GroupId == groupId && g.ManagerId == int.Parse(managerId));
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetEvents()
        {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;


            IQueryable<Event> events;

            if (role == "Admin")
            {
                // Admin: Xem tất cả sự kiện
                events = _db.Events;
            }
            else if (role == "Manager")
            {
                // Manager: Xem sự kiện trong nhóm/phòng ban
                var managedGroupId = GetManagedGroupId(int.Parse(userId)); // Hàm giả định để lấy GroupId
                events = _db.Events.Where(e => e.GroupId == managedGroupId);
            }
            else
            {
                // User: Xem sự kiện cá nhân hoặc được mời tham gia
                events = _db.Events.Where(e => e.UserId == int.Parse(userId) || e.Participants.Any(p => p.UserId == int.Parse(userId)));
            }

            return Ok(events.ToList());
            //using (BTLContext dc = new BTLContext())
            //{

            //var events = _db.Events
            //            .Where(e => role == "Admin" || e.UserId == int.Parse(userId))
            //            .ToList();

            //return Ok(events);


            //    //var events = dc.Events.ToList();
            //    ////return Json(events, JsonRequestBehavior.AllowGet);
            //    ////return Ok(events);
            //    //return Json(events);
            //}

        }

        [HttpPost]
        [Authorize]
        public IActionResult SaveEvent(Event e)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            bool status = false;
            try
            {
                using (BTLContext dc = new BTLContext())
                {
                    if (e.EventId > 0)
                    {
                        // Update the event
                        var v = dc.Events.Where(a => a.EventId == e.EventId).FirstOrDefault();
                        //if (v == null || (role != "Admin" && v.UserId != int.Parse(userId)))
                        //    return Unauthorized();

                        if (role == "User" && e.UserId != int.Parse(userId))
                            return Unauthorized();

                        if (role == "Manager" && !IsManagedGroup(e.GroupId, userId))
                            return Unauthorized();
                        //if (v != null)
                        //{
                            v.Subject = e.Subject;
                            v.Start = e.Start;
                            v.End = e.End;
                            v.Description = e.Description;
                            v.IsFullDay = e.IsFullDay;
                            v.ThemeColor = e.ThemeColor;
                        //}
                    }
                    else
                    {
                        // Khởi tạo đối tượng sự kiện
                        var newEvent = new Event
                        {
                            Subject = e.Subject,
                            Start = e.Start,
                            End = e.End,
                            Description = e.Description,
                            IsFullDay = e.IsFullDay,
                            ThemeColor = e.ThemeColor,
                            //UserId = e.UserId, // Chỉ có Admin/Manager có thể gán sự kiện cho người khác
                            GroupId = e.GroupId // Chỉ Manager/Admin gán cho nhóm
                            //UserId = role == "Admin" ? int.Parse(userId) : e.UserId
                        };

                        // Nếu người dùng không phải Admin hoặc Manager, chỉ có thể tạo sự kiện cá nhân
                        if (role != "Admin" && role != "Manager")
                        {
                            newEvent.UserId = int.Parse(userId); // Gán UserId của người dùng hiện tại vào sự kiện
                        }
                        else
                        {
                            newEvent.UserId = int.Parse(userId);
                            //newEvent.UserId = e.UserId; //Chỉ có Admin / Manager có thể gán sự kiện cho người khác
                            newEvent.GroupId = e.GroupId; // Nếu Admin hoặc Manager, có thể chọn GroupId
                        }

                        //if (role != "Admin" && role != "Manager")
                        //{
                        //    // User chỉ được tạo sự kiện cá nhân
                        //    e.UserId = int.Parse(userId);
                        //}
                        ////var newEvent = new Event
                        ////{
                        ////    Title = eventDto.Title,
                        ////    Description = eventDto.Description,
                        ////    Start = eventDto.Start,
                        ////    End = eventDto.End,
                        ////    Color = eventDto.Color,
                        ////    UserId = eventDto.UserId, // Chỉ có Admin/Manager có thể gán sự kiện cho người khác
                        ////    GroupId = eventDto.GroupId // Chỉ Manager/Admin gán cho nhóm
                        ////};
                        //var newEvent = new Event
                        //{
                        //    Subject = e.Subject,
                        //    Start = e.Start,
                        //    End = e.End,
                        //    Description = e.Description,
                        //    IsFullDay = e.IsFullDay,
                        //    ThemeColor = e.ThemeColor,
                        //    UserId = e.UserId, // Chỉ có Admin/Manager có thể gán sự kiện cho người khác
                        //    GroupId = e.GroupId // Chỉ Manager/Admin gán cho nhóm
                        //    //UserId = role == "Admin" ? int.Parse(userId) : e.UserId
                        //};
                        dc.Events.Add(newEvent);
                        //dc.Events.Add(e);  
                    }
                    dc.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = $"Đã xảy ra lỗi: {ex.Message}" });
            }

            return Json(new { status = status, message = status ? "Lưu sự kiện thành công!" : "Lưu sự kiện thất bại!" });
        }


        [HttpPost]
        public IActionResult DeleteEvent(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var status = false;
            Console.Write(id);
            var existingEvent = _db.Events.Where(a => a.EventId == id).FirstOrDefault();
            if (existingEvent == null)
            {
                status = false;
                return NotFound();
            }

            if (role == "User" && existingEvent.UserId != int.Parse(userId))
            {
                status = false;
                return Unauthorized();
            }

            if (role == "Manager" && !IsManagedGroup(existingEvent.GroupId, userId))
            {
                status = false;
                return Unauthorized();
            }
            _db.Events.Remove(existingEvent);
            _db.SaveChanges();
            status = true;

            //return Ok($"Event with ID {id} deleted.");
            return Json(new { status = status });

            //var status = false;
            //using(BTLContext dc = new BTLContext()) { 
            //    var v = dc.Events.Where(a => a.EventId == eventID).FirstOrDefault();
            //    if(v != null) {
            //        dc.Events.Remove(v);
            //        dc.SaveChanges();
            //        status = true;

            //    } 
            //}
            //return Json(new { status = status });
        }

        [HttpGet]
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            //var userId = HttpContext.Session.GetString("UserId");
            //var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            {
                return Unauthorized();  // Trả về Unauthorized nếu không có session
            }

            return Ok(new { userId, role });
        }


    }
}
