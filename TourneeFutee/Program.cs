namespace TourneeFutee
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestMatrix();
            TestGraph();
        }

        //   TESTS MATRIX
        
        static void TestMatrix()
        {
            Console.WriteLine("=== TESTS MATRIX ===\n");

            // Constructeur : matrice 3x3 remplie de 0
            Matrix m = new Matrix(3, 3, 0);
            Console.WriteLine("Matrice 3x3 initialisée à 0 :");
            m.Print();
            Console.WriteLine();

            // SetValue / GetValue
            m.SetValue(0, 1, 5);
            m.SetValue(1, 2, 3);
            m.SetValue(2, 0, 7);
            Console.WriteLine("Après SetValue(0,1,5), SetValue(1,2,3), SetValue(2,0,7) :");
            m.Print();
            Console.WriteLine("GetValue(0,1) attendu 5 : " + m.GetValue(0, 1));
            Console.WriteLine("GetValue(1,2) attendu 3 : " + m.GetValue(1, 2));
            Console.WriteLine("GetValue(2,0) attendu 7 : " + m.GetValue(2, 0));
            Console.WriteLine();

            // AddRow en milieu (indice 1)
            m.AddRow(1);
            Console.WriteLine("Après AddRow(1) - insertion ligne vide à l'indice 1 (3+1=4 lignes) :");
            m.Print();
            Console.WriteLine("NbRows attendu 4 : " + m.NbRows);
            Console.WriteLine("GetValue(0,1) toujours 5 : " + m.GetValue(0, 1));
            Console.WriteLine("GetValue(2,2) ancienne ligne 1 décalée, attendu 3 : " + m.GetValue(2, 2));
            Console.WriteLine();

            // AddColumn en fin
            m.AddColumn(m.NbColumns);
            Console.WriteLine("Après AddColumn en fin (NbColumns=" + m.NbColumns + ") :");
            m.Print();
            Console.WriteLine();

            // RemoveRow(0)
            m.RemoveRow(0);
            Console.WriteLine("Après RemoveRow(0) :");
            m.Print();
            Console.WriteLine("NbRows attendu 3 : " + m.NbRows);
            Console.WriteLine();

            // RemoveColumn(0)
            m.RemoveColumn(0);
            Console.WriteLine("Après RemoveColumn(0) :");
            m.Print();
            Console.WriteLine("NbColumns attendu 3 : " + m.NbColumns);
            Console.WriteLine();

            

            Console.WriteLine("\n=== FIN TESTS MATRIX ===\n");
        }

        //   TESTS GRAPH
        static void TestGraph()
        {
            Console.WriteLine("=== TESTS GRAPH ===\n");

            // --- Graphe orienté ---
            Console.WriteLine("-- Graphe ORIENTE --");
            Graph gd = new Graph(true);
            Console.WriteLine("Directed attendu True : " + gd.Directed);
            Console.WriteLine("Order attendu 0 : " + gd.Order);

            gd.AddVertex("A", 2);
            gd.AddVertex("B", 1);
            gd.AddVertex("C", 2);
            gd.AddVertex("D", 3);
            Console.WriteLine("Order attendu 4 : " + gd.Order);
            Console.WriteLine("GetVertexValue(A) attendu 2 : " + gd.GetVertexValue("A"));
            Console.WriteLine("GetVertexValue(D) attendu 3 : " + gd.GetVertexValue("D"));

            // Ajout d'arcs
            gd.AddEdge("A", "A", 4); // boucle
            gd.AddEdge("A", "B", 3);
            gd.AddEdge("A", "C", 2);
            gd.AddEdge("B", "A", 1);
            gd.AddEdge("B", "C", 5);
            gd.AddEdge("D", "B", 4);
            gd.AddEdge("D", "C", 2);

            Console.WriteLine("\nGetEdgeWeight(A,B) attendu 3 : " + gd.GetEdgeWeight("A", "B"));
            Console.WriteLine("GetEdgeWeight(B,C) attendu 5 : " + gd.GetEdgeWeight("B", "C"));

            // Dans un graphe orienté, (B,A) existe mais pas forcément (C,A)
            Console.WriteLine("GetEdgeWeight(B,A) attendu 1 : " + gd.GetEdgeWeight("B", "A"));

            // Voisins
            List<string> neighborsA = gd.GetNeighbors("A");
            Console.Write("Voisins de A (attendu A,B,C) : ");
            for (int i = 0; i < neighborsA.Count; i++)
                Console.Write(neighborsA[i] + " ");
            Console.WriteLine();

            List<string> neighborsC = gd.GetNeighbors("C");
            Console.Write("Voisins de C (attendu aucun) : ");
            for (int i = 0; i < neighborsC.Count; i++)
                Console.Write(neighborsC[i] + " ");
            Console.WriteLine("(liste vide = OK)");

            

            // --- Graphe non orienté ---
            Console.WriteLine("\n-- Graphe NON ORIENTE --");
            Graph gu = new Graph(false);
            Console.WriteLine("Directed attendu False : " + gu.Directed);

            gu.AddVertex("A");
            gu.AddVertex("B");
            gu.AddEdge("A", "B", 42);

            Console.WriteLine("GetEdgeWeight(A,B) attendu 42 : " + gu.GetEdgeWeight("A", "B"));
            Console.WriteLine("GetEdgeWeight(B,A) attendu 42 (symétrique) : " + gu.GetEdgeWeight("B", "A"));

            List<string> neighborsB = gu.GetNeighbors("B");
            Console.Write("Voisins de B (attendu A) : ");
            for (int i = 0; i < neighborsB.Count; i++)
                Console.Write(neighborsB[i] + " ");
            Console.WriteLine();


   

            Console.WriteLine("\n=== FIN TESTS GRAPH ===");
        }
    }
}