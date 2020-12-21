
using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Threading.Tasks;
using CG_Biblioteca;

namespace gcgcg
{
    enum Ambiente
    {
        CASA_AZUL,
        CASA_VERMELHA,
        CAMINHO_AZUL,
        CAMINHO_VERMELHO,
        MEIO,
        TRILHA
    }

    internal class Tabuleiro : ObjetoGeometria
    {
        // indica a posição na trilha onde cada jogador (azul e vermelho) iniciam
        private int posJogadorAzul; 
        private int posJogadorVermelho;

        // indica a posição central onde o dado deve ser colocado
        private int posDado;

        // indica a posição da primeira plataforma de cada casa
        private int posCasaAzul;
        private int posCasaVermelha;

        // indicam o índice dos pontos que representam a face de cima
        private int a = 3;
        private int b = 2;
        private int c = 6;
        private int d = 7;

        // quantidade de pontos de cada elemento do tabuleiro (quad)
        private int QUANTIDADE_PONTOS = 8;

        // salva o jogador da vez (1 para azul, 2 para vermelho)
        private int jogador = 1;

        // recebe true quando o jogador da vez tira um 6, para que ele possa jogar novamente
        private bool jogarNovamente = false;

        // objetos do tabuleiro
        private Dado obj_Dado;
        private List<Peca> pecasAzuis = new List<Peca>();
        private List<Peca> pecasVermelhas = new List<Peca>();

        // listas das posições dentro do tabuleiro
        private List<int> trilha = new List<int>();
        private List<int> caminhoVermelho = new List<int>();
        private List<int> caminhoAzul = new List<int>();
        private List<int> casaAzul = new List<int>();
        private List<int> casaVermelha = new List<int>();

