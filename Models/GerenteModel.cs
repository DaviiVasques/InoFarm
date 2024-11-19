using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InoFarm.Models
{
    public class GerenteModel
    {
        [Key]
        public int GerenteId { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Telefone { get; set; }
        public DateTime DataAdmissao { get; set; }
        public ICollection<FrutaModel> FrutasGerenciadas { get; set; }
        public ICollection<UsuarioModel> UsuariosGerenciados { get; set; }

    }
}