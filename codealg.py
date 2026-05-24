import random
import time
import statistics
import math
import csv
import matplotlib.pyplot as plt

# ==========================================
# FUNCIONES GENERADORAS DE EXPERIMENTOS
# ==========================================
def generar_exp1_enteros(n):
    A = [[random.randint(1, 10) for _ in range(n)] for _ in range(n)]
    B = [[random.randint(1, 10) for _ in range(n)] for _ in range(n)]
    return A, B

def generar_exp2_reales_0_1(n):
    A = [[random.random() for _ in range(n)] for _ in range(n)]
    B = [[random.random() for _ in range(n)] for _ in range(n)]
    return A, B

def generar_exp3_estructuradas(n):
    A = [[(i + j + 1) for j in range(n)] for i in range(n)]
    B = [[(i - j) for j in range(n)] for i in range(n)]
    return A, B

def generar_exp4_reales_2_decimales(n):
    A = [[round(random.uniform(1.0, 100.0), 2) for _ in range(n)] for _ in range(n)]
    B = [[round(random.uniform(1.0, 100.0), 2) for _ in range(n)] for _ in range(n)]
    return A, B

def generar_exp5_longlong(n):
    min_val, max_val = 10**15, 10**16
    A = [[random.randint(min_val, max_val) for _ in range(n)] for _ in range(n)]
    B = [[random.randint(min_val, max_val) for _ in range(n)] for _ in range(n)]
    return A, B

# ==========================================
# ALGORITMOS MATEMÁTICOS
# ==========================================
def multiplicacion_clasica(A, B):
    n = len(A)
    C = [[0 for _ in range(n)] for _ in range(n)]
    for i in range(n):
        for j in range(n):
            for k in range(n):
                C[i][j] += A[i][k] * B[k][j]
    return C

def sumar_matrices(A, B):
    n = len(A)
    return [[A[i][j] + B[i][j] for j in range(n)] for i in range(n)]

def restar_matrices(A, B):
    n = len(A)
    return [[A[i][j] - B[i][j] for j in range(n)] for i in range(n)]

def dividir_matriz(A):
    n = len(A)
    mid = n // 2
    return [fila[:mid] for fila in A[:mid]], [fila[mid:] for fila in A[:mid]], \
           [fila[:mid] for fila in A[mid:]], [fila[mid:] for fila in A[mid:]]

def multiplicacion_strassen(A, B):
    n = len(A)
    if n == 1:
        return [[A[0][0] * B[0][0]]]
    
    A11, A12, A21, A22 = dividir_matriz(A)
    B11, B12, B21, B22 = dividir_matriz(B)
    
    S1 = sumar_matrices(A11, A22); S2 = sumar_matrices(B11, B22)
    S3 = sumar_matrices(A21, A22); S4 = restar_matrices(B12, B22)
    S5 = restar_matrices(B21, B11); S6 = sumar_matrices(A11, A12)
    S7 = restar_matrices(A21, A11); S8 = sumar_matrices(B11, B12)
    S9 = restar_matrices(A12, A22); S10 = sumar_matrices(B21, B22)
    
    M1 = multiplicacion_strassen(S1, S2); M2 = multiplicacion_strassen(S3, B11)
    M3 = multiplicacion_strassen(A11, S4); M4 = multiplicacion_strassen(A22, S5)
    M5 = multiplicacion_strassen(S6, B22); M6 = multiplicacion_strassen(S7, S8)
    M7 = multiplicacion_strassen(S9, S10)
    
    C11 = sumar_matrices(restar_matrices(sumar_matrices(M1, M4), M5), M7)
    C12 = sumar_matrices(M3, M5)
    C21 = sumar_matrices(M2, M4)
    C22 = sumar_matrices(sumar_matrices(restar_matrices(M1, M2), M3), M6)
    
    C = [[0 for _ in range(n)] for _ in range(n)]
    mid = n // 2
    for i in range(mid):
        for j in range(mid):
            C[i][j] = C11[i][j]; C[i][j + mid] = C12[i][j]
            C[i + mid][j] = C21[i][j]; C[i + mid][j + mid] = C22[i][j]
    return C

