using Microsoft.AspNetCore.Mvc;
using InoFarm.Classes;
using InoFarm.Data;
using InoFarm.Models;
using Microsoft.Extensions.Logging;

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

        // Tela de Dados Pessoais
        public IActionResult DadosPessoais()
        {
            return View(new UsuarioModel());
        }

        [HttpPost]
        public IActionResult DadosPessoais(UsuarioModel usuario)
        {
            // Armazene os dados temporariamente na sessão, independentemente da validação
            HttpContext.Session.SetObjectAsJson("DadosPessoais", usuario);

            return RedirectToAction("DadosAcesso");
        }

        // Tela de Dados de Acesso
        public IActionResult DadosAcesso()
        {
            var dadosPessoais = HttpContext.Session.GetObjectFromJson<UsuarioModel>(
                "DadosPessoais"
            );
            return View(dadosPessoais);
        }

        [HttpPost]
        public IActionResult DadosAcesso(UsuarioModel usuario, string ConfirmEmail, string ConfirmSenha)
        {
            Console.WriteLine("Iniciando método DadosAcesso (POST)");

            // Exibe os erros do ModelState logo no início do método
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState inválido ao iniciar o método. Erros:");
                foreach (var modelStateEntry in ModelState)
                {
                    foreach (var error in modelStateEntry.Value.Errors)
                    {
                        Console.WriteLine($"Erro no campo {modelStateEntry.Key}: {error.ErrorMessage}");
                    }
                }
            }

            // Recupera os dados pessoais da sessão
            var dadosPessoais = HttpContext.Session.GetObjectFromJson<UsuarioModel>("DadosPessoais");
            if (dadosPessoais == null)
            {
                Console.WriteLine("Erro: Dados pessoais não encontrados na sessão");
                ModelState.AddModelError(string.Empty, "Erro ao recuperar dados pessoais.");
                return View(usuario); // Reenviar o formulário com erro
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState válido");

                if (usuario.Email != ConfirmEmail)
                {
                    Console.WriteLine("Erro: E-mails não correspondem");
                    ModelState.AddModelError("Email", "Os e-mails não correspondem.");
                }

                if (usuario.Senha != ConfirmSenha)
                {
                    Console.WriteLine("Erro: Senhas não correspondem");
                    ModelState.AddModelError("Senha", "As senhas não correspondem.");
                }

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState inválido após validação de e-mail e senha");
                    return View(usuario); // Reenviar o formulário com erros
                }

                Console.WriteLine("ModelState permanece válido após validação de e-mail e senha");

                // Combine os dados pessoais com os dados de acesso
                var usuarioCompleto = new UsuarioModel
                {
                    Nome = dadosPessoais.Nome,
                    Sobrenome = dadosPessoais.Sobrenome,
                    DataNascimento = dadosPessoais.DataNascimento.ToUniversalTime(), // Converter para UTC
                    CPF = dadosPessoais.CPF,
                    Telefone = dadosPessoais.Telefone,
                    Email = usuario.Email,
                    Senha = usuario.Senha,
                };

                // Tente salvar no banco de dados
                try
                {
                    Console.WriteLine("Tentando adicionar usuário ao banco de dados");
                    _context.Usuarios.Add(usuarioCompleto);
                    _context.SaveChanges();
                    Console.WriteLine("Usuário salvo com sucesso no banco de dados");
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao salvar no banco de dados: {ex.Message} {(ex.InnerException?.Message ?? string.Empty)}");
                    // Registrar o erro no log e adicionar detalhes ao ModelState
                    _logger.LogError(ex, "Erro ao salvar usuário no banco de dados");
                    ModelState.AddModelError(
                        string.Empty,
                        $"Erro ao salvar no banco de dados: {ex.Message} {(ex.InnerException?.Message ?? string.Empty)}"
                    );
                }
            }
            else
            {
                Console.WriteLine("ModelState inválido ao iniciar o método");
            }

            Console.WriteLine("Retornando view com erros no ModelState");
            return View(usuario); // Reenviar o formulário com erros
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            // Verifica se o ModelState é válido
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Busca o usuário no banco de dados pelo email
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            // Verifica se o usuário existe e a senha está correta
            if (usuario == null || usuario.Senha != senha)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha incorretos.");
                return View();
            }

            // Armazena o ID do usuário na sessão para autenticação
            HttpContext.Session.SetInt32("UserId", usuario.UsuarioId);

            // Redireciona para uma página inicial ou dashboard após o login bem-sucedido
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            // Remove o ID do usuário da sessão para deslogar
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }
    }
}