        public Tabuleiro(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {      
            // adiciona chão
            addPontosFaces(-9.1f, 9.1f, -0.2f, 0.0f, 9.1f, -9.1f);

            int posTrilha = 8;
            float inicio_x = -0.7f;
            float inicio_z = 8.9f;

            // adiciona os pontos das plataformas da trilha - listaPto[8] a 
            for(int i = 1; i <= 12; i++)
            {
                // se i != 1 começa a contar em 2, pois a primeira peça já foi adicionada
                // se i == 3, 6, 9 ou 12, então possui somente 3 peças na fileira, senão possui 7
                for(int j = (i == 1 ? 1 : 2); (i % 3 == 0 ? j <= 3 : j <= 7); j++)
                {
                    trilha.Add(posTrilha);
                    addPontosFaces(inicio_x-1f, inicio_x, 0.0f, 0.2f, inicio_z, inicio_z-1f);

                    // salva a posição das plataformas importantes do jogo
                    if (i == 1 && j == 2)
                    {
                        posJogadorAzul = posTrilha;
                    }
                    else if (i == 7 && j == 2)
                    {
                        posJogadorVermelho = posTrilha;
                    }

                    posTrilha += 8;

                    // muda as variáveis para a próxima plataforma
                    if ( (i % 3 == 0 && j != 3) || (i % 3 != 0 && j != 7) )
                    {
                        if(i == 1 || i == 3 || i == 5)
                        {
                            inicio_z -= 1.2f;
                        }
                        else if (i == 2 || i == 10 || i == 12)
                        {
                            inicio_x -= 1.2f;
                        }
                        else if (i == 4 || i == 6 || i == 8)
                        {
                            inicio_x += 1.2f;
                        }
                        else if (i == 7 || i == 9 || i == 11)
                        {
                            inicio_z += 1.2f;
                        }
                    }
                }
                
                if (i == 12 || i == 2 || i == 4)
                {
                    inicio_z -= 1.2f;
                }

                else if (i == 1 || i == 9 || i == 11)
                {
                    inicio_x -= 1.2f;
                }
                else if (i == 3 || i == 5 || i == 7)
                {
                    inicio_x += 1.2f;
                }
                else if (i == 6 || i == 8 || i == 10)
                {
                    inicio_z += 1.2f;
                }
            }

            inicio_x = 0.5f;
            inicio_z = 7.7f;

            for(int i = 1; i <= 13; i++)
            {
                if(i <= 6)
                {
                    caminhoAzul.Add(posTrilha);
                }
                else if(i == 7)
                {
                    posDado = posTrilha;
                }
                else
                {
                    caminhoVermelho.Add(posTrilha);
                }

                addPontosFaces(inicio_x-1f, inicio_x, 0.0f, 0.2f, inicio_z, inicio_z-1f);

                posTrilha += 8;

                // muda as variáveis para a próxima plataforma
                inicio_z -= 1.2f;
            }

            // adiciona os pontos da casa azul
                // muros da casa
            casaAzul.Add(posTrilha);
            casaAzul.Add(posTrilha+8);
            casaAzul.Add(posTrilha+16);
            casaAzul.Add(posTrilha+24);
            addPontosFaces(-2.9f, -1.9f, 0.0f, 0.6f, 6.5f, 1.9f);
            addPontosFaces(-8.9f, -2.9f, 0.0f, 0.6f, 2.9f, 1.9f);
            addPontosFaces(-8.9f, -7.9f, 0.0f, 0.6f, 8.9f, 2.9f);
            addPontosFaces(-7.9f, -1.9f, 0.0f, 0.6f, 8.9f, 7.9f);
            
                // plataformas das peças
            posCasaAzul = posTrilha+32;
            casaAzul.Add(posTrilha+32);
            casaAzul.Add(posTrilha+40);
            casaAzul.Add(posTrilha+48);
            casaAzul.Add(posTrilha+56);
            addPontosFaces(-5.0f, -3.6f, 0.0f, 0.2f, 7.2f, 5.8f);
            addPontosFaces(-7.2f, -5.8f, 0.0f, 0.2f, 7.2f, 5.8f);
            addPontosFaces(-5.0f, -3.6f, 0.0f, 0.2f, 5.0f, 3.6f);
            addPontosFaces(-7.2f, -5.8f, 0.0f, 0.2f, 5.0f, 3.6f);

            posTrilha += 64;

            // adiciona os pontos da casa vermelha
                // muros da casa
            casaVermelha.Add(posTrilha);
            casaVermelha.Add(posTrilha+8);
            casaVermelha.Add(posTrilha+16);
            casaVermelha.Add(posTrilha+24);
            addPontosFaces(1.9f, 2.9f, 0.0f, 0.6f, -1.9f, -6.5f);
            addPontosFaces(2.9f, 8.9f, 0.0f, 0.6f, -1.9f, -2.9f);
            addPontosFaces(7.9f, 8.9f, 0.0f, 0.6f, -2.9f, -8.9f);
            addPontosFaces(1.9f, 7.9f, 0.0f, 0.6f, -7.9f, -8.9f);
            
                // plataformas das peças
            posCasaVermelha = posTrilha+32;
            casaVermelha.Add(posTrilha+32);
            casaVermelha.Add(posTrilha+40);
            casaVermelha.Add(posTrilha+48);
            casaVermelha.Add(posTrilha+56);
            addPontosFaces(3.6f, 5.0f, 0.0f, 0.2f, -5.8f, -7.2f);
            addPontosFaces(5.8f, 7.2f, 0.0f, 0.2f, -5.8f, -7.2f);
            addPontosFaces(3.6f, 5.0f, 0.0f, 0.2f, -3.6f, -5.0f);
            addPontosFaces(5.8f, 7.2f, 0.0f, 0.2f, -3.6f, -5.0f);

            posTrilha += 64;

            Ponto4D pto;

            pto = getPontoInicialPeca(Ambiente.MEIO, 0, 0.8f);
            obj_Dado = new Dado(Utilitario.charProximo(), this, pto, 0.8f, new Cor(0, 0, 255, 255), new Cor(255, 0, 0, 255));

            Cor cor = new Cor(0,0,255,255);

            Peca p;

            for(int i = 0; i < 4; i++)
            {
                pto = getPontoInicialPeca(Ambiente.CASA_AZUL, i, 0.6f);
                p = new Peca(Utilitario.charProximo(), this, pto, 0.6f, cor, Ambiente.CASA_AZUL, i);
                pecasAzuis.Add(p);
            }
            
            cor = new Cor(255,0,0,255);

            for(int i = 0; i < 4; i++)
            {
                pto = getPontoInicialPeca(Ambiente.CASA_VERMELHA, i, 0.6f);
                p = new Peca(Utilitario.charProximo(), this, pto, 0.6f, cor, Ambiente.CASA_VERMELHA, i);
                pecasVermelhas.Add(p);
            }
        }

        private void addPontosFaces(float xEsquerda, float xDireita, float yBaixo, float yCima, float zFrente, float zTras)
        {
            base.PontosAdicionar(new Ponto4D(xEsquerda, yBaixo, zFrente));
            base.PontosAdicionar(new Ponto4D(xDireita, yBaixo, zFrente));
            base.PontosAdicionar(new Ponto4D(xDireita, yCima, zFrente));
            base.PontosAdicionar(new Ponto4D(xEsquerda, yCima, zFrente));
            base.PontosAdicionar(new Ponto4D(xEsquerda, yBaixo, zTras));
            base.PontosAdicionar(new Ponto4D(xDireita, yBaixo, zTras));
            base.PontosAdicionar(new Ponto4D(xDireita, yCima, zTras));
            base.PontosAdicionar(new Ponto4D(xEsquerda, yCima, zTras));
        }

        private void addFaces(int posicao, Cor corPrincipal, Cor corLaterais)
        {
            // Sentido anti-horário

            GL.Begin(PrimitiveType.Quads);

            // Face da frente e do fundo
            GL.Color3(corLaterais.CorR/255f,corLaterais.CorG/255f,corLaterais.CorB/255f);
            GL.Normal3(0, 0, 1);        
            GL.Vertex3(base.pontosLista[posicao + 0].X, base.pontosLista[posicao + 0].Y, base.pontosLista[posicao + 0].Z);    // PtoA
            GL.Vertex3(base.pontosLista[posicao + 1].X, base.pontosLista[posicao + 1].Y, base.pontosLista[posicao + 1].Z);    // PtoB
            GL.Vertex3(base.pontosLista[posicao + 2].X, base.pontosLista[posicao + 2].Y, base.pontosLista[posicao + 2].Z);    // PtoC
            GL.Vertex3(base.pontosLista[posicao + 3].X, base.pontosLista[posicao + 3].Y, base.pontosLista[posicao + 3].Z);    // PtoD
            
            GL.Normal3(0, 0, -1);
            GL.Vertex3(base.pontosLista[posicao + 4].X, base.pontosLista[posicao + 4].Y, base.pontosLista[posicao + 4].Z);    // PtoE
            GL.Vertex3(base.pontosLista[posicao + 7].X, base.pontosLista[posicao + 7].Y, base.pontosLista[posicao + 7].Z);    // PtoH
            GL.Vertex3(base.pontosLista[posicao + 6].X, base.pontosLista[posicao + 6].Y, base.pontosLista[posicao + 6].Z);    // PtoG
            GL.Vertex3(base.pontosLista[posicao + 5].X, base.pontosLista[posicao + 5].Y, base.pontosLista[posicao + 5].Z);    // PtoF
            
            // Face de cima (verde)
            GL.Color3(corPrincipal.CorR/255f,corPrincipal.CorG/255f,corPrincipal.CorB/255f);
            GL.Normal3(0, 1, 0);
            GL.Vertex3(base.pontosLista[posicao + 3].X, base.pontosLista[posicao + 3].Y, base.pontosLista[posicao + 3].Z);    // PtoD
            GL.Vertex3(base.pontosLista[posicao + 2].X, base.pontosLista[posicao + 2].Y, base.pontosLista[posicao + 2].Z);    // PtoC
            GL.Vertex3(base.pontosLista[posicao + 6].X, base.pontosLista[posicao + 6].Y, base.pontosLista[posicao + 6].Z);    // PtoG
            GL.Vertex3(base.pontosLista[posicao + 7].X, base.pontosLista[posicao + 7].Y, base.pontosLista[posicao + 7].Z);    // PtoH
            
            // Face de baixo (verde)
            GL.Color3(corPrincipal.CorR/255f,corPrincipal.CorG/255f,corPrincipal.CorB/255f);
            GL.Normal3(0, -1, 0);
            GL.Vertex3(base.pontosLista[posicao + 0].X, base.pontosLista[posicao + 0].Y, base.pontosLista[posicao + 0].Z);    // PtoA
            GL.Vertex3(base.pontosLista[posicao + 4].X, base.pontosLista[posicao + 4].Y, base.pontosLista[posicao + 4].Z);    // PtoE
            GL.Vertex3(base.pontosLista[posicao + 5].X, base.pontosLista[posicao + 5].Y, base.pontosLista[posicao + 5].Z);    // PtoF
            GL.Vertex3(base.pontosLista[posicao + 1].X, base.pontosLista[posicao + 1].Y, base.pontosLista[posicao + 1].Z);    // PtoB
            
            // Face da direita e esquerda (verde escuro)
            GL.Color3(corLaterais.CorR/255f,corLaterais.CorG/255f,corLaterais.CorB/255f);
            GL.Normal3(1, 0, 0);
            GL.Vertex3(base.pontosLista[posicao + 1].X, base.pontosLista[posicao + 1].Y, base.pontosLista[posicao + 1].Z);    // PtoB
            GL.Vertex3(base.pontosLista[posicao + 5].X, base.pontosLista[posicao + 5].Y, base.pontosLista[posicao + 5].Z);    // PtoF
            GL.Vertex3(base.pontosLista[posicao + 6].X, base.pontosLista[posicao + 6].Y, base.pontosLista[posicao + 6].Z);    // PtoG
            GL.Vertex3(base.pontosLista[posicao + 2].X, base.pontosLista[posicao + 2].Y, base.pontosLista[posicao + 2].Z);    // PtoC
            
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(base.pontosLista[posicao + 0].X, base.pontosLista[posicao + 0].Y, base.pontosLista[posicao + 0].Z);    // PtoA
            GL.Vertex3(base.pontosLista[posicao + 3].X, base.pontosLista[posicao + 3].Y, base.pontosLista[posicao + 3].Z);    // PtoD
            GL.Vertex3(base.pontosLista[posicao + 7].X, base.pontosLista[posicao + 7].Y, base.pontosLista[posicao + 7].Z);    // PtoH
            GL.Vertex3(base.pontosLista[posicao + 4].X, base.pontosLista[posicao + 4].Y, base.pontosLista[posicao + 4].Z);    // PtoE

            GL.End();
        }

        protected override void DesenharObjeto()
        {   
            // Cria chão do jogo
            addFaces(0, new Cor(0, 142, 0, 255), new Cor(0, 82, 0, 255));

            // Cria as plataformas da trilha
            foreach (int pos in trilha)
            {
                if(pos == posJogadorAzul)
                    addFaces(pos, new Cor(0, 0, 255, 255), new Cor(0, 0, 179, 255)); // azul
                else if(pos == posJogadorVermelho)
                    addFaces(pos, new Cor(255, 0, 0, 255), new Cor(196, 0, 0, 255)); // vermelho
                else
                    addFaces(pos, new Cor(255, 255, 255, 255), new Cor(218, 218, 218, 255)); // branco
            }
            int cont = 0;
            // Cria as plataformas do caminho azul (caminho que leva o jogador para a vitória)
            foreach (int pos in caminhoAzul)
            {
                cont++;
                addFaces(pos, new Cor(0, 0, 255, 255), new Cor(0, 0, 179, 255)); // azul
                if(cont == 6)
                {
                    GL.LineWidth(2);
                    GL.Begin(PrimitiveType.LineLoop);
                    GL.Color3(1f,1f,1f);
                    GL.Vertex3(base.pontosLista[pos + 3].X, base.pontosLista[pos + 3].Y, base.pontosLista[pos + 3].Z);
                    GL.Vertex3(base.pontosLista[pos + 2].X, base.pontosLista[pos + 2].Y, base.pontosLista[pos + 2].Z);
                    GL.Vertex3(base.pontosLista[pos + 6].X, base.pontosLista[pos + 6].Y, base.pontosLista[pos + 6].Z);
                    GL.Vertex3(base.pontosLista[pos + 7].X, base.pontosLista[pos + 7].Y, base.pontosLista[pos + 7].Z);
                    GL.End();
                }
            }

            // cria plataforma central para o dado
            addFaces(posDado, new Cor(255, 255, 0, 255), new Cor(164, 164, 0, 255)); // amarelo

            cont = 0;
            // Cria as plataformas do caminho vermelho (caminho que leva o jogador para a vitória)
            foreach (int pos in caminhoVermelho)
            {
                cont++;
                addFaces(pos, new Cor(255, 0, 0, 255), new Cor(196, 0, 0, 255)); // vermelho
                if(cont == 0)
                {
                    GL.LineWidth(2);
                    GL.Begin(PrimitiveType.LineLoop);
                    GL.Color3(1f,1f,1f);
                    GL.Vertex3(base.pontosLista[pos + 3].X, base.pontosLista[pos + 3].Y, base.pontosLista[pos + 3].Z);
                    GL.Vertex3(base.pontosLista[pos + 2].X, base.pontosLista[pos + 2].Y, base.pontosLista[pos + 2].Z);
                    GL.Vertex3(base.pontosLista[pos + 6].X, base.pontosLista[pos + 6].Y, base.pontosLista[pos + 6].Z);
                    GL.Vertex3(base.pontosLista[pos + 7].X, base.pontosLista[pos + 7].Y, base.pontosLista[pos + 7].Z);
                    GL.End();
                }
            }

            // Cria as plataformas da casa azul
            foreach (int pos in casaAzul)
            {
                addFaces(pos, new Cor(0, 0, 255, 255), new Cor(0, 0, 179, 255)); // azul
            }

            // Cria as plataformas da casa vermelha
            foreach (int pos in casaVermelha)
            {
                addFaces(pos, new Cor(255, 0, 0, 255), new Cor(196, 0, 0, 255)); // vermelho
            }
        }

        public Ponto4D getPontoInicialPeca(Ambiente amb, int posicao = 0, float tamanho = 0.6f)
        {
            List<Ponto4D> pontos = new List<Ponto4D>();
            int pontoInicial = 0;
            switch(amb)
            {
                case Ambiente.CASA_AZUL:
                    pontoInicial = posCasaAzul + posicao * QUANTIDADE_PONTOS;
                    break;
                case Ambiente.CASA_VERMELHA:
                    pontoInicial = posCasaVermelha + posicao * QUANTIDADE_PONTOS;
                    break;
                // case Ambiente.CAMINHO_AZUL:
                //     pontoInicial = caminhoAzul[posicao];
                //     break;
                // case Ambiente.CAMINHO_VERMELHO:
                //     pontoInicial = caminhoVermelho[posicao];
                //     break;
                case Ambiente.MEIO:
                    pontoInicial = posDado;
                    break;
                case Ambiente.TRILHA:
                    pontoInicial = trilha[posicao == 0 ? posJogadorAzul : posJogadorVermelho];
                    break;
            }

            // sentido anti-horário
            double metadePeca = tamanho / 2;
            double metadePlataforma = (base.pontosLista[pontoInicial + b].X - base.pontosLista[pontoInicial + a].X) / 2;

            return (new Ponto4D(base.pontosLista[pontoInicial + a].X + metadePlataforma - metadePeca, base.pontosLista[pontoInicial + a].Y+0.1, base.pontosLista[pontoInicial + a].Z - metadePlataforma + metadePeca)); // A
        }

        public void enter()
        {
            Task.Run(async delegate
            {
                if(!jogarNovamente)
                {
                    if(jogador == 1)
                        jogador = 2;
                    else
                        jogador = 1;
                    
                    obj_Dado.mudaCor(jogador);
                }
                int numero = obj_Dado.girarDado();

                if(numero == 6)
                    jogarNovamente = true;
                else
                    jogarNovamente = false;
                

                await Task.Delay(5000);
            });
        }

        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Tabuleiro: " + base.rotulo + "\n";
            for (var i = 0; i < 8; i++) // 8 pontos que representam o tabuleiro como um todo
            {
                retorno += "P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]" + "\n";
            }
            return (retorno);
        }

    }
}