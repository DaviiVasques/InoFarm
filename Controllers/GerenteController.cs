using Microsoft.AspNetCore.Mvc;
using InoFarm.Data;
using InoFarm.Models;

namespace InoFarm.Controllers
{
    public class GerenteController : Controller
    {
        private readonly AppDbContext _context;

        public GerenteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult LoginCPF()
        {
            return View();
        }

        // Tela para inserção do CPF (POST)
        [HttpPost]
        public IActionResult LoginCPF(string cpf)
        {
            // Remove formatações do CPF
            cpf = cpf.Replace(".", "").Replace("-", "");

            // Validação de CPF
            if (!ValidarCPF(cpf))
            {
                ViewBag.Mensagem = "CPF inválido. Verifique o formato.";
                ViewBag.Erro = true;
                return View();
            }

            // Verifica se o CPF existe no banco de dados
            var gerente = _context.Gerentes.FirstOrDefault(g => g.Telefone == cpf);
            if (gerente != null)
            {
                // Armazena o CPF na sessão para a próxima etapa
                HttpContext.Session.SetString("GerenteCPF", cpf);
                return RedirectToAction("LoginSenha");
            }

            ViewBag.Mensagem = "CPF não encontrado.";
            ViewBag.Erro = true;
            return View();
        }

        // Tela para inserção da senha (GET)
        [HttpGet]
        public IActionResult LoginSenha()
        {
            // Certifica-se de que o CPF foi armazenado na sessão
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("GerenteCPF")))
            {
                return RedirectToAction("LoginCPF");
            }

            return View();
        }

        // Tela para inserção da senha (POST)
        [HttpPost]
        public IActionResult LoginSenha(string senha)
        {
            var cpf = HttpContext.Session.GetString("GerenteCPF");

            // Certifica-se de que o CPF foi armazenado na sessão
            if (string.IsNullOrEmpty(cpf))
            {
                return RedirectToAction("LoginCPF");
            }

            // Busca o gerente no banco de dados
            var gerente = _context.Gerentes.FirstOrDefault(g => g.Telefone == cpf && g.Senha == senha);
            if (gerente != null)
            {
                // Armazena o ID do gerente na sessão e redireciona para o Dashboard
                HttpContext.Session.SetInt32("GerenteId", gerente.GerenteId);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Mensagem = "Senha incorreta.";
            ViewBag.Erro = true;
            return View();
        }

        private bool ValidarCPF(string cpf)
        {
            if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
                return false;

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = (soma * 10) % 11;
            if (resto == 10 || resto == 11) resto = 0;

            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = (soma * 10) % 11;
            if (resto == 10 || resto == 11) resto = 0;

            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        public IActionResult Dashboard()
    {
        // Dados simulados - substitua com dados reais do banco de dados
        ViewBag.VendasAno = 35725.50;
        ViewBag.VendasMes = 5240.75;
        ViewBag.MargemBrutaMes = 3255.55;
        ViewBag.DespesasMes = 700.00;

        ViewBag.Frutas = new List<string> { "Manga", "Banana", "Morango", "Laranja" };

        ViewBag.FruitSectorData = new
        {
            mango = new { Governo = new List<double> { 1000, 1500, 1200, 1800, 1600, 1900, 2100, 2200, 2300, 2400, 2600, 2700 } },
            banana = new { Governo = new List<double> { 1200, 1600, 1400, 2000, 1800, 2100, 2300, 2400, 2500, 2600, 2800, 3000 } },
            strawberry = new { Governo = new List<double> { 800, 1000, 900, 1300, 1200, 1400, 1600, 1700, 1800, 1900, 2100, 2200 } },
            orange = new { Governo = new List<double> { 1500, 1800, 1600, 2300, 2100, 2500, 2700, 2900, 3000, 3200, 3400, 3600 } }
        };

        return View();
    }

    public IActionResult EntradaSaida()
{
    // Busque os registros de vendas do banco de dados
    var registros = _context.EntradaSaida.ToList(); // Altere para seu DbSet e tabela
    return View(registros);
}


        // Única Action para listar, adicionar, editar e excluir
       [HttpPost]
public IActionResult GerenciarProdutos(string actionType, int? id, [FromBody] FrutaModel model)
{
    if (actionType == "delete" && id.HasValue)
    {
        var fruta = _context.Frutas.Find(id.Value);
        if (fruta != null)
        {
            _context.Frutas.Remove(fruta);
            _context.SaveChanges();
        }
        return Ok(new { success = true, message = "Fruta excluída com sucesso!" });
    }

    if (actionType == "add")
    {
        if (ModelState.IsValid)
        {
            _context.Frutas.Add(model);
            _context.SaveChanges();
            return Ok(new { success = true, message = "Fruta adicionada com sucesso!" });
        }
        return BadRequest(new { success = false, message = "Dados inválidos!" });
    }

    if (actionType == "edit" && id.HasValue)
    {
        var fruta = _context.Frutas.Find(id.Value);
        if (fruta != null)
        {
            fruta.Nome = model.Nome;
            fruta.Descricao = model.Descricao;
            fruta.Preco = model.Preco;
            fruta.formaVenda = model.formaVenda;
            fruta.Quantidade = model.Quantidade;
            _context.SaveChanges();
            return Ok(new { success = true, message = "Fruta editada com sucesso!" });
        }
        return NotFound(new { success = false, message = "Fruta não encontrada!" });
    }

    return BadRequest(new { success = false, message = "Ação inválida!" });
}
    }
}
