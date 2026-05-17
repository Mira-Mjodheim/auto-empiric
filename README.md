# AutoEmpiric

Framework d'orchestration et de validation empirique.

## Architecture

AutoEmpiric repose sur une architecture polyglotte :
- **C#** : Cœur de l'orchestration et logique principale.
- **Python** : Traitement des données et intégrations analytiques.

## Prérequis

- .NET 10.0 SDK
- Python 3.12

## Déploiement

### 1. Composants C#

Les composants se trouvent dans `src/AutoProof.Core`.

### 2. Environnement Python

```bash
python -m venv venv
source venv/bin/activate
```

### 3. Lancement des services

L'exécution peut utiliser la variable d'environnement optionnelle suivante :

- MAX_REASONING_STEPS

## Structure des répertoires

- src/AutoProof.Core : Composants de base pour la validation.
- src/AutoProof.Core/Interfaces : Contrats et abstractions.
- src/AutoProof.Core/Models : Modèles de données.
