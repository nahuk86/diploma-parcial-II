# Documentación del Proyecto: **Sistema de Impresión Concurrente**

## **Introducción**

Este proyecto simula un sistema de impresión concurrente donde múltiples usuarios pueden enviar trabajos de impresión a una impresora compartida. La solución implementa sincronización de hilos para evitar conflictos y asegurar que los trabajos se impriman en el orden correcto. Además, se controla la capacidad de la cola de trabajos para evitar sobrecargas en el sistema.

---

## **Patrones de Diseño Utilizados**

### 1. **Singleton**
   - **¿Qué es?**  
     El patrón Singleton asegura que solo exista una única instancia de una clase en toda la aplicación y proporciona un punto de acceso global a esta instancia.
   - **¿Dónde se implementa?**  
     En la clase `Impresora`.
   - **Código clave:**
     ```csharp
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
     ```
   - **Justificación:**  
     La impresora es un recurso compartido único en este sistema. Al usar el patrón Singleton, se asegura que todos los usuarios interactúen con la misma instancia de la impresora, evitando conflictos.

---

### 2. **Productor-Consumidor**
   - **¿Qué es?**  
     El patrón Productor-Consumidor es una solución para el manejo concurrente de tareas. Los productores generan trabajos y los colocan en una cola, mientras que los consumidores los procesan.
   - **¿Dónde se implementa?**  
     - Los productores son los `Usuarios`, que envían trabajos de impresión.
     - El consumidor es la `Impresora`, que procesa los trabajos en la cola.
   - **Código clave:**
     - Productores (`Usuario`):
       ```csharp
       Impresora.Instancia.EnviarTrabajo(trabajo);
       ```
     - Consumidor (`Impresora`):
       ```csharp
       public void Imprimir()
       {
           lock (mutexCola)
           {
               while (colaDeTrabajos.Count == 0)
                   Monitor.Wait(mutexCola);

               string trabajo = colaDeTrabajos.Dequeue();
           }
       }
       ```
   - **Justificación:**  
     Este patrón asegura que los trabajos se procesen en el orden en que llegan, evitando condiciones de carrera y manteniendo un flujo controlado entre la producción y el consumo.

---

### 3. **Semaphore**
   - **¿Qué es?**  
     Un semáforo controla el acceso a un recurso compartido mediante un contador. Solo permite que un número limitado de hilos accedan al recurso al mismo tiempo.
   - **¿Dónde se implementa?**  
     En la clase `Impresora`, para limitar el tamaño de la cola de trabajos.
   - **Código clave:**
     ```csharp
     private static Semaphore semaphore = new Semaphore(2, 2);

     public void EnviarTrabajo(string trabajo)
     {
         semaphore.WaitOne();
         lock (mutexCola)
         {
             colaDeTrabajos.Enqueue(trabajo);
             Monitor.Pulse(mutexCola);
         }
     }

     public void Imprimir()
     {
         lock (mutexCola)
         {
             string trabajo = colaDeTrabajos.Dequeue();
         }
         semaphore.Release();
     }
     ```
   - **Justificación:**  
     Controlar la capacidad de la cola de trabajos es fundamental para evitar que los productores saturen el sistema. El semáforo limita la cola a dos trabajos simultáneamente.

---

### 4. **Monitor**
   - **¿Qué es?**  
     El `Monitor` coordina la interacción entre hilos permitiendo que esperen hasta que se cumpla una condición, y notifica cuando la condición cambia.
   - **¿Dónde se implementa?**  
     En la clase `Impresora`, para sincronizar la interacción entre los productores y el consumidor.
   - **Código clave:**
     ```csharp
     lock (mutexCola)
     {
         while (colaDeTrabajos.Count == 0)
         {
             Console.WriteLine("No hay trabajos en la cola. Esperando...");
             Monitor.Wait(mutexCola);
         }

         string trabajo = colaDeTrabajos.Dequeue();
         Monitor.Pulse(mutexCola);
     }
     ```
   - **Justificación:**  
     El `Monitor` asegura que la impresora no entre en un bucle innecesario (espera activa) cuando la cola está vacía, y notifica cuando llegan nuevos trabajos.

---

## **Uso de Hilos**

### **¿Qué son los hilos y por qué se usan?**
Un hilo es una unidad de ejecución dentro de un programa. En este proyecto, los hilos permiten que los usuarios (productores) y la impresora (consumidor) trabajen de forma concurrente, simulando un ambiente realista en el que múltiples usuarios pueden enviar trabajos mientras la impresora los procesa.

### **¿Cómo se implementan?**

1. **Creación de Hilos**:
   - Los hilos se crean para ejecutar las tareas concurrentes de los usuarios y de la impresora.
   - Código clave en `Program`:
     ```csharp
     Thread hiloImpresora = new Thread(Impresora.Instancia.Imprimir);
     Thread hiloUsuario1 = new Thread(usuario1.EnviarTrabajos);
     Thread hiloUsuario2 = new Thread(usuario2.EnviarTrabajos);
     Thread hiloUsuario3 = new Thread(usuario3.EnviarTrabajos);

     hiloImpresora.Start();
     hiloUsuario1.Start();
     hiloUsuario2.Start();
     hiloUsuario3.Start();
     ```

2. **Sincronización entre Hilos**:
   - Se utilizan `Semaphore` y `Monitor` para coordinar el acceso a los recursos compartidos (la cola de trabajos) y evitar conflictos.

3. **Espera de Finalización (`Join`)**:
   - Se utiliza `Thread.Join` para esperar a que los hilos productores terminen de enviar trabajos antes de finalizar la ejecución del programa.
   - Código clave:
     ```csharp
     hiloUsuario1.Join();
     hiloUsuario2.Join();
     hiloUsuario3.Join();
     ```

### **Beneficios del Uso de Hilos**

1. **Simulación Realista**:
   - Permite que los usuarios y la impresora trabajen simultáneamente.
2. **Aprovechamiento de la Concurrencia**:
   - Los hilos maximizan el uso de recursos del sistema.
3. **Independencia**:
   - Cada hilo tiene su propia tarea y puede ejecutarse de manera independiente.

---

## **Flujo de Ejecución**

1. Los usuarios (hilos productores) intentan enviar trabajos de impresión.
2. El semáforo controla el acceso a la cola de trabajos, permitiendo hasta 2 trabajos simultáneamente.
3. La impresora (hilo consumidor) procesa los trabajos en el orden en que llegan.
4. Si la cola está vacía, la impresora espera usando `Monitor.Wait`.
5. Los productores notifican a la impresora con `Monitor.Pulse` cuando agregan un nuevo trabajo.
6. Una vez procesado un trabajo, se libera un espacio en la cola mediante `semaphore.Release`.
