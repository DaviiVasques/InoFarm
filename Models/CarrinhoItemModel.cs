using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InoFarm.Models
{
    public class CarrinhoItemModel
    {
        [Key]
        public int carrinhoItemId { get; set; }
        public int IdCarrinho { get; set; } // FK para o Carrinho
        public int IdFruta { get; set; } // FK para a Fruta
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
        public CarrinhoModel Carrinho { get; set; }
        public FrutaModel Fruta { get; set; }
    }
}