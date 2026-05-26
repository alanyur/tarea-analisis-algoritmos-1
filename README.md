# tarea-analisis-algoritmos-1

## Estructura de Archivos
* `codealg.py`: Script principal de experimentación y graficación en Python.
* `codealg.cs`: Script principal de experimentación y generación de datos en C#.

---

## 1. Instrucciones de Ejecución para Python (`codealg.py`)

### Requisitos Previos
* Tener instalado Python 3.x (Recomendado Python 3.8 o superior).
* Tener instalado el gestor de paquetes `pip`.
* Instalar la librería `matplotlib` para la generación de gráficos. Puede instalarla ejecutando el siguiente comando en su terminal:
    ```bash
    pip install matplotlib
    ```

### Ejecución (Visual Studio Code / Terminal)
1.  Abra su terminal o la consola integrada de Visual Studio Code.
2.  Navegue hasta el directorio donde se encuentra el archivo `codealg.py`.
3.  Ejecute el script con el siguiente comando:
    ```bash
    python codealg.py
    ```
4.  **Resultados:** Al finalizar la ejecución, el script generará automáticamente en la misma carpeta:
    * Un archivo `resultados_python.csv` con los tiempos y desviaciones estándar.
    * Cinco archivos `.png` correspondientes a los gráficos Log-Log de cada experimento.

---

## 2. Instrucciones de Ejecución para C# (`codealg.cs`)

### Requisitos Previos
* Tener instalado el SDK de .NET (recomendado .NET 6.0 o superior) o el compilador de C# (`csc`).
* Tener instalado Python y la librería `matplotlib` (el código C# genera un script puente en Python para graficar sus resultados automáticamente).

### Ejecución (Visual Studio Code / Terminal)
1.  Abra su terminal o la consola integrada de Visual Studio Code.
2.  Navegue hasta el directorio donde se encuentra el archivo `codealg.cs`.
3.  Para compilar y ejecutar el archivo directamente, puede utilizar el comando `dotnet run` (si está dentro de un proyecto .NET) o compilarlo manualmente:
    ```bash
    csc codealg.cs
    codealg.exe
    ```
    *(En sistemas basados en Linux/macOS utilizando Mono, ejecute: `mcs codealg.cs` seguido de `mono codealg.exe`)*.

4.  **Resultados:** Al finalizar el benchmarking, el programa generará:
    * Un archivo `resultados_csharp.csv` con los datos empíricos.
    * Un archivo temporal `graficador_csharp.py`.
    * El programa llamará automáticamente a Python para leer los datos y generar los cinco archivos `.png` con los gráficos resultantes de C#.
