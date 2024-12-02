using Microsoft.AspNetCore.Mvc;
using InoFarm.Classes;
using InoFarm.Data;
using InoFarm.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace InoFarm.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(AppDbContext context, ILogger<UsuarioController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ---------------- CADASTRO ---------------- //

        public IActionResult Cadastro()
        {
            return View(new UsuarioModel());
        }

        [HttpPost]
        public IActionResult Cadastro(UsuarioModel usuario)
        {
            HttpContext.Session.SetObjectAsJson("Cadastro", usuario);
            return RedirectToAction("CadastroSegundaParte");
        }

        public IActionResult CadastroSegundaParte()
        {
            var cadastro = HttpContext.Session.GetObjectFromJson<UsuarioModel>("Cadastro");
            return View(cadastro);
        }

        [HttpPost]
        public IActionResult CadastroSegundaParte(UsuarioModel usuario, string ConfirmEmail, string ConfirmSenha)
        {
            var cadastro = HttpContext.Session.GetObjectFromJson<UsuarioModel>("Cadastro");
            if (cadastro == null)
            {
                ModelState.AddModelError(string.Empty, "Erro ao recuperar dados pessoais.");
                return View(usuario);
            }

            if (usuario.Email != ConfirmEmail)
                ModelState.AddModelError("Email", "Os e-mails não correspondem.");

            if (usuario.Senha != ConfirmSenha)
                ModelState.AddModelError("Senha", "As senhas não correspondem.");

            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
                ModelState.AddModelError("Email", "O email já está em uso.");

            if (!ModelState.IsValid)
                return View(usuario);

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
            var usuarioCompleto = new UsuarioModel
            {
                Nome = cadastro.Nome,
                Sobrenome = cadastro.Sobrenome,
                DataNascimento = cadastro.DataNascimento.ToUniversalTime(),
                CPF = cadastro.CPF,
                Telefone = cadastro.Telefone,
                Email = usuario.Email,
                Senha = usuario.Senha
            };

            try
            {
                _context.Usuarios.Add(usuarioCompleto);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar usuário no banco de dados.");
                ModelState.AddModelError(string.Empty, "Erro ao salvar os dados. Tente novamente.");
                return View(usuario);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.Senha))
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha incorretos.");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", usuario.UsuarioId);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }

        // ---------------- CARRINHO ---------------- //

        [HttpGet]
        public IActionResult VisualizarCarrinho()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
                return RedirectToAction("Login");

            var carrinho = _context.Carrinhos
                .Where(c => c.IdUsuario == userId)
                .Select(c => new
                {
                    c.CarrinhoId,
                    c.DataCriacao,
                    Itens = c.Itens.Select(i => new
                    {
                        i.carrinhoItemId,
                        i.Fruta.Nome,
                        i.Fruta.Preco,
                        i.Quantidade,
                        Total = i.Preco * i.Quantidade
                    })
                })
                .FirstOrDefault();

            if (carrinho == null)
                return View(new List<CarrinhoItemModel>());

            return View(carrinho.Itens);
        }

        [HttpPost]
        public IActionResult AdicionarAoCarrinho(int frutaId, int quantidade)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
                return RedirectToAction("Login");

            var fruta = _context.Frutas.FirstOrDefault(f => f.FrutaId == frutaId);
            if (fruta == null)
            {
                ModelState.AddModelError("", "Fruta não encontrada.");
                return RedirectToAction("VisualizarCarrinho");
            }

            var carrinho = _context.Carrinhos.FirstOrDefault(c => c.IdUsuario == userId);
            if (carrinho == null)
            {
                carrinho = new CarrinhoModel
                {
                    IdUsuario = userId,
                    DataCriacao = DateTime.Now,
                    Itens = new List<CarrinhoItemModel>()
                };
                _context.Carrinhos.Add(carrinho);
                _context.SaveChanges();
            }

            var item = carrinho.Itens.FirstOrDefault(i => i.IdFruta == frutaId);
            if (item == null)
            {
                item = new CarrinhoItemModel
                {
                    IdCarrinho = carrinho.CarrinhoId,
                    IdFruta = frutaId,
                    Quantidade = quantidade,
                    Preco = fruta.Preco
                };
                carrinho.Itens.Add(item);
            }
            else
            {
                item.Quantidade += quantidade;
            }

            _context.SaveChanges();
            return RedirectToAction("VisualizarCarrinho");
        }

        [HttpPost]
        public IActionResult EditarItemCarrinho(int itemId, int quantidade)
        {
            var item = _context.CarrinhoItens.FirstOrDefault(i => i.carrinhoItemId == itemId);
            if (item == null)
            {
                ModelState.AddModelError("", "Item não encontrado no carrinho.");
                return RedirectToAction("VisualizarCarrinho");
            }

            if (quantidade <= 0)
            {
                _context.CarrinhoItens.Remove(item);
            }
            else
            {
                item.Quantidade = quantidade;
            }

            _context.SaveChanges();
            return RedirectToAction("VisualizarCarrinho");
        }

        [HttpPost]
        public IActionResult RemoverItemCarrinho(int itemId)
        {
            var item = _context.CarrinhoItens.FirstOrDefault(i => i.carrinhoItemId == itemId);
            if (item == null)
            {
                ModelState.AddModelError("", "Item não encontrado no carrinho.");
                return RedirectToAction("VisualizarCarrinho");
            }

            _context.CarrinhoItens.Remove(item);
            _context.SaveChanges();

            return RedirectToAction("VisualizarCarrinho");
        }
        [HttpGet]
