using System;
using System.Threading;

public class Usuario
{
    private string nombre;

    public Usuario(string nombre)
    {
        this.nombre = nombre;
    }

    Random random = new Random();

    public void EnviarTrabajos()
    {

        for (int i = 0; i < 5; i++) // Cada usuario envía 5 trabajos.
        {
            string trabajo = $"{nombre} - Trabajo {i + 1}";
            Console.WriteLine($"{nombre} intentando enviar: {trabajo}");
            Impresora.Instancia.EnviarTrabajo(trabajo);

            // Pausa aleatoria para simular el tiempo entre envíos.
            int tiempoEspera = random.Next(1000, 3000);
            Thread.Sleep(tiempoEspera);
        }
    }
}