def multiplicacion_hibrida(A, B, n0):
    n = len(A)
    if n <= n0:
        return multiplicacion_clasica(A, B)
    
    A11, A12, A21, A22 = dividir_matriz(A)
    B11, B12, B21, B22 = dividir_matriz(B)
    
    S1 = sumar_matrices(A11, A22); S2 = sumar_matrices(B11, B22)
    S3 = sumar_matrices(A21, A22); S4 = restar_matrices(B12, B22)
    S5 = restar_matrices(B21, B11); S6 = sumar_matrices(A11, A12)
    S7 = restar_matrices(A21, A11); S8 = sumar_matrices(B11, B12)
    S9 = restar_matrices(A12, A22); S10 = sumar_matrices(B21, B22)
    
    M1 = multiplicacion_hibrida(S1, S2, n0); M2 = multiplicacion_hibrida(S3, B11, n0)
    M3 = multiplicacion_hibrida(A11, S4, n0); M4 = multiplicacion_hibrida(A22, S5, n0)
    M5 = multiplicacion_hibrida(S6, B22, n0); M6 = multiplicacion_hibrida(S7, S8, n0)
    M7 = multiplicacion_hibrida(S9, S10, n0)
    
    C11 = sumar_matrices(restar_matrices(sumar_matrices(M1, M4), M5), M7)
    C12 = sumar_matrices(M3, M5)
    C21 = sumar_matrices(M2, M4)
    C22 = sumar_matrices(sumar_matrices(restar_matrices(M1, M2), M3), M6)
    
    C = [[0 for _ in range(n)] for _ in range(n)]
    mid = n // 2
    for i in range(mid):
        for j in range(mid):
            C[i][j] = C11[i][j]; C[i][j + mid] = C12[i][j]
            C[i + mid][j] = C21[i][j]; C[i + mid][j + mid] = C22[i][j]
    return C

