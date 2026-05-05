using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TourneeFutee
{
   
    public class ServicePersistance
    {
        // Attributs privés

        private readonly string _connectionString;

        public ServicePersistance(string serverIp, string dbname, string user, string pwd)
        {
            _connectionString =
                $"server={serverIp};database={dbname};uid={user};pwd={pwd};";

            try
            {
                using (var conn = OpenConnection())
                {
                    // Connexion réussie
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception(
                    $"Impossible de se connecter à la base de données '{dbname}' " +
                    $"sur '{serverIp}' : {ex.Message}", ex);
            }
        }

        // SaveGraph

        public uint SaveGraph(Graph g)
        {
            try
            {
                using (var conn = OpenConnection())
                {
                    uint graphId;
                    string insertGraphSql =
                        "INSERT INTO Graphe (est_oriente) VALUES (@oriented); " +
                        "SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(insertGraphSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@oriented", g.IsOriented ? 1 : 0);
                        graphId = Convert.ToUInt32(cmd.ExecuteScalar());
                    }

                    List<string> names      = g.GetVertexNames();
                    var          nameToDbId = new Dictionary<string, uint>(names.Count);

                    string insertSommetSql =
                        "INSERT INTO Sommet (graphe_id, nom, valeur, ordre_insertion) " +
                        "VALUES (@gid, @nom, @val, @ord); " +
                        "SELECT LAST_INSERT_ID();";

                    for (int i = 0; i < names.Count; i++)
                    {
                        string name  = names[i];
                        float  value = g.GetVertexValue(name);

                        using (var cmd = new MySqlCommand(insertSommetSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@gid", graphId);
                            cmd.Parameters.AddWithValue("@nom", name);
                            cmd.Parameters.AddWithValue("@val", value);
                            cmd.Parameters.AddWithValue("@ord", i);  
                            uint sommetId = Convert.ToUInt32(cmd.ExecuteScalar());
                            nameToDbId[name] = sommetId;
                        }
                    }

                    string insertArcSql =
                        "INSERT INTO Arc (graphe_id, sommet_source, sommet_dest, poids) " +
                        "VALUES (@gid, @src, @dst, @poids);";

                    for (int i = 0; i < names.Count; i++)
                    {
                        List<string> neighbors = g.GetNeighbors(names[i]);
                        foreach (string neighbor in neighbors)
                        {
                            int j = names.IndexOf(neighbor);

                            if (!g.IsOriented && i >= j)
                                continue;

                            float weight = g.GetEdgeWeight(names[i], neighbor);

                            using (var cmd = new MySqlCommand(insertArcSql, conn))
                            {
                                cmd.Parameters.AddWithValue("@gid",   graphId);
                                cmd.Parameters.AddWithValue("@src",   nameToDbId[names[i]]);
                                cmd.Parameters.AddWithValue("@dst",   nameToDbId[neighbor]);
                                cmd.Parameters.AddWithValue("@poids", weight);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    return graphId;
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Erreur lors de la sauvegarde du graphe : {ex.Message}", ex);
            }
        }

       
        public Graph LoadGraph(uint id)
        {
            try
            {
                using (var conn = OpenConnection())
                {
                    bool isOriented;
                    string selectGraphSql =
                        "SELECT est_oriente FROM Graphe WHERE id = @id;";

                    using (var cmd = new MySqlCommand(selectGraphSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new Exception($"Aucun graphe trouvé avec l'id {id}.");
                            isOriented = Convert.ToBoolean(reader["est_oriente"]);
                        }
                    }

                    var graph = new Graph(isOriented: isOriented);

                    
                    var dbIdToName = new Dictionary<uint, string>();

                    string selectSommetSql =
                        "SELECT id, nom, valeur " +
                        "FROM Sommet " +
                        "WHERE graphe_id = @gid " +
                        "ORDER BY ordre_insertion ASC;";

                    using (var cmd = new MySqlCommand(selectSommetSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                uint   sommetId = Convert.ToUInt32(reader["id"]);
                                string nom      = reader["nom"].ToString();
                                float  valeur   = Convert.ToSingle(reader["valeur"]);

                                graph.AddVertex(nom, valeur);
                                dbIdToName[sommetId] = nom;
                            }
                        }
                    }

                    string selectArcSql =
                        "SELECT sommet_source, sommet_dest, poids " +
                        "FROM Arc " +
                        "WHERE graphe_id = @gid;";

                    using (var cmd = new MySqlCommand(selectArcSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                uint  srcId  = Convert.ToUInt32(reader["sommet_source"]);
                                uint  dstId  = Convert.ToUInt32(reader["sommet_dest"]);
                                float poids  = Convert.ToSingle(reader["poids"]);

                                string srcName = dbIdToName[srcId];
                                string dstName = dbIdToName[dstId];

                                graph.AddEdge(srcName, dstName, poids);
                            }
                        }
                    }

                    return graph;
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Erreur lors du chargement du graphe {id} : {ex.Message}", ex);
            }
        }

       
        public uint SaveTour(uint graphId, Tour t)
        {
            try
            {
                using (var conn = OpenConnection())
                {
                    // ── 1. Insérer la tournée ───────────────────────────────
                    uint   tourId;
                    string insertTourneeSql =
                        "INSERT INTO Tournee (graphe_id, cout_total) " +
                        "VALUES (@gid, @cout); " +
                        "SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(insertTourneeSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gid",  graphId);
                        cmd.Parameters.AddWithValue("@cout", t.TotalCost);
                        tourId = Convert.ToUInt32(cmd.ExecuteScalar());
                    }

                    string selectSommetIdSql =
                        "SELECT id FROM Sommet " +
                        "WHERE graphe_id = @gid AND nom = @nom " +
                        "LIMIT 1;";

                    string insertEtapeSql =
                        "INSERT INTO EtapeTournee (tournee_id, numero_ordre, sommet_id) " +
                        "VALUES (@tid, @ord, @sid);";

                    IList<string> vertices = t.Vertices;
                    for (int ordre = 0; ordre < vertices.Count; ordre++)
                    {
                        uint sommetId;
                        using (var cmd = new MySqlCommand(selectSommetIdSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@gid", graphId);
                            cmd.Parameters.AddWithValue("@nom", vertices[ordre]);
                            object result = cmd.ExecuteScalar();
                            if (result == null)
                                throw new Exception(
                                    $"Le sommet '{vertices[ordre]}' n'existe pas " +
                                    $"dans le graphe {graphId} en base de données.");
                            sommetId = Convert.ToUInt32(result);
                        }

                        using (var cmd = new MySqlCommand(insertEtapeSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@tid", tourId);
                            cmd.Parameters.AddWithValue("@ord", ordre);
                            cmd.Parameters.AddWithValue("@sid", sommetId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    return tourId;
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Erreur lors de la sauvegarde de la tournée : {ex.Message}", ex);
            }
        }

        public Tour LoadTour(uint id)
        {
            try
            {
                using (var conn = OpenConnection())
                {
                    float coutTotal;
                    string selectTourneeSql =
                        "SELECT cout_total FROM Tournee WHERE id = @id;";

                    using (var cmd = new MySqlCommand(selectTourneeSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        object result = cmd.ExecuteScalar();
                        if (result == null)
                            throw new Exception($"Aucune tournée trouvée avec l'id {id}.");
                        coutTotal = Convert.ToSingle(result);
                    }

                    var vertices = new List<string>();
                    string selectEtapesSql =
                        "SELECT S.nom " +
                        "FROM EtapeTournee ET " +
                        "JOIN Sommet S ON ET.sommet_id = S.id " +
                        "WHERE ET.tournee_id = @tid " +
                        "ORDER BY ET.numero_ordre ASC;";

                    using (var cmd = new MySqlCommand(selectEtapesSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@tid", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                vertices.Add(reader["nom"].ToString());
                        }
                    }

                    return new Tour(vertices, coutTotal);
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Erreur lors du chargement de la tournée {id} : {ex.Message}", ex);
            }
        }

        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