public IActionResult CarrinhoFinaliza()
{
    int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

    if (userId == 0)
        return RedirectToAction("Login");

    var carrinho = _context.Carrinhos
        .Where(c => c.IdUsuario == userId)
        .Select(c => new
        {
            c.CarrinhoId,
            c.DataCriacao,
            Total = c.Itens.Sum(i => i.Preco * i.Quantidade),
            Itens = c.Itens.Select(i => new
            {
                i.carrinhoItemId,
                i.Fruta.Nome,
                i.Fruta.Preco,
                i.Quantidade,
                TotalItem = i.Preco * i.Quantidade
            }).ToList()
        })
        .FirstOrDefault();

    if (carrinho == null || !carrinho.Itens.Any())
    {
        TempData["Mensagem"] = "Seu carrinho está vazio!";
        return RedirectToAction("VisualizarCarrinho");
    }

    return View(carrinho);
}

[HttpPost]
public IActionResult ProcessarPagamento(string formaPagamento)
{
    int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

    if (userId == 0)
        return RedirectToAction("Login");

    // Obter o carrinho atual do usuário
    var carrinho = _context.Carrinhos
        .Include(c => c.Itens)
        .ThenInclude(i => i.Fruta)
        .FirstOrDefault(c => c.IdUsuario == userId && !c.Finalizado);

    if (carrinho == null || !carrinho.Itens.Any())
    {
        ModelState.AddModelError("", "Carrinho vazio ou não encontrado.");
        return RedirectToAction("VisualizarCarrinho");
    }

    // Finalizar o carrinho
    carrinho.Finalizado = true;
    carrinho.DataFinalização = DateTime.Now;
    carrinho.FormaPagamento = formaPagamento;

    // Atualizar o estoque das frutas
    foreach (var item in carrinho.Itens)
    {
        var fruta = _context.Frutas.FirstOrDefault(f => f.FrutaId == item.IdFruta);
        if (fruta != null)
        {
            if (fruta.Quantidade >= item.Quantidade)
            {
                fruta.Quantidade -= item.Quantidade;
            }
            else
            {
                ModelState.AddModelError("", $"Estoque insuficiente para {fruta.Nome}.");
                return RedirectToAction("VisualizarCarrinho");
            }
        }
    }

    try
    {
        _context.SaveChanges();
        TempData["Sucesso"] = "Compra realizada com sucesso!";
        return RedirectToAction("Index", "Home");
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", "Erro ao processar pagamento. Tente novamente.");
        return RedirectToAction("VisualizarCarrinho");
    }
}



    }
}
