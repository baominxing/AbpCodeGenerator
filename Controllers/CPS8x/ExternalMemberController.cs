using System;
using System.Collections.Generic;
using ABPCodeGenerator.Filters;
using ABPCodeGenerator.Services;
using ABPCodeGenerator.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ABPCodeGenerator.Controllers.CPS8x
{
    public class ExternalMemberController : BaseController
    {
        private readonly ICPS8xCodeGeneratorService templateService;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ExternalMemberController(
            ICPS8xCodeGeneratorService templateService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<BaseController> logger) : base(logger)
        {
            this.templateService = templateService;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View("~/Views/CPS8x/ExternalMember/Index.cshtml");
        }
    }
}