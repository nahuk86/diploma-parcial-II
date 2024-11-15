using System;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        // Crear y arrancar el hilo de la impresora.
        Thread hiloImpresora = new Thread(Impresora.Instancia.Imprimir);
        hiloImpresora.Start();

        // Crear usuarios y arrancar sus hilos.
        Usuario usuario1 = new Usuario("Usuario 1");
        Usuario usuario2 = new Usuario("Usuario 2");
        Usuario usuario3 = new Usuario("Usuario 3");

        Thread hiloUsuario1 = new Thread(usuario1.EnviarTrabajos);
        Thread hiloUsuario2 = new Thread(usuario2.EnviarTrabajos);
        Thread hiloUsuario3 = new Thread(usuario3.EnviarTrabajos);

        hiloUsuario1.Start();
        hiloUsuario2.Start();
        hiloUsuario3.Start();

        // Esperar a que los usuarios terminen de enviar trabajos.
        hiloUsuario1.Join();
        hiloUsuario2.Join();
        hiloUsuario3.Join();

        Console.WriteLine("Todos los trabajos han sido enviados.");
    }
}
