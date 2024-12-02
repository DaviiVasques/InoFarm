using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InoFarm.Models
{
    public class FrutaModel
    {
        [Key]
        public int FrutaId { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string Descricao { get; set; }
        public string formaVenda { get; set; }
        public string ImagemUrl { get; set; }

        public int? GerenteId { get; set; } 
        public GerenteModel? Gerente { get; set; }   

    }
}