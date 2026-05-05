DROP TABLE IF EXISTS EtapeTournee;
DROP TABLE IF EXISTS Tournee;
DROP TABLE IF EXISTS Arc;
DROP TABLE IF EXISTS Sommet;
DROP TABLE IF EXISTS Graphe;

CREATE TABLE Graphe (
    id           INT UNSIGNED  NOT NULL AUTO_INCREMENT,
    est_oriente  TINYINT(1)    NOT NULL DEFAULT 0,
    nom          VARCHAR(100)  NULL,

    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


CREATE TABLE Sommet (
    id               INT UNSIGNED  NOT NULL AUTO_INCREMENT,
    graphe_id        INT UNSIGNED  NOT NULL,
    nom              VARCHAR(50)   NOT NULL,
    valeur           FLOAT         NOT NULL DEFAULT 0,
    ordre_insertion  INT UNSIGNED  NOT NULL,   -- indice 0-based dans le graphe C#

    PRIMARY KEY (id),
    FOREIGN KEY (graphe_id) REFERENCES Graphe(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE Arc (
    id             INT UNSIGNED  NOT NULL AUTO_INCREMENT,
    graphe_id      INT UNSIGNED  NOT NULL,
    sommet_source  INT UNSIGNED  NOT NULL,   -- FK vers Sommet (départ)
    sommet_dest    INT UNSIGNED  NOT NULL,   -- FK vers Sommet (arrivée)
    poids          FLOAT         NOT NULL,

    PRIMARY KEY (id),
    FOREIGN KEY (graphe_id)     REFERENCES Graphe(id)  ON DELETE CASCADE,
    FOREIGN KEY (sommet_source) REFERENCES Sommet(id)  ON DELETE CASCADE,
    FOREIGN KEY (sommet_dest)   REFERENCES Sommet(id)  ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


CREATE TABLE Tournee (
    id          INT UNSIGNED  NOT NULL AUTO_INCREMENT,
    graphe_id   INT UNSIGNED  NOT NULL,
    cout_total  FLOAT         NOT NULL,

    PRIMARY KEY (id),
    FOREIGN KEY (graphe_id) REFERENCES Graphe(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


CREATE TABLE EtapeTournee (
    tournee_id    INT UNSIGNED  NOT NULL,
    numero_ordre  INT UNSIGNED  NOT NULL,   -- position dans la séquence (0-based)
    sommet_id     INT UNSIGNED  NOT NULL,   -- FK vers le sommet visité

    PRIMARY KEY (tournee_id, numero_ordre),
    FOREIGN KEY (tournee_id) REFERENCES Tournee(id) ON DELETE CASCADE,
    FOREIGN KEY (sommet_id)  REFERENCES Sommet(id)  ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


SHOW TABLES;
