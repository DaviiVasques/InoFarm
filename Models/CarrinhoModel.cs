using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InoFarm.Models
{
    public class CarrinhoModel
    {
        [Key]
        public int CarrinhoId { get; set; }
        public int IdUsuario { get; set; }
        public DateTime DataCriacao { get; set; }
        public UsuarioModel Usuario { get; set; }
        public ICollection<CarrinhoItemModel> Itens { get; set; }
    }
}