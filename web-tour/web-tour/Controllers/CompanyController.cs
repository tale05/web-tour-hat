using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_tour.Entities;
using web_tour.Models;

namespace web_tour.Controllers
{
    public class CompanyController : Controller
    {
        private readonly DulichhatComDbtravelContext _context;

        public CompanyController(DulichhatComDbtravelContext context)
        {
            _context = context;
        }

        
    }
}