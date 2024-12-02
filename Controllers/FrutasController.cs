using Microsoft.AspNetCore.Mvc;
using InoFarm.Data;
using InoFarm.Models;

namespace InoFarm.Controllers
{
    public class FrutasController : Controller
    {
        private readonly AppDbContext _context;

        public FrutasController(AppDbContext context)
        {
            _context = context;
        }

        // Método para listar frutas
        public IActionResult ListaFrutas()
        {
            // Consulta todas as frutas do banco de dados
            var frutas = _context.Frutas.ToList();

            // Retorna a view com a lista de frutas
            return View(frutas);
        }
    }
}
