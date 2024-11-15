using System;
using System.Collections.Generic;
using System.Threading;

public class Impresora
{
    private Queue<string> colaDeTrabajos = new Queue<string>();
    private static Impresora instancia;
    private static object mutexCola = new object(); // Para proteger la cola.
    private static Semaphore semaphore = new Semaphore(2, 2); // Limita a 5 trabajos en la cola.

    // Singleton para garantizar una única instancia de Impresora.
    public static Impresora Instancia
    {
        get
        {
            if (instancia == null)
            {
                lock (mutexCola)
                {
                    if (instancia == null)
                        instancia = new Impresora();
                }
            }
            return instancia;
        }
    }

    // Agregar trabajo a la cola.
    public void EnviarTrabajo(string trabajo)
    {
        semaphore.WaitOne(); // Bloquear si no hay espacio en la cola.

        lock (mutexCola)
        {
            colaDeTrabajos.Enqueue(trabajo);
            Console.WriteLine($"Trabajo agregado: {trabajo} (Total en cola: {colaDeTrabajos.Count})");
            Monitor.Pulse(mutexCola); // Notificar a la impresora que hay un trabajo disponible.
        }
    }

    // Obtener y procesar el siguiente trabajo.
    public void Imprimir()
    {
        while (true)
        {
            string trabajo;

            lock (mutexCola)
            {
                while (colaDeTrabajos.Count == 0)
                {
                    Console.WriteLine("No hay trabajos en la cola. Esperando...");
                    Monitor.Wait(mutexCola); // Esperar hasta que haya un trabajo.
                }

                trabajo = colaDeTrabajos.Dequeue();
                Console.WriteLine($"Imprimiendo: {trabajo} (Trabajos restantes en cola: {colaDeTrabajos.Count})");
            }

            // Liberar espacio en la cola después de procesar el trabajo.
            semaphore.Release();

            // Simular el proceso de impresión.
            Thread.Sleep(2000); // Simula el tiempo de impresión.
        }
    }
}
