/*  Projeto: Rocket Turtle 

# INTEGRANTES

    Omar Junior Pereira da Costa 
    Eduardo Moreira de Oliveira 
    Murilo Prado Dias 


# PROGRESSO

(X) 5 tartarugas sendo 4 random e uma o player cria 
( ) Estratégias: desaceleração, pausas para descanso, etc 
( ) Linha de chegada, 0 a 100 metros 
( ) Paralelismo, uma thread por tartaruga, uma thread para exbir a corrida e talvez uma para fazer a logica acontecer 
( ) Usar theread explicitas
( ) Interface com usuário


# REGRAS 

- 5 tartarugas 
- Velocidade random 1 e 5 metros por segundo 
- Enventos de desacelerar e pausar devem ser aleatorios 
- Deve exibir as informações da tartaruga vencedora (qua alcanar 100 metros primeiro) 
- Deve contabilizar o tempo de descanso, pois em empate a tartaruga que tiver maior tempo de descanso 


# Arquitetura
    sku     =  .NETFramework 
    Version =  v4.8

# Class
    Program : Main Class
    Turtle  : Objecto Tartaruga e seus métodos

 */



// Imports
using System;
using System.Collections.Generic;
using System.Media;
using System.Security.AccessControl;
using System.Threading;

namespace RocketTurtle // Todas as class nesse namespace
{
    internal class Program
    {
        // Tartaruga do jogador
        private static Turtle player = new Turtle();
        // Lista para armazenar as threads das tartarugas
        private static List<Thread> corredores = new List<Thread>();
        // Lista de Tartarugas NPCs
        private static List<Turtle> turtles = new List<Turtle>();
        // Usado para sinalizar se a thread deve estar rodando (Set) ou pausada (Reset)
        //private static ManualResetEvent _pauseEvent = new ManualResetEvent(true);
        // Usado para sinalizar que a thread deve parar
        //private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        // Objeto random
        private static Random random = new Random();


        static void Main(string[] args)
        {

            Console.WriteLine("--- ROCKET TURTLE ---");
            PegarInformacoes();









            /*do
            {
            // Exibir informações           
            Console.WriteLine(player.ExibirInfo());
            
                CriarTartaruga(3);
                foreach (Turtle npc in turtles) 
                { 
                    Console.WriteLine(npc.ExibirInfo());
                
                }
            //} while (true);*/

            Console.ReadKey(); // FIM
        }

        static void PegarInformacoes()
        {
            /*
            player.Nome = player.GerarNome();
            player.Cor = "..o";
            player.Comprimento = 23.0f;
            player.Peso = 2.0f;*/

            // Console.Clear();
            Console.Write("Qual o nome da sua tartaruga?: ");
            player.Nome = Console.ReadLine();
            Console.Write("Como ela é? (refência cores e detalhes): ");
            player.Cor = Console.ReadLine();
            player.Comprimento = 23.0f;
            player.Peso = 2.0f;
        }

        static void CriarTartaruga(int quantidade)
        {
            for (int i = 0; i < quantidade; i++)
            {
                Turtle tartaruga = new Turtle();
                tartaruga.Nome = tartaruga.GerarNome();
                tartaruga.Cor = tartaruga.GerarColoracao();
                tartaruga.Peso = RandomFloat(0.200f, 10); // Unidade (KG)
                tartaruga.Comprimento = RandomFloat(5, 100);// Unidade (CM)

                turtles.Add(tartaruga);

            }

        }

        static float RandomFloat(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        private static void Corrida(object parametros)
        {
            /*ParametrosFuncionario paramsFunc = (ParametrosFuncionario)parametros;
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
            */
        }


    }
}
