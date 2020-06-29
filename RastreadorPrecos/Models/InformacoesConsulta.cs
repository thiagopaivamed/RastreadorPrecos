using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RastreadorPrecos.Models
{
    public class InformacoesConsulta
    {
        public int InformacoesConsultaId { get; set; }

        public int ProdutoId { get; set; }

        public double Valor { get; set; }

        public int Dia { get; set; }

        public int Mes { get; set; }

        public int Ano { get; set; }
    }
}
