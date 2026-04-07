using System;
using System.Collections.Generic;

namespace TourneeFutee
{
    public class Matrix
    {
        private List<List<float>> _data;
        private float _defaultValue;

        public Matrix(int nbRows = 0, int nbColumns = 0, float defaultValue = 0)
        {
            if (nbRows < 0 || nbColumns < 0)
                throw new ArgumentOutOfRangeException("Les dimensions ne peuvent pas être négatives.");

            _defaultValue = defaultValue;
            _data = new List<List<float>>();

            for (int i = 0; i < nbRows; i++)
            {
                List<float> row = new List<float>();
                for (int j = 0; j < nbColumns; j++)
                    row.Add(defaultValue);
                _data.Add(row);
            }
        }

        public float DefaultValue
        {
            get { return _defaultValue; }
        }

        public int NbRows
        {
            get { return _data.Count; }
        }

        public int NbColumns
        {
            get
            {
                if (_data.Count > 0)
                    return _data[0].Count;
                return 0;
            }
        }

        public void AddRow(int i)
        {
            if (i < 0 || i > NbRows)
                throw new ArgumentOutOfRangeException(nameof(i), $"Indice {i} invalide (NbRows={NbRows}).");

            var newRow = new List<float>();
            for (int k = 0; k < NbColumns; k++)
                newRow.Add(_defaultValue);
            _data.Insert(i, newRow);
        }

        public void AddColumn(int j)
        {
            if (j < 0 || j > NbColumns)
                throw new ArgumentOutOfRangeException(nameof(j), $"Indice {j} invalide (NbColumns={NbColumns}).");

            foreach (var row in _data)
                row.Insert(j, _defaultValue);
        }

        public void RemoveRow(int i)
        {
            if (i < 0 || i >= NbRows)
                throw new ArgumentOutOfRangeException(nameof(i), $"Indice {i} invalide (NbRows={NbRows}).");

            _data.RemoveAt(i);
        }

        public void RemoveColumn(int j)
        {
            if (j < 0 || j >= NbColumns)
                throw new ArgumentOutOfRangeException(nameof(j), $"Indice {j} invalide (NbColumns={NbColumns}).");

            foreach (var row in _data)
                row.RemoveAt(j);
        }

        public float GetValue(int i, int j)
        {
            if (i < 0 || i >= NbRows || j < 0 || j >= NbColumns)
                throw new ArgumentOutOfRangeException($"Indice ({i},{j}) invalide (NbRows={NbRows}, NbColumns={NbColumns}).");

            return _data[i][j];
        }

        public void SetValue(int i, int j, float v)
        {
            if (i < 0 || i >= NbRows || j < 0 || j >= NbColumns)
                throw new ArgumentOutOfRangeException($"Indice ({i},{j}) invalide (NbRows={NbRows}, NbColumns={NbColumns}).");

            _data[i][j] = v;
        }

        public void Print()
        {
            for (int i = 0; i < NbRows; i++)
            {
                for (int j = 0; j < NbColumns; j++)
                    Console.Write($"{_data[i][j],6}");
                Console.WriteLine();
            }
        }
    }
}