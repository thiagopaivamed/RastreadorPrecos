using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RastreadorPrecos.Models;
using System.Data;

namespace RastreadorPrecos.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly string _connectionString = "User Id=postgres;Password=admin;Host=localhost;Port=5432;Database=RastreadorPrecos";
        private readonly NpgsqlConnection _conexao;

        public ProdutosController()
        {
            _conexao = new NpgsqlConnection(_connectionString);
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                List<Produto> listaProdutos = new List<Produto>();
                string selecaoQuery = "SELECT * FROM produtos";

                NpgsqlCommand comando = new NpgsqlCommand(selecaoQuery, _conexao);
                comando.CommandType = CommandType.Text;

                await _conexao.OpenAsync();
                NpgsqlDataReader leitorDados = await comando.ExecuteReaderAsync();

                while (await leitorDados.ReadAsync())
                {
                    Produto produto = new Produto();
                    produto.ProdutoId = Convert.ToInt32(leitorDados["ProdutoId"]);
                    produto.Nome = leitorDados["Nome"].ToString();
                    produto.LinkProduto = leitorDados["LinkProduto"].ToString();
                    listaProdutos.Add(produto);
                }

                return View(listaProdutos);
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

        [HttpGet]
        public IActionResult NovoProduto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NovoProduto(Produto produto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string insercaoQuery = "INSERT INTO Produtos(Nome, LinkProduto) VALUES(@Nome, @LinkProduto)";
                    NpgsqlCommand comando = new NpgsqlCommand(insercaoQuery, _conexao);
                    comando.CommandType = CommandType.Text;

                    comando.Parameters.AddWithValue("@Nome", produto.Nome);
                    comando.Parameters.AddWithValue("@LinkProduto", produto.LinkProduto);

                    await _conexao.OpenAsync();
                    await comando.ExecuteNonQueryAsync();

                    return RedirectToAction(nameof(Index));
                }

                return View(produto);
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

        [HttpGet]
        public async Task<IActionResult> AtualizarProduto(int produtoId)
        {
            try
            {
                string selecaoQuery = String.Format("SELECT * FROM Produtos WHERE ProdutoId = {0}", produtoId);
                NpgsqlCommand comando = new NpgsqlCommand(selecaoQuery, _conexao);
                Produto produto = new Produto();

                await _conexao.OpenAsync();
                NpgsqlDataReader leitorDados = await comando.ExecuteReaderAsync();

                while (await leitorDados.ReadAsync())
                {
                    produto.ProdutoId = Convert.ToInt32(leitorDados["ProdutoId"]);
                    produto.Nome = leitorDados["Nome"].ToString();
                    produto.LinkProduto = leitorDados["LinkProduto"].ToString();
                }

                return View(produto);

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

        [HttpPost]
        public async Task<IActionResult> AtualizarProduto(int produtoId, Produto produto)
        {
            try
            {
                if (produtoId != produto.ProdutoId)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    string atualizacaoQuery = "UPDATE Produtos SET Nome = @Nome, LinkProduto = @LinkProduto WHERE ProdutoId = @ProdutoId";
                    NpgsqlCommand comando = new NpgsqlCommand(atualizacaoQuery, _conexao);
                    comando.CommandType = CommandType.Text;

                    comando.Parameters.AddWithValue("@ProdutoId", produto.ProdutoId);
                    comando.Parameters.AddWithValue("@Nome", produto.Nome);
                    comando.Parameters.AddWithValue("@LinkProduto", produto.LinkProduto);

                    await _conexao.OpenAsync();
                    await comando.ExecuteNonQueryAsync();

                    return RedirectToAction(nameof(Index));
                }

                return View(produto);
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

        [HttpPost]
        public async Task<IActionResult> ExcluirProduto(int produtoId)
        {
            try
            {
                string exclusãoQuery = "DELETE FROM Produtos WHERE ProdutoId = @ProdutoId";
                NpgsqlCommand comando = new NpgsqlCommand(exclusãoQuery, _conexao);
                comando.CommandType = CommandType.Text;

                comando.Parameters.AddWithValue("@ProdutoId", produtoId);

                await _conexao.OpenAsync();
                await comando.ExecuteNonQueryAsync();
                return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> DetalhesProduto(int produtoId)
        {
            try
            {
                ViewData["ProdutoId"] = produtoId;
                List<InformacoesConsulta> listaIC = new List<InformacoesConsulta>();
                string selecaoQuery = String.Format("SELECT Valor, Dia, Mes, Ano FROM InformacoesConsulta WHERE ProdutoId = {0}", produtoId);
                NpgsqlCommand comando = new NpgsqlCommand(selecaoQuery, _conexao);

                await _conexao.OpenAsync();
                NpgsqlDataReader leitorDados = await comando.ExecuteReaderAsync();

                while(await leitorDados.ReadAsync())
                {
                    InformacoesConsulta ic = new InformacoesConsulta();
                    ic.Valor = Convert.ToDouble(leitorDados["Valor"]);
                    ic.Dia = Convert.ToInt32(leitorDados["Dia"]);
                    ic.Mes = Convert.ToInt32(leitorDados["Mes"]);
                    ic.Ano = Convert.ToInt32(leitorDados["Ano"]);
                    listaIC.Add(ic);
                }

                return View(listaIC);
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

        public async Task<JsonResult> DadosGrafico(int produtoId, int diaInicio, int diaFim, int mes, int ano)
        {
            try
            {
                List<InformacoesConsulta> listaIC = new List<InformacoesConsulta>();
                string selecaoQuery = String.Format("SELECT Valor, Dia, Mes, Ano " +
                    "FROM InformacoesConsulta " +
                    "WHERE (ProdutoId = {0} AND Ano = {1} AND Mes = {2} AND (Dia >= {3} AND Dia <= {4}))", produtoId, ano, mes, diaInicio, diaFim);
                NpgsqlCommand comando = new NpgsqlCommand(selecaoQuery, _conexao);

                await _conexao.OpenAsync();
                NpgsqlDataReader leitorDados = await comando.ExecuteReaderAsync();

                while(await leitorDados.ReadAsync())
                {
                    InformacoesConsulta ic = new InformacoesConsulta();
                    ic.Valor = Convert.ToDouble(leitorDados["Valor"]);
                    ic.Dia = Convert.ToInt32(leitorDados["Dia"]);
                    ic.Mes = Convert.ToInt32(leitorDados["Mes"]);
                    ic.Ano = Convert.ToInt32(leitorDados["Ano"]);
                    listaIC.Add(ic);
                }

                return Json(listaIC);
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
