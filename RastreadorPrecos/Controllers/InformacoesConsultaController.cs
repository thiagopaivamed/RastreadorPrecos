using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using RastreadorPrecos.Models;

namespace RastreadorPrecos.Controllers
{
    public class InformacoesConsultaController : Controller
    {
        private readonly string _connectionString = "User Id=postgres;Password=admin;Host=localhost;Port=5432;Database=RastreadorPrecos";
        private readonly NpgsqlConnection _conexao;

        public InformacoesConsultaController()
        {
            _conexao = new NpgsqlConnection(_connectionString);
        }

        [HttpGet]
        public IActionResult NovaInformacao(int produtoId)
        {
            InformacoesConsulta ic = new InformacoesConsulta
            {
                ProdutoId = produtoId
            };
            return View(ic);
        }

        [HttpPost]
        public async Task<IActionResult> NovaInformacao(InformacoesConsulta ic)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string insercaoQuery = "INSERT INTO InformacoesConsulta(ProdutoId, Valor, Dia, Mes, Ano) VALUES(@ProdutoId, @Valor, @Dia, @Mes, @Ano)";
                    NpgsqlCommand comando = new NpgsqlCommand(insercaoQuery, _conexao);
                    comando.CommandType = CommandType.Text;

                    comando.Parameters.AddWithValue("@ProdutoId", ic.ProdutoId);
                    comando.Parameters.AddWithValue("@Valor", ic.Valor);
                    comando.Parameters.AddWithValue("@Dia", ic.Dia);
                    comando.Parameters.AddWithValue("@Mes", ic.Mes);
                    comando.Parameters.AddWithValue("@Ano", ic.Ano);

                    await _conexao.OpenAsync();
                    await comando.ExecuteNonQueryAsync();
                    return RedirectToAction("DetalhesProduto", "Produtos", new { produtoId = ic.ProdutoId });
                }

                return View(ic);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await _conexao.CloseAsync();
            }
        }
    }
}
