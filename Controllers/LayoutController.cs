using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ABPCodeGenerator.Controllers
{
    public class LayoutController : BaseController
    {
        public LayoutController(ILogger<BaseController> logger) : base(logger)
        {
        }
    }
}