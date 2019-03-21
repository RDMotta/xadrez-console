
using System;
using tabuleiro;
using xadrez;

namespace xadrez_console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PertidaDeXadrez partida = new PertidaDeXadrez();

                while (!partida.PartidaTerminada)
                {
                    try
                    {
                        Console.Clear();
                        Tela.ImprimirPartida(partida);

                        Console.WriteLine();
                        Console.Write("Digite a posição de origem:");
                        Posicao posicaoOrigem = Tela.LerPosicaoXadrez().ToPosicao();
                        partida.ValidarPosicaoOrigem(posicaoOrigem);

                        bool[,] posicoesPossiveis = partida.Tabuleiro.Peca(posicaoOrigem).MovimentosPossiveis();
                        Console.Clear();
                        Tela.ImprimirTabuleiro(partida.Tabuleiro, posicoesPossiveis);

                        Console.WriteLine();
                        Console.Write("Digite a posição de destino:");
                        Posicao posicaoDestino = Tela.LerPosicaoXadrez().ToPosicao();
                        partida.ValidarPosicaoDestino(posicaoOrigem, posicaoDestino);

                        partida.RelizaJogada(posicaoOrigem, posicaoDestino);
                    }
                    catch (TabuleiroException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }
                }
                
            }
            catch (TabuleiroException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}
