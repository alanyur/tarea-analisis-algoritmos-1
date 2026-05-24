using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Globalization;

namespace MultiplicacionMatrices
{
    class ResultadoExperimento
    {
        public string Nombre;
        public bool EsExpHibrido;
        public List<int> Ns = new List<int>();
        public List<double> TClasico = new List<double>();
        public List<double> StdClasico = new List<double>();
        public List<double> TStrassen = new List<double>();
        public List<double> StdStrassen = new List<double>();
        
        // Listas para los 3 umbrales a analizar analíticamente
        public List<double> THib16 = new List<double>(); public List<double> StdHib16 = new List<double>();
        public List<double> THib32 = new List<double>(); public List<double> StdHib32 = new List<double>();
        public List<double> THib64 = new List<double>(); public List<double> StdHib64 = new List<double>();
    }

    class Program
    {
        static Random rand = new Random();

        // Funciones generadoras
        static void GenExp1_Enteros(int n, out double[][] A, out double[][] B)
        {
            A = CreateMatrix(n); B = CreateMatrix(n);
            for (int i=0; i<n; i++) for (int j=0; j<n; j++) {
                A[i][j] = rand.Next(1, 11); B[i][j] = rand.Next(1, 11);
            }
        }

        static void GenExp2_Reales_0_1(int n, out double[][] A, out double[][] B)
        {
            A = CreateMatrix(n); B = CreateMatrix(n);
            for (int i=0; i<n; i++) for (int j=0; j<n; j++) {
                A[i][j] = rand.NextDouble(); B[i][j] = rand.NextDouble();
            }
        }

        static void GenExp3_Estructuradas(int n, out double[][] A, out double[][] B)
        {
            A = CreateMatrix(n); B = CreateMatrix(n);
            for (int i=0; i<n; i++) for (int j=0; j<n; j++) {
                A[i][j] = (double)(i + j + 1); B[i][j] = (double)(i - j);
            }
        }

        static void GenExp4_Reales2Decimales(int n, out double[][] A, out double[][] B)
        {
            A = CreateMatrix(n); B = CreateMatrix(n);
            for (int i=0; i<n; i++) for (int j=0; j<n; j++) {
                A[i][j] = Math.Round(rand.NextDouble() * 100.0, 2); B[i][j] = Math.Round(rand.NextDouble() * 100.0, 2);
            }
        }

        static void GenExp5_LongLong(int n, out double[][] A, out double[][] B)
        {
            A = CreateMatrix(n); B = CreateMatrix(n);
            double min = 100000000000000.0; double max = 900000000000000.0;
            for (int i=0; i<n; i++) for (int j=0; j<n; j++) {
                A[i][j] = Math.Floor(min + (rand.NextDouble() * (max - min))); B[i][j] = Math.Floor(min + (rand.NextDouble() * (max - min)));
            }
        }

        static double[][] CreateMatrix(int n)
        {
            double[][] result = new double[n][];
            for (int i = 0; i < n; i++) result[i] = new double[n];
            return result;
        }

        static double CalcularDesviacionEstandar(List<double> valores, double promedio)
        {
            if (valores.Count <= 1) return 0.0;
            double sumOfSquares = valores.Sum(val => Math.Pow(val - promedio, 2));
            return Math.Sqrt(sumOfSquares / (valores.Count - 1));
        }

        // ==========================================
        // ALGORITMOS MATEMÁTICOS
        // ==========================================
        static double[][] MultiplicacionClasica(double[][] A, double[][] B)
        {
            int n = A.Length;
            double[][] C = CreateMatrix(n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < n; k++)
                        C[i][j] += A[i][k] * B[k][j];
            return C;
        }

        static double[][] SumarMatrices(double[][] A, double[][] B)
        {
            int n = A.Length;
            double[][] C = CreateMatrix(n);
            for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) C[i][j] = A[i][j] + B[i][j];
            return C;
        }

