# ITBA 2Q Tecnicas avanzadas en videojuegos

## Descripcion

Se ha desarrollado un juego estilo FPS, en vista de aplicar conceptos teoricos de juegos en red, como:

* Compresion de datos
* Snapshot interpolation
* Manejo de eventos
* Manejo de perdida de paquetes y latencia (simulados)
* Prediction

## Configuracion

Para correr nuestro proyecto se debe:

1. Clonar el mismo: 
```git clone https://github.com/axelfratoni/TecAvanVid.git```
1. Abrir el proyecto con Unity
1. Configurar:
   * Server/Client
     * Server Port: Puerto del servidor, debe ser el mismo en ambas configuraciones
     * Delay (ms): Latencia simulada
     * Packet loss (0-100%): Probabilidad de que un paquete se pierda 
   * Server
     * Snapshot Interval (ms): Cada cuanto tiempo enviar snapshots 
   * Cliente
     * Client Port: Puerto desde el cual se conecta el cliente
     * Server IP: Direccion IP del servidor
     * Prediction (True/False): Activa desactiva prediction   
   Aclaracion: Se puede tener un cliente y un server corriendo en la misma instacia de Unity, pero se veran de manera
   duplicada la ubicacion de los juegadores
1. Ejecutar el juego o buildearlo.

## Controles

Los controles del juego son:
* W: Arriba
* S: Abajo
* A: Izquierda
* D: Derecha
* Click izquierdo del mouse: Disparo
* Click derecho del mouse: Lanzamiento de granada
* Moviemiento del mouse: Movimiento de camara

## Detalles de implementacion

1. Compresion de datos:
   * Desarrollado en la clase BitBuffer se desarrolo la escritura/lectura de un bit en un buffer de bits; con un control
   cuando el mismo debia ser escrito y pasado a una capa superior. A partir de este metodo; se construyo, basandose en
   el metodo anterior, la escritura de:
     * Sucesion de bits
     * Un entero (limitando entre minimo y maximo, escrito en la minima cantidad de bits posible)
     * Un flotante (a partir de minimo, maximo y precision, se lo lleva a entero)
1. Snapshot interpolation:
   * Debido a una posible perdida de un paquete, y para que el juego se vea fluido entre la llegada de paquetes, se
   interpola la posicion de un jugador entre dos snapshots. Para esto es necesatio tener un buffer de snapshots
   guardados.
1. Comunicacion:
   * Es UDP, ya que esto nos permite no llenar la red de paquetes que son utiles unicamente cuando se envian (como
   snapshots, por ejemplo).
1. Manejo de eventos:
   * Hay eventos del juego, que son necesarios de garantizar su llegada, como el lanzamiento de una granada. Para esto
   se utiliza lista de mensajes a enviar (reliables):
     * Algunos que necesitan ser enviados al instante, sin timeout (lanzamiento de granadas).
     * Otros que pueden esperar un tiempo antes de ser reenviados (pedidos de conexion).
     * Todas las colas reliables son manejados con una logica de ACK para garantizar su llegada.
   * A su vez, los snapshots son eventos descartables, que solo deben ser enviados una vez (unreliables), por lo que no
   son guardados, simplemente se envian.
1. Simulacion de perdida de paquetes y latencia:
   * Latencia: al momento de recibir un paquete, se le aplica un timer previo al procesamiento del mismo
   * Perdida de paquete: al momento de recibir un paquete, se "tira una moneda" con la probabilidad pasa por
   configuracion de descartar el paquete.
1. Prediction:
   * Debido a la diferencia de tiempos entre el server y el cliente, se puede aplicar prediction en el cliente para
   simular de mejor manera la "realidad" del servidor. Para esto, se guarda los inputs del cliente, y al recibir un
   snapshot del servidor, se aplica todos los input efectuados luego de haber enviado el input de ese snapshot
   (identificado por numero de secuencia).
1. Logica del juego:
   * Para la logica del juego se han bajado Assets del store de Unity, y se ha aplicado logica de un FPS:
     * Disparos: Se aplica un raycast entre la pistola y el centro de mira del jugador. El usuario envia el imput de
      disparo y el server lo aplica.
     * Granada: Se envia un evento de granada y el server crea y actualiza la entidad.
     * Vida: Cada disparo/granada disminuye la vida de un jugador al impactarlo.
