/******************************************************************************
#Projeto: 
   Rocket Turtle 

# Author
    Omar Costa <omarcosta152@gmail.com>
    
# Arquitetura
    sku     :  .NETFramework 
    Version :  v4.8
    Objetivo:  A escolha dessa arquiterua foi considerando o programa como um jogo real, 
               sendo assim, buscamos otimizar ao máximo a criação de NPC enquanto o Player escolhe um nome e a cor do seu personagem

# Class
    Program : Lógica da corrida de tartarugas
    Turtle  : Objecto Tartaruga e seus métodos

*******************************************************************************/

// Imports
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;


namespace RocketTurtle // Todas as class nesse namespace
{
    class Program
    {

        private static Turtle player = new Turtle(); // Tartaruga do jogador
        private static List<Thread> corredores = new List<Thread>(); // Lista para armazenar as threads das tartarugas
        private static List<Turtle> videoAssistantReferee = new List<Turtle>(); // Lista de Tartarugas NPCs que chegaram a 100 durante a corrida
        private static ConcurrentDictionary<int, string> classificacao = new ConcurrentDictionary<int, string>(); // Dicionario listar o progresso da corrida
        private static ConcurrentDictionary<int, string> eventos = new ConcurrentDictionary<int, string>(); // Dicionario para LOG do que cada corredora faz
        private static CancellationTokenSource cts = new CancellationTokenSource(); // Usado para sinalizar que a thread deve parar
        private static ManualResetEvent PauseEvent = new ManualResetEvent(true); // Usado para sinalizar se a thread deve estar rodando (Set) ou pausada (Reset)
        private static Random random = new Random(); // Objeto random
        private static readonly List<int> numerosDivinos = new List<int> { 3, 7, 12, 33, 42, 108 }; // Números "Divinos" em Cultura Pop ou Religião
        