        static double[][] RestarMatrices(double[][] A, double[][] B)
        {
            int n = A.Length;
            double[][] C = CreateMatrix(n);
            for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) C[i][j] = A[i][j] - B[i][j];
            return C;
        }

        static void DividirMatriz(double[][] A, out double[][] A11, out double[][] A12, out double[][] A21, out double[][] A22)
        {
            int mid = A.Length / 2;
            A11 = CreateMatrix(mid); A12 = CreateMatrix(mid);
            A21 = CreateMatrix(mid); A22 = CreateMatrix(mid);
            for (int i = 0; i < mid; i++) for (int j = 0; j < mid; j++) {
                A11[i][j] = A[i][j]; A12[i][j] = A[i][j + mid];
                A21[i][j] = A[i + mid][j]; A22[i][j] = A[i + mid][j + mid];
            }
        }

        static double[][] MultiplicacionStrassen(double[][] A, double[][] B)
        {
            int n = A.Length;
            if (n == 1) { double[][] C = CreateMatrix(1); C[0][0] = A[0][0] * B[0][0]; return C; }

            double[][] A11, A12, A21, A22, B11, B12, B21, B22;
            DividirMatriz(A, out A11, out A12, out A21, out A22); DividirMatriz(B, out B11, out B12, out B21, out B22);

            double[][] S1 = SumarMatrices(A11, A22); double[][] S2 = SumarMatrices(B11, B22);
            double[][] S3 = SumarMatrices(A21, A22); double[][] S4 = RestarMatrices(B12, B22);
            double[][] S5 = RestarMatrices(B21, B11); double[][] S6 = SumarMatrices(A11, A12);
            double[][] S7 = RestarMatrices(A21, A11); double[][] S8 = SumarMatrices(B11, B12);
            double[][] S9 = RestarMatrices(A12, A22); double[][] S10 = SumarMatrices(B21, B22);

            double[][] M1 = MultiplicacionStrassen(S1, S2); double[][] M2 = MultiplicacionStrassen(S3, B11);
            double[][] M3 = MultiplicacionStrassen(A11, S4); double[][] M4 = MultiplicacionStrassen(A22, S5);
            double[][] M5 = MultiplicacionStrassen(S6, B22); double[][] M6 = MultiplicacionStrassen(S7, S8);
            double[][] M7 = MultiplicacionStrassen(S9, S10);

            double[][] C11 = SumarMatrices(RestarMatrices(SumarMatrices(M1, M4), M5), M7);
            double[][] C12 = SumarMatrices(M3, M5);
            double[][] C21 = SumarMatrices(M2, M4);
            double[][] C22 = SumarMatrices(SumarMatrices(RestarMatrices(M1, M2), M3), M6);

            double[][] finalC = CreateMatrix(n); int mid = n / 2;
            for (int i = 0; i < mid; i++) for (int j = 0; j < mid; j++) {
                finalC[i][j] = C11[i][j]; finalC[i][j + mid] = C12[i][j];
                finalC[i + mid][j] = C21[i][j]; finalC[i + mid][j + mid] = C22[i][j];
            }
            return finalC;
        }

        static double[][] MultiplicacionHibrida(double[][] A, double[][] B, int n0)
        {
            int n = A.Length;
            if (n <= n0) return MultiplicacionClasica(A, B);

            double[][] A11, A12, A21, A22, B11, B12, B21, B22;
            DividirMatriz(A, out A11, out A12, out A21, out A22); DividirMatriz(B, out B11, out B12, out B21, out B22);

            double[][] S1 = SumarMatrices(A11, A22); double[][] S2 = SumarMatrices(B11, B22);
            double[][] S3 = SumarMatrices(A21, A22); double[][] S4 = RestarMatrices(B12, B22);
            double[][] S5 = RestarMatrices(B21, B11); double[][] S6 = SumarMatrices(A11, A12);
            double[][] S7 = RestarMatrices(A21, A11); double[][] S8 = SumarMatrices(B11, B12);
            double[][] S9 = RestarMatrices(A12, A22); double[][] S10 = SumarMatrices(B21, B22);

            double[][] M1 = MultiplicacionHibrida(S1, S2, n0); double[][] M2 = MultiplicacionHibrida(S3, B11, n0);
            double[][] M3 = MultiplicacionHibrida(A11, S4, n0); double[][] M4 = MultiplicacionHibrida(A22, S5, n0);
            double[][] M5 = MultiplicacionHibrida(S6, B22, n0); double[][] M6 = MultiplicacionHibrida(S7, S8, n0);
            double[][] M7 = MultiplicacionHibrida(S9, S10, n0);

            double[][] C11 = SumarMatrices(RestarMatrices(SumarMatrices(M1, M4), M5), M7);
            double[][] C12 = SumarMatrices(M3, M5);
            double[][] C21 = SumarMatrices(M2, M4);
            double[][] C22 = SumarMatrices(SumarMatrices(RestarMatrices(M1, M2), M3), M6);

            double[][] finalC = CreateMatrix(n); int mid = n / 2;
            for (int i = 0; i < mid; i++) for (int j = 0; j < mid; j++) {
                finalC[i][j] = C11[i][j]; finalC[i][j + mid] = C12[i][j];
                finalC[i + mid][j] = C21[i][j]; finalC[i + mid][j + mid] = C22[i][j];
            }
            return finalC;
        }

        delegate void GeneradorExperimento(int n, out double[][] A, out double[][] B);

        static void Main(string[] args)
        {
            int[] tamanosN = new int[] { 16, 32, 64, 128, 256, 512 };
            int repeticiones = 32;

            var experimentos = new Dictionary<string, GeneradorExperimento>();
            experimentos.Add("Exp1_Enteros", GenExp1_Enteros);
            experimentos.Add("Exp2_Reales_0a1", GenExp2_Reales_0_1);
            experimentos.Add("Exp3_Estructuradas", GenExp3_Estructuradas);
            experimentos.Add("Exp4_Reales2Dec", GenExp4_Reales2Decimales);
            experimentos.Add("Exp5_LongLong", GenExp5_LongLong);

            Stopwatch sw = new Stopwatch();
            List<ResultadoExperimento> listaResultados = new List<ResultadoExperimento>();

            Console.WriteLine("--- INICIANDO BENCHMARKING ---");

            foreach (var exp in experimentos)
            {
                Console.WriteLine(string.Format("=== Ejecutando {0} ===", exp.Key));
                ResultadoExperimento res = new ResultadoExperimento();
                res.Nombre = exp.Key;
                res.EsExpHibrido = (exp.Key == "Exp5_LongLong"); // SOLO ACTIVO EN LONG LONG

                foreach (int n in tamanosN)
                {
                    Console.Write(string.Format(" Midiendo tamano n={0}... ", n));
                    List<double> tiemposClasico = new List<double>();
                    List<double> tiemposStrassen = new List<double>();
                    List<double> tiemposHib16 = new List<double>(), tiemposHib32 = new List<double>(), tiemposHib64 = new List<double>();

                    for (int i = 0; i < repeticiones; i++)
                    {
                        double[][] A, B;
                        exp.Value(n, out A, out B);

                        sw.Restart(); MultiplicacionClasica(A, B); sw.Stop();
                        tiemposClasico.Add(sw.Elapsed.TotalSeconds);

                        sw.Restart(); MultiplicacionStrassen(A, B); sw.Stop();
                        tiemposStrassen.Add(sw.Elapsed.TotalSeconds);

                        // SI ES EL EXPERIMENTO LONG LONG, EVALÚA LOS UMBRALES HÍBRIDOS
                        if (res.EsExpHibrido)
                        {
                            sw.Restart(); MultiplicacionHibrida(A, B, 16); sw.Stop(); tiemposHib16.Add(sw.Elapsed.TotalSeconds);
                            sw.Restart(); MultiplicacionHibrida(A, B, 32); sw.Stop(); tiemposHib32.Add(sw.Elapsed.TotalSeconds);
                            sw.Restart(); MultiplicacionHibrida(A, B, 64); sw.Stop(); tiemposHib64.Add(sw.Elapsed.TotalSeconds);
                        }
                    }

                    res.Ns.Add(n);
                    
                    double pClas = tiemposClasico.Average();
                    res.TClasico.Add(pClas); res.StdClasico.Add(CalcularDesviacionEstandar(tiemposClasico, pClas));
                    
                    double pStras = tiemposStrassen.Average();
                    res.TStrassen.Add(pStras); res.StdStrassen.Add(CalcularDesviacionEstandar(tiemposStrassen, pStras));
                    
                    if (res.EsExpHibrido)
                    {
                        double pHib16 = tiemposHib16.Average(); res.THib16.Add(pHib16); res.StdHib16.Add(CalcularDesviacionEstandar(tiemposHib16, pHib16));
                        double pHib32 = tiemposHib32.Average(); res.THib32.Add(pHib32); res.StdHib32.Add(CalcularDesviacionEstandar(tiemposHib32, pHib32));
                        double pHib64 = tiemposHib64.Average(); res.THib64.Add(pHib64); res.StdHib64.Add(CalcularDesviacionEstandar(tiemposHib64, pHib64));
                    }
                    Console.WriteLine("Completado.");
                }
                listaResultados.Add(res);
            }

            Console.WriteLine("\nGenerando CSV (resultados_csharp.csv)...");
            CultureInfo ci = CultureInfo.InvariantCulture;
            using (StreamWriter file = new StreamWriter("resultados_csharp.csv"))
            {
                file.WriteLine("Experimento,Tamano_n,Prom_Clasico,Std_Clasico,Prom_Strassen,Std_Strassen,Prom_Hib(n0=16),Prom_Hib(n0=32),Prom_Hib(n0=64)");
                foreach (var res in listaResultados)
                {
                    for (int i = 0; i < res.Ns.Count; i++)
                    {
                        if (res.EsExpHibrido)
                        {
                            file.WriteLine(string.Format(ci, "{0},{1},{2:F6},{3:F6},{4:F6},{5:F6},{6:F6},{7:F6},{8:F6}", 
                                res.Nombre, res.Ns[i], res.TClasico[i], res.StdClasico[i], res.TStrassen[i], res.StdStrassen[i], res.THib16[i], res.THib32[i], res.THib64[i]));
                        }
                        else
                        {
                            file.WriteLine(string.Format(ci, "{0},{1},{2:F6},{3:F6},{4:F6},{5:F6},,,", 
                                res.Nombre, res.Ns[i], res.TClasico[i], res.StdClasico[i], res.TStrassen[i], res.StdStrassen[i]));
                        }
                    }
                }
            }

            Console.WriteLine("Generando graficos en Python...");
            StringBuilder pyCode = new StringBuilder();
            pyCode.AppendLine("import matplotlib.pyplot as plt\nimport sys\nexperimentos = {}");

            foreach (var res in listaResultados)
            {
                pyCode.AppendLine(string.Format("experimentos['{0}'] = {{}}", res.Nombre));
                pyCode.AppendLine(string.Format("experimentos['{0}']['n'] = [{1}]", res.Nombre, string.Join(",", res.Ns)));
                pyCode.AppendLine(string.Format("experimentos['{0}']['clasico'] = [{1}]", res.Nombre, string.Join(",", res.TClasico.Select(x => x.ToString(ci)))));
                pyCode.AppendLine(string.Format("experimentos['{0}']['strassen'] = [{1}]", res.Nombre, string.Join(",", res.TStrassen.Select(x => x.ToString(ci)))));
                pyCode.AppendLine(string.Format("experimentos['{0}']['is_hybrid'] = {1}", res.Nombre, res.EsExpHibrido ? "True" : "False"));
                
                if (res.EsExpHibrido)
                {
                    pyCode.AppendLine(string.Format("experimentos['{0}']['hib16'] = [{1}]", res.Nombre, string.Join(",", res.THib16.Select(x => x.ToString(ci)))));
                    pyCode.AppendLine(string.Format("experimentos['{0}']['hib32'] = [{1}]", res.Nombre, string.Join(",", res.THib32.Select(x => x.ToString(ci)))));
                    pyCode.AppendLine(string.Format("experimentos['{0}']['hib64'] = [{1}]", res.Nombre, string.Join(",", res.THib64.Select(x => x.ToString(ci)))));
                }
            }

            pyCode.AppendLine(@"
for exp_name, data in experimentos.items():
    ns = data['n']
    plt.figure(figsize=(10, 6))
    
    plt.loglog(ns, data['clasico'], 'o-', linewidth=2, label='Clasico Empirico')
    plt.loglog(ns, data['strassen'], 's-', linewidth=2, label='Strassen Puro')
    
    if data['is_hybrid']:
        plt.loglog(ns, data['hib16'], '^-', linewidth=2, label='Hibrido (n0=16)')
        plt.loglog(ns, data['hib32'], 'v-', linewidth=2, label='Hibrido (n0=32)')
        plt.loglog(ns, data['hib64'], 'd-', linewidth=2, label='Hibrido (n0=64)')
    
    c_clas = data['clasico'][-1] / (ns[-1]**3)
    plt.loglog(ns, [c_clas * (n**3) for n in ns], 'k--', alpha=0.5, label='Teorico O(n^3)')
    
    c_stras = data['strassen'][-1] / (ns[-1]**2.807)
    plt.loglog(ns, [c_stras * (n**2.807) for n in ns], 'r--', alpha=0.5, label='Teorico O(n^2.81)')
    
    plt.xlabel('Tamano de Matriz (n) [Escala Log]')
    plt.ylabel('Tiempo Promedio (segundos) [Escala Log]')
    plt.title('Comparacion C# - ' + exp_name)
    plt.xticks(ns, [str(val) for val in ns])
    plt.legend()
    plt.grid(True, which='both', ls='--', alpha=0.5)
    
    plt.savefig('CSharp_Grafico_' + exp_name + '.png')
    plt.close()
");
            File.WriteAllText("graficador_csharp.py", pyCode.ToString());

            try { Process.Start(new ProcessStartInfo { FileName="python", Arguments="graficador_csharp.py", UseShellExecute=false }).WaitForExit(); }
            catch (Exception) { Console.WriteLine("\nError al llamar a Python. Instale matplotlib."); }
            
            Console.WriteLine("Proceso finalizado con exito.");
        }
    }
}