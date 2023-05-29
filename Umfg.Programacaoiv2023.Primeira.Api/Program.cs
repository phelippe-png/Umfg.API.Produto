using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Umfg.Programacaoiv2023.RestFulProduto.Api;

namespace UUmfg.Programacaoiv2023.RestFulProduto.Api
{
    public class Program
    {
        private static List<Produto> _lista = new List<Produto>()
        {
            new Produto("COCA COLA 350ML", 0000000000003, 4,99),
            new Produto("GUARAN� ANTARCTICA", 000004322368, 4,29),
        };

        public static void Main(string[] args)
        {
            var app = WebApplication.Create(args);

            app.MapGet("api/v1/produto", ObterTodosProdutosAsync);
            app.MapGet("api/v1/produto/{id}", ObterProdutoPorIdAsync);
            app.MapPost("api/v1/produto", CadastrarProdutoAsync);
            app.MapPut("api/v1/produto/{id}", AtualizarProdutoAsync);
            app.MapDelete("api/v1/produto", RemoverTodosProdutosAsync);
            app.MapDelete("api/v1/produto/{id}", RemoverProdutoAsync);

            app.Run();
        }

        public static async Task ObterTodosProdutosAsync(HttpContext context)
        {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsJsonAsync(_lista);
        }

        public static async Task ObterProdutoPorIdAsync(HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out Guid id))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Par�metro id n�o foi enviado! Verifique.");
                return;
            }

            var produto = _lista.FirstOrDefault(x => x.Id == id);

            if (produto == null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync($"Produto n�o encontrado para o id: {id}. Verifique.");
                return;
            }

            context.Response.StatusCode = 200;
            await context.Response.WriteAsJsonAsync(produto);
        }

        public static async Task CadastrarProdutoAsync(HttpContext context)
        {
            var produto = await context.Request.ReadFromJsonAsync<Produto>();

            if (produto == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("N�o foi possivel cadastrar o produto! Verifique.");
                return;
            }

            _lista.Add(produto);

            context.Response.StatusCode = (int)HttpStatusCode.Created;
            await context.Response.WriteAsJsonAsync(produto);
        }

        public static async Task AtualizarProdutoAsync(HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out Guid id))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("Par�metro id n�o foi enviado! Verifique.");

                return;
            }

            var produto = _lista.FirstOrDefault(x => x.Id == id);

            if (produto == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync($"Produto n�o encontrado para o id: {id}. Verfique.");

                return;
            }

            _lista.Remove(produto);

            produto.Descricao = (await context.Request.ReadFromJsonAsync<Produto>()).Descricao;

            _lista.Add(produto);

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsJsonAsync(produto);
        }

        public static async Task RemoverTodosProdutosAsync(HttpContext context)
        {
            _lista.Clear();

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync("Todos os produtos foram removidos com sucesso!");
        }

        public static async Task RemoverProdutoAsync(HttpContext context)
        {
            if (!context.Request.RouteValues.TryGet("id", out Guid id))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("Par�metro id n�o foi enviado! Verifique.");

                return;
            }

            var produto = _lista.FirstOrDefault(x => x.Id == id);

            if (produto == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync($"Produto n�o encontrado para o id: {id}. Verifique.");

                return;
            }

            _lista.Remove(produto);
            await context.Response.WriteAsync("Produto removido com sucesso!");
        }
    }
}