using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    public class Tour
    {
        private List<(string source, string destination)> _segments;
        private float _cost;

        public Tour()
        {
            _segments = new List<(string, string)>();
            _cost = float.PositiveInfinity;
        }

        public Tour(List<(string source, string destination)> segments, float cost)
        {
            _segments = new List<(string, string)>(segments);
            _cost = cost;
        }

        public float Cost
        {
            get { return _cost; }
        }

        public int NbSegments
        {
            get { return _segments.Count; }
        }

        public bool ContainsSegment((string source, string destination) segment)
        {
            return _segments.Contains(segment);
        }

        public void Print()
        {
            Console.WriteLine("Coût total : " + Cost);
            foreach (var seg in _segments)
            {
                Console.WriteLine(seg.source + " -> " + seg.destination);
            }
        }
    }
}