# 2025-1C-4061-FIFO

## Descripción

Repositorio del equipo "FIFO" (Grupo 2, Primer Cuatrimestre año 2025, curso K3061/4061)  para el TP de la materia Técnicas de Gráficos por Computadora de la asignatura electiva [Técnicas de Gráficos por Computadora](http://tgc-utn.github.io/) (TGC) en la carrera de Ingeniería en Sistemas de Información. Universidad Tecnológica Nacional, Facultad Regional Buenos Aires (UTN-FRBA).

## Descripcion de jugabilidad

El objetivo del juego es guiar una esfera a través de un entorno en tercera persona, atravesando los cuatro puntos de control antes de que transcurran cinco minutos.

El entorno presenta iluminación basada en el modelo **Blinn-Phong**, con **texturas de base** y **mapas de normales** aplicados en todos los objetos del escenario, excepto en los power-ups y checkpoints. Además, se utiliza **environment mapping** en la bola de metal para simular reflejos.


### Controles

- **A, W, S, D** o **←, ↑, ↓, →**: controlan el movimiento de la bola y también permiten navegar por el menú.
- **Click derecho (arrastrar)**: rota la cámara en tercera persona.
- **Barra espaciadora**: hace que la bola salte.
- **R**: reinicia la bola al último checkpoint alcanzado.
- **ESC**: abre o cierra el menú del juego.
- **Enter**: selecciona una opción del menú.
- **G**: muestra información útil para debug en pantalla

### Power Ups
* **Power-up de rayo**: acelera la bola al atravesarlo. Cuanto más cálido el color, mayor la aceleración (rojo > naranja > amarillo).
* **Power-up de flecha**: hace saltar a la bola al atravesarlo. Cuanto más cálido el color, mayor el salto (rojo > naranja > amarillo).

### Checkpoints
Marcados con banderas azules, los puntos de control actúan como referencias clave en el recorrido. Al atravesar uno, se guarda automáticamente la posición como punto de reaparición en caso de reinicio, y además se contabiliza como progreso hacia la victoria. Para ganar el juego, es necesario atravesar los cuatro puntos de control antes de que transcurran cinco minutos.

### HUD
* **Minimapa**: muestra la ubicación de los checkpoints y de la pelota en tiempo real, facilitando la orientación del jugador dentro del nivel.
* **Cronómetro**: indica el tiempo transcurrido desde el inicio de la partida, fundamental para cumplir el objetivo dentro del límite de tiempo.
* **Barra de progreso**: muestra visualmente cuántos checkpoints han sido completados y cuántos faltan para finalizar el nivel.


### Otros elementos

- **Cajas claras**: son dinámicas (se pueden mover).
- **Cajas oscuras**: son estáticas (no se mueven).
- **Bola de goma**: salta mucho.
- **Bola de metal**: tiene poca fricción y cuenta con environment mapping.
- **Bola de piedra**: es muy pesada.
- Las diferencias físicas entre las bolas se perciben claramente al jugar.
- **Autorespawn**: si la bola pasa aproximadamente 10 segundos sin tocar nada, vuelve automáticamente al último checkpoint.

## Integrantes

Apellido, Nombre | Legajo | Foto
------------ | ------------- | -------------
| Lucas Agustín Máspero | 161542-7 |<img src="Integrantes/LucasMaspero.jpeg" height="200">  |
| Santiago Malirat Canga | 213857-8 |<img src="Integrantes/SantiagoMaliratCanga.png" height="200">  |

## Capturas

Ver carpeta "Capturas"

<img src="Capturas/Captura1.png">
<br/>
<img src="Capturas/Captura2.png">
<br/>
<img src="Capturas/Captura3.png">
<br/>

## Game Play

Ver carpeta "Gameplay"

<video src="Gameplay/Gameplay1.mp4" controls width="800"></video>
<video src="Gameplay/Gameplay2.mp4" controls width="800"></video>
<video src="Gameplay/Gameplay3.mp4" controls width="800"></video>