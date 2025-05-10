/*  Projeto: Rocket Turtle 

# INTEGRANTES

    Omar Junior Pereira da Costa 
    Eduardo Moreira de Oliveira 
    Murilo Prado Dias 


# PROGRESSO

(X) 5 tartarugas sendo 4 random e uma o player cria 
(X) Estratégias: desaceleração, pausas para descanso, etc 
(X) Linha de chegada, 0 a 100 metros 
(X) Paralelismo, uma thread por tartaruga, uma thread para exbir a corrida e talvez uma para fazer a logica acontecer 
(X) Usar theread explicitas
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Media;
using System.Threading;


namespace RocketTurtle // Todas as class nesse namespace
{
    internal class Program
    {
        // Tartaruga do jogador
        private static Turtle player = new Turtle();
        // private static Turtle vencedora = new Turtle();
        // Lista para armazenar as threads das tartarugas
        private static List<Thread> corredores = new List<Thread>();
        private static ConcurrentDictionary<int, string> classificacao = new ConcurrentDictionary<int, string>();
        private static ConcurrentDictionary<int, string> eventos = new ConcurrentDictionary<int, string>();
        // Lista de Tartarugas NPCs
        private static List<Turtle> videoAssistantReferee = new List<Turtle>();
        // Usado para sinalizar se a thread deve estar rodando (Set) ou pausada (Reset)
        // private static ManualResetEvent _pauseEvent = new ManualResetEvent(true);
        // Usado para sinalizar que a thread deve parar
        //private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static CancellationToken token = cts.Token;
        private static ManualResetEvent PauseEvent = new ManualResetEvent(true);

        static object cancellock = new object();


        // Objeto random
        private static Random random = new Random();


        static void Main(string[] args)
        {
            int quantidadeTartarugas = 4; // Quantidade de tartarugas a serem criadas
            Turtle vencedora = new Turtle();


            Console.WriteLine("--- ROCKET TURTLE ---");
            // PegarInformacoes();

            // Inicialmente será criado duas threads,
            // - Uma para o jogador (finaliza quando ele termina de digitar)
            // - E outra para gerar as tartarugas
            // A escolha dessa arquiterua foi considerando o programa como um jogo real, sendo assim...
            // Buscamos otimizar ao máximo a criação de NPC enquanto o Player escolhe um nome e a cor do seu personagem
            Thread criarJogador = new Thread(PegarInformacoes); // Thread para criar o jogador
            // Thread para criar tartarugas
            Thread criartartatugas = new Thread(() => CriarTartaruga(quantidadeTartarugas)); // Máximo 10 se não falta "coloração"

            criarJogador.Start();
            criartartatugas.Start();

            // Espera a thread terminar antes de continuar
            criarJogador.Join();
            criartartatugas.Join();

            
            /*
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
            */


            // Criamos uma thread para a atualização do placar
            Thread placar = new Thread(AtualizarPlacar);
            placar.Start();

            // Foi criado uma thread para cada tartaruga quando criamos a tartaruga
            // A tartaruga do jogador também foi inclusa na lista de Threads "corredoras"
            // Dessa forma temos 5 threads rodando em paralelo para os corredores + uma para o placar
            // O foreach é usado para inicializar as threads
            foreach (Thread t in corredores)
                t.Start(); // Espera todas as threads terminarem



            foreach (Thread t in corredores)
                t.Join(); // Espera todas as threads terminarem

            placar.Join(); // Espera pelo placar final

            AtualizarPlacar();

            vencedora = videoAssistantReferee[0]; // Inicializa a vencedora com a primeira tartaruga da lista

            if (videoAssistantReferee.Count > 1)
            {
                Console.WriteLine($"\nA corrida foi acirrada,precisou até de V.A.R.");
                foreach (Turtle tf in videoAssistantReferee) // TF = tartaruga finalista 
                {
                    Console.WriteLine($"Tartaruga: {tf.Nome} chegou com {tf.Descanso}s de descanso.");
                    if (tf.Descanso >= vencedora.Descanso)
                    {
                        vencedora = tf;  // Atualiza a vencedora se encontrar uma tartaruga com menos tempo de descanso
                    }
                }
            }
            if (vencedora.Nome == player.Nome)
                Console.WriteLine($"\nParabéns {(player.Nome).ToUpper()}, você venceu a corrida!");
            else
                Console.WriteLine($"\nÉ uma pena, você perdeu!!!\nA tartaruga {(vencedora.Nome).ToUpper()} venceu a corrida!");

            Console.WriteLine(vencedora.ExibirInfo());


            //Console.ReadKey(); // FIM
        }

        static void PegarInformacoes() // Criar o player + sua thread
        {
            // player.Nome = player.GerarNome();
            Console.Write("Qual o nome da sua tartaruga?: ");
            player.Nome = Console.ReadLine();
            // player.Cor = "Dourado como o alvorecer";
            Console.Write("Como ela é? (refência cores e detalhes): ");
            player.Cor = Console.ReadLine();
            // Os atributos abaixo estão fixo para agilizar a demonstração,
            // Mas pode ser random ou pedir o usuário digitar e depois converter para o tipo correto
            player.Comprimento = 23.0f; 
            player.Peso = 2.0f;
            player.Posicao = 0; // Posição inicial, sempre 0
            player.Descanso = 0; // Sempre 0, a medida que descansa vai acumulando
            player.Resistencia = RandomFloat(10, 51); // Tempo de correr sem cansar

            // Cria uma thread para o jogador e adiciona na lista de corredores
            Thread tPlayer = new Thread(() => Corrida(player)); // Usando método direto
            corredores.Add(tPlayer);
        }

        static void CriarTartaruga(int quantidade) // Método para criar tartarugas + threads
        {
            for (int i = 0; i < quantidade; i++)
            {
                // Cria uma nova tartaruga com atributos random
                Turtle tartaruga = new Turtle();
                tartaruga.Nome = tartaruga.GerarNome();
                tartaruga.Cor = tartaruga.GerarColoracao();
                tartaruga.Peso = RandomFloat(0.200f, 10); // Unidade (KG)
                tartaruga.Comprimento = RandomFloat(5, 100);// Unidade (CM)
                tartaruga.Posicao = 0; // Posição inicial
                tartaruga.Descanso = 0;
                tartaruga.Resistencia = RandomFloat(10, 31);

                // Cria uma thread para cada tartaruga
                Thread t = new Thread(() => Corrida(tartaruga)); // Usando método direto
                corredores.Add(t);

            }
        }

        static float RandomFloat(float min, float max) // Método para gerar um número random entre dois valores
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        private static void Corrida(Turtle corredor) // Método para a corrida (logica do programa)
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            int tempoDescanso = 0;
            int intervencaoDivina = 0;
            // Números "Divinos" em Cultura Pop ou Religião
            List<int> numerosDivinos = new List<int> { 3, 7, 12, 33, 42, 108 };

            while (true)
            {
                if (!cts.IsCancellationRequested)
                {
                    //Console.WriteLine($"\tDeveria cancelar {corredor.Nome}");
                    //break;

                    corredor.Posicao += random.Next(1, 6); // Adiciona entre 1 e 5 metros
                    ProgressoCorrida(id, corredor.Posicao, corredor.Nome);
                    AtualizarEventos(id, $"{(corredor.Nome).ToUpper()} corre como o vento...");
                    //Console.WriteLine($"Tartaruga: {corredor.Nome} ({id}) | Progresso: {corredor.Posicao}%");


                    // Redução de resistência
                    if (corredor.Posicao % 2 == 0) // Se a posicao for PAR remove 2 de resistencia
                        corredor.Resistencia = corredor.Resistencia - 2;
                    else // Se a posicao for IMPAR remove 3 de resistencia
                        corredor.Resistencia = corredor.Resistencia - 3;

                    // Evento de DESCANSO
                    if (corredor.Resistencia <= 0) // Se a resistencia for menor ou igual a 0
                    {
                        // Console.WriteLine($"Tartaruga: {corredor.Nome} ({id}) está cansada.");
                        AtualizarEventos(id, $"{(corredor.Nome).ToUpper()} está cansada.");
                        corredor.Resistencia = RandomFloat(10, 51); // Reinicia a resistencia
                        // Console.WriteLine($"Tartaruga {corredor.Nome} ({id}) descansou.");
                        tempoDescanso = random.Next(1, 5); // Descanso entre 1 e 5 segundos
                        Thread.Sleep(tempoDescanso * 500); // 500 = 0,5s * Tempo de descanso, sendo um total de 2.5s
                        corredor.Descanso += tempoDescanso;
                    }

                    // Evento de desaceleração
                    intervencaoDivina = random.Next(1, 500); // 1 a 500 para não ficar muito frenquênte
                    if (numerosDivinos.Contains(intervencaoDivina)) // Se o número for divino
                    {


                        //Console.WriteLine($"Tartaruga: {corredor.Nome} ({id}) está desacelerando sem motivo...");
                        AtualizarEventos(id, $"{(corredor.Nome).ToUpper()} está desacelerando sem motivo...");
                        Thread.Sleep(intervencaoDivina * 100); // 1 segundo de desaceleração
                    }
                    // Controla o intervalo de tempo entre normal da thread
                    // Os demais eventos geram um tempo extra assim fazendo com que a tartaruga demore mais para chegar
                    Thread.Sleep(1000); // Mudar para 1000 = 1s
                }
                else
                {
                    ProgressoCorrida(id, corredor.Posicao, (corredor.Nome).ToUpper());
                    AtualizarEventos(id, $"{(corredor.Nome).ToUpper()} correu {corredor.Posicao} metros.");
                    break;
                }

                if (corredor.Posicao >= 100)
                {
                    corredor.Posicao = 100;
                    videoAssistantReferee.Add(corredor);
                    cts.Cancel(); // Garantir que apenas uma tartaruga chame o cancel

                    //lock (cancellock)
                    //{
                    // cts.Cancel();
                    //Thread.Sleep(100);
                    //}

                    AtualizarEventos(id, $"Tartaruga: {corredor.Nome} ({id}) chegou a {corredor.Posicao} metros.");
                    ProgressoCorrida(id, corredor.Posicao, corredor.Nome);
                    break;
                }
                //ProgressoCorrida(id, corredor.Posicao, corredor.Nome);
            }
            // Console.WriteLine($"Tartaruga: {corredor.Nome} ({id}) chegou a {corredor.Posicao} metros.");
            //ProgressoCorrida(id, corredor.Posicao, corredor.Nome);
            //AtualizarEventos(id, $"{corredor.Nome} ({id}) chegou a {corredor.Posicao} metros.");

        }

        static void ProgressoCorrida(int id, int posicao, string nome)
        {
            string progresso = "[";

            // Explicitly cast 'posicao' to double to resolve ambiguity  
            int running = (int)(posicao / 2);           // Trunca naturalmente
            int idle = 50 - running;

            for (int i = 0; i < running; i++)
                progresso += ":";

            for (int j = 0; j < idle; j++)
                progresso += "_";

            progresso += $"] {posicao}% | {(nome).ToUpper()}";
                        
            progresso = $"[{id}] {progresso}";
            classificacao[id] = progresso;
        }

        static void AtualizarEventos(int id, string msg) // Atualiza o que acontece com a tartaruga
        {
            string log = $"[{id}] {msg}";
            eventos[id] = log;
        }

        static void AtualizarPlacar()
        {
            do // O DO While foi usado para garantir que seja iniciado pelo menos uma vez, mesmo se o sinal de cancelamento for disparado
            {
                Console.Clear();
                Console.WriteLine("Classificação em tempo 'real' (x100)\n");

                foreach (var c in classificacao.Values)
                    Console.WriteLine(c);

                Console.WriteLine("\nNarração (FLASH, bicho-preguiça da Zootopia):\n");

                foreach (var e in eventos.Values)
                    Console.WriteLine(e);

                Thread.Sleep(800);
            } while (!cts.IsCancellationRequested);
        }
    }


}