# ==========================================
# RUTINA PRINCIPAL DE BENCHMARKING
# ==========================================
if __name__ == "__main__":
    tamanos_n = [16, 32, 64, 128, 256, 512]
    repeticiones = 32
    umbrales_n0_a_probar = [16, 32, 64] # Umbrales a evaluar analíticamente
    
    experimentos = {
        "Exp1_EnterosAleatorios": generar_exp1_enteros,
        "Exp2_Reales_0a1": generar_exp2_reales_0_1,
        "Exp3_Estructuradas": generar_exp3_estructuradas,
        "Exp4_Reales2Decimales": generar_exp4_reales_2_decimales,
        "Exp5_LongLong": generar_exp5_longlong
    }

    print("--- INICIANDO BENCHMARKING ---")
    resultados_globales = {}
    
    for nombre_exp, funcion_generadora in experimentos.items():
        print(f"\nEjecutando {nombre_exp}")
        is_hibrido_target = (nombre_exp == "Exp5_LongLong")
        
        resultados_globales[nombre_exp] = {
            'n': [], 't_clasico': [], 'std_clasico': [], 't_strassen': [], 'std_strassen': []
        }
        
        if is_hibrido_target:
            for n0 in umbrales_n0_a_probar:
                resultados_globales[nombre_exp][f't_hib_{n0}'] = []
                resultados_globales[nombre_exp][f'std_hib_{n0}'] = []
        
        for n in tamanos_n:
            print(f"  Midiendo n={n} ({repeticiones} reps)...")
            tiempos_clasico, tiempos_strassen = [], []
            tiempos_hibrido = {n0: [] for n0 in umbrales_n0_a_probar}
            
            for _ in range(repeticiones):
                A, B = funcion_generadora(n)
                
                # Clásico
                t_ini = time.perf_counter()
                multiplicacion_clasica(A, B)
                tiempos_clasico.append(time.perf_counter() - t_ini)
                
                # Strassen
                t_ini = time.perf_counter()
                multiplicacion_strassen(A, B)
                tiempos_strassen.append(time.perf_counter() - t_ini)
                
                # Híbrido (SÓLO SI ES EL EXPERIMENTO LONG LONG)
                if is_hibrido_target:
                    for n0 in umbrales_n0_a_probar:
                        t_ini = time.perf_counter()
                        multiplicacion_hibrida(A, B, n0)
                        tiempos_hibrido[n0].append(time.perf_counter() - t_ini)
                
            resultados_globales[nombre_exp]['n'].append(n)
            resultados_globales[nombre_exp]['t_clasico'].append(statistics.mean(tiempos_clasico))
            resultados_globales[nombre_exp]['std_clasico'].append(statistics.stdev(tiempos_clasico) if repeticiones > 1 else 0)
            resultados_globales[nombre_exp]['t_strassen'].append(statistics.mean(tiempos_strassen))
            resultados_globales[nombre_exp]['std_strassen'].append(statistics.stdev(tiempos_strassen) if repeticiones > 1 else 0)
            
            if is_hibrido_target:
                for n0 in umbrales_n0_a_probar:
                    resultados_globales[nombre_exp][f't_hib_{n0}'].append(statistics.mean(tiempos_hibrido[n0]))
                    resultados_globales[nombre_exp][f'std_hib_{n0}'].append(statistics.stdev(tiempos_hibrido[n0]) if repeticiones > 1 else 0)

    # CREACIÓN DEL ARCHIVO CSV
    print("\nGenerando CSV (resultados_python.csv)...")
    with open('resultados_python.csv', mode='w', newline='') as file:
        writer = csv.writer(file)
        headers = ['Experimento', 'Tamano_n', 'Prom_Clasico', 'Std_Clasico', 'Prom_Strassen', 'Std_Strassen']
        headers += [f'Prom_Hibrido(n0={n0})' for n0 in umbrales_n0_a_probar]
        headers += [f'Std_Hibrido(n0={n0})' for n0 in umbrales_n0_a_probar]
        writer.writerow(headers)
        
        for nombre_exp, datos in resultados_globales.items():
            is_hibrido_target = (nombre_exp == "Exp5_LongLong")
            for i in range(len(datos['n'])):
                row = [
                    nombre_exp, datos['n'][i],
                    f"{datos['t_clasico'][i]:.6f}", f"{datos['std_clasico'][i]:.6f}",
                    f"{datos['t_strassen'][i]:.6f}", f"{datos['std_strassen'][i]:.6f}"
                ]
                
                if is_hibrido_target:
                    for n0 in umbrales_n0_a_probar:
                        row.append(f"{datos[f't_hib_{n0}'][i]:.6f}")
                    for n0 in umbrales_n0_a_probar:
                        row.append(f"{datos[f'std_hib_{n0}'][i]:.6f}")
                else:
                    row += [''] * (len(umbrales_n0_a_probar) * 2) # Celdas vacías para los otros exp
                    
                writer.writerow(row)

    # GRAFICACIÓN LOG-LOG
    print("Generando gráficos...")
    for nombre_exp, datos in resultados_globales.items():
        is_hibrido_target = (nombre_exp == "Exp5_LongLong")
        ns = datos['n']
        plt.figure(figsize=(10, 6))
        
        plt.loglog(ns, datos['t_clasico'], 'o-', linewidth=2, label='Clásico Empírico')
        plt.loglog(ns, datos['t_strassen'], 's-', linewidth=2, label='Strassen Puro')
        
        if is_hibrido_target:
            markers = ['^-', 'v-', 'd-']
            for idx, n0 in enumerate(umbrales_n0_a_probar):
                plt.loglog(ns, datos[f't_hib_{n0}'], markers[idx], linewidth=2, label=f'Híbrido (n0={n0})')
        
        c_clas = datos['t_clasico'][-1] / (ns[-1] ** 3)
        plt.loglog(ns, [c_clas * (n ** 3) for n in ns], 'k--', alpha=0.5, label='Teórico O(n^3)')
        
        c_stras = datos['t_strassen'][-1] / (ns[-1] ** 2.807)
        plt.loglog(ns, [c_stras * (n ** 2.807) for n in ns], 'r--', alpha=0.5, label='Teórico O(n^2.81)')
        
        plt.xlabel('Tamaño de Matriz (n) [Escala Log]')
        plt.ylabel('Tiempo Promedio (segundos) [Escala Log]')
        plt.title(f'Comparación de Algoritmos - {nombre_exp}')
        plt.xticks(ns, [str(val) for val in ns])
        plt.legend()
        plt.grid(True, which="both", ls="--", alpha=0.5)
        
        plt.savefig(f'Python_Grafico_{nombre_exp}.png')
        plt.close()
        
    print("Finalizado con éxito.")