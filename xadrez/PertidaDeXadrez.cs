using System.Collections.Generic;
using tabuleiro;

namespace xadrez 
{
    class PertidaDeXadrez
    {
        
        public int Turno { get; private set; }
        public Cor JogadorAtual { get; private set; }
        public Tabuleiro Tabuleiro { get; private set; }
        public Peca VulneravelEmPassant { get; private set; }
        public bool PartidaTerminada { get; private set; }
        public bool PartidaEmXeque { get; private set; }

        private HashSet<Peca> Pecas;
        private HashSet<Peca> PecasCapturadas;
        


        public PertidaDeXadrez()
        {
            this.Tabuleiro = new Tabuleiro(8, 8);
            this.Turno = 1;
            this.JogadorAtual = Cor.Branca;
            this.PartidaEmXeque = false;
            this.PartidaTerminada = false;
            this.Pecas = new HashSet<Peca>();
            this.PecasCapturadas = new HashSet<Peca>();
            this.VulneravelEmPassant = null;
            ColocarPecas();
        }


        public Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca peca = this.Tabuleiro.RetirarPeca(origem);
            peca.IncremetarMovimentos();
            Peca pecaCapturada = this.Tabuleiro.RetirarPeca(destino);
            this.Tabuleiro.ColocarPeca(peca, destino);

