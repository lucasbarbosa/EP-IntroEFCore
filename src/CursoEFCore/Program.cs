using CursoEFCore.Domain;
using CursoEFCore.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CursoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            #region CriarBanco 1

            // PM > Update-Database
            // PM > Update-Database PrimeiraMigracao

            #endregion

            #region CriarBanco 2

            //using var db = new Data.ApplicationContext();
            //db.Database.Migrate();

            #endregion

            #region Migrações pendentes

            //using var db = new Data.ApplicationContext();
            //var existeMigracaoPendente = db.Database.GetPendingMigrations().Any();
            //if (existeMigracaoPendente)
            //{
            //    // Executa regra
            //}

            #endregion

            #region Operações

            //InserirDados();
            //InserirDadosEmMassa();
            ConsultarDados();
            //CadastrarPedido();
            //ConsultarPedidoCarregamentoAdiantado();
            //AtualizarDados();
            //RemoverRegistro();

            #endregion

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }

        #region Private Methods

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste 5",
                CodigoBarras = "1234567891015",
                Valor = 50m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = false
            };

            using var db = new Data.ApplicationContext();
            db.Produtos.Add(produto);
            db.Set<Produto>().Add(produto);
            db.Entry(produto).State = EntityState.Added;
            db.Add(produto);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registro(s): {registros}");
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Rafael Almeida",
                CEP = "99999000",
                Cidade = "Itabaiana",
                Estado = "SE",
                Telefone = "99000001111",
            };

            var listaClientes = new[]
            {
                new Cliente
                {
                    Nome = "Teste 1",
                    CEP = "99999000",
                    Cidade = "Itabaiana",
                    Estado = "SE",
                    Telefone = "99000001115",
                },
                new Cliente
                {
                    Nome = "Teste 2",
                    CEP = "99999000",
                    Cidade = "Itabaiana",
                    Estado = "SE",
                    Telefone = "99000001116",
                },
            };


            using var db = new Data.ApplicationContext();
            //db.AddRange(produto, cliente);
            //db.Set<Cliente>().AddRange(listaClientes);
            db.Clientes.AddRange(listaClientes);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registro(s): {registros}");
        }

        private static void ConsultarDados()
        {
            using var db = new Data.ApplicationContext();
            var consultaPorSintaxe = (from c in db.Clientes where c.Id > 0 select c).ToList();
            var consultaPorMetodo = db.Clientes
                //.AsNoTracking() // Não buscar os dados em memória, e sim direto no banco.
                .Where(p => p.Id > 0)
                .OrderBy(p => p.Id)
                .ToList();

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando Cliente: {cliente.Id}");
                
                //db.Clientes.Find(cliente.Id); // Primeiro busca o que está em memória
                db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);
            }
        }

        private static void CadastrarPedido()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                 {
                     new PedidoItem
                     {
                         ProdutoId = produto.Id,
                         Desconto = 0,
                         Quantidade = 1,
                         Valor = 10,
                     }
                 }
            };

            db.Pedidos.Add(pedido);

            db.SaveChanges();
        }

        private static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();
            var pedidos = db
                .Pedidos
                //.Include("Itens")
                .Include(p => p.Itens)
                    .ThenInclude(p => p.Produto)
                .ToList();

            Console.WriteLine(pedidos.Count);
        }

        private static void AtualizarDados()
        {
            using var db = new Data.ApplicationContext();

            #region Update 1

            //var cliente = db.Clientes.Find(1);
            //cliente.Nome = "Cliente Alterado Passo 1";
            //db.Clientes.Update(cliente); // Altera todas as colunas

            #endregion

            #region Update 2

            //var cliente = db.Clientes.FirstOrDefault(p => p.Id == 1);
            //cliente.Nome = "Cliente Alterado Passo 2";

            #endregion

            #region Update 3

            //var cliente = db.Clientes.FirstOrDefault(p => p.Id == 1);
            //cliente.Nome = "Cliente Alterado Passo 3";
            //db.Entry(cliente).State = EntityState.Modified;

            #endregion

            #region Update 4

            var cliente = new Cliente
            {
                Id = 1
            };

            var clienteDesconectado = new
            {
                Nome = "Cliente Desconectado Passo 4",
                Telefone = "7966669999"
            };

            db.Attach(cliente);
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);

            #endregion

            db.SaveChanges();
        }

        private static void RemoverRegistro()
        {
            using var db = new Data.ApplicationContext();

            #region Delete 1

            //var cliente = db.Clientes.Find(2);
            ////db.Clientes.Remove(cliente);
            //db.Remove(cliente);

            #endregion

            #region Delete 2

            var cliente = new Cliente { Id = 3 };
            db.Entry(cliente).State = EntityState.Deleted;

            #endregion

            db.SaveChanges();
        }

        #endregion
    }
}