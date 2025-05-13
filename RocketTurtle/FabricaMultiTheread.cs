using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RocketTurtle
{
    /*internal class FabricaMultithread
    {
        // Estrutura simples para passar parâmetros para a thread do funcionário
        public class ParametrosFuncionario
        {
            public string Nome { get; set; }
            public int TempoTrabalhoSegundos { get; set; }
            public string MensagemInicio { get; set; }

            public CancellationToken CancellationToken { get; set; }
            public ManualResetEvent PauseEvent { get; set; }


        }

        static void Main(string[] args)
        {
            Console.WriteLine("--- Gerenciador da Fábrica Multithread ---");

            // Lista para armazenar as threads dos funcionários (para usar o Join depois)
            List<Thread> funcionarios = new List<Thread>();


            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ManualResetEvent pauseEvent = new ManualResetEvent(true); // Começa não pausado



            var paramsFunc1 = new ParametrosFuncionario
            {
                Nome = "João",
                TempoTrabalhoSegundos = 5,
                CancellationToken =
                cancellationTokenSource.Token,
                PauseEvent = pauseEvent,
                MensagemInicio = "Número 1 - João"
            };
            Thread funcionario1 = new Thread(Trabalhar); // Usando método direto
            funcionarios.Add(funcionario1);


            var paramsFunc2 = new ParametrosFuncionario
            {
                Nome = "Maria",
                TempoTrabalhoSegundos = 7,
                CancellationToken =
                cancellationTokenSource.Token,
                PauseEvent = pauseEvent,
                MensagemInicio = "Número 2 - Maria"
            };
            Thread funcionario2 = new Thread(new ThreadStart(() => Trabalhar(paramsFunc2))); // Usando ThreadStart com lambda
            funcionarios.Add(funcionario2);


            var paramsFunc3 = new ParametrosFuncionario
            {
                Nome = "Pedro",
                TempoTrabalhoSegundos = 6,
                CancellationToken =
                cancellationTokenSource.Token,
                PauseEvent = pauseEvent,
                MensagemInicio = "Número 3 - Pedro"
            };
            Thread funcionario3 = new Thread(new ParameterizedThreadStart(Trabalhar)); // Usando ParameterizedThreadStart
            funcionarios.Add(funcionario3);

            var paramsFunc4 = new ParametrosFuncionario
            {
                Nome = "Omar",
                TempoTrabalhoSegundos = 1,
                CancellationToken =
                cancellationTokenSource.Token,
                PauseEvent = pauseEvent,
                MensagemInicio = "Ah, Omar! Funcionário do mês"
            };
            Thread funcionario4 = new Thread(Trabalhar); // Usando método direto
            funcionarios.Add(funcionario4);


            // Inicie as threads criadas.
            Console.WriteLine("Iniciando o turno dos funcionários...");

            funcionario1.Start(paramsFunc1);
            funcionario2.Start(); // Para este exemplo, o lambda já passa os parâmetros. Se usar ParameterizedThreadStart, passe aqui: funcionarioX.Start(parametros);
            funcionario3.Start(paramsFunc3);
            funcionario4.Start(paramsFunc4);


            Console.WriteLine("\nTurno em andamento. Digite 'P' para Pausar, 'R' para Retomar, 'F' para Finalizar.");

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.P)
                    {
                        Console.WriteLine("\n[GERENTE]: Pausando o trabalho...");

                        pauseEvent.Reset();
                    }
                    else if (key == ConsoleKey.R)
                    {
                        Console.WriteLine("\n[GERENTE]: Retomando o trabalho...");

                        pauseEvent.Set();
                    }
                    else if (key == ConsoleKey.F)
                    {
                        Console.WriteLine("\n[GERENTE]: Fim do expediente. Sinalizando para finalizar...");

                        cancellationTokenSource.Cancel();
                        break; // Sai do loop principal
                    }
                }
                // Pequeno sleep para evitar que o loop consuma 100% da CPU enquanto espera por input
                Thread.Sleep(50);
            }


            Console.WriteLine("\n[GERENTE]: Aguardando todos os funcionários terminarem...");

            foreach (var funcThread in funcionarios)
            {
                funcThread.Join();
                Console.WriteLine($"[GERENTE]: Funcionário {funcThread.ManagedThreadId} terminou.");
            }

            Console.WriteLine("\n[GERENTE]: Todos os funcionários terminaram o expediente e a fábrica está fechando. Pressione Enter para sair.");
            Console.ReadLine();
        }

        // Método que simula o trabalho de um funcionário.
        // Será executado por cada thread de funcionário.
        private static void Trabalhar(object parametros)
        {
            ParametrosFuncionario paramsFunc = (ParametrosFuncionario)parametros;
            string nome = paramsFunc.Nome;
            int tempoTotalSegundos = paramsFunc.TempoTrabalhoSegundos;
            string msg = paramsFunc.MensagemInicio;
            CancellationToken cancellationToken = paramsFunc.CancellationToken;
            ManualResetEvent pauseEvent = paramsFunc.PauseEvent;

            Random rnd = new Random();

            Console.WriteLine($"Funcionário {nome} ({Thread.CurrentThread.ManagedThreadId}) começou a trabalhar em uma tarefa que leva {tempoTotalSegundos} segundos." +
                $"\t\n{msg}");


            for (int i = 0; i < tempoTotalSegundos; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine($"Funcionário {nome} ({Thread.CurrentThread.ManagedThreadId}) " +
                        $"recebeu sinal de cancelamento e vai parar.");
                    break;
                }

                pauseEvent.WaitOne(); // Bloqueia se o evento estiver 'reset' (false)


                Console.WriteLine($"Funcionário {nome} ({Thread.CurrentThread.ManagedThreadId}) completou etapa {i + 1}/{tempoTotalSegundos}.");
                // Simula o tempo de trabalho para esta etapa
                int tempo = rnd.Next(500, 1501);
                Thread.Sleep(tempo);
                // Thread.Sleep(1000); // 1 segundo por etapa
            }

            //Console.WriteLine($"Funcionário {nome} ({Thread.CurrentThread.ManagedThreadId}) terminou sua tarefa ou foi cancelado.");
            Console.WriteLine($"Funcionário {nome} parou devido ao fim do expediente.");
        }

    }*/
}
