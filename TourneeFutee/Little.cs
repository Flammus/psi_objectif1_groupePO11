using System;
using System.Collections.Generic;
using System.Reflection;

namespace TourneeFutee
{
    public class Little
    {
        private Graph _graph;
        private List<string> _cities;
        private Tour _bestTour;

        public Little(Graph graph)
        {
            _graph = graph;
            _cities = new List<string>();

            var field = graph.GetType().GetField("_vertices", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                var vertices = (System.Collections.IEnumerable)field.GetValue(graph);
                foreach (var v in vertices)
                {
                    var nameField = v.GetType().GetField("Name", BindingFlags.Public | BindingFlags.Instance);
                    _cities.Add((string)nameField.GetValue(v));
                }
            }
        }

        public Tour ComputeOptimalTour()
        {
            int n = _cities.Count;
            if (n == 0) return new Tour();

            _bestTour = new Tour();
            Matrix initialMatrix = new Matrix(n, n, 0f);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        initialMatrix.SetValue(i, j, float.PositiveInfinity);
                    }
                    else
                    {
                        try
                        {
                            float weight = _graph.GetEdgeWeight(_cities[i], _cities[j]);
                            initialMatrix.SetValue(i, j, weight);
                        }
                        catch
                        {
                            initialMatrix.SetValue(i, j, float.PositiveInfinity);
                        }
                    }
                }
            }

            float initialLowerBound = ReduceMatrix(initialMatrix);
            BranchAndBound(initialMatrix, initialLowerBound, new List<(string, string)>());

            return _bestTour;
        }

        private void BranchAndBound(Matrix currentMatrix, float lowerBound, List<(string, string)> includedSegments)
        {
            if (includedSegments.Count == _cities.Count)
            {
                if (lowerBound < _bestTour.Cost)
                {
                    _bestTour = new Tour(includedSegments, lowerBound);
                }
                return;
            }

            if (lowerBound >= _bestTour.Cost) return;

            var maxRegret = GetMaxRegret(currentMatrix);
            if (maxRegret.i == -1) return;

            string source = _cities[maxRegret.i];
            string dest = _cities[maxRegret.j];
            var segment = (source, dest);

            Matrix rightMatrix = CopyMatrix(currentMatrix);

            for (int col = 0; col < rightMatrix.NbColumns; col++) rightMatrix.SetValue(maxRegret.i, col, float.PositiveInfinity);
            for (int row = 0; row < rightMatrix.NbRows; row++) rightMatrix.SetValue(row, maxRegret.j, float.PositiveInfinity);

            var newIncluded = new List<(string, string)>(includedSegments);
            newIncluded.Add(segment);

            for (int r = 0; r < rightMatrix.NbRows; r++)
            {
                for (int c = 0; c < rightMatrix.NbColumns; c++)
                {
                    if (rightMatrix.GetValue(r, c) != float.PositiveInfinity)
                    {
                        if (IsForbiddenSegment((_cities[r], _cities[c]), newIncluded, _cities.Count))
                        {
                            rightMatrix.SetValue(r, c, float.PositiveInfinity);
                        }
                    }
                }
            }

            float rightLowerBound = lowerBound + ReduceMatrix(rightMatrix);

            if (rightLowerBound < _bestTour.Cost)
            {
                BranchAndBound(rightMatrix, rightLowerBound, newIncluded);
            }

            Matrix leftMatrix = CopyMatrix(currentMatrix);
            leftMatrix.SetValue(maxRegret.i, maxRegret.j, float.PositiveInfinity);

            float leftLowerBound = lowerBound + ReduceMatrix(leftMatrix);

            if (leftLowerBound < _bestTour.Cost)
            {
                BranchAndBound(leftMatrix, leftLowerBound, includedSegments);
            }
        }

        public static float ReduceMatrix(Matrix m)
        {
            float totalReduction = 0f;
            int nbRows = m.NbRows;
            int nbCols = m.NbColumns;

            for (int i = 0; i < nbRows; i++)
            {
                float min = float.PositiveInfinity;
                for (int j = 0; j < nbCols; j++)
                {
                    float val = m.GetValue(i, j);
                    if (val < min) min = val;
                }

                if (min > 0f && min < float.PositiveInfinity)
                {
                    totalReduction += min;
                    for (int j = 0; j < nbCols; j++)
                    {
                        float val = m.GetValue(i, j);
                        if (val != float.PositiveInfinity) m.SetValue(i, j, val - min);
                    }
                }
            }

            for (int j = 0; j < nbCols; j++)
            {
                float min = float.PositiveInfinity;
                for (int i = 0; i < nbRows; i++)
                {
                    float val = m.GetValue(i, j);
                    if (val < min) min = val;
                }

                if (min > 0f && min < float.PositiveInfinity)
                {
                    totalReduction += min;
                    for (int i = 0; i < nbRows; i++)
                    {
                        float val = m.GetValue(i, j);
                        if (val != float.PositiveInfinity) m.SetValue(i, j, val - min);
                    }
                }
            }

            return totalReduction;
        }

        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            int maxI = -1;
            int maxJ = -1;
            float maxRegret = -1f;

            for (int i = 0; i < m.NbRows; i++)
            {
                for (int j = 0; j < m.NbColumns; j++)
                {
                    if (m.GetValue(i, j) == 0f)
                    {
                        float minRow = float.PositiveInfinity;
                        for (int c = 0; c < m.NbColumns; c++)
                        {
                            if (c != j)
                            {
                                float val = m.GetValue(i, c);
                                if (val < minRow) minRow = val;
                            }
                        }

                        float minCol = float.PositiveInfinity;
                        for (int r = 0; r < m.NbRows; r++)
                        {
                            if (r != i)
                            {
                                float val = m.GetValue(r, j);
                                if (val < minCol) minCol = val;
                            }
                        }

                        float regret = 0f;
                        if (minRow < float.PositiveInfinity) regret += minRow;
                        if (minCol < float.PositiveInfinity) regret += minCol;

                        if (regret > maxRegret)
                        {
                            maxRegret = regret;
                            maxI = i;
                            maxJ = j;
                        }
                    }
                }
            }
            return (maxI, maxJ, maxRegret);
        }

        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {
            string current = segment.destination;
            int length = 1;

            while (true)
            {
                bool foundNext = false;
                foreach (var edge in includedSegments)
                {
                    if (edge.source == current)
                    {
                        current = edge.destination;
                        length++;
                        foundNext = true;
                        break;
                    }
                }

                if (!foundNext) break;
                if (current == segment.source) return length < nbCities;
                if (length > nbCities) break;
            }

            current = segment.source;
            length = 1;

            while (true)
            {
                bool foundPrev = false;
                foreach (var edge in includedSegments)
                {
                    if (edge.destination == current)
                    {
                        current = edge.source;
                        length++;
                        foundPrev = true;
                        break;
                    }
                }

                if (!foundPrev) break;
                if (current == segment.destination) return length < nbCities;
                if (length > nbCities) break;
            }

            return false;
        }

        private Matrix CopyMatrix(Matrix m)
        {
            Matrix copy = new Matrix(m.NbRows, m.NbColumns, m.DefaultValue);
            for (int i = 0; i < m.NbRows; i++)
            {
                for (int j = 0; j < m.NbColumns; j++)
                {
                    copy.SetValue(i, j, m.GetValue(i, j));
                }
            }
            return copy;
        }
    }
}