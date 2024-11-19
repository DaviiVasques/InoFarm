using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InoFarm.Models
{
    public class UsuarioModel
    {
        [Key]
        public int UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public int? GerenteId { get; set; } 
        public GerenteModel? Gerente { get; set; }
        public ICollection<CarrinhoModel>?  Carrinhos { get; set; }

    }
}