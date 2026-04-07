using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    public class Graph
    {
        private class Vertex
        {
            public string Name;
            public float Value;
            public Vertex(string name, float value) { Name = name; Value = value; }
        }

        private List<Vertex> _vertices;
        private Matrix _matrix;
        private bool _directed;
        private float _noEdgeValue;

        public Graph(bool directed, float noEdgeValue = 0)
        {
            _directed = directed;
            _noEdgeValue = noEdgeValue;
            _vertices = new List<Vertex>();
            _matrix = new Matrix(0, 0, noEdgeValue);
        }

        public int Order
        {
            get { return _vertices.Count; }
        }

        public bool Directed
        {
            get { return _directed; }
        }

        // Retourne l'indice du sommet 'name', ou -1 s'il est absent
        private int IndexOf(string name)
        {
            for (int i = 0; i < _vertices.Count; i++)
                if (_vertices[i].Name == name) return i;
            return -1;
        }

        private void CheckVertex(string name)
        {
            if (IndexOf(name) == -1)
                throw new ArgumentException($"Le sommet '{name}' n'existe pas.");
        }

        // --- Gestion des sommets ---

        public void AddVertex(string name, float value = 0)
        {
            if (IndexOf(name) != -1)
                throw new ArgumentException($"Un sommet nommé '{name}' existe déjà.");

            int n = _vertices.Count;
            _vertices.Add(new Vertex(name, value));
            _matrix.AddRow(n);
            _matrix.AddColumn(n);
        }

        public void RemoveVertex(string name)
        {
            int i = IndexOf(name);
            if (i == -1)
                throw new ArgumentException($"Le sommet '{name}' n'existe pas.");

            _vertices.RemoveAt(i);
            _matrix.RemoveRow(i);
            _matrix.RemoveColumn(i);
        }

        public float GetVertexValue(string name)
        {
            int i = IndexOf(name);
            if (i == -1)
                throw new ArgumentException($"Le sommet '{name}' n'existe pas.");
            return _vertices[i].Value;
        }

        public void SetVertexValue(string name, float value)
        {
            int i = IndexOf(name);
            if (i == -1)
                throw new ArgumentException($"Le sommet '{name}' n'existe pas.");
            _vertices[i].Value = value;
        }

        public List<string> GetNeighbors(string vertexName)
        {
            List<string> neighborNames = new List<string>();
            int i = IndexOf(vertexName);
            if (i == -1)
                throw new ArgumentException($"Le sommet '{vertexName}' n'existe pas.");

            for (int j = 0; j < _vertices.Count; j++)
                if (_matrix.GetValue(i, j) != _noEdgeValue)
                    neighborNames.Add(_vertices[j].Name);

            return neighborNames;
        }

        // --- Gestion des arcs ---

        public void AddEdge(string sourceName, string destinationName, float weight = 1)
        {
            int i = IndexOf(sourceName);
            int j = IndexOf(destinationName);
            if (i == -1) throw new ArgumentException($"Le sommet '{sourceName}' n'existe pas.");
            if (j == -1) throw new ArgumentException($"Le sommet '{destinationName}' n'existe pas.");
            if (_matrix.GetValue(i, j) != _noEdgeValue)
                throw new ArgumentException($"Un arc de '{sourceName}' vers '{destinationName}' existe déjà.");

            _matrix.SetValue(i, j, weight);
            if (!_directed)
                _matrix.SetValue(j, i, weight);
        }

        public void RemoveEdge(string sourceName, string destinationName)
        {
            int i = IndexOf(sourceName);
            int j = IndexOf(destinationName);
            if (i == -1) throw new ArgumentException($"Le sommet '{sourceName}' n'existe pas.");
            if (j == -1) throw new ArgumentException($"Le sommet '{destinationName}' n'existe pas.");
            if (_matrix.GetValue(i, j) == _noEdgeValue)
                throw new ArgumentException($"L'arc de '{sourceName}' vers '{destinationName}' n'existe pas.");

            _matrix.SetValue(i, j, _noEdgeValue);
            if (!_directed)
                _matrix.SetValue(j, i, _noEdgeValue);
        }

        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            int i = IndexOf(sourceName);
            int j = IndexOf(destinationName);
            if (i == -1) throw new ArgumentException($"Le sommet '{sourceName}' n'existe pas.");
            if (j == -1) throw new ArgumentException($"Le sommet '{destinationName}' n'existe pas.");
            if (_matrix.GetValue(i, j) == _noEdgeValue)
                throw new ArgumentException($"L'arc de '{sourceName}' vers '{destinationName}' n'existe pas.");

            return _matrix.GetValue(i, j);
        }

        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            int i = IndexOf(sourceName);
            int j = IndexOf(destinationName);
            if (i == -1) throw new ArgumentException($"Le sommet '{sourceName}' n'existe pas.");
            if (j == -1) throw new ArgumentException($"Le sommet '{destinationName}' n'existe pas.");
            if (_matrix.GetValue(i, j) == _noEdgeValue)
                throw new ArgumentException($"L'arc de '{sourceName}' vers '{destinationName}' n'existe pas.");

            _matrix.SetValue(i, j, weight);
            if (!_directed)
                _matrix.SetValue(j, i, weight);
        }
    }
}