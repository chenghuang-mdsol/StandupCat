using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using StandupAggragation.Core.Models;
using StandupAggragation.Core.Services;

namespace StandupAggregation.Web.Controllers
{
    public class HipChatHistoryController : Controller
    {
        // GET: HipChatHistory
        
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.TableData = GetTableData();
            if (System.Web.HttpContext.Current != null)
            {
                string name = System.Web.HttpContext.Current.User.Identity.Name;
                HipChatLoginService service = new HipChatLoginService();
                var userInfo = service.GetUser(name);
                ViewBag.CurrentUser = userInfo;
            }
            return View();
        }
        private IList<IStandupMessage> GetTableData()
        {
            var key = ConfigurationManager.AppSettings["AuthTokens.Admin"];
            var room = ConfigurationManager.AppSettings["RoomName"];
            var bot = ConfigurationManager.AppSettings["BotName"];
            IStandupService service = new StandupService(key);
            var result = service.GetAllStandupHistory(room, bot);
            return result;
        }
    }
}