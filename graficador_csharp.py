import matplotlib.pyplot as plt
import sys
experimentos = {}
experimentos['Exp1_Enteros'] = {}
experimentos['Exp1_Enteros']['n'] = [16,32,64,128,256,512]
experimentos['Exp1_Enteros']['clasico'] = [1.384375E-05,4.9209375E-05,0.0004043,0.00358313125,0.03407250625,0.4147468]
experimentos['Exp1_Enteros']['strassen'] = [0.000331915625,0.001591759375,0.01074325625,0.079057884375,0.643209440625,6.5295069]
experimentos['Exp1_Enteros']['is_hybrid'] = False
experimentos['Exp2_Reales_0a1'] = {}
experimentos['Exp2_Reales_0a1']['n'] = [16,32,64,128,256,512]
experimentos['Exp2_Reales_0a1']['clasico'] = [1.01E-05,8.8959375E-05,0.000705634375,0.005664209375,0.048854434375,0.4158948625]
experimentos['Exp2_Reales_0a1']['strassen'] = [0.000385184375,0.00285649375,0.019226309375,0.129272046875,0.90552563125,6.543242953125]
experimentos['Exp2_Reales_0a1']['is_hybrid'] = False
experimentos['Exp3_Estructuradas'] = {}
experimentos['Exp3_Estructuradas']['n'] = [16,32,64,128,256,512]
experimentos['Exp3_Estructuradas']['clasico'] = [1.0834375E-05,8.500625E-05,0.000694015625,0.006002528125,0.042756925,0.2704009125]
experimentos['Exp3_Estructuradas']['strassen'] = [0.000447709375,0.0028266875,0.019232815625,0.14029158125,0.83416489375,4.123176465625]
experimentos['Exp3_Estructuradas']['is_hybrid'] = False
experimentos['Exp4_Reales2Dec'] = {}
experimentos['Exp4_Reales2Dec']['n'] = [16,32,64,128,256,512]
experimentos['Exp4_Reales2Dec']['clasico'] = [5.29375E-06,4.866875E-05,0.00041979375,0.003587390625,0.0292146125,0.256108559375]
experimentos['Exp4_Reales2Dec']['strassen'] = [0.00025166875,0.0015437625,0.010764009375,0.07862386875,0.545849346875,3.915612696875]
experimentos['Exp4_Reales2Dec']['is_hybrid'] = False
experimentos['Exp5_LongLong'] = {}
experimentos['Exp5_LongLong']['n'] = [16,32,64,128,256,512]
experimentos['Exp5_LongLong']['clasico'] = [1.0415625E-05,4.6275E-05,0.000397640625,0.00419909375,0.033200053125,0.253370253125]
experimentos['Exp5_LongLong']['strassen'] = [0.00079515,0.0017554,0.0132761125,0.091336521875,0.644045215625,3.922716075]
experimentos['Exp5_LongLong']['is_hybrid'] = True
experimentos['Exp5_LongLong']['hib16'] = [2.5015625E-05,9.885E-05,0.000459078125,0.003621228125,0.031260903125,0.218306871875]
experimentos['Exp5_LongLong']['hib32'] = [8.984375E-06,5.1375E-05,0.00039739375,0.00335334375,0.026597934375,0.173510153125]
experimentos['Exp5_LongLong']['hib64'] = [1.4228125E-05,4.730625E-05,0.000440540625,0.003765925,0.027347540625,0.182150471875]

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

