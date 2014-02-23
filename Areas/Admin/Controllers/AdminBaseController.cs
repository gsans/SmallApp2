using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmallApp2.Areas.Admin.Controllers
{    
    [Authorize] //sets all controllers and actions to be from authorised users
    public class AdminBaseController : Controller
    {
    }
}