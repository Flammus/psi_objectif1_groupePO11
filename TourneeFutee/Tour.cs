using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    
    public class Tour
    {
       
        private readonly List<string> _vertices;
        private readonly float _cost;

        public Tour()
        {
            _vertices = new List<string>();
            _cost     = float.PositiveInfinity;
        }

        public Tour(List<string> vertices, float cost)
        {
            _vertices = new List<string>(vertices);
            _cost     = cost;
        }

        public Tour(List<(string source, string destination)> segments, float cost)
        {
            _cost     = cost;
            _vertices = BuildVertexSequence(segments);
        }

        public float Cost => _cost;

        public float TotalCost => _cost;

        public IList<string> Vertices => _vertices.AsReadOnly();

        public int NbSegments => Math.Max(0, _vertices.Count - 1);
        //Meth publiques
        public bool ContainsSegment((string source, string destination) segment)
        {
            for (int i = 0; i < _vertices.Count - 1; i++)
                if (_vertices[i] == segment.source && _vertices[i + 1] == segment.destination)
                    return true;
            return false;
        }

        public void Print()
        {
            Console.WriteLine("Coût total : " + _cost);
            for (int i = 0; i < _vertices.Count - 1; i++)
                Console.WriteLine(_vertices[i] + " -> " + _vertices[i + 1]);
        }
        //Meth privées
        private static List<string> BuildVertexSequence(
            List<(string source, string destination)> segments)
        {
            if (segments == null || segments.Count == 0)
                return new List<string>();

            var next = new Dictionary<string, string>(segments.Count);
            foreach (var seg in segments)
                next[seg.source] = seg.destination;

            string start   = segments[0].source;
            var    sequence = new List<string> { start };
            string current  = start;

            for (int step = 0; step < segments.Count; step++)
            {
                if (!next.TryGetValue(current, out string nextVertex))
                    break;           // chaîne cassée 

                sequence.Add(nextVertex);
                current = nextVertex;

                if (current == start)
                    break;           // cycle fermé 
            }

            return sequence;
        }
    }
}
