using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InoFarm.Models;
using InoFarm.Data;

namespace InoFarm.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var frutas = _context.Frutas.Take(5); // Obtenha a lista de frutas do banco de dados
        return View(frutas);
    }

    public IActionResult QuemSomos()
    {
        return View();
    }
}
