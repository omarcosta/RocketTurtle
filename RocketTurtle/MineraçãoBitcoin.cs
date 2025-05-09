using System;
using System.Threading;

public class ThreadMineControl
{
    // Usado para sinalizar se a thread deve estar rodando (Set) ou pausada (Reset)
    private static ManualResetEvent _pauseEvent = new ManualResetEvent(true);
    // Usado para sinalizar que a thread deve parar
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    public static int gold = 0;

    public static void Main(string[] args)
    {
        Console.WriteLine("Criando e iniciando a Mineração...");

        // Cria uma nova thread
        Thread miningThread = new Thread(DoMining);

        Console.WriteLine("Thread de trabalho iniciada. Pressione:");
        Console.WriteLine(" 'I' para Iniciar"); // ok
        Console.WriteLine(" 'P' para Pausar"); // ok
        Console.WriteLine(" 'R' para Resumir"); // ok
        Console.WriteLine(" 'F' para Finalizar"); // ok
        Console.WriteLine(" 'S' para Pausar");
        Console.WriteLine(" 'E' para sair"); //ok
        
        // Exec
        
        bool threadExec = true;
        
        char command;
        do
        {
            Console.WriteLine("\nPressione 'I' para iniciar o trabalho...");
            //command = Console.ReadKey(true).KeyChar;
            command = Console.ReadKey().KeyChar;
            
            
            if (command == 'I' || command == 'i')
            {
                // Inicia a thread
                miningThread.Start();
            }
            else if (command == 'E' || command == 'e')
            {
                // Finaliza
                threadExec = false;
            }
            else if (command == 'S' || command == 's')
            {
                // Finaliza
                //threadExec = false;
                Console.WriteLine($"\nThread de trabalho: Contagem = {gold}\nStatus: {miningThread.ThreadState}");
                
            }
            else if (command == 'P' || command == 'p')
            {
                Console.WriteLine("\nSolicitando Pausar...");
                // Sinaliza para a thread pausar (reseta o evento)
                _pauseEvent.Reset();
            }
            else if (command == 'R' || command == 'r')
            {
                Console.WriteLine("\nSolicitando Resumir...");
                // Sinaliza para a thread continuar (seta o evento)
                _pauseEvent.Set();
            }
            else if (command == 'F' || command == 'f')
            {
                Console.WriteLine("\nSolicitando Finalizar...");
                // Sinaliza o cancelamento para a thread
                _cancellationTokenSource.Cancel();
                // Espera a thread terminar a execução
                miningThread.Join();
                Console.WriteLine("Thread finalizada.");
                break; // Sai do loop
            }

        } while (threadExec); // Loop até pressionar Enter (para sair)

        // Garante que a thread foi finalizada ao sair do loop principal (se não foi finalizada por 'F')
        if (miningThread.IsAlive)
        {
             Console.WriteLine("\nFinalizando a thread (pressionado Enter)...");
             _cancellationTokenSource.Cancel();
             miningThread.Join();
             Console.WriteLine("Acabou o trabalho...");
        }


        Console.WriteLine("\nPrograma principal finalizado.");
    }

    // Método que será executado na nova thread
    public static void DoMining()
    {
        Console.WriteLine("Thread de trabalho iniciada.");
        //int gold = 0;

        // A thread continuará rodando até que o token de cancelamento seja solicitado
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            // Espera aqui se o evento de pausa não estiver setado (pausado)
            _pauseEvent.WaitOne();

            // Verifica novamente o cancelamento após possivelmente sair da espera
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                break; // Sai do loop se o cancelamento foi solicitado
            }

            // Simula algum trabalho
            //Console.WriteLine($"Thread de trabalho: Contagem = {gold++}");
            gold++;
            Thread.Sleep(1000); // Espera um pouco para não imprimir muito rápido
        }

        Console.WriteLine("Thread de trabalho finalizando.");
    }
}