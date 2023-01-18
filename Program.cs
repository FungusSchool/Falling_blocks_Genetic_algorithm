using System.Collections.Generic;
using System;
using Raylib_cs;

namespace Dodgers
{
    class Program
    {

        static int windowW = 800;
        static int windowH = 600;

        static Random generator = new Random();
        static Rectangle coin = new Rectangle(200, -100, 50, 50);
        static Rectangle player = new Rectangle(100, windowH - 50, 200, 25);
        static Rectangle enemy = new Rectangle(400, -100, 50, 50);
        static string[] coinCords = File.ReadAllLines("coinCords.txt");
        static string[] enemyCords = File.ReadAllLines("enemyCords.txt");
        static int coinTurn = 0;
        static int enemyTurn = 0;
        static int points = 0;
        static float time = 0;
        static int speedTime = 15;
        static int speed = 2;
        static int life = 3;

        static bool aiMove = false;
        static int motionMultiplier = 48;
        static int geneLenght = 100;
        //static AI lastBestGuy = new AI("0");

        static void Main(string[] args)
        {
            // Initialisering
            //--------------------------------------------------------------------------------------

            Raylib.InitWindow(windowW, windowH, "GeneticAlgorithm");
            Raylib.SetTargetFPS(120);

            // TODO: Infoga variabler och objekt här



            //AI Variables 
            int populationsSize = 100;
            int generations = 20000;
            int eliteSize = 3;
            int mutationRate = 20;

            AI lastBestGuy = new AI("0");

            List<AI> population = new List<AI>();
            List<AI> matingPool = new List<AI>();
            List<AI> newPopulation = new List<AI>();
            List<List<AI>> generationsList = new List<List<AI>>();

            GeneratePopulation(populationsSize, ref population);
            // Run each generation
            /*             for (motionMultiplier = ; motionMultiplier < 60; motionMultiplier++)
                        { */
            for (var i = 0; i < generations; i++)
            {
                //Console.WriteLine("1 Last best guy" + lastBestGuy.fitness);
                generationsList.Add(new List<AI>());
                //Console.WriteLine("Population Size: " + populationsSize);
                Console.WriteLine("Generation: " + i + " motionMultiplier: " + motionMultiplier);
                //Console.WriteLine("2 Last best guy" + lastBestGuy.fitness);

                int counter = 0;
                foreach (AI ai in population)
                {
                    //Console.WriteLine("3 Last best guy" + lastBestGuy.fitness);

                    // Reset all the game variables
                    coinTurn = 0;
                    enemyTurn = 0;
                    points = 0;
                    time = 0;
                    speedTime = 15;
                    speed = 2;
                    life = 3;
                    coin.x = 200;
                    coin.y = -100;
                    player.x = 100;
                    enemy.x = 400;
                    enemy.y = -100;

                    //Console.WriteLine("3.5 Last best guy" + lastBestGuy.fitness);
                    // if (ai == lastBestGuy)
                    // {
                    //     throw new Exception("Found it!");
                    // }
                    if (ai.fitness == -1)
                    ai.fitness = ai.PlayGame();
                   // Console.WriteLine("4 Last best guy" + lastBestGuy.fitness);

                    counter++;
                    //Console.WriteLine("AI: " + counter);
                    // Console.WriteLine("Genes: " + ai.genes);
                    //Console.WriteLine(ai.fitness + " points");
                    if (ai.fitness > 11)
                    {
                        //Console.WriteLine("5 Last best guy" + lastBestGuy.fitness);

                        File.AppendAllText("ai.txt", ai.fitness.ToString() + ". Generation: " + i.ToString() + " Genes: " + ai.genes + " Moves: " + ai.moves + " motionMultiplier: " + motionMultiplier + Environment.NewLine);

                    }
                    generationsList[i].Add(ai);
                }
                if (i == generations - 1)
                {
                    //Console.WriteLine("6 Last best guy" + lastBestGuy.fitness);

                    generationsList[i].Sort((x, y) => y.fitness.CompareTo(x.fitness));
                    foreach (AI ai in generationsList[i])
                    {
                        Console.WriteLine(ai.fitness + " points");
                    }
                }
                //Console.WriteLine("Last Best Guy: " + lastBestGuy.fitness + " points" + " Genes: " + lastBestGuy.genes + " Moves: " + lastBestGuy.moves);
                newGeneration(populationsSize, generations, eliteSize, mutationRate, ref population, ref matingPool, ref newPopulation, ref lastBestGuy);
                population = newPopulation;
                //Console.WriteLine("7 Last best guy" + lastBestGuy.fitness);

            }

            //}

            //--------------------------------------------------------------------------------------

        }

        static void Speed(float time, int speedTime, int speed, ref int life)
        {
            if (time > speedTime)
            {
                speedTime += 15;
                speed += 1;
            }
            //Coin
            coin.y += speed;
            if (coin.y > windowH)
            {
                life--;
                ResetCoin();

            }
            //Enemy
            enemy.y += speed;
            if (enemy.y > windowH)
            {
                ResetEnemy();
            }
        }

