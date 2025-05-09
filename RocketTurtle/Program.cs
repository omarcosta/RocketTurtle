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
using System.Runtime.CompilerServices;
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
        public CancellationToken CancellationToken { get; set; }
        public ManualResetEvent PauseEvent { get; set; }

        // Objeto random
        private static Random random = new Random();


        static void Main(string[] args)
        {

            Console.WriteLine("--- ROCKET TURTLE ---");
            // PegarInformacoes();

            // Inicialmente será criado duas threads, uma para o jogador e outra para as tartarugas
            // A escolha dessa arquiterua foi considerando o programa como um jogo real, e assim,
            // Otmizando ao máximo a criação de NPC enquanto o Player escolhe um nome e a cor do seu personagem
            Thread criarJogador = new Thread(PegarInformacoes); // Thread para criar o jogador
            Thread criartartatugas = new Thread(() => CriarTartaruga(4)); // Thread para criar tartarugas

            criarJogador.Start();
            criartartatugas.Start();

            // Espera a thread terminar antes de continuar
            criarJogador.Join(); 
            criartartatugas.Join();

            // Exibir informações           
            /*Console.WriteLine(player.ExibirInfo());            
            foreach (Turtle npc in turtles)
                Console.WriteLine(npc.ExibirInfo());*/
            


            Console.WriteLine("==> A grande corrida vai começar:..");
            Console.WriteLine("==> Ajuste seu casco, e se prepare...");
            Thread.Sleep(1000);
            Console.WriteLine("\t> 3...");
            Thread.Sleep(1000);
            Console.WriteLine("\t> 2...");
            Thread.Sleep(1000);
            Console.WriteLine("\t> 1...");
            Thread.Sleep(1000);
            Console.WriteLine("\t> Vaiii...");
            Thread.Sleep(1000);

            foreach (Thread t in corredores)
                t.Start(); // Espera todas as threads terminarem

            foreach (Thread t in corredores)
                t.Join(); // Espera todas as threads terminarem



            Console.ReadKey(); // FIM
        }

        static void PegarInformacoes()
        {
            
            // Console.Clear();
            //player.Nome = player.GerarNome();
            Console.Write("Qual o nome da sua tartaruga?: ");
            player.Nome = Console.ReadLine();
            //player.Cor = "Dourado como o alvorecer";
            Console.Write("Como ela é? (refência cores e detalhes): ");
            player.Cor = Console.ReadLine();
            player.Comprimento = 23.0f;
            player.Peso = 2.0f;
            player.Posicao = 0; // Posição inicial
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ManualResetEvent pauseEvent = new ManualResetEvent(true); // Começa não pausado]
            player.CancellationToken = cancellationTokenSource.Token;
            player.PauseEvent = pauseEvent;

            Thread tPlayer = new Thread(() => Corrida(player)); // Usando método direto
            corredores.Add(tPlayer);
        }

        static void CriarTartaruga(int quantidade)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ManualResetEvent pauseEvent = new ManualResetEvent(true); // Começa não pausado]

            for (int i = 0; i < quantidade; i++)
            {
                Turtle tartaruga = new Turtle();
                tartaruga.Nome = tartaruga.GerarNome();
                tartaruga.Cor = tartaruga.GerarColoracao();
                tartaruga.Peso = RandomFloat(0.200f, 10); // Unidade (KG)
                tartaruga.Comprimento = RandomFloat(5, 100);// Unidade (CM)
                tartaruga.Posicao = 0; // Posição inicial
                tartaruga.CancellationToken = cancellationTokenSource.Token;
                tartaruga.PauseEvent = pauseEvent;

                turtles.Add(tartaruga);

            }

            foreach (Turtle npc in turtles)
            {
                Thread t = new Thread(() => Corrida(npc)); // Usando método direto
                corredores.Add(t);
            }

        }

        static float RandomFloat(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        private static void Corrida(Turtle corredor)
        {
            while (corredor.Posicao <= 100) 
            {
                if (corredor.CancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine($"\tDeveria cancelar {corredor.Nome}");
                    break;
                }

                corredor.Posicao += random.Next(1, 6); // Adiciona entre 1 e 5 metros
                Console.WriteLine($"Tartaruga: {corredor.Nome} ({Thread.CurrentThread.ManagedThreadId}) | Progresso: {corredor.Posicao}% | {100-corredor.Posicao} metros da chegada.");
                Thread.Sleep(1000);

            }
        Console.WriteLine($"Tartaruga {corredor.Nome} ({Thread.CurrentThread.ManagedThreadId}) chegou a {corredor.Posicao} metros.");

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
