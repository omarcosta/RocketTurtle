using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace RocketTurtle
{
    internal class Turtle
    {
        // Atributos principais
        public string Nome { get; set; }
        public float Peso { get; set; }
        public string Cor { get; set; }
        public float Comprimento { get; set; }

        // Controle
        public int Posicao { get; set; } // Posição de 0 a 100 na corrida
        public double Resistencia { get; set; } // Tempo de correr sem cansar
        public double Velocidade { get; set; } // Velocidade variavel
        public int Dencanso { get; set; } // Tempo de descanso

        // Lista de nomes
        private static List<string> listaNomes = new List<string>
        {"TurboShell", "CascoVeloz", "Flashuga", "NitroTuga", "Relâmpago",
         "Casquinha Voadora", "Supershell", "Boltuga", "Tartarápido", "Foguetuga",
         "Rodinha", "JetShell", "Sonicasca", "Pé de Pano", "Casco Quente",
         "Sprintuga", "LaminaVerde", "Fumaça", "Velotuga", "RasgaCasco",
         "Tempestade", "Corridão", "CascoRacer", "PneuLento", "Turboleta",
         "ZigTuga", "Ligeirinho", "Correria", "Rápidus", "Tracajato"
        };

        // Lista de colorações
        private static  List<string> listaCores = new List<string>
        {
            "Verde musgo com casco enferrujado",
            "Azul com tattoos de raios",
            "Vermelha flamejante com listras douradas",
            "Preta com brilho metálico furtivo",
            "Roxa néon com detalhes em pixel art",
            "Amarela fosforescente com casco rachado",
            "Cinza grafite com olhos cibernéticos",
            "Laranja queimado com manchas de óleo",
            "Branco nevado com escamas prateadas",
            "Turquesa cristalina com padrão tribal"
        };

        private Random rnd = new Random();

        public string ExibirInfo()
        {
            string tartarugaInfo = "";
            tartarugaInfo += ($"\n\tNome: {this.Nome}");
            tartarugaInfo += ($"\n\tCor: {this.Cor}");
            tartarugaInfo += ($"\n\tComprimento: {this.Comprimento:F2} cm");
            tartarugaInfo += ($"\n\tPeso: {this.Peso:F2} kg");
            
            return tartarugaInfo;
        }

        public string GerarNome()
        {
            
            int i = rnd.Next(listaNomes.Count);
            string nameRnd = listaNomes[i];
            listaNomes.RemoveAt(i);
            
            return nameRnd;
        }

        public string GerarColoracao()
        {
            
            int i = rnd.Next(listaCores.Count);
            string corRnd = listaCores[i];
            listaCores.RemoveAt(i);

            return corRnd;
        }



    }
}