        public static void Collision(ref int points, ref int life)
        {
            //with Coin
            if (Raylib.CheckCollisionRecs(player, coin))
            {
                ResetCoin();
                points += 1;
            }
            //With Enemy
            if (Raylib.CheckCollisionRecs(player, enemy))
            {

                ResetEnemy();
                life -= 1;
            }
        }
        static void ResetCoin()
        {
            coin.y = -100;
            coin.x = int.Parse(coinCords[coinTurn]);
            coinTurn = upTickAndReset(coinTurn, coinCords.Length - 1);

            // Run the AI next move
            aiMove = true;
        }
        static void ResetEnemy()
        {
            enemy.y = -100;
            // Console.WriteLine(enemyTurn);
            enemy.x = int.Parse(enemyCords[enemyTurn]);
            enemyTurn = upTickAndReset(enemyTurn, enemyCords.Length - 1);
            ;
        }
        static int upTickAndReset(int tick, int max)
        {
            tick++;
            if (tick > max)
            {
                tick = 0;
            }
            return tick;
        }
        static void playerInsideWindow()
        {
            if (player.x < 0)
            {
                player.x = 0;
            }
            if (player.x > windowW - 200)
            {
                player.x = windowW - 200;
            }
        }
        static void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DARKBLUE);

            Raylib.DrawRectangleRec(coin, Color.GOLD);
            Raylib.DrawRectangleRec(player, Color.GRAY);
            Raylib.DrawRectangleRec(enemy, Color.RED);

            Raylib.DrawText($"Points:{points} Life:{life} Time:{((int)time)}", 10, 10, 20, Color.GREEN);

            Raylib.EndDrawing();
        }
        static void GeneratePopulation(int size, ref List<AI> population)
        {
            string[] genesArray = File.ReadAllLines("GenesList.txt");
            for (int i = 0; i < size; i++)
            {
                population.Add(new AI(genesArray[i]));
            }
        }
        static void newGeneration(int populationsSize, int generations, int eliteSize, int mutationRate, ref List<AI> population, ref List<AI> matingPool, ref List<AI> newPopulation, ref AI lastBestGuy)
        {
            //Console.WriteLine("8 Last best guy" + lastBestGuy.fitness);

            // make a list of the best 10
            population.Sort((x, y) => y.fitness.CompareTo(x.fitness) == 0 ? x.moves.CompareTo(y.moves) : y.fitness.CompareTo(x.fitness));
            AI bestguy = population[0];
            //Console.WriteLine("Best guy: " + bestguy.fitness + " last: " + lastBestGuy.fitness);
            if (bestguy.fitness < lastBestGuy.fitness)
            {
                bestguy = lastBestGuy;
                Console.WriteLine("Best guy became worse");
            }
            lastBestGuy = bestguy;
            //Console.WriteLine("2 Best guy: " + bestguy.fitness + " last: " + lastBestGuy.fitness);

            matingPool.Clear();
            for (int i = 0; i < eliteSize; i++)
            {
                matingPool.Add(population[i]);
            }

            newPopulation.Clear();
            newPopulation.Add(bestguy);
            while (newPopulation.Count < populationsSize)
            {
                // pick two parents
                int parentA = generator.Next(0, eliteSize);
                int parentB = generator.Next(0, eliteSize);

                // make a child
                string childGenes = "";
                for (int j = 0; j < geneLenght; j++)
                {
                    if (generator.Next(0, 100) < mutationRate)
                    {
                        childGenes += generator.Next(0, 10);
                    }
                    else
                    {
                        if (generator.Next(0, 2) == 0)
                        {
                            childGenes += matingPool[parentA].genes[j];
                        }
                        else
                        {
                            childGenes += matingPool[parentB].genes[j];
                        }
                    }
                }
                newPopulation.Add(new AI(childGenes));
            }
        }
        class AI
        {
            public string genes;
            public int fitness = -1;
            public int moves;
            public AI(string genes)
            {
                this.genes = genes;
            }

            public int PlayGame()
            {
                moves = 0;
                int geneNumber = 0;
                // Animationsloopen

                while (!Raylib.WindowShouldClose() && life > 0)
                {
                    // Updatering
                    //----------------------------------------------------------------------------------
                    //Time
                    //Console.WriteLine("10 Last best guy" + lastBestGuy.fitness);

                    time += Raylib.GetFrameTime();
                    //speed
                    //Console.WriteLine("11 Last best guy" + lastBestGuy.fitness);

                    Speed(time, speedTime, speed, ref life);
                    //Input
                    //Console.WriteLine("12 Last best guy" + lastBestGuy.fitness);

                    if (aiMove)
                    {
                        InterpretGenes(genes, geneNumber);
                        geneNumber += 2;
                        moves++;
                    }
                    aiMove = false;
                    //Kollision
                    //Console.WriteLine("13Last best guy" + lastBestGuy.fitness);

                    Collision(ref points, ref life);
                    //Console.WriteLine("14 Last best guy" + lastBestGuy.fitness);

                    //----------------------------------------------------------------------------------

                    // Rita
                    //----------------------------------------------------------------------------------
                    //Draw();
                    //----------------------------------------------------------------------------------
                }
                //Console.WriteLine("15 Last best guy" + lastBestGuy.fitness);

                return points;
            }
            static void InterpretGenes(string genes, int geneNumber)
            {
                if (geneNumber >= genes.Length)
                {
                    File.AppendAllText("ai.txt", "ran out" + Environment.NewLine);
                    return;
                }
                char[] gene = genes.ToCharArray();
                // Bool ofr the direction true is left and false is right


                // Check which direction to move
                if (int.Parse(gene[geneNumber].ToString()) <= 4)
                {

                    // Move left
                    player.x -= (int.Parse(gene[geneNumber + 1].ToString()) * motionMultiplier);
                    playerInsideWindow();

                }
                else
                {
                    // Move right

                    player.x += (int.Parse(gene[geneNumber + 1].ToString()) * motionMultiplier);
                    playerInsideWindow();
                }


            }

        }
        static void Menu(List<List<AI>> generationsList)
        {

        }
    }
}