        static void Main(string[] args)
        {
            int quantidadeTartarugas = 4; // Quantidade de tartarugas a serem criadas

            Console.WriteLine("--- ROCKET TURTLE ---");

            // Inicialmente será criado duas threads,
            // 1) Uma para o jogador (finaliza quando ele termina de digitar)
            // 2) Gerar as tartarugas de formar random e cria uma thread para cada uma
            Thread criarJogador = new Thread(PegarInformacoes); // Thread para criar o jogador
            Thread criartartatugas = new Thread(() => CriarTartaruga(quantidadeTartarugas)); // Thread para criar tartarugas, máximo 10 se não falta "coloração"

            criarJogador.Start(); // Enquanto o jogador digitar, a outra thread faz um "preload" da tartarugas
            criartartatugas.Start();

            criarJogador.Join();
            criartartatugas.Join();

            Introducao();

            /* Foi criado uma thread para cada tartaruga quando criamos a tartaruga e adicionadas a lista "corredoras"
            A tartaruga do jogador também foi inclusa na lista de Threads "corredoras"
            Dessa forma teremos: 
                > 5 ou N threads rodando em paralelo para representando os corredores
                > 1 thread para as atualizações do placar
                > A thread Main controlando os eventos de pausa, retomada e cancelamento*/

            // O foreach é usado para inicializar as threads dos corredores
            foreach (Thread t in corredores)
                t.Start(); // Espera todas as threads terminarem

            // Criamos uma thread para a atualização do placar
            Thread placar = new Thread(AtualizarPlacar);
            placar.Start();


            // Thread MAIN que controla eventos de Pausa, Retomada e Cancelamento das demais Threads          
            while (!cts.Token.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.P)
                    {
                        PauseEvent.Reset();
                        Thread.Sleep(150); // Aguarda um pouco para garantir que a pausa foi aplicada
                        Console.ForegroundColor = ConsoleColor.Red; // Muda a cor
                        Console.WriteLine("\n[!ALERTA]: A corrida foi pausada por motivos da administração.");
                        Console.ResetColor(); // Reseta a cor

                    }
                    else if (key == ConsoleKey.R)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n[!ALERTA]: Acabou a moleza, bora correr...");
                        Console.ResetColor();
                        Thread.Sleep(1000); // Aguarda 1s para retomar a corrida
                        PauseEvent.Set();
                    }
                    else if (key == ConsoleKey.F)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[!ERROR]: COMO ASSIM???\nA corrida foi cancelada produção?");
                        Thread.Sleep(1000); // Aguarda 1s para cancelar
                        Console.ResetColor();
                        cts.Cancel();
                        break; // Sai do loop principal
                    }
                }
                // Pequeno sleep para evitar que o loop consuma 100% da CPU enquanto espera por input
                Thread.Sleep(50);
            }


            placar.Join(); // Espera pelo placar final
            foreach (Thread t in corredores)
                t.Join(); // Espera todas as threads terminarem

            AtualizarPlacar(); // Garantir que o placar seja atualizado depois que todas as threads terminarem

            Turtle vencedora = new Turtle(); // Cria uma nova tartaruga para armazenar a vencedora

            if (videoAssistantReferee.Count != 0) // Se a corrida for forçada a finalizar essa lista ficará vazia
            {
                vencedora = videoAssistantReferee[0]; // Inicializa a vencedora com a primeira tartaruga da lista

                if (videoAssistantReferee.Count > 1)
                {
                    Console.WriteLine($"\nUAUU!!\nA corrida foi acirrada,precisou até de V.A.R.");
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
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nParabéns {(player.Nome).ToUpper()}, você venceu a corrida!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nÉ uma pena, você perdeu!!!\nA tartaruga {(vencedora.Nome).ToUpper()} venceu a corrida!");
                }

                Console.WriteLine(vencedora.ExibirInfo());
            }
            else // Se a corrida for cancelada, não houve vencedores
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[!ALERTA]: A corrida foi cancelada, não houve vencedores.");
            }


            Console.ReadKey(); // FIM
        }

        static void PegarInformacoes() // Criar o player + sua thread
        {
            Console.Write("Qual o nome da sua tartaruga?: ");
            player.Nome = Console.ReadLine();
            Console.Write("Como ela é? (refência cores e detalhes): ");
            player.Cor = Console.ReadLine();

            /* Os atributos abaixo estão fixo para agilizar a demonstração,
            Mas pode ser random ou pedir o usuário digitar e depois converter para o tipo correto */
            player.Comprimento = 23.0f;
            player.Peso = 2.0f;
            player.Posicao = 6; // Posição inicial, sempre 0, mas vamos dar uma moral para o Player e deixar um passo na frente :)
            player.Descanso = 0; // Sempre 0, a medida que descansa vai acumulando
            player.Resistencia = RandomFloat(15, 51); // Tempo de correr sem cansar

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
                Thread t = new Thread(() => Corrida(tartaruga)); // Criando a thread e passando a tartaruga como parâmetro
                corredores.Add(t); // Adiciona a thread na lista de corredores

            }
        }

        static float RandomFloat(float min, float max) // Método para gerar um número random entre dois valores
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        static void Corrida(Turtle corredor) // Método para a corrida (logica do programa)
        {

            int id = Thread.CurrentThread.ManagedThreadId;
            int tempoDescanso = 0;
            int intervencaoDivina = 0;

            while (true) // A corrida
            {
                if (!cts.IsCancellationRequested) // Se não teve sinal de cancelamento
                {
                    PauseEvent.WaitOne(); // Pausa a execução da thread se o evento estiver pausado
                    corredor.Posicao += random.Next(1, 6); // Adiciona entre 1 e 5 metros
                    ProgressoCorrida(id, corredor.Posicao, corredor.Nome);
                    AtualizarEventos(id, $"{(corredor.Nome).ToUpper()} corre como o vento...");

                    // Redução de resistência
                    if (corredor.Posicao % 2 == 0) // Se a posicao for PAR remove 2 de resistencia
                        corredor.Resistencia = corredor.Resistencia - 2;
                    else // Se a posicao for IMPAR remove 3 de resistencia
                        corredor.Resistencia = corredor.Resistencia - 3;

                    // Evento de DESCANSO
                    if (corredor.Resistencia <= 0) // Se a resistencia for menor ou igual a 0
                    {
                        AtualizarEventos(id, $"{(corredor.Nome).ToUpper()} está cansada.");
                        corredor.Resistencia = RandomFloat(10, 51); // Reinicia a resistencia
                        tempoDescanso = random.Next(1, 6); // Descanso entre 1 e 5 segundos
                        Thread.Sleep(tempoDescanso * 500); // 500 = 0,5s * Tempo de descanso, sendo um total de 2.5s
                        corredor.Descanso += tempoDescanso;
                    }

                    // Evento de desaceleração
                    intervencaoDivina = random.Next(1, 500); // 1 a 500 para não ficar muito frenquênte
                    if (numerosDivinos.Contains(intervencaoDivina)) // Se o número for divino
                    {
                        AtualizarEventos(id, $"{(corredor.Nome).ToUpper()} está desacelerando sem motivo...");
                        Thread.Sleep(1000); // 1s de parada, que considerando o intervalo de movimentação da impressão que a velocidade foi reduzida pela metade.
                    }

                    // Tempo padrão entre para aumentar a posição
                    Thread.Sleep(1000); // Os eventos geram um tempo extra assim fazendo com que a tartaruga demore mais para chegar
                }
                else // Se teve sinal de cancelamento 
                {
                    //ProgressoCorrida(id, corredor.Posicao, (corredor.Nome).ToUpper());
                    //AtualizarEventos(id, $"{(corredor.Nome).ToUpper()} correu {corredor.Posicao} metros.");
                    break;
                }

                if (corredor.Posicao >= 100) // A(s) tartaruga(s) vencedoras (chegou a 100 primeiro)
                {
                    corredor.Posicao = 100;
                    videoAssistantReferee.Add(corredor);
                    cts.Cancel(); // Emite o sinal para cancelar
                    // lock (cancellock) ProgressoCorrida(id, corredor.Posicao, (corredor.Nome).ToUpper());
                    //ProgressoCorrida(id, corredor.Posicao, corredor.Nome);
                    //AtualizarEventos(id, $"Tartaruga: {corredor.Nome} ({id}) chegou a {corredor.Posicao} metros.");
                    break;
                }

            } // Fim da corrida

            ProgressoCorrida(id, corredor.Posicao, corredor.Nome);
            AtualizarEventos(id, $"Tartaruga: {corredor.Nome} ({id}) chegou a {corredor.Posicao} metros.");
        }

        static void ProgressoCorrida(int id, int posicao, string nome)
        {

            string progresso = "[";

            // Explicitly cast 'posicao' to double to resolve ambiguity  
            int running = (int)(posicao / 2);  // Trunca naturalmente
            int idle = 50 - running;

            for (int i = 0; i < running; i++)
                progresso += ":";

            for (int j = 0; j < idle; j++)
                progresso += "_";

            progresso += $"] {posicao}% | {(nome).ToUpper()}";

            progresso = $"[{id}] {progresso}";
            classificacao[id] = progresso;
        }

        static void AtualizarEventos(int id, string msg)
        { // Atualiza o que acontece com a tartaruga
            string log = $"[{id}] {msg}";
            eventos[id] = log;
        }

        static void Introducao()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n==> A grande corrida vai começar:..");
            Console.WriteLine("==> Ajuste seu casco, e se prepare...");
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\t> 3...");
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\t> 2...");
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\t> 1...");
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\t> GGOOOooo...");
            Console.ResetColor();
            Thread.Sleep(1000);
        }

        static void AtualizarPlacar()
        {
            do // O DO While foi usado para garantir que seja iniciado pelo menos uma vez, mesmo se o sinal de cancelamento for disparado
            {
                PauseEvent.WaitOne(); // Pausa a execução da thread se o evento estiver pausado
                Console.Clear();
                Console.WriteLine("Classificação em tempo 'real' (x100)\n");

                Console.BackgroundColor = ConsoleColor.DarkBlue;

                foreach (var c in classificacao.Values)
                    Console.WriteLine(c);

                Console.ResetColor(); // Reseta a cor

                Console.WriteLine("\nNarração (FLASH, bicho-preguiça da Zootopia):\n");

                foreach (var e in eventos.Values)
                    Console.WriteLine(e);

                Console.WriteLine("\n[Poderes da ADMINISTRAÇÃO] \nDigite: \n\t'P' para Pausar \n\t'R' para Retomar \n\t'F' para Finalizar.");
                Thread.Sleep(800);
            } while (!cts.IsCancellationRequested);
        }
    }


}