            if (pecaCapturada != null)
            {
                PecasCapturadas.Add(pecaCapturada);
            }
            //Roque pequeno
            if (peca is Rei && (destino.Coluna == origem.Coluna + 2))
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca torre = Tabuleiro.RetirarPeca(origemTorre);
                torre.IncremetarMovimentos();
                Tabuleiro.ColocarPeca(torre, destinoTorre);                   
            }
            //Roque grande
            if (peca is Rei && (destino.Coluna == origem.Coluna - 2))
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca torre = Tabuleiro.RetirarPeca(origemTorre);
                torre.IncremetarMovimentos();
                Tabuleiro.ColocarPeca(torre, destinoTorre);
            }
            //en passant
            if (peca is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == null)
                {
                    Posicao pos;
                    if (peca.Cor == Cor.Branca)
                    {
                        pos = new Posicao(destino.Linha + 1, destino.Coluna);                            
                    }
                    else
                    {
                        pos = new Posicao(destino.Linha - 1, destino.Coluna);
                    }
                    pecaCapturada = Tabuleiro.RetirarPeca(pos);
                    PecasCapturadas.Add(pecaCapturada);
                }
            }

            return pecaCapturada;
        }

        public void DesfazerMoviemnto(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca peca = Tabuleiro.RetirarPeca(destino);
            peca.DecremetarMovimentos();
            if (pecaCapturada != null)
            {
                Tabuleiro.ColocarPeca(pecaCapturada, destino);
                PecasCapturadas.Remove(pecaCapturada);
            }
            Tabuleiro.ColocarPeca(peca, origem);

            //Roque pequeno
            if (peca is Rei && (destino.Coluna == origem.Coluna + 2))
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca torre = Tabuleiro.RetirarPeca(destinoTorre);
                torre.DecremetarMovimentos();
                Tabuleiro.ColocarPeca(torre, origemTorre);
            }

            //Roque grande
            if (peca is Rei && (destino.Coluna == origem.Coluna - 2))
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca torre = Tabuleiro.RetirarPeca(destinoTorre);
                torre.DecremetarMovimentos();
                Tabuleiro.ColocarPeca(torre, origemTorre);
            }

            //en passant
            if (peca is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == VulneravelEmPassant)
                {

                    Peca pecaPeao = Tabuleiro.RetirarPeca(destino);
                    Posicao pos;
                    if (peca.Cor == Cor.Branca)
                    {
                        pos = new Posicao(3, destino.Coluna);
                    }
                    else
                    {
                        pos = new Posicao(4, destino.Coluna);
                    }
                    Tabuleiro.ColocarPeca(pecaPeao, pos);            
                }
            }
        }

        public void RelizaJogada(Posicao origem , Posicao destino)
        {
            Peca pecaCapturada = ExecutaMovimento(origem, destino);
            if (EstahEmXeque(JogadorAtual))
            {
                DesfazerMoviemnto(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }
                       
            PartidaEmXeque = EstahEmXeque(Adversaria(JogadorAtual));
            Peca peca = Tabuleiro.Peca(destino);

            if (peca is Peao)
            {
                if ((peca.Cor == Cor.Branca && destino.Linha == 0) || (peca.Cor == Cor.Preta && destino.Linha == 7))
                {
                    peca = Tabuleiro.RetirarPeca(destino);
                    Pecas.Remove(peca);
                    Peca dama = new Rainha(Tabuleiro, peca.Cor);
                    Tabuleiro.ColocarPeca(dama, destino);
                    Pecas.Add(dama);
                }
            }

            if (TestarXequeMate(Adversaria(JogadorAtual)))
            {
               PartidaTerminada = true;
            }
            else
            {
               Turno++;
               MudarJogador();
            }
            //teste en passant
            VulneravelEmPassant = (peca is Peao && ((destino.Linha == origem.Linha - 2) || (destino.Linha == origem.Linha + 2))) ? peca : null;           
        }

        public bool TestarXequeMate(Cor cor)
        {
            if (!EstahEmXeque(cor))
            {
                return false;
            }
            foreach (Peca peca in PegarPecasEmJogo(cor))
            {
                bool[,] mat = peca.MovimentosPossiveis();
                for (int i=0; i< Tabuleiro.Linhas; i++)
                {
                    for (int j=0; j< Tabuleiro.Colunas; j++)
                    {
                        Posicao origem = peca.Posicao;
                        Posicao destino = new Posicao(i, j);
                        Peca pecaCapturada = ExecutaMovimento(origem, destino);
                        bool testeXeque = EstahEmXeque(cor);
                        DesfazerMoviemnto(origem, destino, pecaCapturada);
                        if (!testeXeque)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void ValidarPosicaoOrigem(Posicao pos)
        {
            if (Tabuleiro.Peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição escolhida!");
            }
            if (JogadorAtual != Tabuleiro.Peca(pos).Cor)
            {
                throw new TabuleiroException("A peça escolhida não é sua!");
            }
            if (!Tabuleiro.Peca(pos).ExisteMovimentosPossiveis())
            {
                throw new TabuleiroException("Não existe posição possivel para mover!");
            }
        }

        public void ValidarPosicaoDestino(Posicao origem, Posicao destino)
        {
            if (!Tabuleiro.Peca(origem).PodeMoverPara(destino))
            {
                throw new TabuleiroException("Posicao de destino invalida!");
            }
        }

        public void ColocarNovaPeca(char coluna, int linha, Peca peca)
        {
            Tabuleiro.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            Pecas.Add(peca);
        }

        public HashSet<Peca> PegarPecasCapturadas(Cor cor)
        {
            HashSet<Peca> pecasDaCor = new HashSet<Peca>();
            foreach (Peca peca in PecasCapturadas)
            {
                if (peca.Cor == cor)
                {
                    pecasDaCor.Add(peca);
                }
            }
            return pecasDaCor;
        }

        public HashSet<Peca> PegarPecasEmJogo(Cor cor)
        {
            HashSet<Peca> pecasDaCor = new HashSet<Peca>();
            foreach (Peca peca in Pecas)
            {
                if (peca.Cor == cor)
                {
                    pecasDaCor.Add(peca);
                }
            }
            pecasDaCor.ExceptWith(PegarPecasCapturadas(cor));
            return pecasDaCor;
        }

        public bool EstahEmXeque(Cor cor)
        {
            Peca rei = PegarRei(cor);
            if (rei == null)
            {
                throw new TabuleiroException("Não existe um rei da cor " + cor + " no tabuleiro!");
            }

            foreach(Peca peca in PegarPecasEmJogo(Adversaria(cor)))
            {
                bool[,] mat = peca.MovimentosPossiveis();
                if (mat[rei.Posicao.Linha, rei.Posicao.Coluna])
                {
                    return true;
                }
            }
            return false;
        }

        private Cor Adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            }
            else
            {
                return Cor.Branca;
            }
        }

        private Peca PegarRei(Cor cor)
        {
            foreach (Peca peca in PegarPecasEmJogo(cor))
            {
                if (peca is Rei)
                {
                    return peca;
                }
            }
            return null;
        }

        private void MudarJogador()
        {
            if (JogadorAtual == Cor.Branca)
            {
                JogadorAtual = Cor.Preta;
            }
            else
            {
                JogadorAtual = Cor.Branca;
            }
        }

        private void ColocarPecas()
        {
            ColocarNovaPeca('a', 1, new Torre(Tabuleiro, Cor.Branca));
            ColocarNovaPeca('b', 1, new Cavalo(Tabuleiro, Cor.Branca));
            ColocarNovaPeca('c', 1, new Bispo(Tabuleiro, Cor.Branca));
            ColocarNovaPeca('d', 1, new Rainha(Tabuleiro, Cor.Branca));
            ColocarNovaPeca('e', 1, new Rei(Tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('f', 1, new Bispo(Tabuleiro, Cor.Branca));
            ColocarNovaPeca('g', 1, new Cavalo(Tabuleiro, Cor.Branca));
            ColocarNovaPeca('h', 1, new Torre(Tabuleiro, Cor.Branca));

            ColocarNovaPeca('a', 2, new Peao(Tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('b', 2, new Peao(Tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('c', 2, new Peao(Tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('d', 2, new Peao(Tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('e', 2, new Peao(Tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('f', 2, new Peao(Tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('g', 2, new Peao(Tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('h', 2, new Peao(Tabuleiro, Cor.Branca, this));


            ColocarNovaPeca('a', 8, new Torre(Tabuleiro, Cor.Preta));
            ColocarNovaPeca('b', 8, new Cavalo(Tabuleiro, Cor.Preta));
            ColocarNovaPeca('c', 8, new Bispo(Tabuleiro, Cor.Preta));
            ColocarNovaPeca('d', 8, new Rainha(Tabuleiro, Cor.Preta));
            ColocarNovaPeca('e', 8, new Rei(Tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('f', 8, new Bispo(Tabuleiro, Cor.Preta));
            ColocarNovaPeca('g', 8, new Cavalo(Tabuleiro, Cor.Preta));
            ColocarNovaPeca('h', 8, new Torre(Tabuleiro, Cor.Preta));

            ColocarNovaPeca('a', 7, new Peao(Tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('b', 7, new Peao(Tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('c', 7, new Peao(Tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('d', 7, new Peao(Tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('e', 7, new Peao(Tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('f', 7, new Peao(Tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('g', 7, new Peao(Tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('h', 7, new Peao(Tabuleiro, Cor.Preta, this));

        }



    }
}
