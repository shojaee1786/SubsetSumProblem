using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GA_Subset_sum
{
    public partial class MainForm : Form
    {
        int N;       // Number of members
        int Goal;    // Goal number
        int[] myset;
        int population_size = 40;
        int[] chromosome;        // array for chromosomes which represent choice variable   0 or 1
        int[,] population;
        int[,] Offsprings;
        double[] fitness_value;
        double[] OffspringsFitnessValue;
        int NegativeTotal = 0;
        int PositiveTotal = 0;
        int counter = 0;
        string outpput = "";
        Int64 generation;
        double MutationRate = 0.15;
        int TournamentSize = 10;
        int[] tour;
        
        
        public MainForm()
        {
            InitializeComponent();
        }
       
        void CreateInitialPopulation()
        {
            Random rnd = new Random();
            int tt = 0;
            for (int i = 0; i < population_size; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    tt = rnd.Next(1, 10000);
                    if (tt < 5000)
                        population[i,j] = 0;
                    else
                        population[i,j] = 1;
                }
            }
        }

        double CalculateFitness(int[,] x, int index)
        {
            int sum = 0;
            for (int i = 0; i < N; i++)
            {
                sum += myset[i] * x[index,i];
            }

            int MaxDistance = 0;

            MaxDistance = Math.Max(PositiveTotal - Goal, Goal - NegativeTotal);
            MaxDistance = Math.Max(MaxDistance, Goal);

            double t1 = Math.Abs(sum - Goal);
            double t2 = (double)(t1 / MaxDistance);
            double t3 = Math.Sqrt(t2);
            double t4 = 1 - t3;
            return t4;
        }

        bool EvaluatePopulation()
        {
            for (int i = 0; i < population_size; i++)
            {
                fitness_value[i] = CalculateFitness(population, i);
                if (fitness_value[i] == 1)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (population[i, j] == 1)
                        {
                            counter++;
                            outpput += myset[j].ToString() + " ";
                        }
                    }
                    if (counter > 0)
                    {
                        outpput = counter.ToString() + "\n" + outpput.TrimEnd();
                        Output_t.Text = outpput;
                        return true;
                    }
                }
            }

            counter = 0;
            outpput = "";
            return false;
        }

        int Find_Max_Fitness(int[] x)
        {
            int Max = x[0];

            for (int i = 1; i < 10; i++)
            {
                if(fitness_value[x[i]] > fitness_value[Max])
                    Max = x[i];
            }
            return Max;
        }

        void TournamentSelection()
        {
            int[] k = new int[10];
            Random myrnd = new Random();

            for (int j = 0; j < TournamentSize; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    k[i] = myrnd.Next(0, population_size);
                }
                tour[j] = Find_Max_Fitness(k);
            }
        }

        void CrossOver()
        {
            int Start = 1;
            int End = 0;
            int j = 0;
            Random myrnd = new Random();

            while (Start >= End)
            {
                Start = myrnd.Next(0, N);
                End = myrnd.Next(0, N);
            }
            for (int i = 0; i < TournamentSize - 1; i += 2)
            {
                for (j = 0; j < Start; j++)
                {
                    Offsprings[i, j] = population[tour[i], j];
                    Offsprings[i + 1, j] = population[tour[i + 1], j];
                }
                for (j = Start; j < End; j++)
                {
                    Offsprings[i, j] = population[tour[i + 1], j];
                    Offsprings[i + 1, j] = population[tour[i], j];
                }
                for (j = End; j < N; j++)
                {
                    Offsprings[i, j] = population[tour[i], j];
                    Offsprings[i + 1, j] = population[tour[i + 1], j];
                }
            }
        }

        void Mutate()
        {
            Random myrnd = new Random();
            for (int i = 0; i < TournamentSize; i++)
                for (int j = 0; j < N; j++)
                {
                    if (myrnd.NextDouble() < MutationRate)
                    {
                        if (Offsprings[i, j] == 0)
                            Offsprings[i, j] = 1;
                        else
                            Offsprings[i, j] = 0;
                    }
                }
        }

        bool EvaluateOffsprings()
        {
            for (int i = 0; i < TournamentSize; i++)
            {
                OffspringsFitnessValue[i] = CalculateFitness(Offsprings, i);
                if (OffspringsFitnessValue[i] == 1)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (Offsprings[i, j] == 1)
                        {
                            counter++;
                            outpput += myset[j].ToString() + " ";
                        }
                    }
                    if (counter > 0)
                    {
                        outpput = counter.ToString() + "\n" + outpput.TrimEnd();
                        Output_t.Text = outpput;
                        return true;
                    }
                }
            }
            counter = 0;
            outpput = "";
            return false;
        }

        void CreateNextGeneration()
        {
            int Max = 0;
            for (int i = 1; i < TournamentSize; i++)
                if (OffspringsFitnessValue[i] > OffspringsFitnessValue[Max])
                    Max = i;

            int Min = 0;
            for (int j = 1; j < population_size; j++)
                if (fitness_value[j] < fitness_value[Min])
                    Min = j;

            for (int k = 0; k < N; k++)
                population[Min, k] = Offsprings[Max, k];

        }

        void runGA()
        {
            CreateInitialPopulation();

            if (EvaluatePopulation() == true)
                return;

            if(radioButton1.Checked == true)
                while (true)
                {
                    TournamentSelection();
                    CrossOver();
                    Mutate();
                    if (EvaluateOffsprings() == true)
                        return;
                    CreateNextGeneration();
                }

            if(radioButton2.Checked == true)
            {
                generation = Int64.Parse(Generation_t.Text);
                while (generation > 0)
                {
                    TournamentSelection();
                    CrossOver();
                    Mutate();
                    if (EvaluateOffsprings() == true)
                        return;
                    CreateNextGeneration();
                    generation--;
                }
            }


        }

        private void Browse_b_Click(object sender, EventArgs e)
        {
            outpput = "";
            counter = 0;
            Output_t.Clear();
            fitness_value = new double[population_size];
            OffspringsFitnessValue = new double[TournamentSize];
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Browse for input file ...";
            dlg.Filter = "Text Documents(*.txt)|*.txt";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                String[] input = File.ReadAllLines(dlg.FileName);
                N = int.Parse(input[0].Substring(0, input[0].IndexOf(' ')));
                population = new int[population_size, N];
                Offsprings = new int[TournamentSize, N];
                tour = new int[10];
                myset = new int[N];
                chromosome = new int[N];
                Goal = int.Parse(input[0].Substring(input[0].IndexOf(' ') + 1, input[0].Length - input[0].IndexOf(' ') - 1));
                string temp = input[1];
                int i;
                for (i = 0; i < N - 1; i++)
                {
                    myset[i] = int.Parse(temp.Substring(0, temp.IndexOf(' ')));
                    temp = temp.Substring(temp.IndexOf(' ') + 1, temp.Length - temp.IndexOf(' ') - 1);
                }
                myset[i] = int.Parse(temp);

                for (i = 0; i < N; i++)
                {
                    if (myset[i] < 0)
                        NegativeTotal += myset[i];
                    else
                        PositiveTotal += myset[i];
                }

                runGA();
            }
        }
    }
}
